using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BlockBall.Settings;
using BlockBall.Physics;

[RequireComponent(typeof(Camera))]
public class PlayerCameraController : MonoBehaviour 
{
    // -------------------------------------------------------------------------------------------
	public float SpeedFactor = 1.0f;
	public float BreakFactor = 10.0f;
    public float MaxAngularVelocity = 200.0f;
    public float JumpForce = 5.0f;
	public int MilisecondsBeforeGroundContractIsValidAsJump = 300;
    public bool CanOnlyConrolledIfGrounded = false;

    public float Distance = 10.0f;
    public float XSpeed = 250.0f;
    public float YSpeed = 120.0f;

    public float YMinLimit = -80.0f;
    public float YMaxLimit = 80.0f;
    public float ZoomMinLimit = 1f;
    public float ZoomMaxLimit = 100f;

    public PhysicObjekt ObjectControlled;

    public Quaternion OffsetRotation = Quaternion.identity;

	public float Scale = 0.41f;

    // -----------------------------------------------------------------------------------------
    // Jump buffering variables
    private TimeSpan xJumpTime = TimeSpan.Zero;
    private TimeSpan xGroundContactTime = TimeSpan.Zero;
    private bool wasGroundedLastFrame = false;
    // -----------------------------------------------------------------------------------------
	private HashSet<Block> pSeeThroughList = new HashSet<Block>();
	private HashSet<Block> pNewSeeThroughList = new HashSet<Block>();
    
    // Reference to PhysicsSettings for mode checking
    private PhysicsSettings physicsSettings;

    // -------------------------------------------------------------------------------------------
	public enum MOVEMENT_TYPE
	{
		BREAK = 0,
		FORWARD,
		RIGHT,
		LEFT,
		BACKWARD
	}
        
    // -------------------------------------------------------------------------------------------
    void Awake() 
    {
        // Load PhysicsSettings at runtime using Resources
        physicsSettings = Resources.Load<PhysicsSettings>("PhysicsSettings");
        if (physicsSettings == null)
        {
            Debug.LogWarning("PhysicsSettings asset not found in PlayerCameraController. Defaulting to original behavior. Ensure PhysicsSettings.asset is in Resources folder.");
        }
        else
        {
            Debug.Log("PhysicsSettings loaded successfully in PlayerCameraController from Resources. Mode: " + physicsSettings.physicsMode);
        }
    }

    // -------------------------------------------------------------------------------------------
    void Start () 
	{
		this.ObjectControlled.GetComponent<Rigidbody>().maxAngularVelocity = this.MaxAngularVelocity;
    }

    // -------------------------------------------------------------------------------------------
    void Update()
    {
        if (WorldStateManager.GamePaused)
            return;
        
        if (this.ObjectControlled != null && Input.GetKeyDown(KeyCode.Space))
        {
            bool isGrounded = this.ObjectControlled.HasGroundContact();
            if (isGrounded || xGroundContactTime > TimeSpan.Zero)
            {
                this.Jump();
                xGroundContactTime = TimeSpan.Zero;
                if (physicsSettings != null && physicsSettings.enableJumpBufferingLogging)
                    Debug.Log($"PlayerCameraController.Update: Jump executed immediately for {this.name}. Grounded={isGrounded}, GroundContactTime={xGroundContactTime.TotalMilliseconds}ms");
            }
            else
            {
                xJumpTime = TimeSpan.FromMilliseconds(physicsSettings != null ? physicsSettings.jumpInputBufferTime : 300);
                if (physicsSettings != null && physicsSettings.enableJumpBufferingLogging)
                    Debug.Log($"PlayerCameraController.Update: Jump input queued for {this.name}. Buffer duration: {xJumpTime.TotalMilliseconds}ms");
            }
        }
        
        if (this.ObjectControlled != null)
        {
            if (this.transform.hasChanged)
            {
                pNewSeeThroughList.Clear();

                var z = this.GetComponent<Camera>().nearClipPlane;
                AddRemoveBlocksHitByRayToList(new Vector3(0.5f, 0.5f, z));
                AddRemoveBlocksHitByRayToList(new Vector3(0, 0, z));
                AddRemoveBlocksHitByRayToList(new Vector3(1, 0, z));
                AddRemoveBlocksHitByRayToList(new Vector3(0, 1, z));
                AddRemoveBlocksHitByRayToList(new Vector3(1, 1, z));

                foreach(var pBlock in pSeeThroughList)
                {
                    pBlock.SetSeeThrough(false);
                }
                pSeeThroughList.Clear();

                foreach(var pBlock in pNewSeeThroughList)
                {
                    pBlock.SetSeeThrough(true);
                }

                // swap lists
                var pTempList = pSeeThroughList;
                pSeeThroughList = pNewSeeThroughList;
                pNewSeeThroughList = pTempList;
            }
        }
    }

