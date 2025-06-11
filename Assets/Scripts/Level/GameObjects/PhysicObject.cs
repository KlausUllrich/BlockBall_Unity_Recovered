using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BlockBall.Settings;
using BlockBall.Physics;

[RequireComponent(typeof(Rigidbody))]
public class PhysicObjekt : GrafikObjekt 
{
    //-----------------------------------------------------------------------------------------------------------------
    public const float DefaultGravityStrength = 9.81f;
    public static Vector3 DefaultGravityDirection = Vector3.down;
    public static Vector3 DefaultForwardDirection = Vector3.forward;

    //-----------------------------------------------------------------------------------------------------------------
    public float GravityStrength { get { return this.fGravityStrength; } }
    public Vector3 GravityDirection { get { return this.vGravityDirection; } }
    public Vector3 Gravity { get { return this.vGravity; } }
    public Vector3 ForwardDirection { get { return this.vForwardDirection; } }
    public Vector3 RightDirection { get { return this.vRightDirection; } }
    public Quaternion Orientation { get { return this.qOrientation; } }

    //-----------------------------------------------------------------------------------------------------------------
    protected float fGravityStrength = DefaultGravityStrength;
    protected Vector3 vGravityDirection = DefaultGravityDirection;
    protected Vector3 vGravity = new Vector3(0, DefaultGravityStrength, 0);
    protected Vector3 vLastGravityDirection = DefaultGravityDirection;
    protected Vector3 vForwardDirection = DefaultForwardDirection;
    protected Vector3 vRightDirection = Vector3.Cross(DefaultForwardDirection, DefaultGravityDirection);
    protected Quaternion qOrientation = Quaternion.identity;

    protected List<GameObject> axGroundContactGameObjects = new List<GameObject>();

    // TEST: Add option to use Unity's built-in gravity instead of custom gravity system
    public bool UseUnityGravity = false;  // Set to true in Inspector to test Unity gravity
    
    // Reference to PhysicsSettings for mode checking
    private PhysicsSettings physicsSettings;
    private PhysicsMode physicsMode;

    protected Rigidbody rb;

    private bool wasGroundedLastFrame = false;

    //-----------------------------------------------------------------------------------------------------------------
    public virtual void SetGravityDirection(Vector3 vDirection)
    {
        this.vLastGravityDirection = this.vGravityDirection;
        this.vGravityDirection = vDirection.normalized;
        this.SetGravityStrength(this.fGravityStrength);

        // adjust forward vector
        var qRotTo = Quaternion.FromToRotation(this.vLastGravityDirection, this.vGravityDirection);
        this.vForwardDirection = qRotTo * this.vForwardDirection;

        this.UpdateBufferedValues();
    }

    //-----------------------------------------------------------------------------------------------------------------
    public virtual void SetGravityStrength(float fStrength)
    {
        this.fGravityStrength = fStrength;
        this.vGravity = this.vGravityDirection * this.fGravityStrength;
    }

    //-----------------------------------------------------------------------------------------------------------------
    public void ClipGravity()
    {
        if (this.vGravityDirection.sqrMagnitude == (1 * 1))
            return;

        var vNewDir = this.vGravityDirection;
        float absX = Mathf.Abs(vNewDir.x);
        float absY = Mathf.Abs(vNewDir.y);
        float absZ = Mathf.Abs(vNewDir.z);

        if (absX >= absY)
        {
            if (absX >= absZ)
                if (vNewDir.x >= 0)
                    vNewDir = new Vector3(1, 0, 0);
                else
                    vNewDir = new Vector3(-1, 0, 0);
            else
                if (vNewDir.z >= 0)
                    vNewDir = new Vector3(0, 0, 1);
                else
                    vNewDir = new Vector3(0, 0, -1);
        }
        else
        {
            if (absY >= absZ)
                if (vNewDir.y >= 0)
                    vNewDir = new Vector3(0, 1, 0);
                else
                    vNewDir = new Vector3(0, -1, 0);
            else
                if (vNewDir.z >= 0)
                    vNewDir = new Vector3(0, 0, 1);
                else
                    vNewDir = new Vector3(0, 0, -1);
        }
        this.SetGravityDirection(vNewDir);
    }

    //-----------------------------------------------------------------------------------------------------------------
    public void SetForwardDirection(Vector3 vDirection)
    {
        this.vForwardDirection = vDirection;
        this.UpdateBufferedValues();
    }

    //-----------------------------------------------------------------------------------------------------------------
    void UpdateBufferedValues()
    {
        this.vRightDirection = Vector3.Cross(this.vForwardDirection, this.vGravityDirection);
        this.vForwardDirection = Vector3.Cross(this.vGravityDirection, this.vRightDirection);
        
        this.qOrientation = Quaternion.LookRotation(this.vForwardDirection, -this.vGravityDirection);
    }

