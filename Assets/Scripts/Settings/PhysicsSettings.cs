using UnityEngine;
using BlockBall.Physics;

namespace BlockBall.Settings
{
    [CreateAssetMenu(fileName = "PhysicsSettings", menuName = "BlockBall/Physics Settings")]
    public class PhysicsSettings : ScriptableObject
    {
        // Physics Mode for migration
        [Header("Physics Mode Selection")]
        [Tooltip("Select the active physics mode: UnityPhysics uses built-in Unity physics, CustomPhysics uses a fully custom implementation, Hybrid combines elements of both for transition.")]
        public BlockBall.Physics.PhysicsMode physicsMode = BlockBall.Physics.PhysicsMode.UnityPhysics;
        
        // Gravity value for custom physics mode
        [Tooltip("Gravity value for custom physics mode")]
        public float gravity = -9.81f;
        
        [Header("Logging and Debugging")]
        [Tooltip("Enable detailed logging for physics migration to debug mode-specific behaviors, velocity capping, and force application. Useful for troubleshooting but may impact performance with excessive log output.")]
        public bool enableMigrationLogging = false;
        
        [Tooltip("Enable detailed logging for jump buffering to track input detection, buffering state, and jump execution. Helps diagnose timing issues but can produce verbose output.")]
        public bool enableJumpBufferingLogging = false;
        
        [Header("Jump Buffering Settings")]
        [Tooltip("Duration (in seconds) to buffer a jump input if the player is not grounded. Ensures jump intent is not lost during brief loss of ground contact. Range from 0.05s to 0.5s.")]
        [Range(0.05f, 0.5f)]
        public float jumpInputBufferTime = 0.2f;
        
        [Tooltip("Duration (in seconds) to consider the player grounded after losing contact. Prevents immediate fall detection and allows buffered jumps. Range from 0.05s to 0.5s.")]
        [Range(0.05f, 0.5f)]
        public float groundContactBufferTime = 0.2f;
        
        [Header("Unity Physics (Legacy) Mode Settings")]
        [Tooltip("Jump force applied in UnityPhysics mode. This is the legacy value adjusted for Unity's built-in physics system. Increased by 50% in Unity mode for gameplay feel. Range from 2 to 10.")]
        [Range(2f, 10f)]
        public float legacyJumpForce = 5.0f;
        
        [Tooltip("Speed factor for movement in UnityPhysics mode. Adjusts the responsiveness of legacy movement forces. Range from 0.5 to 2.0.")]
        [Range(0.5f, 2.0f)]
        public float legacySpeedFactor = 1.0f;
        
        [Tooltip("Breaking factor to slow down movement in UnityPhysics mode. Higher values result in faster deceleration when braking. Range from 5 to 20.")]
        [Range(5f, 20f)]
        public float legacyBreakFactor = 10.0f;
        
        [Tooltip("Total speed limit (in blocks per second, 1 block = 1 Unity unit) for objects in UnityPhysics mode. Caps total velocity to ensure consistent movement. Range from 0.5 to 5.0.")]
        [Range(3.0f, 10.0f)]
        public float totalSpeedLimit = 3.0f;
        
        [Tooltip("Linear drag for the Rigidbody in UnityPhysics mode. Controls how quickly the ball slows down linearly. Higher values mean faster slowdown, reducing rolling distance. Current observed value is 1.0. Range from 0.0 to 5.0.")]
        [Range(0.0f, 5.0f)]
        public float linearDrag = 1.0f;
        
        [Tooltip("Angular drag for the Rigidbody in UnityPhysics mode. Controls how quickly the ball stops rotating. Higher values mean faster rotational slowdown, affecting rolling behavior. Current observed value is 2.5. Range from 0.0 to 10.0.")]
        [Range(0.0f, 10.0f)]
        public float angularDrag = 2.5f;
        
        [Header("Hybrid Mode Settings")]
        [Tooltip("Maximum speed limit (in blocks per second, 1 block = 1 Unity unit) for objects in Hybrid mode. Caps total velocity combining Unity and custom physics elements. Uses the same value as UnityPhysics mode for consistency. Range from 0.5 to 5.0.")]
        [Range(3.0f, 10.0f)]
        public float hybridSpeedLimit = 3.0f; // Same as totalSpeedLimit by default
        
        [Tooltip("Breaking factor to slow down movement in Hybrid mode. Higher values result in faster deceleration when braking. Range from 5 to 20.")]
        [Range(5f, 20f)]
        public float hybridBreakFactor = 10.0f;
        
        [Header("Custom Physics Mode Settings")]
        [Tooltip("Maximum speed limit (in blocks per second, 1 block = 1 Unity unit) for objects in CustomPhysics mode. Caps velocity in the fully custom implementation for predictable behavior. Range from 0.5 to 5.0.")]
        [Range(0.5f, 5.0f)]
        public float physicsSpeedLimit = 2.0f;
        
        [Tooltip("Breaking factor to slow down movement in CustomPhysics mode. Higher values result in faster deceleration when braking. Range from 5 to 20.")]
        [Range(5f, 20f)]
        public float customBreakFactor = 10.0f;
        
        [Tooltip("Number of substeps for deterministic physics calculations in CustomPhysics mode. Higher values improve precision but impact performance. Range from 1 to 10.")]
        [Range(1, 10)]
        public int deterministicSubsteps = 4;
        
