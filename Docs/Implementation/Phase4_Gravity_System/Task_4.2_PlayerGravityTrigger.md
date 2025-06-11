# Task 4.2: Create PlayerGravityTrigger Component

## Objective
Implement trigger component for gravity zones that detects player entry/exit and provides target gravity direction.

## Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    /// <summary>
    /// Trigger component for gravity zones - detects player entry/exit
    /// Provides target gravity direction based on zone configuration
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class PlayerGravityTrigger : MonoBehaviour
    {
        [Header("Gravity Zone Configuration")]
        [SerializeField] private Vector3 targetGravityDirection = Vector3.down;
        [SerializeField] private Vector3 pivotPoint;
        [SerializeField] private bool isAttractionZone = true;
        
        [Header("Debug Visualization")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color gizmoColor = Color.cyan;
        
        public Vector3 TargetGravityDirection => targetGravityDirection;
        public Vector3 PivotPoint => transform.TransformPoint(pivotPoint);
        
        private void Awake()
        {
            // Ensure collider is trigger
            var collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }
            else
            {
                Debug.LogError($"{nameof(PlayerGravityTrigger)} on {name} requires a Collider!");
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var playerGravity = other.GetComponent<PlayerGravityManager>();
            if (playerGravity != null)
            {
                playerGravity.OnTriggerEntered(this);
                Debug.Log($"Player entered gravity trigger {name}");
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            var playerGravity = other.GetComponent<PlayerGravityManager>();
            if (playerGravity != null)
            {
                playerGravity.OnTriggerExited(this);
                Debug.Log($"Player exited gravity trigger {name}");
            }
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showGizmos) return;
            
            // Draw trigger zone
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            
            var collider = GetComponent<Collider>();
            if (collider is BoxCollider box)
            {
                Gizmos.DrawCube(box.center, box.size);
            }
            else if (collider is SphereCollider sphere)
            {
                Gizmos.DrawSphere(sphere.center, sphere.radius);
            }
            
            // Draw target gravity direction
            Vector3 worldPivot = transform.TransformPoint(pivotPoint);
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(worldPivot, targetGravityDirection * 2f);
            
            // Draw pivot point
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(worldPivot, 0.2f);
        }
        #endif
    }
}
```

## Validation Steps
1. Attach to GravitySwitch prefabs
2. Verify trigger detection on player entry/exit
3. Confirm target gravity direction calculation
4. Test pivot point distance for multi-zone selection
5. Check debug visualization accuracy

## Related Documents
- **Task_4.1_PlayerGravityManager.md**: Manager component logic
- **03_Integration_Tasks.md**: Integration with GravitySwitchHelper