    // -------------------------------------------------------------------------------------------
    void FixedUpdate () 
    {
        if (WorldStateManager.GamePaused)
            return;

        if (this.ObjectControlled == null)
            return;
        
        // Jump buffering logic in FixedUpdate for physics synchronization
        xJumpTime -= TimeSpan.FromSeconds(Time.fixedDeltaTime);
        xGroundContactTime -= TimeSpan.FromSeconds(Time.fixedDeltaTime);

        bool isGrounded = this.ObjectControlled.HasGroundContact();
        
        // Update ground contact buffer time when transitioning to grounded state
        if (isGrounded && !wasGroundedLastFrame)
        {
            xGroundContactTime = TimeSpan.FromMilliseconds(physicsSettings != null ? physicsSettings.groundContactBufferTime : 200);
            if (physicsSettings != null && physicsSettings.enableJumpBufferingLogging)
                Debug.Log($"PlayerCameraController.FixedUpdate: Ground contact detected for {this.name}. Buffer set to {xGroundContactTime.TotalMilliseconds}ms");
        }
        wasGroundedLastFrame = isGrounded;
        
        // Check if a jump is queued and the object is now grounded or within ground contact buffer
        if (xJumpTime > TimeSpan.Zero && (isGrounded || xGroundContactTime > TimeSpan.Zero))
        {
            this.Jump();
            xJumpTime = TimeSpan.Zero; // Reset jump buffer after executing jump
            xGroundContactTime = TimeSpan.Zero; // Reset ground contact buffer after jump
            if (physicsSettings != null && physicsSettings.enableJumpBufferingLogging)
                Debug.Log($"PlayerCameraController.FixedUpdate: Executed queued jump for {this.name}. Grounded={isGrounded}, GroundContactTime={xGroundContactTime.TotalMilliseconds}ms");
        }
        
        if (physicsSettings != null && physicsSettings.enableJumpBufferingLogging)
            Debug.Log($"PlayerCameraController.FixedUpdate: State for {this.name} - Grounded={isGrounded}, JumpTime={xJumpTime.TotalMilliseconds}ms, GroundContactTime={xGroundContactTime.TotalMilliseconds}ms");
        
        // Movement logic remains unchanged
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            Move (MOVEMENT_TYPE.FORWARD);
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            Move (MOVEMENT_TYPE.BACKWARD);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            Move (MOVEMENT_TYPE.RIGHT);
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            Move(MOVEMENT_TYPE.LEFT);
        }
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.Keypad0))
        {
            Move(MOVEMENT_TYPE.BREAK);
        }

        // rotation
        var fXAxis = Input.GetAxis("Mouse X");  // rotates around y-axis
        var fYAxis = Input.GetAxis("Mouse Y");  // rotates around x-axis
        if (fXAxis != 0 || fYAxis != 0)
        {
            if (fXAxis != 0)
            {
                var fXEuler = fXAxis * XSpeed * 0.02f;
                var vNewForward = Quaternion.AngleAxis(fXEuler, -this.ObjectControlled.GravityDirection) * this.ObjectControlled.ForwardDirection;
                this.ObjectControlled.SetForwardDirection(vNewForward);
            }

            if (fYAxis != 0)
            {
                var vEuler = this.OffsetRotation.eulerAngles;
                vEuler.x -= fYAxis * YSpeed * 0.02f;
                vEuler.x = ClampAngle(vEuler.x, this.YMinLimit, this.YMaxLimit);
                //vEuler.y = 0;
                //vEuler.z = 0;

                this.OffsetRotation = Quaternion.Euler(vEuler);
            }
        }

        // zoom
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            this.Distance = Mathf.Clamp(this.Distance += 1, this.ZoomMinLimit, this.ZoomMaxLimit);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            this.Distance = Mathf.Clamp(this.Distance -= 1, this.ZoomMinLimit, this.ZoomMaxLimit);
        }
    }

    // -------------------------------------------------------------------------------------------
    void AddRemoveBlocksHitByRayToList(Vector3 vViewPortPoint)
    {
        if (this.ObjectControlled == null)
            return;

        var vCamRightDir = this.GetComponent<Camera>().transform.right;
        var vCamUpDir = this.GetComponent<Camera>().transform.up;
        Vector3 vTargetPoint = this.ObjectControlled.transform.position - vCamRightDir * 0.5f * Scale - vCamUpDir * 0.5f * Scale;
        vTargetPoint += vCamRightDir * (vViewPortPoint.x * Scale);
        vTargetPoint += vCamUpDir * (vViewPortPoint.y * Scale);

        Vector3 vWorldPoint = GetComponent<Camera>().ViewportToWorldPoint(vViewPortPoint);
        var vDistance = vTargetPoint - vWorldPoint;
        var pRay = new Ray(vWorldPoint, vDistance.normalized);

        var pHits = Physics.RaycastAll(pRay, vDistance.magnitude);
        foreach (var pHit in pHits)
        {
            var pBlockHit = pHit.collider.gameObject.GetComponent<Block>();
            if (pBlockHit != null)
            {
                if (!pNewSeeThroughList.Contains(pBlockHit))
                    pNewSeeThroughList.Add(pBlockHit);
                if (pSeeThroughList.Contains(pBlockHit))
                    pSeeThroughList.Remove(pBlockHit);
            }
        }
    }

    // -------------------------------------------------------------------------------------------
    void OnDrawGizmos()
    {
        var z = this.GetComponent<Camera>().nearClipPlane;
        DrawRay(new Vector3(0.5f, 0.5f, z));
        DrawRay(new Vector3(0, 0, z));
        DrawRay(new Vector3(1, 0, z));
        DrawRay(new Vector3(0, 1, z));
        DrawRay(new Vector3(1, 1, z));
    }

    // -------------------------------------------------------------------------------------------
    void DrawRay(Vector3 vViewPortPoint)
    {
        if (this.ObjectControlled == null)
            return;

        var vCamRightDir = this.GetComponent<Camera>().transform.right;
        var vCamUpDir = this.GetComponent<Camera>().transform.up;
        Vector3 vTargetPoint = this.ObjectControlled.transform.position - vCamRightDir * 0.5f * Scale - vCamUpDir * 0.5f * Scale;
        vTargetPoint += vCamRightDir * (vViewPortPoint.x * Scale);
        vTargetPoint += vCamUpDir * (vViewPortPoint.y * Scale);

        Vector3 vWorldPoint = GetComponent<Camera>().ViewportToWorldPoint(vViewPortPoint);
        var vDistance = vTargetPoint - vWorldPoint;

        var vTarget = vWorldPoint + vDistance.normalized * vDistance.magnitude;
        Gizmos.DrawLine(vWorldPoint, vTarget);
    }

    // -------------------------------------------------------------------------------------------
	void LateUpdate () 
	{
		if (WorldStateManager.GamePaused)
			return;

        // position and rotate
        if (this.ObjectControlled != null)
        {
            this.transform.rotation = this.ObjectControlled.Orientation * this.OffsetRotation;
            this.transform.position = this.ObjectControlled.transform.position - ((this.transform.rotation * Vector3.forward) * this.Distance);
        }
	}

    // -------------------------------------------------------------------------------------------
    static float ClampAngle(float fAngle, float fMin, float fMax)
    {
        while (fAngle < -180)
            fAngle += 360;

        while (fAngle > 180)
            fAngle -= 360;

        return Mathf.Clamp(fAngle, fMin, fMax);
    }

    // -------------------------------------------------------------------------------------------
    public void Jump()
    {
        // Detailed logging for debugging physics interference (Task 0B.6)
        Debug.Log($"PlayerCameraController.Jump: Object={this.name}, PhysicsMode={(physicsSettings != null ? physicsSettings.physicsMode.ToString() : "Unknown")}, Rigidbody.IsKinematic={this.ObjectControlled.GetComponent<Rigidbody>().isKinematic}, Rigidbody.UseGravity={this.ObjectControlled.GetComponent<Rigidbody>().useGravity}, Velocity={this.ObjectControlled.GetComponent<Rigidbody>().velocity.ToString("F3")}");
        
        // Use jump force from PhysicsSettings if available, otherwise default to original value
        float jumpForce = physicsSettings != null ? physicsSettings.legacyJumpForce * (physicsSettings.physicsMode == PhysicsMode.UnityPhysics ? 1.5f : 1.0f) : 5.0f; // Increase jump force by 50% in UnityPhysics mode
        
        // Check physics mode to decide whether to apply jump force
        if (physicsSettings != null && physicsSettings.physicsMode == PhysicsMode.CustomPhysics)
        {
            Debug.Log($"PlayerCameraController.Jump: Skipped applying jump force due to PhysicsMode=CustomPhysics");
            return; // Skip applying forces in CustomPhysics mode
        }
        
        Vector3 jumpVector = -this.ObjectControlled.GravityDirection * jumpForce;
        this.ObjectControlled.GetComponent<Rigidbody>().velocity = jumpVector;
        Debug.Log($"PlayerCameraController.Jump: Applied Jump Force={jumpForce}, Direction={jumpVector.ToString("F3")}, New Velocity={this.ObjectControlled.GetComponent<Rigidbody>().velocity.ToString("F3")}, PhysicsMode={(physicsSettings != null ? physicsSettings.physicsMode.ToString() : "Unknown")}");
    }
	
	// -------------------------------------------------------------------------------------------
	public void Move(MOVEMENT_TYPE eType)
	{
        // Detailed logging for debugging physics interference (Task 0B.6)
        Debug.Log($"PlayerCameraController.Move: Object={this.name}, PhysicsMode={(physicsSettings != null ? physicsSettings.physicsMode.ToString() : "Unknown")}, Rigidbody.IsKinematic={this.ObjectControlled.GetComponent<Rigidbody>().isKinematic}, Rigidbody.UseGravity={this.ObjectControlled.GetComponent<Rigidbody>().useGravity}, Velocity={this.ObjectControlled.GetComponent<Rigidbody>().velocity.ToString("F3")}");
        
        if (this.ObjectControlled == null)
            return;

        if (this.CanOnlyConrolledIfGrounded && this.ObjectControlled.HasGroundContact() == false)
            return;

        // Check physics mode before applying movement forces
        if (physicsSettings != null && physicsSettings.physicsMode == PhysicsMode.CustomPhysics)
        {
            Debug.Log($"PlayerCameraController.Move: Skipped applying movement forces due to PhysicsMode=CustomPhysics");
            return;
        }

        // Log decision to apply movement forces
        Debug.Log($"PlayerCameraController.Move: Applying movement forces for {this.name} in PhysicsMode={(physicsSettings != null ? physicsSettings.physicsMode.ToString() : "Default(UnityPhysics)").ToString()}");
        
        var vForward = this.ObjectControlled.ForwardDirection;
        var vRight = this.ObjectControlled.RightDirection;
        var pRigidBody = this.ObjectControlled.GetComponent<Rigidbody>();
		
		switch(eType)
		{
		case MOVEMENT_TYPE.BREAK:
			if (pRigidBody.velocity.magnitude < 0.1f)
				pRigidBody.velocity = Vector3.zero;
			else
				pRigidBody.AddForce(pRigidBody.velocity.normalized * -this.BreakFactor);
			break;
		case MOVEMENT_TYPE.FORWARD:
			pRigidBody.AddTorque(vRight * this.SpeedFactor);
			pRigidBody.AddForce(vForward*8.0f * this.SpeedFactor);
			break;
		case MOVEMENT_TYPE.LEFT:
			pRigidBody.AddTorque(vForward * this.SpeedFactor);
			pRigidBody.AddForce(-vRight*8.0f * this.SpeedFactor);
			break;
		case MOVEMENT_TYPE.RIGHT:
			pRigidBody.AddTorque(-vForward * this.SpeedFactor);
			pRigidBody.AddForce(vRight*8.0f * this.SpeedFactor);
			break;
		case MOVEMENT_TYPE.BACKWARD:
			pRigidBody.AddTorque(-vRight * this.SpeedFactor);
			pRigidBody.AddForce(-vForward*3.0f * this.SpeedFactor);
			break;
		}
        Debug.Log($"PlayerCameraController.Move: Applied Force={pRigidBody.velocity.ToString("F3")} and Torque={pRigidBody.angularVelocity.ToString("F3")} to {this.name}, New Velocity={pRigidBody.velocity.ToString("F3")}");
        UnityEngine.Debug.Log("Applied movement forces for " + eType.ToString() + " in " + (physicsSettings != null ? physicsSettings.physicsMode.ToString() : "default") + " mode.");
    }

    // -------------------------------------------------------------------------------------------
}
