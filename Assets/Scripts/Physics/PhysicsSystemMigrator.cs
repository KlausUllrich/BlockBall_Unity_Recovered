// File: PhysicsSystemMigrator.cs
// Purpose: Manages migration between old and new physics systems
// Version: 1.0.0
// Date: 2025-06-12

using UnityEngine;
using BlockBall.Settings;

namespace BlockBall.Physics
{
    /// <summary>
    /// Handles smooth migration between existing PhysicsObjectWrapper and new BallPhysics system.
    /// Allows gradual transition and A/B testing of physics implementations.
    /// </summary>
    [RequireComponent(typeof(PhysicsObjectWrapper))]
    public class PhysicsSystemMigrator : MonoBehaviour
    {
        [Header("Migration Settings")]
        [Tooltip("Enable new BallPhysics system alongside existing wrapper")]
        public bool enableNewPhysics = false;
        
        [Tooltip("Use new physics for movement (overrides wrapper movement)")]
        public bool useNewMovement = false;
        
        [Tooltip("Use new physics for jumping (overrides wrapper jumping)")]
        public bool useNewJumping = false;
        
        [Tooltip("Use new physics for ground detection (overrides wrapper detection)")]
        public bool useNewGroundDetection = false;
        
        [Header("Debug Comparison")]
        [Tooltip("Show comparison data between old and new systems")]
        public bool showDebugComparison = false;
        
        [Tooltip("Log performance differences between systems")]
        public bool logPerformanceComparison = false;
        
        // Component references
        private PhysicsObjectWrapper oldWrapper;
        private BallPhysics newBallPhysics;
        private PhysicsSettings physicsSettings;
        
        // Performance tracking
        private float oldSystemTime = 0f;
        private float newSystemTime = 0f;
        private int frameCounter = 0;
        
        // State comparison
        private Vector3 oldPosition;
        private Vector3 newPosition;
        private Vector3 oldVelocity;
        private Vector3 newVelocity;
        
        #region Unity Lifecycle
        
        void Awake()
        {
            oldWrapper = GetComponent<PhysicsObjectWrapper>();
            LoadPhysicsSettings();
        }
        
        void Start()
        {
            if (enableNewPhysics)
            {
                SetupNewPhysics();
            }
        }
        
        void Update()
        {
            if (!enableNewPhysics) return;
            
            frameCounter++;
            
            if (showDebugComparison)
            {
                CaptureSystemStates();
            }
            
            if (logPerformanceComparison && frameCounter % 60 == 0) // Log every second
            {
                LogPerformanceComparison();
            }
        }
        
        #endregion
        
        #region Setup and Configuration
        
        private void LoadPhysicsSettings()
        {
            physicsSettings = Resources.Load<PhysicsSettings>("PhysicsSettings");
            if (physicsSettings == null)
            {
                UnityEngine.Debug.LogWarning("PhysicsSettings not found, migration disabled");
                enableNewPhysics = false;
            }
        }
        
        private void SetupNewPhysics()
        {
            // Add BallPhysics component if not present
            newBallPhysics = GetComponent<BallPhysics>();
            if (newBallPhysics == null)
            {
                newBallPhysics = gameObject.AddComponent<BallPhysics>();
                UnityEngine.Debug.Log("Added BallPhysics component for migration testing");
            }
            
            // Configure based on migration settings
            ConfigurePhysicsComponents();
            
            UnityEngine.Debug.Log("Physics migration setup complete");
        }
        
        private void ConfigurePhysicsComponents()
        {
            if (physicsSettings == null) return;
            
            // When using new physics, we may need to modify old wrapper behavior
            if (useNewMovement || useNewJumping || useNewGroundDetection)
            {
                // The old wrapper will need to check these flags and delegate accordingly
                // This would require modification to PhysicsObjectWrapper, but for now we log
                UnityEngine.Debug.Log($"Migration flags set - Movement: {useNewMovement}, Jumping: {useNewJumping}, Ground: {useNewGroundDetection}");
            }
        }
        
        #endregion
        
        #region System Integration
        
        /// <summary>
        /// Called by PhysicsObjectWrapper to check if new movement should be used
        /// </summary>
        public bool ShouldUseNewMovement()
        {
            return enableNewPhysics && useNewMovement && newBallPhysics != null;
        }
        
        /// <summary>
        /// Called by PhysicsObjectWrapper to check if new jumping should be used
        /// </summary>
        public bool ShouldUseNewJumping()
        {
            return enableNewPhysics && useNewJumping && newBallPhysics != null;
        }
        
        /// <summary>
        /// Called by PhysicsObjectWrapper to check if new ground detection should be used
        /// </summary>
        public bool ShouldUseNewGroundDetection()
        {
            return enableNewPhysics && useNewGroundDetection && newBallPhysics != null;
        }
        
        /// <summary>
        /// Get ground state from new physics system
        /// </summary>
        public bool GetNewGroundState()
        {
            return newBallPhysics != null ? newBallPhysics.IsGrounded : false;
        }
        
        /// <summary>
        /// Process movement input through new physics system
        /// </summary>
        public void ProcessNewMovementInput(Vector2 input)
        {
            if (newBallPhysics != null)
            {
                // The new system processes input internally
                // This is a placeholder for integration
            }
        }
        
        /// <summary>
        /// Process jump input through new physics system
        /// </summary>
        public void ProcessNewJumpInput(bool jumpPressed)
        {
            if (newBallPhysics != null)
            {
                newBallPhysics.HandleJumpInput();
            }
        }
        
        #endregion
        
        #region Debug and Comparison
        
        private void CaptureSystemStates()
        {
            // Capture old system state
            oldPosition = oldWrapper.Position;
            oldVelocity = oldWrapper.Velocity;
            
            // Capture new system state
            if (newBallPhysics != null)
            {
                newPosition = newBallPhysics.Position;
                newVelocity = newBallPhysics.Velocity;
            }
        }
        
