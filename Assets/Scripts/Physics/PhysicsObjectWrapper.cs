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
    
    public void IntegrateVelocityVerlet(float dt)
    {
        if (rigidBody == null) rigidBody = GetComponent<Rigidbody>();
        if (physicsSettings == null)
        {
            // Load settings at runtime if not already loaded
            physicsSettings = Resources.Load<BlockBall.Settings.PhysicsSettings>("PhysicsSettings");
            if (physicsSettings != null)
            {
                physicsMode = physicsSettings.physicsMode;
                if (physicsSettings.enableMigrationLogging)
                    Debug.Log($"PhysicsObjectWrapper.IntegrateVelocityVerlet: Loaded PhysicsSettings at runtime. Mode: {physicsMode}");
            }
            else
            {
                if (physicsSettings.enableMigrationLogging)
                    Debug.LogWarning($"PhysicsObjectWrapper.IntegrateVelocityVerlet: PhysicsSettings asset not found. Defaulting to UnityPhysics mode.");
                physicsMode = PhysicsMode.UnityPhysics;
            }
        }

        // Apply different physics logic based on mode
        switch (physicsMode)
        {
            case PhysicsMode.CustomPhysics:
                // Custom physics mode: Cap velocity at physicsSpeedLimit
                float customSpeedLimit = physicsSettings != null ? physicsSettings.physicsSpeedLimit : 2.0f;
                Vector3 customVelocity = rigidBody.velocity;
                // Implement basic movement forces in CustomPhysics mode
                if (this.gameObject.name.Contains("Player") || this.gameObject.name.Contains("Sphere")) // Assuming player object
                {
                    float forwardForce = physicsSettings != null ? physicsSettings.forwardForceMagnitude : 8.0f;
                    float sidewaysForce = physicsSettings != null ? physicsSettings.sidewaysForceMagnitude : 8.0f;
                    float backwardForce = physicsSettings != null ? physicsSettings.backwardForceMagnitude : 3.0f;
                    float inputScale = physicsSettings != null ? physicsSettings.inputForceScale : 1.0f;
                    Vector3 movementForce = Vector3.zero;
                    if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                        movementForce += Vector3.forward * forwardForce * inputScale;
                    if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                        movementForce += Vector3.back * backwardForce * inputScale;
                    if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                        movementForce += Vector3.right * sidewaysForce * inputScale;
                    if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                        movementForce += Vector3.left * sidewaysForce * inputScale;
                    if (movementForce != Vector3.zero)
                    {
                        rigidBody.AddForce(movementForce, ForceMode.Force);
                        if (physicsSettings != null && physicsSettings.enableMigrationLogging)
                            Debug.Log($"CustomPhysics mode for {this.name}: Applied movement force {movementForce.magnitude}");
                    }
                }
                
                if (customVelocity.magnitude > customSpeedLimit)
                {
                    customVelocity = customVelocity.normalized * customSpeedLimit;
                    rigidBody.velocity = customVelocity;
                    if (physicsSettings != null && physicsSettings.enableMigrationLogging)
                        Debug.Log($"CustomPhysics mode velocity capped for {this.name} from {customVelocity.magnitude} to {customSpeedLimit}. Current Velocity: {rigidBody.velocity.magnitude}");
                }
                else if (physicsSettings != null && physicsSettings.enableMigrationLogging)
                {
                    Debug.Log($"CustomPhysics mode velocity not capped for {this.name}. Current Velocity: {customVelocity.magnitude}, Limit: {customSpeedLimit}");
                }
                break;

            case PhysicsMode.Hybrid:
                // Hybrid mode: Apply custom gravity but use Unity physics for other forces
                float hybridLimit = physicsSettings != null ? physicsSettings.hybridSpeedLimit : 3.0f;
                // Always apply custom gravity in Hybrid mode
                Vector3 gravityForce = Physics.gravity * rigidBody.mass;
                rigidBody.AddForce(gravityForce, ForceMode.Force);
                
                Vector3 hybridVelocity = rigidBody.velocity;
                if (hybridVelocity.magnitude > hybridLimit)
                {
                    hybridVelocity = hybridVelocity.normalized * hybridLimit;
                    rigidBody.velocity = hybridVelocity;
                    if (physicsSettings != null && physicsSettings.enableMigrationLogging)
                        Debug.Log($"Hybrid mode velocity capped for {this.name} from {hybridVelocity.magnitude} to {hybridLimit}. Current Velocity: {rigidBody.velocity.magnitude}");
                }
                else if (physicsSettings != null && physicsSettings.enableMigrationLogging)
                {
                    Debug.Log($"Hybrid mode velocity not capped for {this.name}. Current Velocity: {hybridVelocity.magnitude}, Limit: {hybridLimit}");
                }
                break;

            case PhysicsMode.UnityPhysics:
                // Unity physics mode: Cap velocity at totalSpeedLimit
                float unitySpeedLimit = physicsSettings != null ? physicsSettings.totalSpeedLimit : 3.0f;
                Vector3 unityVelocity = rigidBody.velocity;
                if (unityVelocity.magnitude > unitySpeedLimit)
                {
                    unityVelocity = unityVelocity.normalized * unitySpeedLimit;
                    rigidBody.velocity = unityVelocity;
                    if (physicsSettings != null && physicsSettings.enableMigrationLogging)
                        Debug.Log($"UnityPhysics mode velocity capped for {this.name} from {unityVelocity.magnitude} to {unitySpeedLimit}. Current Velocity: {rigidBody.velocity.magnitude}");
                }
                else if (physicsSettings != null && physicsSettings.enableMigrationLogging)
                {
                    Debug.Log($"UnityPhysics mode velocity not capped for {this.name}. Current Velocity: {unityVelocity.magnitude}, Limit: {unitySpeedLimit}");
                }
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
