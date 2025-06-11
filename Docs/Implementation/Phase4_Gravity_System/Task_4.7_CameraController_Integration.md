# Task 4.7: CameraController Integration

## Objective
Adjust the camera controller to follow gravity direction changes, ensuring 'gravity down' remains visually downward for the player after gravity snaps.

## Integration Steps
1. **Gravity Direction Awareness**:
   - Access `PlayerGravityManager.CurrentGravityDirection` to determine the current gravity orientation.
   - Update camera target orientation to align with gravity direction.
2. **Smooth Rotation on Snap**:
   - Implement smooth camera rotation using `PhysicsSettings.Instance.cameraRotationSmoothTime` when gravity snaps on trigger exit.
   - Avoid jarring movements during instant transitions inside triggers.
3. **Player-Centric Focus**:
   - Ensure camera remains focused on player ball, adjusting perspective based on new gravity.
   - Maintain relative offset (e.g., third-person view) regardless of gravity direction.
4. **Validation of Visual Consistency**:
   - Test that level geometry appears correctly oriented relative to gravity (e.g., floor below, ceiling above).

## Code Snippet
```csharp
// In PlayerCameraController.cs
using UnityEngine;

namespace BlockBall.Physics
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private PlayerGravityManager gravityManager;
        [SerializeField] private Vector3 cameraOffset = new Vector3(0, 5, -10);
        
        private Quaternion targetRotation;
        private PhysicsSettings physicsSettings;
        
        void Start()
        {
            physicsSettings = PhysicsSettings.Instance;
            if (gravityManager == null)
            {
                gravityManager = playerTransform.GetComponent<PlayerGravityManager>();
            }
            UpdateCameraTargetRotation();
        }
        
        void LateUpdate()
        {
            UpdateCameraPosition();
            UpdateCameraRotation();
        }
        
        private void UpdateCameraPosition()
        {
            // Maintain relative offset to player based on gravity direction
            Vector3 gravityDown = -gravityManager.CurrentGravityDirection;
            Vector3 rotatedOffset = Quaternion.LookRotation(Vector3.Cross(gravityDown, Vector3.right), gravityDown) * cameraOffset;
            Vector3 targetPosition = playerTransform.position + rotatedOffset;
            transform.position = targetPosition;
        }
        
        private void UpdateCameraRotation()
        {
            if (gravityManager.IsTransitioning)
            {
                UpdateCameraTargetRotation();
            }
            
            // Smoothly interpolate to target rotation
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                Time.deltaTime / physicsSettings.cameraRotationSmoothTime
            );
        }
        
        private void UpdateCameraTargetRotation()
        {
            // Align camera so 'down' matches gravity direction
            Vector3 gravityDown = -gravityManager.CurrentGravityDirection;
            Vector3 forward = Vector3.Cross(gravityDown, Vector3.right).normalized;
            if (forward == Vector3.zero) forward = Vector3.Cross(gravityDown, Vector3.forward).normalized;
            targetRotation = Quaternion.LookRotation(forward, -gravityDown);
        }
    }
}
```

## Validation Steps
1. Confirm camera aligns 'down' with current gravity direction
2. Verify smooth rotation during gravity snap on trigger exit
3. Test camera stability during instant gravity change inside trigger
4. Ensure player remains in view with correct perspective

## Related Documents
- **Task_4.5_PlayerComponent_Integration.md**: Player class integration
- **03_Integration_Tasks_Summary.md**: Overview of integration sequence
- **1_BlockBall_Physics_Spec.md**: Spec for camera behavior on gravity snap
