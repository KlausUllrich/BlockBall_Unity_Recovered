using UnityEngine;
using BlockBall.Physics;

namespace BlockBall.Settings
{
    [CreateAssetMenu(fileName = "PhysicsSettings", menuName = "BlockBall/Physics Settings")]
    public class PhysicsSettings : ScriptableObject
    {
        // Physics Mode for migration
        [Header("Migration Settings")]
        [Tooltip("Select the physics system mode for migration: Unity (existing), Custom (new system), or Hybrid (transitional).")]
        public BlockBall.Physics.PhysicsMode physicsMode = BlockBall.Physics.PhysicsMode.UnityPhysics;
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
        
        [Tooltip("Current gravity used in PlayerCameraController")]
        public float legacyGravity = -9.81f;
        
        [Header("Target Custom Physics Parameters")]
        [Tooltip("Target jump height in Unity units (6 Bixels)")]
        [Range(0.5f, 1.5f)]
        public float targetJumpHeight = 0.75f;
        
        [Tooltip("Maximum input speed limit")]
        [Range(4f, 10f)]
        public float inputSpeedLimit = 6.0f;
        
        [Tooltip("Physics calculation speed limit")]
        [Range(4f, 10f)]
        public float physicsSpeedLimit = 6.5f;
        
        [Tooltip("Total speed limit (input + physics)")]
        [Range(5f, 12f)]
        public float totalSpeedLimit = 7.0f;
        
        [Header("Deterministic Math Parameters")]
        [Tooltip("Scale factor for fixed-point arithmetic in physics calculations")]
        [Range(1000f, 10000000f)]
        public float fixedPointScale = 1000000.0f;
        
        [Tooltip("Accumulation threshold for physics state updates")]
        [Range(100f, 10000f)]
        public float accumulationThreshold = 1000.0f;
        
        [Header("Performance Settings")]
        [Tooltip("Maximum time (ms) allowed for a single physics update")]
        [Range(1f, 10f)]
        public float maxPhysicsUpdateMs = 2.0f;
        
        [Tooltip("Physics update frequency (Hz)")]
        [Range(30f, 120f)]
        public float physicsUpdateHz = 50.0f;
        
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
