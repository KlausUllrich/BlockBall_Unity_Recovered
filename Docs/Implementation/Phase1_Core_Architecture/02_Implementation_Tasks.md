# Phase 1: Implementation Tasks

## Task 1.1: Create PhysicsSettings ScriptableObject

### Objective
Create a **single source of truth** for all physics configuration with user-friendly interface for non-technical users.

### Implementation Steps
1. **Create Script**: `Assets/Scripts/Physics/PhysicsSettings.cs`
2. **Design user-friendly interface** with intuitive sliders and tooltips
3. **Include conversion functions** from user-friendly values to physics calculations
4. **Setup asset creation** in Unity's Create menu

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    /// <summary>
    /// Single source of truth for all physics settings.
    /// Provides user-friendly interface for non-technical users.
    /// </summary>
    [CreateAssetMenu(fileName = "PhysicsSettings", menuName = "BlockBall/Physics Settings")]
    public class PhysicsSettings : ScriptableObject
    {
        [Header("Basic Movement (User-Friendly)")]
        [Tooltip("How fast the ball rolls at maximum speed (Unity units/second)")]
        [Range(3f, 15f)]
        public float maxRollSpeed = 6f;
        
        [Tooltip("How quickly the ball accelerates when rolling (1=slow, 10=instant)")]
        [Range(1f, 10f)]
        public float rollResponsiveness = 5f;
        
        [Tooltip("How high the ball jumps (Unity units)")]
        [Range(0.5f, 2f)]
        public float jumpHeight = 0.75f; // 6 Bixels
        
        [Tooltip("How far the ball can jump while moving (Unity units)")]
        [Range(1f, 4f)]
        public float maxJumpDistance = 1.5f; // 12 Bixels
        
        [Header("Physics Feel")]
        [Tooltip("How bouncy surfaces are (0=no bounce, 1=perfect bounce)")]
        [Range(0f, 1f)]
        public float bounciness = 0.3f;
        
        [Tooltip("How much surfaces slow the ball down (0=ice, 1=sandpaper)")]
        [Range(0f, 2f)]
        public float friction = 0.8f;
        
        [Tooltip("How strongly gravity pulls the ball down")]
        [Range(5f, 20f)]
        public float gravityStrength = 9.81f;
        
        [Header("Advanced Control")]
        [Tooltip("Time to smoothly change gravity direction (seconds)")]
        [Range(0.1f, 1f)]
        public float gravityTransitionTime = 0.3f;
        
        [Tooltip("How much control you have while airborne (0=none, 1=full)")]
        [Range(0f, 1f)]
        public float airControl = 0.3f;
        
        [Tooltip("Time window to jump after leaving a ledge (seconds)")]
        [Range(0f, 0.3f)]
        public float coyoteTime = 0.1f;
        
        [Tooltip("Time window to buffer jump inputs (seconds)")]
        [Range(0f, 0.3f)]
        public float jumpBufferTime = 0.15f;
        
        [Header("Speed Control System")]
        [Tooltip("Maximum speed from player input only")]
        [Range(4f, 12f)]
        public float inputSpeedLimit = 6f;
        
        [Tooltip("Maximum speed including physics forces")]
        [Range(5f, 15f)]
        public float physicsSpeedLimit = 6.5f;
        
        [Tooltip("Absolute maximum speed (safety limit)")]
        [Range(6f, 20f)]
        public float totalSpeedLimit = 7f;
        
        // CONVERSION FUNCTIONS: User-friendly values → Physics calculations
        
        /// <summary>
        /// Convert user-friendly roll responsiveness to acceleration value
        /// </summary>
        public float GetRollAcceleration()
        {
            // Map 1-10 range to 2-20 m/s² acceleration
            return Mathf.Lerp(2f, 20f, rollResponsiveness / 10f);
        }
        
        /// <summary>
        /// Calculate jump force needed for desired jump height
        /// </summary>
        public float GetJumpForce(float ballMass)
        {
            // F = ma, where a = sqrt(2gh) for jump height h
            return ballMass * Mathf.Sqrt(2f * gravityStrength * jumpHeight);
        }
        
        /// <summary>
        /// Calculate maximum angular velocity for rolling
        /// </summary>
        public float GetMaxAngularVelocity(float ballRadius = 0.5f)
        {
            return maxRollSpeed / ballRadius;
        }
        
        /// <summary>
        /// Get input acceleration curve (ease-in/ease-out feel)
        /// </summary>
        public float GetInputAccelerationCurve(float currentSpeed, float targetSpeed)
        {
            float speedRatio = currentSpeed / maxRollSpeed;
            float acceleration = GetRollAcceleration();
            
            // Reduce acceleration as we approach max speed (ease-out feel)
            if (speedRatio > 0.8f)
            {
                float reduceRatio = (speedRatio - 0.8f) / 0.2f; // 0-1 over final 20%
                acceleration *= Mathf.Lerp(1f, 0.1f, reduceRatio);
            }
            
            return acceleration;
        }
        
        /// <summary>
        /// Calculate slope effect on movement (45° = no acceleration, >45° = deceleration)
        /// </summary>
        public float GetSlopeAccelerationFactor(float slopeAngleDegrees)
        {
            if (slopeAngleDegrees <= 45f)
            {
                // 0-45°: full to no acceleration
                return Mathf.Lerp(1f, 0f, slopeAngleDegrees / 45f);
            }
            else
            {
                // >45°: deceleration (negative factor)
                float excessAngle = slopeAngleDegrees - 45f;
                return -Mathf.Min(excessAngle / 45f, 1f); // Cap at -1 (full deceleration)
            }
        }
        
        /// <summary>
        /// Apply exponential speed decay near limits
        /// </summary>
        public float ApplySpeedDecay(float currentSpeed, float speedLimit)
        {
            if (currentSpeed <= speedLimit * 0.95f) return currentSpeed;
            
            float excessRatio = (currentSpeed - speedLimit * 0.95f) / (speedLimit * 0.05f);
            float decayFactor = Mathf.Exp(-2f * excessRatio); // Exponential decay
            
            return speedLimit * 0.95f + (speedLimit * 0.05f * decayFactor);
        }
        
        // VALIDATION: Ensure values are reasonable
        private void OnValidate()
        {
            // Ensure speed limits are ordered correctly
            if (inputSpeedLimit > physicsSpeedLimit)
                physicsSpeedLimit = inputSpeedLimit + 0.5f;
                
            if (physicsSpeedLimit > totalSpeedLimit)
                totalSpeedLimit = physicsSpeedLimit + 0.5f;
                
            // Ensure jump distance is achievable with jump height
            float theoreticalMaxDistance = 2f * Mathf.Sqrt(jumpHeight * gravityStrength);
            if (maxJumpDistance > theoreticalMaxDistance)
            {
                Debug.LogWarning($"Jump distance {maxJumpDistance} may be unreachable with jump height {jumpHeight}. Theoretical max: {theoreticalMaxDistance:F2}");
            }
        }
    }
}
```

### Asset Creation Steps
1. **Create Default Asset**: Right-click → Create → BlockBall → Physics Settings
2. **Name**: "DefaultPhysicsSettings"
3. **Location**: `Assets/Resources/` (for automatic loading)
4. **Configure**: Set values according to BlockBall_Physics_Spec.md requirements

### Validation Steps
1. **Inspector Interface**: All sliders work with tooltips visible
2. **Value Ranges**: Cannot set invalid combinations
3. **Conversion Functions**: Return expected physics values
4. **Asset Loading**: Can be loaded via Resources.Load()

---

## Task 1.2: Create BlockBallPhysicsManager

### Objective
Create the central physics coordination system that integrates with PhysicsSettings.

### Implementation Steps
1. **Create Script**: `Assets/Scripts/Physics/BlockBallPhysicsManager.cs`
2. **Reference PhysicsSettings**: Load and use centralized configuration
3. **Implement Features**: Fixed timestep, object registry, profiling

### Code Template
```csharp
using System.Collections.Generic;
using UnityEngine;

