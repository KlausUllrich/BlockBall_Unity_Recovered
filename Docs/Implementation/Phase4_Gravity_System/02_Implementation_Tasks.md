# Phase 4: Player Gravity System Implementation Tasks

## Task 4.1: Create PlayerGravityManager Component

### Objective
Implement the core player-specific gravity management system that handles trigger-based gravity transitions and cardinal direction snapping.

### Code Template
```csharp
using UnityEngine;
using System.Collections.Generic;

namespace BlockBall.Physics
{
    /// <summary>
    /// Manages gravity for a specific player ball - PLAYER ONLY, no environment effects
    /// Transitions smooth inside triggers, snaps to cardinal on exit
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerGravityManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PhysicsSettings physicsSettings;
        [SerializeField] private Rigidbody ballRigidbody;
        
        [Header("Current Gravity State")]
        [SerializeField] private Vector3 currentGravityDirection = Vector3.down;
        [SerializeField] private GravityTransitionState transitionState = GravityTransitionState.FreeSpace;
        
        // Trigger zone management
        private PlayerGravityTrigger activeTrigger = null;
        private List<PlayerGravityTrigger> overlappingTriggers = new List<PlayerGravityTrigger>();
        
        // Transition control
        private Vector3 transitionStartDirection;
        private Vector3 transitionTargetDirection;
        private float transitionTimer = 0f;
        
        // Cardinal directions for snapping
        private static readonly Vector3[] CardinalDirections = {
            Vector3.right, Vector3.left, Vector3.up, 
            Vector3.down, Vector3.forward, Vector3.back
        };
        
        public enum GravityTransitionState
        {
            FreeSpace,          // Not in any trigger, using cardinal direction
            InsideTrigger,      // Inside trigger, smooth transition to target
            ExitingTrigger      // Just exited trigger, snapping to cardinal
        }
        
        void Start()
        {
            if (physicsSettings == null)
                physicsSettings = Resources.Load<PhysicsSettings>("PhysicsSettings");
                
            if (ballRigidbody == null)
                ballRigidbody = GetComponent<Rigidbody>();
            
            // Disable Unity's global gravity for this player
            ballRigidbody.useGravity = false;
        }
        
        void FixedUpdate()
        {
            UpdateGravityTransition();
            ApplyGravityForce();
        }
        
        private void UpdateGravityTransition()
        {
            switch (transitionState)
            {
                case GravityTransitionState.FreeSpace:
                    // No transition needed, maintain current cardinal direction
                    break;
                    
                case GravityTransitionState.InsideTrigger:
                    // Smooth transition to trigger's target direction
                    transitionTimer += Time.fixedDeltaTime;
                    float t = transitionTimer / physicsSettings.gravityTransitionTime;
                    
                    if (t >= 1f)
                    {
                        // Transition complete
                        currentGravityDirection = transitionTargetDirection;
                        transitionState = GravityTransitionState.FreeSpace;
                    }
                    else
                    {
                        // Smooth interpolation
                        currentGravityDirection = Vector3.Slerp(
                            transitionStartDirection, 
                            transitionTargetDirection, 
                            Mathf.SmoothStep(0f, 1f, t)
                        ).normalized;
                    }
                    break;
                    
                case GravityTransitionState.ExitingTrigger:
                    // Immediate snap to nearest cardinal
                    currentGravityDirection = SnapToNearestCardinal(currentGravityDirection);
                    transitionState = GravityTransitionState.FreeSpace;
                    break;
            }
        }
        
        private void ApplyGravityForce()
        {
            // Apply gravity force ONLY to this player ball
            Vector3 gravityForce = currentGravityDirection * physicsSettings.gravityStrength * ballRigidbody.mass;
            ballRigidbody.AddForce(gravityForce, ForceMode.Force);
        }
        
        private Vector3 SnapToNearestCardinal(Vector3 direction)
        {
            Vector3 closest = Vector3.down; // fallback
            float maxDot = -1f;
            
            foreach (var cardinal in CardinalDirections)
            {
                float dot = Vector3.Dot(direction.normalized, cardinal);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    closest = cardinal;
                }
            }
            
            return closest;
        }
        
        // Called by PlayerGravityTrigger when player enters trigger zone
        public void OnTriggerEntered(PlayerGravityTrigger trigger)
        {
            overlappingTriggers.Add(trigger);
            UpdateActiveTrigger();
        }
        
        // Called by PlayerGravityTrigger when player exits trigger zone
        public void OnTriggerExited(PlayerGravityTrigger trigger)
        {
            overlappingTriggers.Remove(trigger);
            
            if (activeTrigger == trigger)
            {
                // Exiting the active trigger - SNAP TO CARDINAL
                transitionState = GravityTransitionState.ExitingTrigger;
                activeTrigger = null;
                UpdateActiveTrigger();
            }
        }
        
        private void UpdateActiveTrigger()
        {
            if (overlappingTriggers.Count == 0)
            {
                activeTrigger = null;
                return;
            }
            
            // Find closest trigger by pivot point distance
            PlayerGravityTrigger closest = null;
            float closestDistance = float.MaxValue;
            
            Vector3 playerPosition = transform.position;
            foreach (var trigger in overlappingTriggers)
            {
                float distance = Vector3.Distance(playerPosition, trigger.PivotPoint);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = trigger;
                }
            }
            
            if (closest != activeTrigger)
            {
                // New trigger is now active - start smooth transition
                activeTrigger = closest;
                StartTransitionToTrigger(closest);
            }
        }
        
        private void StartTransitionToTrigger(PlayerGravityTrigger trigger)
        {
            transitionStartDirection = currentGravityDirection;
            transitionTargetDirection = trigger.TargetGravityDirection;
            transitionTimer = 0f;
            transitionState = GravityTransitionState.InsideTrigger;
        }
        
        // Public properties for external access
        public Vector3 CurrentGravityDirection => currentGravityDirection;
        public bool IsTransitioning => transitionState != GravityTransitionState.FreeSpace;
        
        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // Draw gravity direction
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, currentGravityDirection * 2f);
            
            // Draw transition state
            UnityEditor.Handles.Label(transform.position + Vector3.up, 
                $"Gravity State: {transitionState}\nDirection: {currentGravityDirection}");
        }
        #endif
    }
}
```