        [Tooltip("Performance factor for CustomPhysics mode. Adjusts update frequency or calculation detail. Higher values prioritize performance over accuracy. Range from 1 to 5.")]
        [Range(1, 5)]
        public int performanceFactor = 2;
        
        [Header("Player Input Force Settings (All Modes)")]
        [Tooltip("Speed limit for player input forces. Intended to cap velocity driven by input, but currently not implemented in movement logic. For future use or legacy reference. Range from 4 to 10.")]
        [Range(4f, 10f)]
        public float inputSpeedLimit = 6.0f;
        
        [Tooltip("Input force scaling factor for movement across all modes. 1.0 represents original behavior, 0.2 is 20% strength, 3.0 is 300% strength. Multiplies base force magnitudes. Range from 0.2 to 3.0.")]
        [Range(0.2f, 3.0f)]
        public float inputForceScale = 1.0f; // Renamed to clarify it's a scaling factor
        
        [Tooltip("Base force magnitude for forward movement in all modes. Defines the strength of forward input before scaling. Matches original hardcoded value. Range from 5 to 15.")]
        [Range(5f, 15f)]
        public float forwardForceMagnitude = 8.0f; // Matches original hardcoded value
        
        [Tooltip("Base force magnitude for sideways movement (left/right) in all modes. Defines the strength of lateral input before scaling. Matches original hardcoded value. Range from 5 to 15.")]
        [Range(5f, 15f)]
        public float sidewaysForceMagnitude = 8.0f; // Matches original hardcoded value
        
        [Tooltip("Base force magnitude for backward movement in all modes. Defines the strength of backward input before scaling. Matches original hardcoded value. Range from 5 to 15.")]
        [Range(5f, 15f)]
        public float backwardForceMagnitude = 3.0f; // Matches original hardcoded value
        
        [Header("Deterministic Math Configuration")]
        [Tooltip("Scale factor for fixed-point math (higher values increase precision but risk overflow)")]
        [Range(1000f, 10000000f)]
        public long fixedPointScale = 1000000;
        
        [Tooltip("Threshold for accumulation errors in fixed-point calculations")]
        [Range(100f, 10000f)]
        public long accumulationThreshold = 1000;
        
        [Header("Performance Settings")]
        [Tooltip("Maximum milliseconds per physics update (cap to prevent slowdown)")]
        [Range(1f, 10f)]
        public float maxPhysicsUpdateMs = 2.0f;
        
        [Tooltip("Physics update frequency in Hz (e.g., 50Hz = 20ms per update)")]
        [Range(30f, 120f)]
        public int physicsUpdateHz = 50;
        
        [Header("Advanced Physics Settings (Future CustomPhysics)")]
        [Tooltip("Fixed timestep for custom physics simulation in seconds (50Hz = 0.02s)")]
        [Range(0.01f, 0.05f)]
        public float customPhysicsTimestep = 0.02f;
        
        [Tooltip("Maximum substeps to prevent spiral of death during frame drops")]
        [Range(1, 16)]
        public int maxPhysicsSubsteps = 8;
        
        [Tooltip("Enable zero-allocation mode using object pooling for better performance")]
        public bool useObjectPooling = true;
        
        [Tooltip("Enable energy conservation monitoring and warnings")]
        public bool enableEnergyConservation = true;
        
        [Header("Advanced Ball Physics Parameters")]
        [Tooltip("Maximum speed achievable through player input (blocks per second)")]
        [Range(1f, 10f)]
        public float maxInputSpeed = 5f;
        
        [Tooltip("Maximum speed from physics forces like gravity (blocks per second)")]
        [Range(5f, 15f)]
        public float maxPhysicsSpeed = 10f;
        
        [Tooltip("Absolute maximum speed limit (blocks per second)")]
        [Range(10f, 20f)]
        public float maxTotalSpeed = 15f;
        
        [Tooltip("Jump height in Unity units (6 Bixels = 0.75 units)")]
        [Range(0.5f, 2f)]
        public float jumpHeight = 0.75f;
        
        [Tooltip("Jump input buffer time in seconds")]
        [Range(0.05f, 0.2f)]
        public float jumpBufferTime = 0.1f;
        
        [Tooltip("Coyote time for jumping after leaving ground")]
        [Range(0.1f, 0.3f)]
        public float coyoteTime = 0.15f;
        
        [Tooltip("Friction when rolling on surfaces")]
        [Range(0.1f, 1f)]
        public float rollingFriction = 0.3f;
        
        [Tooltip("Friction when sliding on steep surfaces")]
        [Range(0.05f, 0.5f)]
        public float slidingFriction = 0.1f;
        
        [Tooltip("Air resistance multiplier")]
        [Range(0.8f, 1f)]
        public float airDrag = 0.95f;
        
        [Tooltip("Angle threshold for determining if surface is walkable (degrees)")]
        [Range(30f, 60f)]
        public float slopeLimit = 45f;
        
        [Tooltip("Distance to check for ground contact")]
        [Range(0.5f, 0.7f)]
        public float groundCheckDistance = 0.51f;
        
        // Conversion utility for physics parameter validation
        public float GetTargetJumpHeight()
        {
            // Empirical conversion formula (to be calibrated)
            return (legacyJumpForce * legacyJumpForce) / (2 * gravity);
        }
        
        public void ValidateParameters()
        {
            // Commented out unreachable code due to 'if (!true)'
            // if (!true) return;
        }
    }
}