namespace BlockBall.Physics
{
    public class BlockBallPhysicsManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private PhysicsSettings physicsSettings;
        
        [Header("System Settings")]
        [Range(30, 120)] public int targetPhysicsHz = 50;
        [Range(1, 10)] public int maxSubsteps = 8;
        [SerializeField] private bool enableProfiling = true;
        [SerializeField] private bool showPhysicsDebug = false;
        
        // Singleton instance
        public static BlockBallPhysicsManager Instance { get; private set; }
        
        // Physics settings access
        public PhysicsSettings Settings => physicsSettings;
        
        // Physics objects registry
        private List<IPhysicsObject> physicsObjects = new List<IPhysicsObject>();
        private float accumulator = 0f;
        private float fixedTimestep;
        
        // Profiling
        private PhysicsProfiler profiler;
        
        private void Awake()
        {
            InitializeSingleton();
            LoadPhysicsSettings();
            InitializeSystems();
        }
        
        private void InitializeSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("Multiple BlockBallPhysicsManager instances detected!");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void LoadPhysicsSettings()
        {
            if (physicsSettings == null)
            {
                physicsSettings = Resources.Load<PhysicsSettings>("DefaultPhysicsSettings");
                if (physicsSettings == null)
                {
                    Debug.LogError("No PhysicsSettings found! Creating default settings.");
                    physicsSettings = CreateDefaultSettings();
                }
            }
        }
        
