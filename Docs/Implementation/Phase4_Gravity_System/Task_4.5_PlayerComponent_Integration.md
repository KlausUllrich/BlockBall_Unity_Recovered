# Task 4.5: Player Component Integration

## Objective
Extend the Player class to integrate `PlayerGravityComponent` and respond to gravity direction changes, updating physics and camera orientation.

## Integration Steps
1. **Add PlayerGravityComponent**:
   - Attach `PlayerGravityManager` to Player GameObject as a component.
   - Ensure reference to `ballPhysics` for gravity direction updates.
2. **Update Physics Response**:
   - Modify `ballPhysics.SetGravityDirection` call to use current gravity from `PlayerGravityManager.CurrentGravityDirection`.
   - Ensure physics calculations (ground detection, contact points) respect new gravity direction.
3. **Camera Orientation Link**:
   - Update camera target orientation based on gravity direction during snap on exit.
   - Use `PhysicsSettings.Instance.cameraRotationSmoothTime` for smooth rotation.
4. **State Machine Adaptation**:
   - Adjust player state machine to handle gravity transitions in all states (grounded, airborne, rolling).

## Code Snippet
```csharp
// In Player.cs
private PlayerGravityManager gravityManager;

void Awake()
{
    gravityManager = GetComponent<PlayerGravityManager>();
    // Other initialization
}

void UpdateCameraOrientation()
{
    Vector3 gravityDir = gravityManager.CurrentGravityDirection;
    // Adjust camera target to align 'down' with gravity
    // Use PhysicsSettings.Instance.cameraRotationSmoothTime for smoothing
}

void FixedUpdate()
{
    // Update physics with current gravity direction
    ballPhysics.SetGravityDirection(gravityManager.CurrentGravityDirection);
    // Other physics updates
}
```

## Validation Steps
1. Confirm `PlayerGravityManager` component is active on Player
2. Verify physics updates reflect current gravity direction
3. Test camera orientation adjusts to gravity snap on trigger exit
4. Ensure player states remain consistent during gravity changes

## Related Documents
- **Task_4.1_PlayerGravityManager.md**: Core gravity component logic
- **Task_4.7_CameraController_Integration.md**: Detailed camera adjustment
- **03_Integration_Tasks_Summary.md**: Overview of integration sequence