    //-----------------------------------------------------------------------------------------------------------------
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Runtime loading of PhysicsSettings (Task 0B.1)
        physicsSettings = Resources.Load<PhysicsSettings>("PhysicsSettings");
        if (physicsSettings == null)
        {
            Debug.LogWarning("PhysicsSettings asset not found. Defaulting to original behavior. Ensure PhysicsSettings.asset is in Resources folder.");
            physicsMode = PhysicsMode.UnityPhysics; 
        }
        else
        {
            Debug.Log("PhysicsSettings loaded successfully in PhysicObject from Resources. Mode: " + physicsSettings.physicsMode);
            physicsMode = physicsSettings.physicsMode;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    protected virtual void FixedUpdate()
    {
        if (WorldStateManager.GamePaused)
            return;

        // Detailed logging for debugging physics interference (Task 0B.6)
        bool currentGroundContact = HasGroundContact();
        Debug.Log($"PhysicObject.FixedUpdate: Object={this.name}, PhysicsMode={(physicsSettings != null ? physicsSettings.physicsMode.ToString() : "Unknown")}, Rigidbody.IsKinematic={rb.isKinematic}, Rigidbody.UseGravity={rb.useGravity}, Velocity={rb.velocity.ToString("F3")}, GroundContact={currentGroundContact}, GroundContactCount={axGroundContactGameObjects.Count}");
        
        // Check for transition in ground contact state for jump buffering debugging
        if (currentGroundContact != wasGroundedLastFrame)
        {
            Debug.Log($"PhysicObject.FixedUpdate: Ground contact state changed for {this.name}. New state: {(currentGroundContact ? "Grounded" : "Airborne")}, GroundContactCount={axGroundContactGameObjects.Count}");
        }
        wasGroundedLastFrame = currentGroundContact;
        
        // Apply custom gravity logic if not using Unity physics
        if (rb.useGravity == false)
        {
            // Check physics mode to decide whether to apply custom gravity
            if (physicsSettings != null && (physicsSettings.physicsMode == PhysicsMode.CustomPhysics || physicsSettings.physicsMode == PhysicsMode.Hybrid))
            {
                Debug.Log($"PhysicObject.FixedUpdate: Skipped applying custom gravity forces due to PhysicsMode={physicsSettings.physicsMode.ToString()}");
                return; // Skip applying forces in CustomPhysics or Hybrid mode
            }

            if (HasGroundContact() == false)
            {
                Vector3 gravityForce = GravityDirection * fGravityStrength * rb.mass;
                rb.AddForce(gravityForce, ForceMode.Force);
                Debug.Log($"PhysicObject.FixedUpdate: Applied custom gravity force={gravityForce.ToString("F3")} to {this.name}, New Velocity={rb.velocity.ToString("F3")}");
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    void Start()
    {
        // Configure gravity system based on UseUnityGravity setting
        var rigidbody = this.GetComponent<Rigidbody>();
        
        if (UseUnityGravity)
        {
            // Use Unity's built-in gravity system
            rigidbody.useGravity = true;
            // Set Unity's global gravity to match our custom gravity direction and strength
            Physics.gravity = this.vGravityDirection * this.fGravityStrength;
            UnityEngine.Debug.Log($"Using Unity gravity: {Physics.gravity}");
        }
        else
        {
            // Use custom gravity system (original behavior)
            rigidbody.useGravity = false;
            UnityEngine.Debug.Log($"Using custom gravity: {this.Gravity}");
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    void OnCollisionEnter(Collision pCollision)
    {
        CalcGroundCollision(pCollision);
    }

    //-----------------------------------------------------------------------------------------------------------------
    void OnCollisionStay(Collision pCollision)
    {
        CalcGroundCollision(pCollision);
    }

    //-----------------------------------------------------------------------------------------------------------------
    void CalcGroundCollision(Collision pCollision)
    {
        var pBaseObject = pCollision.gameObject.GetComponent(typeof(GrafikObjekt));
        if (pBaseObject != null)
        {
            foreach (var contact in pCollision.contacts)
            {
                var vectorToCollition = contact.normal;

                var fDot = Vector3.Dot(vectorToCollition.normalized, this.GravityDirection);
                if ((-fDot) > Definitions.BallTouchingTheGroundThresholdAsDotProductResult)
                {
                    this.OnGroundContact(pCollision.gameObject, true);
                    // if a contact was found we can return 
                    return;
                }
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    void OnCollisionExit(Collision pCollision)
    {
        var pBaseObject = pCollision.gameObject.GetComponent(typeof(GrafikObjekt));
        if (pBaseObject != null)
        {
            this.OnGroundContact(pCollision.gameObject, false);
        }
    }

    // -------------------------------------------------------------------------------------------
    protected virtual void OnGroundContact(GameObject go, bool value)
    {
        if (value)
        {
            if (!axGroundContactGameObjects.Contains(go))
                axGroundContactGameObjects.Add(go);
        }
        else
        {
            if (axGroundContactGameObjects.Contains(go))
                axGroundContactGameObjects.Remove(go);
        }
    }

    // -------------------------------------------------------------------------------------------
    public bool HasGroundContact()
    {
        return (axGroundContactGameObjects.Count > 0);
    }

    //-----------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.position + (this.GravityDirection * 20));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(this.transform.position, this.transform.position + (this.ForwardDirection * 20));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.transform.position, this.transform.position + (this.RightDirection * 20));
    }
#endif

    //-----------------------------------------------------------------------------------------------------------------
}
