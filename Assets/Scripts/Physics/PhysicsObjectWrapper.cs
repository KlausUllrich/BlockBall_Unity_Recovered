// File: Assets/Scripts/Physics/PhysicsObjectWrapper.cs
// Purpose: Wrapper for existing Player script to maintain compatibility during physics migration
using UnityEngine;

public class PhysicsObjectWrapper : MonoBehaviour, IPhysicsObject
{
    private Player playerScript;
    private Rigidbody rigidBody;
    
    // Placeholder state for cases where Player doesn't have state management
    private PhysicsObjectState currentState = PhysicsObjectState.Airborne;
    
    void Awake()
    {
        playerScript = GetComponent<Player>();
        rigidBody = GetComponent<Rigidbody>();
        
        if (playerScript == null)
            throw new System.Exception("PhysicsObjectWrapper requires Player component");
        
        // Initialize state based on initial conditions if possible
        if (rigidBody != null && Mathf.Abs(rigidBody.velocity.y) < 0.01f)
            currentState = PhysicsObjectState.Grounded;
    }
    
    // Implement interface by delegating to existing Player script where possible
    public Vector3 Position 
    { 
        get => transform.position; 
        set => transform.position = value; 
    }
    
    public Vector3 Velocity 
    { 
        get => rigidBody != null ? rigidBody.velocity : Vector3.zero; 
        set { if (rigidBody != null) rigidBody.velocity = value; } 
    }
    
    public Vector3 AngularVelocity 
    { 
        get => rigidBody != null ? rigidBody.angularVelocity : Vector3.zero; 
        set { if (rigidBody != null) rigidBody.angularVelocity = value; } 
    }
    
    public float Mass 
    { 
        get => rigidBody != null ? rigidBody.mass : 1.0f; 
        set { if (rigidBody != null) rigidBody.mass = value; } 
    }
    
    public Vector3 GravityDirection 
    { 
        get 
        { 
            // Check for a method or property that might exist
            try { return playerScript.GetType().GetProperty("GravityDirection") != null ? (Vector3)playerScript.GetType().GetProperty("GravityDirection").GetValue(playerScript) : Physics.gravity.normalized; } 
            catch { return Physics.gravity.normalized; } 
        }
        set 
        { 
            // Check for a setter method that might exist
            try { var method = playerScript.GetType().GetMethod("SetGravityDirection"); if (method != null) method.Invoke(playerScript, new object[] { value }); } 
            catch { /* Placeholder for when method doesn't exist */ } 
        }
    }
    
    public bool IsGrounded
    { 
        get 
        { 
            // Check if method or property exists via reflection to avoid compile errors
            try { var prop = playerScript.GetType().GetProperty("IsGrounded"); if (prop != null) return (bool)prop.GetValue(playerScript); } 
            catch { /* Fall back to state or simple check */ } 
            return currentState == PhysicsObjectState.Grounded; 
        } 
    }
    
    public bool HasGroundContact()
    {
        // Check if method exists via reflection
        try { var method = playerScript.GetType().GetMethod("HasGroundContact"); if (method != null) return (bool)method.Invoke(playerScript, null); } 
        catch { /* Fall back to state or simple check */ } 
        return currentState == PhysicsObjectState.Grounded; 
    }
    
    public PhysicsObjectState CurrentState 
    { 
        get 
        { 
            // Check if property exists via reflection
            try { var prop = playerScript.GetType().GetProperty("CurrentState"); if (prop != null) return (PhysicsObjectState)prop.GetValue(playerScript); } 
            catch { /* Use our own state tracking */ } 
            return currentState; 
        } 
    }
    
    public void SetState(PhysicsObjectState state)
    {
        // Check if method exists via reflection
        try { var method = playerScript.GetType().GetMethod("SetState"); if (method != null) method.Invoke(playerScript, new object[] { (int)state }); } 
        catch { /* Track state ourselves if method doesn't exist */ currentState = state; } 
    }
    
    public void IntegrateVelocityVerlet(float deltaTime)
    {
        // Placeholder for custom physics integration
        // Initially delegates to Unity physics or does nothing if method doesn't exist
        try { var method = playerScript.GetType().GetMethod("UpdatePhysics"); if (method != null) method.Invoke(playerScript, new object[] { deltaTime }); } 
        catch { /* No action if method doesn't exist, Unity handles physics */ } 
    }
    
    public void ApplyForce(Vector3 force, ForceMode mode = ForceMode.Force)
    {
        if (rigidBody != null)
            rigidBody.AddForce(force, mode);
    }
    
    public void ApplyTorque(Vector3 torque, ForceMode mode = ForceMode.Force)
    {
        if (rigidBody != null)
            rigidBody.AddTorque(torque, mode);
    }
    
    public void OnPhysicsCollision(Collision collision)
    {
        // Check if method exists via reflection
        try { var method = playerScript.GetType().GetMethod("OnCollision"); if (method != null) method.Invoke(playerScript, new object[] { collision }); } 
        catch { /* No action if method doesn't exist */ } 
    }
    
    public void OnPhysicsTrigger(Collider other, bool isEnter)
    {
        try 
        { 
            if (isEnter) 
            { 
                var enterMethod = playerScript.GetType().GetMethod("OnTriggerEnter"); 
                if (enterMethod != null) enterMethod.Invoke(playerScript, new object[] { other }); 
            } 
            else 
            { 
                var exitMethod = playerScript.GetType().GetMethod("OnTriggerExit"); 
                if (exitMethod != null) exitMethod.Invoke(playerScript, new object[] { other }); 
            } 
        } 
        catch { /* No action if methods don't exist */ } 
    }
}
