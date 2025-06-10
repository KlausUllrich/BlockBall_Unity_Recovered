# Phase 4: Integration Tasks

## Task 4.4: Enhance GravitySwitchHelper

### Objective
Update the existing GravitySwitchHelper to work with the new PlayerGravityComponent system.

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Level
{
    /// <summary>
    /// Enhanced gravity switch helper with player-specific gravity support
    /// </summary>
    public class GravitySwitchHelper : MonoBehaviour
    {
        [Header("Gravity Configuration")]
        [SerializeField] private Vector3 axisStart = Vector3.zero;
        [SerializeField] private Vector3 axisEnd = Vector3.up;
        [SerializeField] private bool isAttraction = true;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugGizmos = true;
        [SerializeField] private Color gizmoColor = Color.cyan;
        
        private void OnTriggerStay(Collider pCollider)
        {
            // Check if it's a player
            var player = pCollider.GetComponent<Player>();
            if (player?.PlayerGravity == null) return;
            
            // Transform axis to world space
            Vector3 worldAxisStart = transform.TransformPoint(axisStart);
            Vector3 worldAxisEnd = transform.TransformPoint(axisEnd);
            
            // Calculate new gravity direction based on player position
            Vector3 playerPosition = player.transform.position;
            Vector3 newDirection = BlockBall.Physics.GravityDirectionUtility.CalculateGravityFromLine(
                worldAxisStart, worldAxisEnd, playerPosition, isAttraction);
            
            // Apply smooth gravity transition while in trigger
            player.PlayerGravity.SetTargetGravity(newDirection, smooth: true);
        }
        
        private void OnTriggerExit(Collider pCollider)
        {
            // Check if it's a player
            var player = pCollider.GetComponent<Player>();
            if (player?.PlayerGravity == null) return;
            
            // Snap to nearest cardinal direction on exit
            player.PlayerGravity.SnapToNearestCardinal();
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showDebugGizmos) return;
            
            // Draw gravity axis line
            Vector3 worldStart = transform.TransformPoint(axisStart);
            Vector3 worldEnd = transform.TransformPoint(axisEnd);
            
            Gizmos.color = gizmoColor;
            Gizmos.DrawLine(worldStart, worldEnd);
            
            // Draw direction indicators
            Vector3 direction = (worldEnd - worldStart).normalized;
            Vector3 center = (worldStart + worldEnd) * 0.5f;
            
            if (isAttraction)
            {
                // Draw arrows pointing toward the line
                Gizmos.DrawRay(center + Vector3.right * 0.5f, -Vector3.right * 0.3f);
                Gizmos.DrawRay(center - Vector3.right * 0.5f, Vector3.right * 0.3f);
            }
            else
            {
                // Draw arrows pointing away from the line
                Gizmos.DrawRay(center, Vector3.right * 0.3f);
                Gizmos.DrawRay(center, -Vector3.right * 0.3f);
            }
            
            // Draw trigger zone
            var collider = GetComponent<Collider>();
            if (collider != null)
            {
                Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
                Gizmos.matrix = transform.localToWorldMatrix;
                
                if (collider is BoxCollider box)
                    Gizmos.DrawCube(box.center, box.size);
                else if (collider is SphereCollider sphere)
                    Gizmos.DrawSphere(sphere.center, sphere.radius);
            }
        }
        #endif
    }
}
```

### Integration Steps
1. Update existing GravitySwitchHelper.cs with this enhanced version
2. Ensure all existing GravitySwitch prefabs continue working
3. Test smooth transitions inside triggers
4. Verify cardinal snapping on trigger exit
5. Confirm debug gizmos show correct gravity direction

---

## Task 4.5: Integrate with Player Component

### Objective
Add PlayerGravityComponent to the Player class and ensure proper initialization.

### Code Changes for Player.cs
```csharp
// Add to Player.cs

[Header("Gravity System")]
[SerializeField] private PlayerGravityComponent playerGravity;

