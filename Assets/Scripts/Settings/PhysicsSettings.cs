using UnityEngine;
using BlockBall.Physics;

namespace BlockBall.Settings
{
    [CreateAssetMenu(fileName = "PhysicsSettings", menuName = "BlockBall/Physics Settings")]
    public class PhysicsSettings : ScriptableObject
    {
        // Physics Mode for migration
        [Header("Physics Mode Selection")]
        [Tooltip("Select the active physics mode: UnityPhysics (default), CustomPhysics, or Hybrid.")]
        public BlockBall.Physics.PhysicsMode physicsMode = BlockBall.Physics.PhysicsMode.UnityPhysics;
        
        [Header("Migration and Debugging")]
        [Tooltip("Enable detailed logging for physics migration debugging.")]
        public bool enableMigrationLogging = true;
        [Tooltip("Enable detailed logging specifically for jump buffering debugging.")]
        public bool enableJumpBufferingLogging = true;
        [Tooltip("Validate parameter conversion during migration.")]
        public bool validateParameterConversion = true;
        [Tooltip("Enable debug logging for jump buffering.")]
        public bool enableJumpBufferingDebugLogging = false;
        
        [Header("Legacy Parameters")]
        [Tooltip("Legacy jump force from the original system")]
        [Range(1f, 10f)]
        public float legacyJumpForce = 5.0f;
        
        [Tooltip("Legacy speed factor for movement")]
        [Range(0.1f, 5f)]
        public float legacySpeedFactor = 1.0f;
        
        [Tooltip("Legacy break factor for stopping movement")]
        [Range(1f, 20f)]
        public float legacyBreakFactor = 10.0f;
        
        [Tooltip("Legacy gravity value from the original system")]
        public float legacyGravity = -9.81f;
        
        [Header("Target Parameters")]
        [Tooltip("Target jump height for custom physics (informational, for conversion)")]
        [Range(0.5f, 1.5f)]
        public float targetJumpHeight = 0.75f;
        
        [Tooltip("Speed limit for player input forces")]
        [Range(4f, 10f)]
        public float inputSpeedLimit = 6.0f;
        
        [Tooltip("Speed limit for physics calculations")]
        [Range(4f, 10f)]
        public float physicsSpeedLimit = 6.5f;
        
        [Tooltip("Total speed limit combining input and physics")]
        [Range(5f, 12f)]
        public float totalSpeedLimit = 7.0f;
        
        [Header("Jump Buffering Parameters")]
        [Tooltip("Time in milliseconds to buffer a jump input before it is discarded if not grounded")]
        public int jumpInputBufferTime = 300;
        
        [Tooltip("Time in milliseconds after ground contact during which a jump can still be triggered")]
        public int groundContactBufferTime = 200;
        
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
        
        // Conversion utility for physics parameter validation
        public float ConvertJumpForceToHeight()
        {
            // Empirical conversion formula (to be calibrated)
            float gravity = legacyGravity;
            return (legacyJumpForce * legacyJumpForce) / (2 * gravity);
        }
        
        public void ValidateParameters()
        {
            if (!validateParameterConversion) return;
            
            float convertedHeight = ConvertJumpForceToHeight();
            if (Mathf.Abs(convertedHeight - targetJumpHeight) > 0.1f && enableMigrationLogging)
            {
                UnityEngine.Debug.LogWarning($"Jump height mismatch: Legacy jump force converts to {convertedHeight:F2}, but target is {targetJumpHeight:F2}. Adjust parameters for consistency.");
            }
        }
    }
}
