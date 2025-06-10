# Phase 0: Migration Strategy Implementation Tasks

## Phase 0A: Foundation & Compatibility Layer (Week 1-2)

### Task 1: Create Enhanced PhysicsSettings ScriptableObject
**Objective**: Create centralized physics configuration with migration support

```csharp
[CreateAssetMenu(fileName = "PhysicsSettings", menuName = "BlockBall/Physics Settings")]
public class PhysicsSettings : ScriptableObject
{
    [Header("Migration Settings")]
    public PhysicsMode physicsMode = PhysicsMode.UnityPhysics;
    public bool enableMigrationLogging = true;
    public bool validateParameterConversion = true;
    
    [Header("Legacy Unity Physics Parameters")]
    [Tooltip("Current jump force used in PlayerCameraController")]
    [Range(1f, 10f)]
    public float legacyJumpForce = 5.0f;
    
    [Tooltip("Current speed factor from PlayerCameraController")]
    [Range(0.1f, 5f)]
    public float legacySpeedFactor = 1.0f;
    
    [Tooltip("Current break factor from PlayerCameraController")]
    [Range(1f, 20f)]
    public float legacyBreakFactor = 10.0f;
    
    [Header("Target Custom Physics Parameters")]
    [Tooltip("Target jump height in Unity units (6 Bixels)")]
    [Range(0.5f, 1.5f)]
    public float targetJumpHeight = 0.75f;
    
    [Tooltip("Maximum input speed limit")]
    [Range(4f, 10f)]
    public float inputSpeedLimit = 6.0f;
    
    [Tooltip("Physics calculation speed limit")]
    [Range(5f, 12f)]
    public float physicsSpeedLimit = 6.5f;
    
    [Tooltip("Absolute maximum speed")]
    [Range(6f, 15f)]
    public float totalSpeedLimit = 7.0f;
    
    [Header("Deterministic Math Settings")]
    [Tooltip("Fixed-point scale for critical calculations")]
    public int fixedPointScale = 1000000;
    
    [Tooltip("Error accumulation threshold")]
    [Range(100f, 10000f)]
    public float accumulationThreshold = 1000f;
    
    [Header("Performance Targets")]
    [Tooltip("Maximum physics update time in milliseconds")]
    [Range(0.5f, 5f)]
    public float maxPhysicsUpdateMs = 2.0f;
    
    [Tooltip("Target physics update frequency")]
    [Range(30, 120)]
    public int physicsUpdateHz = 50;
    
    // Parameter conversion utilities
    public float ConvertJumpForceToHeight()
    {
        // Empirical conversion formula (to be calibrated)
        float gravity = Physics.gravity.magnitude;
        return (legacyJumpForce * legacyJumpForce) / (2 * gravity);
    }
    
    public float ConvertSpeedFactorToLimit()
    {
        // Convert AddForce factor to speed limit (to be calibrated)
        return legacySpeedFactor * 6.0f; // Base conversion
    }
    
    public void ValidateSettings()
    {
        // Ensure parameter consistency
        if (physicsSpeedLimit <= inputSpeedLimit)
            physicsSpeedLimit = inputSpeedLimit + 0.5f;
        if (totalSpeedLimit <= physicsSpeedLimit)
            totalSpeedLimit = physicsSpeedLimit + 0.5f;
    }
}

public enum PhysicsMode
{
    UnityPhysics,      // Current system (fallback)
    HybridPhysics,     // Partial custom implementation
    CustomPhysics,     // Full custom system
    ValidationMode     // Side-by-side comparison
}
```

**Acceptance Criteria:**
- [ ] PhysicsSettings asset created in `Assets/Settings/`
- [ ] All current physics parameters mapped to settings
- [ ] Parameter conversion functions implemented
- [ ] Settings validation working
- [ ] Inspector interface user-friendly with tooltips

### Task 2: Implement IPhysicsObject Wrapper Interface
**Objective**: Create compatibility layer for existing PhysicObjekt