### Validation Steps
1. Add component to Player GameObject
2. Assign PhysicsSettings reference
3. Verify gravity applies only to player ball
4. Test that rigidbody.useGravity is disabled
5. Confirm cardinal direction snapping works

---

## Task 4.2: Create PlayerGravityTrigger Component

### Objective
Create trigger zones that detect player entry/exit and communicate with PlayerGravityManager.

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    /// <summary>
    /// Trigger component for gravity switches - detects player entry/exit
    /// Replaces the old GravitySwitchHelper system
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class PlayerGravityTrigger : MonoBehaviour
    {
        [Header("Gravity Configuration")]
        [Tooltip("The gravity direction this trigger applies")]
        [SerializeField] private Vector3 targetGravityDirection = Vector3.down;
        
        [Tooltip("The center point for distance calculations")]
        [SerializeField] private Transform pivotTransform;
        
        [Header("Trigger Settings")]
        [Tooltip("Only affect objects on this layer")]
        [SerializeField] private LayerMask playerLayerMask = 1;
        
        private Collider triggerCollider;
        
        // Public properties
        public Vector3 TargetGravityDirection => targetGravityDirection.normalized;
        public Vector3 PivotPoint => pivotTransform != null ? pivotTransform.position : transform.position;
        
        void Start()
        {
            triggerCollider = GetComponent<Collider>();
            triggerCollider.isTrigger = true;
            
            // Ensure we have a pivot point
            if (pivotTransform == null)
                pivotTransform = transform;
        }
        
        void OnTriggerEnter(Collider other)
        {
            // Check if this is a player ball
            if (!IsPlayerBall(other))
                return;
                
            PlayerGravityManager gravityManager = other.GetComponent<PlayerGravityManager>();
            if (gravityManager != null)
            {
                gravityManager.OnTriggerEntered(this);
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            // Check if this is a player ball
            if (!IsPlayerBall(other))
                return;
                
            PlayerGravityManager gravityManager = other.GetComponent<PlayerGravityManager>();
            if (gravityManager != null)
            {
                gravityManager.OnTriggerExited(this);
            }
        }
        
        private bool IsPlayerBall(Collider other)
        {
            // Check layer mask
            return (playerLayerMask.value & (1 << other.gameObject.layer)) != 0;
        }
        
        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // Draw trigger bounds
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            if (GetComponent<BoxCollider>())
            {
                BoxCollider box = GetComponent<BoxCollider>();
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (GetComponent<SphereCollider>())
            {
                SphereCollider sphere = GetComponent<SphereCollider>();
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            }
            
            // Draw gravity direction
            Gizmos.color = Color.green;
            Vector3 worldPos = transform.position;
            Gizmos.DrawRay(worldPos, targetGravityDirection.normalized * 3f);
            
            // Draw pivot point
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(PivotPoint, 0.2f);
        }
        
        void OnDrawGizmosSelected()
        {
            // Enhanced visualization when selected
            Gizmos.color = Color.cyan;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, 
                $"Target Gravity: {targetGravityDirection}\nPivot: {PivotPoint}");
        }
        #endif
    }
}
```

### Validation Steps
1. Add to each GravitySwitch prefab
2. Configure targetGravityDirection
3. Set pivotTransform reference
4. Test trigger detection with player
5. Verify layer mask filtering works

---

## Task 4.3: Update Existing GravitySwitch Integration

### Objective
Integrate new trigger system with existing GravitySwitch prefabs and maintain backward compatibility.

### Implementation Steps

#### Step 1: Update GravitySwitch.cs
Add the new PlayerGravityTrigger component to existing GravitySwitch objects:

```csharp
// Add this to existing GravitySwitch.cs
namespace BlockBall.GameObjects
{
    public partial class GravitySwitch : MonoBehaviour
    {
        [Header("New Gravity System")]
        [SerializeField] private PlayerGravityTrigger gravityTrigger;
        
