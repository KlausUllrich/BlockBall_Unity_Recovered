// File: Assets/Scripts/Physics/PhysicsObjectWrapper.cs
// Purpose: Wrapper for existing Player script to maintain compatibility during physics migration
using UnityEngine;
using BlockBall.Physics;

public class PhysicsObjectWrapper : MonoBehaviour, IPhysicsObject
{
    private Player playerScript;
    private Rigidbody rigidBody;
    
    // Placeholder state for cases where Player doesn't have state management
    private PhysicsObjectState currentState = PhysicsObjectState.Airborne;
    
    // Reference to PhysicsSettings for mode selection
    private BlockBall.Settings.PhysicsSettings physicsSettings;
    private PhysicsMode physicsMode = PhysicsMode.UnityPhysics; // Default until settings are loaded
    
    void Awake()
    {
        playerScript = GetComponent<Player>();
        rigidBody = GetComponent<Rigidbody>();
        
        if (playerScript == null)
            throw new System.Exception("PhysicsObjectWrapper requires Player component");
        
        // Load PhysicsSettings (assuming it's a ScriptableObject asset)
        try
        {
            physicsSettings = Resources.Load<BlockBall.Settings.PhysicsSettings>("PhysicsSettings");
            if (physicsSettings == null)
            {
                // Try loading from Assets/Settings/ directly if not in Resources
                physicsSettings = UnityEditor.AssetDatabase.LoadAssetAtPath<BlockBall.Settings.PhysicsSettings>("Assets/Settings/PhysicsSettings.asset");
                if (physicsSettings == null)
                {
                    Debug.LogError("PhysicsSettings asset not found at Assets/Settings/PhysicsSettings.asset. Create it via Assets > Create > BlockBall > Physics Settings, then save it as 'PhysicsSettings.asset' in Assets/Settings folder.");
                    // Fallback to default settings if not found
                    physicsSettings = ScriptableObject.CreateInstance<BlockBall.Settings.PhysicsSettings>();
                    Debug.LogWarning("Using default PhysicsSettings instance as fallback.");
                }
                else
                {
                    physicsMode = physicsSettings.physicsMode;
                    Debug.Log("PhysicsSettings loaded successfully from Assets/Settings/. Mode: " + physicsMode.ToString());
                }
            }
            else
            {
                physicsMode = physicsSettings.physicsMode;
                Debug.Log("PhysicsSettings loaded successfully from Resources. Mode: " + physicsMode.ToString());
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading PhysicsSettings: " + e.Message);
            // Fallback to default settings if loading fails
            physicsSettings = ScriptableObject.CreateInstance<BlockBall.Settings.PhysicsSettings>();
            Debug.LogWarning("Using default PhysicsSettings instance as fallback due to loading error.");
        }
        
        // Initialize state based on initial conditions if possible
        if (rigidBody != null && Mathf.Abs(rigidBody.velocity.y) < 0.01f)
            currentState = PhysicsObjectState.Grounded;
    }
    
    void FixedUpdate()
    {
        // Ensure IntegrateVelocityVerlet is called during physics updates
        IntegrateVelocityVerlet(Time.fixedDeltaTime);
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
        set { if (rigidBody != null) rigidBody.velocity = DeterministicMath.RoundVector(value); } 
    }
    
    public Vector3 AngularVelocity 
    { 
        get => rigidBody != null ? rigidBody.angularVelocity : Vector3.zero; 
        set { if (rigidBody != null) rigidBody.angularVelocity = DeterministicMath.RoundVector(value); } 
    }
    
    public float Mass 
    { 
        get => rigidBody != null ? rigidBody.mass : 1.0f; 
        set { if (rigidBody != null) rigidBody.mass = DeterministicMath.Round(value); } 
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
            try { var method = playerScript.GetType().GetMethod("SetGravityDirection"); if (method != null) method.Invoke(playerScript, new object[] { DeterministicMath.Normalize(value) }); } 
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
        // Use deterministic delta time for physics integration
        float dt = DeterministicMath.DeltaTime;
        
        // Enhanced logging to confirm method is called (Task 0B.6)
        Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Object={name}, DeltaTime={dt.ToString("F4")}, PhysicsMode={physicsMode.ToString()}");
        
        // Log the current physics mode for debugging
        Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Current Physics Mode for {name}: {physicsMode.ToString()}");
        
        // Log PhysicsSettings values to confirm they are accessed correctly
        if (physicsSettings != null)
        {
            Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: PhysicsSettings Values for {name} - Legacy Gravity: {physicsSettings.legacyGravity}, Physics Mode: {physicsSettings.physicsMode}");
        }
        else
        {
            Debug.LogWarning($"PhysicsObjectWrapper.IntegrateVelocityVerlet: PhysicsSettings is null for {name}, using fallback or default values.");
        }
        
        // Choose physics approach based on mode from PhysicsSettings
        switch (physicsMode)
        {
            case PhysicsMode.UnityPhysics:
                // Delegate to Unity physics or Player script
                if (rigidBody != null && !rigidBody.useGravity)
                {
                    rigidBody.useGravity = true; // Allow Unity physics to apply gravity
                    Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: UnityPhysics mode for {name}: Set Rigidbody.useGravity to true for Unity physics. Rigidbody State - IsKinematic: {rigidBody.isKinematic}, Constraints: {rigidBody.constraints}");
                }
                try { var method = playerScript.GetType().GetMethod("UpdatePhysics"); if (method != null) { method.Invoke(playerScript, new object[] { dt }); Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Invoked UpdatePhysics method from Player script in UnityPhysics mode for {name}."); } } 
                catch { Debug.LogWarning($"PhysicsObjectWrapper.IntegrateVelocityVerlet: UnityPhysics mode for {name}: No UpdatePhysics method found, letting Unity handle physics."); /* No action, let Unity handle physics */ }
                break;
            
            case PhysicsMode.CustomPhysics:
                // Custom physics integration using DeterministicMath
                if (rigidBody != null)
                {
                    // Log Rigidbody state for debugging
                    Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: CustomPhysics mode for {name}: Rigidbody - IsKinematic: {rigidBody.isKinematic}, UseGravity: {rigidBody.useGravity}, Constraints: {rigidBody.constraints}, Pre-update velocity: {rigidBody.velocity.ToString("F3")}");
                    
                    // Disable Unity gravity to apply custom gravity
                    if (rigidBody.useGravity)
                    {
                        rigidBody.useGravity = false;
                        Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: CustomPhysics mode for {name}: Set Rigidbody.useGravity to false to apply custom physics.");
                    }
                    
                    // Use gravity value from PhysicsSettings if available, otherwise default to Unity's gravity
                    float gravityValue = physicsSettings != null ? physicsSettings.legacyGravity : -9.81f;
                    // Adjust gravity multiplier based on mode for behavioral distinction (Task 0B.4)
                    float gravityMultiplier = 2.0f; // Increased from 1.5x to 2.0x for more noticeable falling speed
                    float effectiveGravity = gravityValue * gravityMultiplier;
                    Vector3 gravity = Vector3.up * effectiveGravity;
                    Vector3 newVelocity = DeterministicMath.Add(rigidBody.velocity, DeterministicMath.Multiply(gravity, dt));
                    
                    // Apply custom speed control for Phase 0C
                    float speedLimit = physicsSettings != null ? physicsSettings.physicsSpeedLimit : 6.5f;
                    if (newVelocity.magnitude > speedLimit)
                    {
                        newVelocity = newVelocity.normalized * speedLimit;
                        Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: CustomPhysics mode for {name}: Applied speed limit of {speedLimit}. Velocity capped from {rigidBody.velocity.magnitude.ToString("F3")} to {newVelocity.magnitude.ToString("F3")}");
                    }
                    
                    rigidBody.velocity = DeterministicMath.RoundVector(newVelocity);
                    
                    // Log updated velocity for debugging
                    Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: CustomPhysics mode for {name}: Applied custom gravity with 2.0x multiplier. Post-update velocity: {rigidBody.velocity.ToString("F3")}");
                }
                else
                {
                    Debug.LogWarning($"PhysicsObjectWrapper.IntegrateVelocityVerlet: CustomPhysics mode for {name}: No Rigidbody found.");
                }
                break;
            
            case PhysicsMode.Hybrid:
                // Hybrid mode: Attempt Player script update, fallback to custom with Unity as base
                bool playerScriptUpdated = false;
                try { 
                    var method = playerScript.GetType().GetMethod("UpdatePhysics"); 
                    if (method != null) { 
                        method.Invoke(playerScript, new object[] { dt }); 
                        Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Hybrid mode for {name}: Invoked UpdatePhysics method from Player script for input forces."); 
                        playerScriptUpdated = true;
                    } 
                } 
                catch { 
                    Debug.LogWarning($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Hybrid mode for {name}: No UpdatePhysics method found, skipping Player script update.");
                }
                
                if (rigidBody != null)
                {
                    // Log Rigidbody state for debugging
                    Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Hybrid mode for {name}: Rigidbody - IsKinematic: {rigidBody.isKinematic}, UseGravity: {rigidBody.useGravity}, Constraints: {rigidBody.constraints}, Pre-update velocity: {rigidBody.velocity.ToString("F3")}");
                    
                    // Disable Unity gravity to apply custom gravity
                    if (rigidBody.useGravity)
                    {
                        rigidBody.useGravity = false;
                        Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Hybrid mode for {name}: Set Rigidbody.useGravity to false to apply custom physics.");
                    }
                    
                    // Use gravity value from PhysicsSettings if available, otherwise default to Unity's gravity
                    float hybridGravityValue = physicsSettings != null ? physicsSettings.legacyGravity : -9.81f;
                    // Adjust gravity multiplier based on mode for behavioral distinction (Task 0B.4)
                    float hybridGravityMultiplier = 1.0f; 
                    float hybridEffectiveGravity = hybridGravityValue * hybridGravityMultiplier;
                    Vector3 hybridGravity = Vector3.up * hybridEffectiveGravity;
                    Vector3 hybridNewVelocity = DeterministicMath.Add(rigidBody.velocity, DeterministicMath.Multiply(hybridGravity, dt));
                    
                    // Apply custom speed control for Phase 0C, considering input from old system
                    float hybridSpeedLimit = physicsSettings != null ? physicsSettings.totalSpeedLimit : 7.0f;
                    if (hybridNewVelocity.magnitude > hybridSpeedLimit)
                    {
                        hybridNewVelocity = hybridNewVelocity.normalized * hybridSpeedLimit;
                        Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Hybrid mode for {name}: Applied total speed limit of {hybridSpeedLimit}. Velocity capped from {rigidBody.velocity.magnitude.ToString("F3")} to {hybridNewVelocity.magnitude.ToString("F3")}");
                    }
                    
                    rigidBody.velocity = DeterministicMath.RoundVector(hybridNewVelocity);
                    
                    // Log updated velocity for debugging
                    Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Hybrid mode for {name}: Applied custom gravity with 0.5x multiplier to balance with old system. Post-update velocity: {rigidBody.velocity.ToString("F3")}");
                }
                else
                {
                    Debug.LogWarning($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Hybrid mode for {name}: No Rigidbody found.");
                }
                break;
            
            default:
                Debug.LogWarning($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Unknown PhysicsMode for {name}, defaulting to UnityPhysics");
                if (rigidBody != null && !rigidBody.useGravity)
                {
                    rigidBody.useGravity = true; // Allow Unity physics to apply gravity
                    Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Default case for {name}: Set Rigidbody.useGravity to true for Unity physics. Rigidbody State - IsKinematic: {rigidBody.isKinematic}, Constraints: {rigidBody.constraints}");
                }
                try { var method = playerScript.GetType().GetMethod("UpdatePhysics"); if (method != null) { method.Invoke(playerScript, new object[] { dt }); Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Default case for {name}: Invoked UpdatePhysics method from Player script."); } } 
                catch { Debug.LogWarning($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Default case for {name}: No UpdatePhysics method found, letting Unity handle physics."); /* No action, let Unity handle physics */ }
                break;
        }
    }
    
    public void ApplyForce(Vector3 force, ForceMode mode = ForceMode.Force)
    {
        if (rigidBody != null)
            rigidBody.AddForce(DeterministicMath.RoundVector(force), mode);
    }
    
    public void ApplyTorque(Vector3 torque, ForceMode mode = ForceMode.Force)
    {
        if (rigidBody != null)
            rigidBody.AddTorque(DeterministicMath.RoundVector(torque), mode);
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
    
    void OnDrawGizmos()
    {
        // Draw visual indicators for the active physics mode
        switch (physicsMode)
        {
            case PhysicsMode.UnityPhysics:
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
                Gizmos.DrawLine(transform.position, transform.position + Vector3.right * 2f);
                Gizmos.DrawLine(transform.position, transform.position + Vector3.forward * 2f);
                break;
            case PhysicsMode.CustomPhysics:
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
                Gizmos.DrawLine(transform.position, transform.position + Vector3.right * 2f);
                Gizmos.DrawLine(transform.position, transform.position + Vector3.forward * 2f);
                break;
            case PhysicsMode.Hybrid:
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
                Gizmos.DrawLine(transform.position, transform.position + Vector3.right * 2f);
                Gizmos.DrawLine(transform.position, transform.position + Vector3.forward * 2f);
                break;
        }
    }
}