```csharp
public interface IPhysicsObject
{
    // Core physics properties
    Vector3 Position { get; set; }
    Vector3 Velocity { get; set; }
    Vector3 AngularVelocity { get; set; }
    float Mass { get; set; }
    
    // BlockBall-specific properties
    Vector3 GravityDirection { get; set; }
    bool IsGrounded { get; }
    bool HasGroundContact();
    
    // State management
    PhysicsObjectState CurrentState { get; }
    void SetState(PhysicsObjectState state);
    
    // Integration methods
    void IntegrateVelocityVerlet(float deltaTime);
    void ApplyForce(Vector3 force, ForceMode mode = ForceMode.Force);
    void ApplyTorque(Vector3 torque, ForceMode mode = ForceMode.Force);
    
    // Collision callbacks
    void OnPhysicsCollision(Collision collision);
    void OnPhysicsTrigger(Collider other, bool isEnter);
}

public enum PhysicsObjectState
{
    Grounded,
    Airborne,
    Sliding,
    Transitioning
}

// Wrapper implementation for existing PhysicObjekt
public class PhysicsObjectWrapper : MonoBehaviour, IPhysicsObject
{
    private PhysicObjekt physicObject;
    private Rigidbody rigidBody;
    
    void Awake()
    {
        physicObject = GetComponent<PhysicObjekt>();
        rigidBody = GetComponent<Rigidbody>();
        
        if (physicObject == null)
            throw new System.Exception("PhysicsObjectWrapper requires PhysicObjekt component");
    }
    
    // Implement interface by delegating to existing PhysicObjekt
    public Vector3 Position 
    { 
        get => transform.position; 
        set => transform.position = value; 
    }
    
    public Vector3 Velocity 
    { 
        get => rigidBody.velocity; 
        set => rigidBody.velocity = value; 
    }
    
    public Vector3 GravityDirection 
    { 
        get => physicObject.GravityDirection; 
        set => physicObject.SetGravityDirection(value); 
    }
    
    public bool HasGroundContact() => physicObject.HasGroundContact();
    
    // ... implement remaining interface methods
}
```

**Acceptance Criteria:**
- [ ] IPhysicsObject interface defined with all required methods
- [ ] PhysicsObjectWrapper implemented and tested
- [ ] Existing PhysicObjekt functionality accessible through interface
- [ ] No breaking changes to existing code
- [ ] Integration tested with Player and BallObject classes

### Task 3: Create DeterministicMath Utility Library
**Objective**: Implement consistent math operations for deterministic physics

```csharp
public static class DeterministicMath
{
    private const int DEFAULT_FIXED_SCALE = 1000000; // 6 decimal places
    
    #region Fixed-Point Arithmetic
    public static int ToFixed(float value, int scale = DEFAULT_FIXED_SCALE)
    {
        return (int)(value * scale);
    }
    
    public static float FromFixed(int value, int scale = DEFAULT_FIXED_SCALE)
    {
        return (float)value / scale;
    }
    
    public static int FixedMultiply(int a, int b, int scale = DEFAULT_FIXED_SCALE)
    {
        return (int)(((long)a * b) / scale);
    }
    
    public static int FixedDivide(int a, int b, int scale = DEFAULT_FIXED_SCALE)
    {
        return (int)(((long)a * scale) / b);
    }
    #endregion
    
    #region Deterministic Math Functions
    // Use consistent precision across platforms
    public static float DeterministicSqrt(float value)
    {
        return (float)Math.Sqrt((double)value);
    }
    
    public static float DeterministicSin(float value)
    {
        return (float)Math.Sin((double)value);
    }
    
    public static float DeterministicCos(float value)
    {
        return (float)Math.Cos((double)value);
    }
    
    public static float DeterministicAtan2(float y, float x)
    {
        return (float)Math.Atan2((double)y, (double)x);
    }
    #endregion
    
    #region Error Accumulation Prevention
    public static Vector3 NormalizeAccumulation(Vector3 vector, float threshold = 1000f)
    {
        float magnitude = vector.magnitude;
        if (magnitude > threshold)
        {
            return vector.normalized * threshold;
        }
        return vector;
    }
    
    public static float ClampAccumulation(float value, float min = -1000f, float max = 1000f)
    {
        return Mathf.Clamp(value, min, max);
    }
    
    public static Quaternion NormalizeRotation(Quaternion rotation)
    {
        // Prevent quaternion drift
        return Quaternion.Normalize(rotation);
    }
    #endregion
    
    #region Platform Consistency
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public static void EnsureConsistentFPU()
    {
        // Platform-specific floating-point unit setup
        #if UNITY_STANDALONE_WIN
        // Windows-specific FPU settings
        #elif UNITY_STANDALONE_OSX
        // macOS-specific settings
        #endif
    }
    #endregion
}
```

**Acceptance Criteria:**
- [ ] Fixed-point arithmetic functions implemented and tested
- [ ] Deterministic math functions provide consistent results across platforms
- [ ] Error accumulation prevention utilities working
- [ ] Platform-specific consistency measures implemented
- [ ] Performance impact measured and acceptable (<0.1ms overhead)

### Task 4: Create Physics Performance Profiler
**Objective**: Monitor physics performance and validate targets