        private void LogPerformanceComparison()
        {
            if (oldSystemTime > 0f && newSystemTime > 0f)
            {
                float timeDifference = newSystemTime - oldSystemTime;
                float percentDifference = (timeDifference / oldSystemTime) * 100f;
                
                UnityEngine.Debug.Log($"Physics Performance Comparison - Old: {oldSystemTime:F4}ms, New: {newSystemTime:F4}ms, Difference: {percentDifference:F1}%");
            }
        }
        
        /// <summary>
        /// Enable/disable migration features at runtime
        /// </summary>
        [ContextMenu("Toggle New Movement")]
        public void ToggleNewMovement()
        {
            useNewMovement = !useNewMovement;
            UnityEngine.Debug.Log($"New movement system: {(useNewMovement ? "ENABLED" : "DISABLED")}");
        }
        
        [ContextMenu("Toggle New Jumping")]
        public void ToggleNewJumping()
        {
            useNewJumping = !useNewJumping;
            UnityEngine.Debug.Log($"New jumping system: {(useNewJumping ? "ENABLED" : "DISABLED")}");
        }
        
        [ContextMenu("Toggle New Ground Detection")]
        public void ToggleNewGroundDetection()
        {
            useNewGroundDetection = !useNewGroundDetection;
            UnityEngine.Debug.Log($"New ground detection: {(useNewGroundDetection ? "ENABLED" : "DISABLED")}");
        }
        
        [ContextMenu("Enable All New Systems")]
        public void EnableAllNewSystems()
        {
            enableNewPhysics = true;
            useNewMovement = true;
            useNewJumping = true;
            useNewGroundDetection = true;
            
            if (newBallPhysics == null)
            {
                SetupNewPhysics();
            }
            
            UnityEngine.Debug.Log("All new physics systems ENABLED");
        }
        
        [ContextMenu("Disable All New Systems")]
        public void DisableAllNewSystems()
        {
            enableNewPhysics = false;
            useNewMovement = false;
            useNewJumping = false;
            useNewGroundDetection = false;
            
            UnityEngine.Debug.Log("All new physics systems DISABLED (using legacy)");
        }
        
        #endregion
        
        #region Debug Visualization
        
        void OnGUI()
        {
            if (!showDebugComparison || !enableNewPhysics) return;
            
            GUILayout.BeginArea(new Rect(10, 100, 400, 300));
            GUILayout.Label("Physics Migration Comparison", GUI.skin.label);
            
            GUILayout.Label("Old System:");
            GUILayout.Label($"  Position: {oldPosition:F3}");
            GUILayout.Label($"  Velocity: {oldVelocity:F3}");
            
            GUILayout.Space(5);
            
            GUILayout.Label("New System:");
            GUILayout.Label($"  Position: {newPosition:F3}");
            GUILayout.Label($"  Velocity: {newVelocity:F3}");
            
            GUILayout.Space(5);
            
            Vector3 positionDiff = newPosition - oldPosition;
            Vector3 velocityDiff = newVelocity - oldVelocity;
            
            GUILayout.Label("Differences:");
            GUILayout.Label($"  Position: {positionDiff:F3} (mag: {positionDiff.magnitude:F3})");
            GUILayout.Label($"  Velocity: {velocityDiff:F3} (mag: {velocityDiff.magnitude:F3})");
            
            GUILayout.Space(10);
            
            GUILayout.Label("Active Systems:");
            GUILayout.Label($"  Movement: {(useNewMovement ? "NEW" : "OLD")}");
            GUILayout.Label($"  Jumping: {(useNewJumping ? "NEW" : "OLD")}");
            GUILayout.Label($"  Ground Detection: {(useNewGroundDetection ? "NEW" : "OLD")}");
            
            GUILayout.EndArea();
        }
        
        void OnDrawGizmos()
        {
            if (!showDebugComparison || !enableNewPhysics) return;
            
            // Draw comparison visualization
            if (newBallPhysics != null)
            {
                // Old system (red)
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(oldPosition, 0.4f);
                Gizmos.DrawLine(oldPosition, oldPosition + oldVelocity * 0.5f);
                
                // New system (green)
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(newPosition, 0.45f);
                Gizmos.DrawLine(newPosition, newPosition + newVelocity * 0.5f);
                
                // Difference line (yellow)
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(oldPosition, newPosition);
            }
        }
        
        #endregion
        
        #region Validation
        
        /// <summary>
        /// Validate that migration setup is correct
        /// </summary>
        [ContextMenu("Validate Migration Setup")]
        public void ValidateMigrationSetup()
        {
            bool isValid = true;
            
            if (oldWrapper == null)
            {
                UnityEngine.Debug.LogError("PhysicsObjectWrapper not found - required for migration");
                isValid = false;
            }
            
            if (enableNewPhysics && newBallPhysics == null)
            {
                UnityEngine.Debug.LogError("BallPhysics component missing but new physics enabled");
                isValid = false;
            }
            
            if (physicsSettings == null)
            {
                UnityEngine.Debug.LogError("PhysicsSettings not found - required for migration");
                isValid = false;
            }
            
            if (physicsSettings?.physicsMode != PhysicsMode.CustomPhysics && enableNewPhysics)
            {
                UnityEngine.Debug.LogWarning("PhysicsSettings mode is not CustomPhysics but new physics enabled");
            }
            
            if (isValid)
            {
                UnityEngine.Debug.Log("Migration setup validation passed");
            }
            else
            {
                UnityEngine.Debug.LogError("Migration setup validation failed - check errors above");
            }
        }
        
        #endregion
    }
}