public PlayerGravityComponent PlayerGravity => playerGravity;

private void Awake()
{
    // Existing Awake code...
    
    // Initialize gravity component
    if (playerGravity == null)
        playerGravity = GetComponent<PlayerGravityComponent>();
        
    if (playerGravity == null)
    {
        Debug.LogError($"Player {name} missing PlayerGravityComponent!");
    }
}

private void Start()
{
    // Existing Start code...
    
    // Setup gravity event listeners
    if (playerGravity != null)
    {
        playerGravity.OnGravityChanged += HandleGravityChanged;
    }
}

private void OnDestroy()
{
    // Cleanup gravity event listeners
    if (playerGravity != null)
    {
        playerGravity.OnGravityChanged -= HandleGravityChanged;
    }
}

private void HandleGravityChanged(Vector3 newGravityDirection)
{
    // Update camera orientation
    if (cameraController != null)
    {
        Vector3 upDirection = -newGravityDirection;
        cameraController.SetGravityUp(upDirection);
    }
    
    // Update ground detection for physics
    if (ballPhysics != null)
    {
        ballPhysics.SetGravityDirection(newGravityDirection);
    }
    
    Debug.Log($"Player {name} gravity changed to: {newGravityDirection}");
}
```

### Integration Steps
1. Add PlayerGravityComponent to Player prefab
2. Update Player.cs with gravity integration code
3. Ensure proper initialization order
4. Test gravity changes affect camera and physics
5. Verify multiple players have independent gravity

---

## Task 4.6: Create Physics Settings Asset

### Objective
Create default PhysicsSettings asset and setup project configuration.

### Asset Creation Steps
1. **Create Default Asset:**
   - Right-click in Project window
   - Create > BlockBall > Physics Settings
   - Name: "DefaultPhysicsSettings"
   - Place in Resources folder for automatic loading

2. **Configure Default Values:**
   ```
   Gravity Strength: 9.81
   Gravity Transition Time: 0.3
   Jump Height: 4.0
   Roll Acceleration: 8.0
   Max Roll Speed: 15.0
   Bounciness: 0.3
   Friction: 0.8
   ```

3. **Setup Level-Specific Overrides:**
   - Create PhysicsSettings variants for different level types
   - Example: "LowGravityPhysics" for space levels
   - Example: "HighSpeedPhysics" for racing levels

### Resource Loading Code
```csharp
// Add to a PhysicsManager or GameManager
public static PhysicsSettings LoadPhysicsSettings(string settingsName = "DefaultPhysicsSettings")
{
    var settings = Resources.Load<PhysicsSettings>(settingsName);
    if (settings == null)
    {
        Debug.LogError($"Could not load PhysicsSettings: {settingsName}");
        return CreateDefaultSettings();
    }
    return settings;
}

private static PhysicsSettings CreateDefaultSettings()
{
    var settings = ScriptableObject.CreateInstance<PhysicsSettings>();
    // Set default values...
    return settings;
}
```

---

## Task 4.7: Performance Optimization

### Objective
Ensure gravity system meets performance requirements (<0.1ms per update).

### Optimization Strategies

#### 1. Update Frequency Optimization
```csharp
// Only update gravity when transitioning
private void FixedUpdate()
{
    if (isTransitioning)
    {
        ProcessGravityTransition();
    }
    
    // Always apply gravity (lightweight operation)
    ApplyGravity();
}
```

#### 2. Vector Calculation Caching
```csharp
// Cache gravity force calculation
private Vector3 cachedGravityForce;
private bool gravityForceDirty = true;

private void ApplyGravity()
{
    if (gravityForceDirty)
    {
        cachedGravityForce = currentGravityDirection * physicsSettings.gravityStrength;
        gravityForceDirty = false;
    }
    
    ballRigidbody.AddForce(cachedGravityForce, ForceMode.Acceleration);
}
```

#### 3. Transition State Pooling
```csharp
// Avoid allocation during transitions
private static readonly Stack<GravityTransitionState> transitionPool = new Stack<GravityTransitionState>();