        private void InitializeSystems()
        {
            fixedTimestep = 1f / targetPhysicsHz;
            profiler = new PhysicsProfiler();
            
            Debug.Log($"Physics initialized: {targetPhysicsHz}Hz timestep, max {maxSubsteps} substeps");
        }
        
        private void FixedUpdate()
        {
            if (enableProfiling) profiler.BeginPhysicsFrame();
            
            ProcessPhysicsSteps();
            
            if (enableProfiling) profiler.EndPhysicsFrame();
        }
        
        private void ProcessPhysicsSteps()
        {
            accumulator += Time.fixedDeltaTime;
            
            int substeps = 0;
            while (accumulator >= fixedTimestep && substeps < maxSubsteps)
            {
                UpdatePhysicsObjects(fixedTimestep);
                accumulator -= fixedTimestep;
                substeps++;
            }
            
            // Visual interpolation for smooth rendering
            float alpha = accumulator / fixedTimestep;
            InterpolatePhysicsObjects(alpha);
        }
        
        // ... rest of physics object management code ...
        
        private PhysicsSettings CreateDefaultSettings()
        {
            var settings = ScriptableObject.CreateInstance<PhysicsSettings>();
            // Set default values matching BlockBall_Physics_Spec.md
            return settings;
        }
    }
}
```

---

## Task 1.3: Create IPhysicsObject Interface

### Objective
Define a standardized contract for all physics-enabled objects.

### Implementation Steps
1. **Create Interface**: `Assets/Scripts/Physics/IPhysicsObject.cs`
2. **Define Contract**: Position, velocity, acceleration, mass
3. **Add Callbacks**: Pre/during/post physics step hooks

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    public interface IPhysicsObject
    {
        // Physics state properties
        Vector3 Position { get; set; }
        Vector3 Velocity { get; set; }
        Vector3 Acceleration { get; set; }
        float Mass { get; }
        
        // Physics step callbacks
        void PrePhysicsStep(float deltaTime);
        void PhysicsStep(float deltaTime);
        void PostPhysicsStep(float deltaTime);
        
        // Object identification
        string PhysicsObjectName { get; }
        bool IsPhysicsEnabled { get; }
    }
    
    public interface IInterpolatable
    {
        void Interpolate(float alpha);
        Vector3 PreviousPosition { get; }
        Vector3 CurrentPosition { get; }
    }
}
```

### Documentation Requirements
- **Interface Documentation**: XML comments for all members
- **Usage Examples**: How to implement the interface
- **Best Practices**: Performance considerations

---

## Task 1.4: Create VelocityVerletIntegrator

### Objective
Implement the core Velocity Verlet integration algorithm.