```csharp
public class PhysicsProfiler : MonoBehaviour
{
    [Header("Profiling Settings")]
    public bool enableProfiling = true;
    public bool logToConsole = false;
    public int sampleWindow = 60; // frames
    
    [Header("Performance Targets")]
    public float targetPhysicsUpdateMs = 2.0f;
    public float targetGravityUpdateMs = 0.1f;
    public float targetSpeedControlMs = 0.2f;
    
    // Performance tracking
    private Queue<float> physicsUpdateTimes = new Queue<float>();
    private Queue<float> gravityUpdateTimes = new Queue<float>();
    private Queue<float> speedControlTimes = new Queue<float>();
    private Queue<int> memoryAllocations = new Queue<int>();
    
    // Current frame stats
    public float CurrentPhysicsUpdateMs { get; private set; }
    public float AveragePhysicsUpdateMs { get; private set; }
    public float MaxPhysicsUpdateMs { get; private set; }
    public int CurrentMemoryAllocations { get; private set; }
    
    // Performance status
    public bool IsPerformanceTargetMet => AveragePhysicsUpdateMs <= targetPhysicsUpdateMs;
    
    public void BeginPhysicsUpdate()
    {
        if (!enableProfiling) return;
        
        System.GC.Collect(); // Force GC before measurement
        currentUpdateStartTime = Time.realtimeSinceStartup * 1000f;
        startMemory = System.GC.GetTotalMemory(false);
    }
    
    public void EndPhysicsUpdate()
    {
        if (!enableProfiling) return;
        
        float updateTime = (Time.realtimeSinceStartup * 1000f) - currentUpdateStartTime;
        int memoryDelta = (int)(System.GC.GetTotalMemory(false) - startMemory);
        
        RecordPhysicsUpdate(updateTime, memoryDelta);
        
        if (logToConsole && updateTime > targetPhysicsUpdateMs)
        {
            Debug.LogWarning($"Physics update exceeded target: {updateTime:F2}ms > {targetPhysicsUpdateMs}ms");
        }
    }
    
    private void RecordPhysicsUpdate(float updateTime, int memoryDelta)
    {
        // Update current stats
        CurrentPhysicsUpdateMs = updateTime;
        CurrentMemoryAllocations = memoryDelta;
        
        // Add to rolling window
        physicsUpdateTimes.Enqueue(updateTime);
        memoryAllocations.Enqueue(memoryDelta);
        
        // Maintain window size
        while (physicsUpdateTimes.Count > sampleWindow)
        {
            physicsUpdateTimes.Dequeue();
            memoryAllocations.Dequeue();
        }
        
        // Calculate averages
        AveragePhysicsUpdateMs = physicsUpdateTimes.Average();
        MaxPhysicsUpdateMs = physicsUpdateTimes.Max();
    }
    
    public PerformanceReport GenerateReport()
    {
        return new PerformanceReport
        {
            averagePhysicsUpdateMs = AveragePhysicsUpdateMs,
            maxPhysicsUpdateMs = MaxPhysicsUpdateMs,
            targetMet = IsPerformanceTargetMet,
            averageMemoryAllocations = memoryAllocations.Count > 0 ? memoryAllocations.Average() : 0,
            sampleCount = physicsUpdateTimes.Count
        };
    }
    
    // Performance monitoring fields
    private float currentUpdateStartTime;
    private long startMemory;
}

[System.Serializable]
public struct PerformanceReport
{
    public float averagePhysicsUpdateMs;
    public float maxPhysicsUpdateMs;
    public bool targetMet;
    public float averageMemoryAllocations;
    public int sampleCount;
}
```

**Acceptance Criteria:**
- [ ] Performance profiler tracks physics update times accurately
- [ ] Memory allocation tracking working
- [ ] Performance targets configurable and monitored
- [ ] Reports generated with useful metrics
- [ ] Integration with existing physics components completed

## Phase 0A Completion Checklist

### Technical Implementation
- [ ] PhysicsSettings ScriptableObject created and configured
- [ ] IPhysicsObject interface implemented with PhysicsObjectWrapper
- [ ] DeterministicMath utility library completed and tested
- [ ] PhysicsProfiler implemented and integrated
- [ ] All code compiled without errors or warnings

### Integration Testing
- [ ] PhysicsSettings works with existing PlayerCameraController parameters
- [ ] PhysicsObjectWrapper maintains PhysicObjekt functionality
- [ ] DeterministicMath provides consistent results across test platforms
- [ ] PhysicsProfiler accurately measures existing Unity physics performance
- [ ] No regression in existing gameplay behavior

### Documentation
- [ ] Code properly commented with XML documentation
- [ ] Integration guide updated with Phase 0A changes
- [ ] Performance baselines documented
- [ ] Parameter conversion formulas documented and validated

### Quality Assurance
- [ ] Unit tests created for critical functions
- [ ] Integration tests validate existing system compatibility
- [ ] Performance impact measured and within acceptable limits
- [ ] Rollback procedures tested and validated

**Phase 0A Success Criteria**: All existing functionality preserved while foundation for migration established. Performance profiling active, parameter conversion working, and compatibility layer functional.

Ready to proceed to Phase 0B: Hybrid Implementation upon completion of all Phase 0A tasks.
