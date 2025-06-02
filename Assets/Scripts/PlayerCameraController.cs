using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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

    // -------------------------------------------------------------------------------------------
	private TimeSpan xJumpTime = TimeSpan.Zero;
	private HashSet<Block> pSeeThroughList = new HashSet<Block>();
	private HashSet<Block> pNewSeeThroughList = new HashSet<Block>();

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
		
		xJumpTime -= TimeSpan.FromSeconds(Time.fixedDeltaTime);

        if (this.ObjectControlled != null)
        {
			if (Input.GetKeyDown(KeyCode.Space))
            {
                if (this.ObjectControlled.HasGroundContact())
                {
                    this.Jump();
                }
                else
                {
                    xJumpTime = new TimeSpan(0, 0, 0, 0, MilisecondsBeforeGroundContractIsValidAsJump);
                }
            }

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
    void FixedUpdate () 
	{
		if (WorldStateManager.GamePaused)
			return;

        if (this.ObjectControlled == null)
            return;
		
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
        if (this.ObjectControlled == null)
            return;

        // Calc the similarty of the current velecity and the intended jump vector
        var vUp = -this.ObjectControlled.GravityDirection;
        var vJump = vUp * this.JumpForce;
        float fScalar = Vector3.Dot(vJump, this.ObjectControlled.GetComponent<Rigidbody>().velocity);
        float fSimilarty = fScalar / vJump.magnitude;
        // add the difference between intended and actual 
        var vDifference2Intended = vUp * (vJump.magnitude - fSimilarty);
        this.ObjectControlled.GetComponent<Rigidbody>().velocity += vDifference2Intended;
        // null jump time
        this.xJumpTime = TimeSpan.Zero;
    }
	
	// -------------------------------------------------------------------------------------------
	public void Move(MOVEMENT_TYPE eType)
	{
        if (this.ObjectControlled == null)
            return;

        if (this.CanOnlyConrolledIfGrounded && this.ObjectControlled.HasGroundContact() == false)
            return;

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
	}

    // -------------------------------------------------------------------------------------------
}
