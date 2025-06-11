# Task 4.1: Create PlayerGravityManager Component

## Objective
Implement the core player-specific gravity management system that handles trigger-based gravity transitions and cardinal direction snapping.

## Code Template
```csharp
using UnityEngine;
using System.Collections.Generic;

namespace BlockBall.Physics
{
    /// <summary>
    /// Manages gravity for a specific player ball - PLAYER ONLY, no environment effects
    /// Transitions instant inside triggers, snaps to cardinal on exit
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
        private Vector3 transitionTargetDirection;
        
        // Cardinal directions for snapping
        private static readonly Vector3[] CardinalDirections = {
            Vector3.right, Vector3.left, Vector3.up, 
            Vector3.down, Vector3.forward, Vector3.back
        };
        
        public enum GravityTransitionState
        {
            FreeSpace,          // Not in any trigger, using cardinal direction
            InsideTrigger,      // Inside trigger, instant transition to target
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
                    // Instant transition to trigger's target direction
                    currentGravityDirection = transitionTargetDirection;
                    transitionState = GravityTransitionState.FreeSpace;
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
                // New trigger is now active - start instant transition
                activeTrigger = closest;
                StartTransitionToTrigger(closest);
            }
        }
        
        private void StartTransitionToTrigger(PlayerGravityTrigger trigger)
        {
            transitionTargetDirection = trigger.TargetGravityDirection;
            transitionState = GravityTransitionState.InsideTrigger;
        }
        
        // Public properties for external access
        public Vector3 CurrentGravityDirection => currentGravityDirection;
        public bool IsTransitioning => transitionState != GravityTransitionState.FreeSpace;
        
        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // Draw gravity direction
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, currentGravityDirection * 2f);
        }
        #endif
    }
}
```

## Validation Steps
1. Add component to Player GameObject
2. Verify gravity force applies only to player ball
3. Confirm instant transition on entering trigger
4. Confirm snap to cardinal direction on exit
5. Test multi-zone handling selects closest trigger

## Related Documents
- **Task_4.2_PlayerGravityTrigger.md**: Trigger component logic
- **03_Integration_Tasks.md**: Integration with player and camera