private GravityTransitionState GetPooledTransitionState()
{
    return transitionPool.Count > 0 ? transitionPool.Pop() : new GravityTransitionState();
}

private void ReturnTransitionState(GravityTransitionState state)
{
    state.Reset();
    transitionPool.Push(state);
}
```

#### 4. Profiling Integration
```csharp
#if UNITY_EDITOR
using Unity.Profiling;

private static readonly ProfilerMarker gravityUpdateMarker = new ProfilerMarker("PlayerGravity.Update");
private static readonly ProfilerMarker gravityTransitionMarker = new ProfilerMarker("PlayerGravity.Transition");

private void ProcessGravityTransition()
{
    using (gravityTransitionMarker.Auto())
    {
        // Transition logic...
    }
}
#endif
```

### Performance Validation
1. **Unity Profiler**: Monitor PlayerGravityComponent update time
2. **Target**: <0.1ms per player per frame
3. **Stress Test**: 10+ players with different gravity states
4. **Memory**: Zero allocation during normal operation

---

## Task 4.8: Testing and Validation

### Unit Tests
Create automated tests for core gravity functionality:

```csharp
[Test]
public void CardinalDirectionSnapping_VariousInputs_ReturnsNearestCardinal()
{
    // Test various input directions snap to correct cardinals
    Assert.AreEqual(Vector3.down, GravityDirectionUtility.GetNearestCardinalDirection(Vector3.down));
    Assert.AreEqual(Vector3.down, GravityDirectionUtility.GetNearestCardinalDirection(new Vector3(0.1f, -0.9f, 0.1f)));
    Assert.AreEqual(Vector3.right, GravityDirectionUtility.GetNearestCardinalDirection(new Vector3(0.8f, 0.2f, 0.1f)));
}

[Test]
public void GravityTransition_SmoothInterpolation_CompletesCorrectly()
{
    // Test smooth transitions complete in expected time
    var gravityComponent = CreateTestGravityComponent();
    gravityComponent.SetTargetGravity(Vector3.right, smooth: true);
    
    // Simulate fixed update calls
    for (int i = 0; i < 20; i++) // Assuming 0.3s / 0.02s fixed timestep
    {
        gravityComponent.SimulateFixedUpdate();
    }
    
    Assert.IsFalse(gravityComponent.IsTransitioning);
    Assert.AreEqual(Vector3.right, gravityComponent.CurrentGravityDirection);
}
```

### Integration Tests
1. **Multi-Player Independence**: Verify each player has separate gravity
2. **Trigger Integration**: Test GravitySwitchHelper with PlayerGravityComponent
3. **Camera Integration**: Ensure camera follows gravity changes
4. **Physics Integration**: Verify ball physics work with custom gravity

### Manual Test Cases
1. **Basic Functionality**:
   - Player enters gravity switch → smooth transition
   - Player exits gravity switch → snap to cardinal
   - Multiple gravity switches in sequence
   
2. **Edge Cases**:
   - Rapid trigger enter/exit
   - Multiple players in same trigger
   - Overlapping gravity triggers
   
3. **Performance**:
   - 10+ players with active gravity transitions
   - Profiler shows <0.1ms per player
   - No memory allocation during operation

---

## Documentation Requirements

### API Documentation
- Complete XML documentation for all public classes and methods
- Code examples for common usage patterns
- Performance guidelines and best practices

### User Guide
- **Physics Settings Configuration**: How to create and modify physics settings
- **Level Design Guidelines**: Best practices for gravity switch placement
- **Multi-Player Setup**: Configuring independent player gravity systems
- **Troubleshooting**: Common issues and solutions

### Developer Notes
- **Architecture Decisions**: Why player-specific vs global gravity
- **Performance Considerations**: Optimization techniques used
- **Future Improvements**: Planned enhancements and extensions
- **Integration Points**: How system integrates with other physics components