        void Start()
        {
            // Ensure we have the new trigger component
            if (gravityTrigger == null)
                gravityTrigger = GetComponent<PlayerGravityTrigger>();
                
            if (gravityTrigger == null)
                gravityTrigger = gameObject.AddComponent<PlayerGravityTrigger>();
            
            // Configure trigger based on switch settings
            ConfigureGravityTrigger();
        }
        
        private void ConfigureGravityTrigger()
        {
            // Set gravity direction based on switch type
            Vector3 gravityDirection = CalculateGravityDirection();
            
            // Access private field using reflection (temporary compatibility)
            var field = typeof(PlayerGravityTrigger).GetField("targetGravityDirection", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(gravityTrigger, gravityDirection);
        }
        
        private Vector3 CalculateGravityDirection()
        {
            // Use existing switch logic to determine gravity direction
            // This depends on your current GravitySwitch implementation
            return transform.up; // placeholder - adapt to your existing code
        }
    }
}
```

#### Step 2: Migration Script
Create a script to automatically update existing GravitySwitch prefabs:

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace BlockBall.Physics.Editor
{
    public class GravitySwitchMigrationTool : EditorWindow
    {
        [MenuItem("BlockBall/Migrate Gravity Switches")]
        public static void ShowWindow()
        {
            GetWindow<GravitySwitchMigrationTool>("Gravity Switch Migration");
        }
        
        void OnGUI()
        {
            GUILayout.Label("Gravity Switch Migration Tool", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            if (GUILayout.Button("Add PlayerGravityTrigger to All GravitySwitches"))
            {
                MigrateAllGravitySwitches();
            }
            
            GUILayout.Space(10);
            GUILayout.Label("This will add PlayerGravityTrigger components to all GravitySwitch prefabs in the scene.");
        }
        
        private void MigrateAllGravitySwitches()
        {
            var gravitySwitches = FindObjectsOfType<GravitySwitch>();
            int migratedCount = 0;
            
            foreach (var gravitySwitch in gravitySwitches)
            {
                if (gravitySwitch.GetComponent<PlayerGravityTrigger>() == null)
                {
                    gravitySwitch.gameObject.AddComponent<PlayerGravityTrigger>();
                    migratedCount++;
                }
            }
            
            Debug.Log($"Migrated {migratedCount} GravitySwitch objects with PlayerGravityTrigger components.");
        }
    }
}
#endif
```

### Validation Steps
1. Run migration tool on existing scenes
2. Verify all GravitySwitch objects now have PlayerGravityTrigger
3. Test existing gravity switch functionality
4. Confirm backward compatibility maintained

---

## Task 4.4: PhysicsSettings Integration

### Objective
Create the centralized PhysicsSettings ScriptableObject for all gravity configuration.

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    /// <summary>
    /// Single source of truth for all physics settings
    /// Prevents hardcoded values and provides user-friendly configuration
    /// </summary>
    [CreateAssetMenu(fileName = "PhysicsSettings", menuName = "BlockBall/Physics Settings")]
    public class PhysicsSettings : ScriptableObject
    {
        [Header("Gravity System")]
        [Tooltip("Base gravity strength affecting player ball only")]
        [Range(1f, 30f)]
        public float gravityStrength = 9.81f;
        
