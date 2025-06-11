# Task 4.6: GravitySwitchHelper Enhancement

## Objective
Update `GravitySwitchHelper` to support player-specific gravity changes, ensuring compatibility with existing GravitySwitch prefabs and integrating with `PlayerGravityTrigger`.

## Integration Steps
1. **Player-Specific Logic**:
   - Modify `GravitySwitchHelper` to detect and affect only the player ball entering the trigger zone.
   - Ensure no impact on environment objects or other players.
2. **Trigger Integration**:
   - Link `GravitySwitchHelper` with `PlayerGravityTrigger` to provide target gravity direction based on switch configuration.
   - Use `PlayerGravityTrigger.OnTriggerEntered` and `OnTriggerExited` for event handling.
3. **Configuration Mapping**:
   - Map existing GravitySwitch prefab settings (e.g., target direction, pivot point) to `PlayerGravityTrigger` properties.
4. **Debug Visualization**:
   - Enhance gizmo drawing to visualize gravity direction and pivot point for debugging.

## Code Snippet
```csharp
// In GravitySwitchHelper.cs
using UnityEngine;

namespace BlockBall.Physics
{
    public class GravitySwitchHelper : MonoBehaviour
    {
        [SerializeField] private Vector3 targetGravityDirection = Vector3.down;
        [SerializeField] private Vector3 pivotPointOffset = Vector3.zero;
        [SerializeField] private Color debugColor = Color.cyan;

        private PlayerGravityTrigger gravityTrigger;

        void Awake()
        {
            gravityTrigger = GetComponent<PlayerGravityTrigger>();
            if (gravityTrigger == null)
            {
                gravityTrigger = gameObject.AddComponent<PlayerGravityTrigger>();
            }
            // Configure trigger with GravitySwitch settings
            // Note: Actual configuration may use reflection or direct field access
            // to maintain backward compatibility with existing prefabs
        }

        void Start()
        {
            // Ensure trigger is set up with correct direction and pivot
            // This may involve custom inspector or initialization logic
        }

        // Called when player enters the switch's trigger zone
        private void OnPlayerEntered(PlayerGravityManager player)
        {
            // Legacy callback - now handled by PlayerGravityTrigger
            Debug.Log($"Player entered gravity switch {name}");
        }

        // Called when player exits the switch's trigger zone
        private void OnPlayerExited(PlayerGravityManager player)
        {
            // Legacy callback - now handled by PlayerGravityTrigger
            Debug.Log($"Player exited gravity switch {name}");
        }

        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = debugColor;
            Vector3 worldPivot = transform.position + pivotPointOffset;
            Gizmos.DrawSphere(worldPivot, 0.2f); // Pivot point
            Gizmos.DrawRay(worldPivot, targetGravityDirection * 2f); // Direction
        }
        #endif
    }
}
```

## Validation Steps
1. Verify `GravitySwitchHelper` integrates with `PlayerGravityTrigger`
2. Confirm existing GravitySwitch prefabs function without modification
3. Test that only player ball is affected by gravity switch
4. Check debug gizmos accurately represent direction and pivot

## Related Documents
- **Task_4.2_PlayerGravityTrigger.md**: Trigger component logic
- **03_Integration_Tasks_Summary.md**: Overview of integration sequence
- **01_Overview.md**: High-level mission for player-specific gravity