### Implementation Steps
1. **Create Static Class**: `Assets/Scripts/Physics/VelocityVerletIntegrator.cs`
2. **Implement Algorithm**: Position, acceleration, velocity updates
3. **Add Validation**: Prevent NaN/infinity values

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    public static class VelocityVerletIntegrator
    {
        public static void Integrate(IPhysicsObject obj, float deltaTime)
        {
            // Validate input
            if (obj == null)
            {
                Debug.LogError("Cannot integrate null physics object!");
                return;
            }
            
            if (deltaTime <= 0f)
            {
                Debug.LogWarning("Invalid deltaTime for integration!");
                return;
            }
            
            // Store current values
            Vector3 currentPosition = obj.Position;
            Vector3 currentVelocity = obj.Velocity;
            Vector3 currentAcceleration = obj.Acceleration;
            
            // Velocity Verlet Integration:
            // x(t+dt) = x(t) + v(t)*dt + 0.5*a(t)*dt²
            Vector3 newPosition = currentPosition + 
                                 currentVelocity * deltaTime + 
                                 0.5f * currentAcceleration * deltaTime * deltaTime;
            
            // Update position first
            obj.Position = newPosition;
            
            // Calculate new acceleration at new position
            Vector3 newAcceleration = CalculateAcceleration(obj);
            
            // v(t+dt) = v(t) + 0.5*(a(t) + a(t+dt))*dt
            Vector3 newVelocity = currentVelocity + 
                                 0.5f * (currentAcceleration + newAcceleration) * deltaTime;
            
            // Update velocity and acceleration
            obj.Velocity = newVelocity;
            obj.Acceleration = newAcceleration;
            
            // Validate results
            ValidatePhysicsState(obj);
        }
        
        private static Vector3 CalculateAcceleration(IPhysicsObject obj)
        {
            // This will be expanded in later phases
            // For now, just return gravity
            return Vector3.down * 9.81f;
        }
        
        private static void ValidatePhysicsState(IPhysicsObject obj)
        {
            // Check for NaN or infinity values
            if (!IsValidVector3(obj.Position))
            {
                Debug.LogError($"Invalid position detected: {obj.Position}");
                obj.Position = Vector3.zero;
            }
            
            if (!IsValidVector3(obj.Velocity))
            {
                Debug.LogError($"Invalid velocity detected: {obj.Velocity}");
                obj.Velocity = Vector3.zero;
            }
            
            if (!IsValidVector3(obj.Acceleration))
            {
                Debug.LogError($"Invalid acceleration detected: {obj.Acceleration}");
                obj.Acceleration = Vector3.zero;
            }
        }
        
        private static bool IsValidVector3(Vector3 v)
        {
            return !float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z) &&
                   !float.IsInfinity(v.x) && !float.IsInfinity(v.y) && !float.IsInfinity(v.z);
        }
    }
}
```

### Testing Requirements
- **Energy Conservation Test**: Track total energy over time
- **Stability Test**: High-speed integration without explosion
- **Accuracy Test**: Compare with analytical solutions

---

## Task 1.5: Create PhysicsProfiler

### Objective
Implement performance monitoring for physics system.

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    public class PhysicsProfiler
    {
        private const int FRAME_SAMPLES = 60;
        private float[] frameTimes = new float[FRAME_SAMPLES];
        private int frameIndex = 0;
        private float frameStartTime = 0f;
        
        // Performance targets
        private const float TARGET_FRAME_TIME = 0.002f; // 2ms
        private const float WARNING_FRAME_TIME = 0.001f; // 1ms
        
        public void BeginPhysicsFrame()
        {
            frameStartTime = Time.realtimeSinceStartup;
        }
        
        public void EndPhysicsFrame()
        {
            float frameTime = Time.realtimeSinceStartup - frameStartTime;
            frameTimes[frameIndex] = frameTime;
            frameIndex = (frameIndex + 1) % FRAME_SAMPLES;
            
            // Performance warnings
            if (frameTime > TARGET_FRAME_TIME)
            {
                Debug.LogWarning($"Physics frame exceeded target: {frameTime:F4}ms");
            }
        }
        
        public float GetAverageFrameTime()
        {
            float total = 0f;
            for (int i = 0; i < FRAME_SAMPLES; i++)
            {
                total += frameTimes[i];
            }
            return total / FRAME_SAMPLES;
        }
        
        public void DrawDebugInfo()
        {
            float avgTime = GetAverageFrameTime();
            string color = avgTime > TARGET_FRAME_TIME ? "red" : "green";
            
            GUILayout.Label($"<color={color}>Physics: {avgTime:F4}ms avg</color>");
            GUILayout.Label($"Target: {TARGET_FRAME_TIME:F4}ms");
        }
    }
}
```

## SSOT configuration
- ensure all gameplay relevant physics settings can be edited by the user
- ensure there is only one single source of truth for all physics settings
- try to make editing of physics values comfortable to users not familiar with the physics engine or the formulae behind it

## Documentation Requirements for Each Task
1. **Code Comments**: XML documentation for all public members
2. **Usage Examples**: How to use each component
3. **Performance Notes**: Memory allocation and timing considerations
4. **Error Handling**: What can go wrong and how it's handled
5. **SSOT Configuration**: How to ensure only one source of truth for physics settings