        [Tooltip("Time to smoothly transition between gravity directions inside triggers")]
        [Range(0.1f, 2f)]  
        public float gravityTransitionTime = 0.3f;
        
        [Header("Player Ball Physics")]
        [Tooltip("Jump height in Unity units")]
        [Range(0.5f, 10f)]
        public float jumpHeight = 3f;
        
        [Tooltip("Maximum rolling speed")]
        [Range(3f, 15f)]
        public float maxRollSpeed = 8f;
        
        [Tooltip("Rolling acceleration")]
        [Range(1f, 20f)]
        public float rollAcceleration = 10f;
        
        [Header("Surface Properties")]
        [Tooltip("Default surface bounciness")]
        [Range(0f, 1f)]
        public float defaultBounciness = 0.3f;
        
        [Tooltip("Default surface friction")]
        [Range(0f, 2f)]
        public float defaultFriction = 0.8f;
        
        [Header("Advanced Settings")]
        [Tooltip("Maximum number of physics substeps per frame")]
        [Range(1, 10)]
        public int maxSubsteps = 8;
        
        [Tooltip("Fixed timestep for physics (50Hz = 0.02)")]
        [Range(0.01f, 0.05f)]
        public float fixedTimestep = 0.02f;
        
        // Calculated properties for physics systems
        public float GetJumpForce(float ballMass)
        {
            // Calculate force needed for desired jump height
            // F = ma, where a gives velocity for height h: v = sqrt(2gh)
            float jumpVelocity = Mathf.Sqrt(2f * gravityStrength * jumpHeight);
            return ballMass * jumpVelocity / fixedTimestep;
        }
        
        public float GetTerminalVelocity()
        {
            // Terminal velocity calculation for air resistance
            return maxRollSpeed * 1.5f; // 50% higher than max roll speed
        }
        
        public float GetDragCoefficient()
        {
            // Air resistance calculation
            return gravityStrength / (GetTerminalVelocity() * GetTerminalVelocity());
        }
        
        #if UNITY_EDITOR
        [Header("Debug Visualization")]
        public bool showGravityDebug = true;
        public bool showTransitionDebug = false;
        public Color gravityRayColor = Color.red;
        public Color transitionRayColor = Color.yellow;
        #endif
    }
}
```

### Asset Creation Steps
1. Right-click in Project window
2. Create → BlockBall → Physics Settings
3. Name it "PhysicsSettings"
4. Place in Resources folder for easy loading
5. Configure all values according to game requirements

### Validation Steps
1. Create PhysicsSettings asset
2. Reference from PlayerGravityManager
3. Test real-time value changes in play mode
4. Verify all calculated properties work correctly

---

## Integration Testing Checklist

### Basic Functionality
- [ ] Player gravity only affects player ball, not environment
- [ ] Gravity transitions smooth inside trigger zones (0.3s default)
- [ ] Gravity snaps to cardinal direction when exiting triggers
- [ ] Multiple triggers handled correctly (closest pivot wins)
- [ ] All six cardinal directions work correctly

### Edge Cases
- [ ] Player entering multiple overlapping triggers
- [ ] Player exiting trigger while transitioning
- [ ] Trigger with non-cardinal target direction
- [ ] Very fast player movement through triggers
- [ ] Player stuck at trigger boundary

### Performance
- [ ] No frame rate drops during gravity transitions
- [ ] Memory allocation stays at zero during gameplay
- [ ] Gravity calculations under 0.1ms per update

### Integration
- [ ] Camera follows gravity changes correctly
- [ ] Ball physics state machine works with gravity changes
- [ ] Collision detection accounts for gravity direction
- [ ] UI elements orient correctly with gravity

### Compatibility
- [ ] Existing GravitySwitch prefabs work unchanged
- [ ] Save/load game states preserve gravity settings
- [ ] Multiplayer handles independent player gravity
- [ ] Level editor supports new gravity system

## Completion Criteria

Phase 4 is complete when:
1. All implementation tasks pass validation
2. Integration testing checklist is 100% complete
3. Performance targets are met consistently
4. No regressions in existing functionality
5. Documentation is updated with any changes made during implementation

The gravity system must handle the critical requirement: **gravity transitions only happen inside triggers, and always snap to cardinal directions when exiting any trigger zone.**
