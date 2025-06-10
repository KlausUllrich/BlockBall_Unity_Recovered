# Phase 4: Player Gravity System Overview

## Mission Statement
Implement a **player-specific gravity system** that handles gravity direction changes **only when inside gravity switch trigger zones**. Gravity transitions are smooth inside triggers, but **snap to the nearest cardinal direction when exiting triggers**. This system affects only the player ball, not the environment or other players.

## Phase Objectives
1. **Player-Specific Gravity**: Gravity changes affect only the player ball, never the environment
2. **Trigger-Zone-Only Transitions**: Gravity changes occur **only inside gravity switch trigger zones**
3. **Exit Behavior**: **Immediate snap to nearest cardinal direction when exiting any trigger zone**
4. **Smooth Internal Transitions**: Gradual transition to switch direction while inside trigger
5. **Multi-Zone Handling**: Closest zone by pivot point controls gravity when overlapping

## Context & Dependencies
**Requires Phase 1**: Core physics architecture and Velocity Verlet integration
**Requires Phase 2**: Ball physics component and state machine  
**Requires Phase 3**: Collision system for ground detection during gravity changes
**Integrates With**: Existing GravitySwitch.cs and GravitySwitchHelper.cs

## Key Technical Specifications

### Gravity Mechanics (Player-Only)
- **Scope**: Affects **only the player ball**, never environment or other players
- **Default Direction**: -Y (downward) when no triggers active
- **Gravity Strength**: Configurable via PhysicsSettings (default: 9.81 m/s²)
- **Cardinal Directions**: Six directions only (±X, ±Y, ±Z)

### Trigger-Based Behavior (CRITICAL)
- **Inside Trigger Zone**: 
  - Smooth transition to switch's target direction (0.3s interpolation)
  - Player can experience non-cardinal gravity directions during transition
- **Exit Trigger Zone**: 
  - **IMMEDIATE snap to nearest cardinal direction**
  - No gradual transition - instant change
  - Snap calculation uses Vector3 dot product with six cardinal vectors
- **Multiple Zones**: Zone with closest pivot point to ball controls gravity

### Integration with Existing Code
- **GravitySwitch Objects**: Use existing prefab system with enhanced trigger logic
- **GravitySwitchHelper**: Replace with new PlayerGravityTrigger component
- **Player Component**: Add PlayerGravityManager component
- **Camera Integration**: Camera follows gravity changes (existing behavior maintained)

## Phase 4 Deliverables

### Core Components
1. **PlayerGravityComponent.cs**: Player-specific gravity management
2. **PhysicsSettings.cs**: ScriptableObject for all physics configuration
3. **GravitySwitchTrigger.cs**: Enhanced trigger logic (replaces Helper)
4. **GravityDirectionUtility.cs**: Cardinal direction calculation utilities

### Enhanced Existing Components  
- **Player.cs**: Integrate gravity management
- **GravitySwitch.cs**: Connect with new trigger system
- **PlayerCameraController.cs**: Ensure camera follows gravity changes

## Success Criteria
- [ ] Player gravity changes smoothly inside trigger zones
- [ ] Player gravity snaps to cardinal directions on trigger exit  
- [ ] Multiple players can have different gravity directions simultaneously
- [ ] Environment and other objects unaffected by player gravity changes
- [ ] All physics settings configurable through inspector
- [ ] Performance: <0.1ms per gravity update
- [ ] Integration with existing GravitySwitch prefabs works seamlessly

## Technical Challenges & Solutions

### Challenge 1: Player-Specific Physics
**Issue**: Unity's Physics.gravity affects all objects globally
**Solution**: Disable Physics.gravity, use custom gravity force per player via Rigidbody.AddForce()
**Implementation**: PlayerGravityManager applies gravity force only to player rigidbody

### Challenge 2: Trigger Exit Snapping
**Issue**: Determining nearest cardinal direction from any arbitrary gravity state
**Solution**: Calculate dot product with all six cardinal directions, select highest
**Implementation**: 
```csharp
private Vector3 SnapToNearestCardinal(Vector3 currentGravity)
{
    Vector3[] cardinals = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
    Vector3 closest = Vector3.down; // fallback
    float maxDot = -1f;
    
    foreach (var cardinal in cardinals)
    {
        float dot = Vector3.Dot(currentGravity.normalized, cardinal);
        if (dot > maxDot)
        {
            maxDot = dot;
            closest = cardinal;
        }
    }
    return closest;
}
```

### Challenge 3: Trigger Zone Detection
**Issue**: Reliable detection of entering/exiting gravity switch triggers
**Solution**: Use OnTriggerEnter/OnTriggerExit with proper layer filtering
**Implementation**: PlayerGravityTrigger component on each GravitySwitch prefab

### Challenge 4: Smooth vs Instant Transitions
**Issue**: Different behavior inside vs outside triggers
**Solution**: State-based system with GravityTransitionState enum
**Implementation**: 
- **InsideTrigger**: Smooth interpolation to target direction
- **ExitingTrigger**: Instant snap to nearest cardinal
- **FreeSpace**: Maintain current cardinal direction

## Physics Settings Architecture

### Single Source of Truth: PhysicsSettings.cs
```csharp
[CreateAssetMenu(fileName = "PhysicsSettings", menuName = "BlockBall/Physics Settings")]
public class PhysicsSettings : ScriptableObject
{
    [Header("Gravity Settings")]
    [Tooltip("Base gravity strength (Earth = 9.81)")]
    [Range(1f, 20f)]
    public float gravityStrength = 9.81f;
    
    [Tooltip("Time to transition between gravity directions")]
    [Range(0.1f, 1f)]  
    public float gravityTransitionTime = 0.3f;
    
    [Header("User-Friendly Ball Physics")]
    [Tooltip("How high the ball jumps (in Unity units)")]
    [Range(1f, 10f)]
    public float jumpHeight = 4f;
    
    [Tooltip("How fast the ball accelerates when rolling")]
    [Range(1f, 20f)]
    public float rollAcceleration = 8f;
    
    [Tooltip("Maximum rolling speed")]
    [Range(5f, 30f)]
    public float maxRollSpeed = 15f;
}
```

## Integration Strategy

### Phase 1-3 Compatibility
- **Core Physics**: Uses existing Velocity Verlet integration
- **Ball Physics**: Extends existing BallPhysics component
- **Collision System**: Leverages existing collision detection for ground contact

### Existing Code Integration
```csharp
// Enhance existing GravitySwitchHelper.cs
void OnTriggerStay(Collider pCollider)
{
    var player = pCollider.GetComponent<Player>();
    if (player?.PlayerGravity != null)
    {
        var newDirection = CalculateNewGravityDirection(player.transform.position);
        player.PlayerGravity.SetTargetGravity(newDirection, smooth: true);
    }
}

void OnTriggerExit(Collider pCollider)  
{
    var player = pCollider.GetComponent<Player>();
    if (player?.PlayerGravity != null)
    {
        player.PlayerGravity.SnapToNearestCardinal();
    }
}
```

## Risk Mitigation
- **Backward Compatibility**: Existing GravitySwitch prefabs continue working
- **Incremental Testing**: Test with single player before multi-player scenarios
- **Performance Monitoring**: Profile gravity updates during development
- **Fallback System**: Default to downward gravity if system fails

## Documentation Requirements
- **Physics Settings Guide**: How to configure physics for level designers
- **Multi-Player Setup**: Configuring independent player gravity
- **Trigger Zone Design**: Best practices for gravity switch placement
- **Performance Optimization**: Memory and CPU optimization techniques

---

## CORRECTED Architecture Summary

### ❌ REMOVED (Incorrect Global Approach)
- Global GravityManager singleton
- World-wide gravity state management  
- Complex transition interpolation system
- Environment gravity effects

### ✅ ADDED (Correct Player-Specific Approach)
- Player-specific gravity components
- Trigger-based activation system
- Cardinal direction snapping on exit
- User-friendly physics settings
- Single source of truth configuration
- Multi-player independent gravity support

### 🔄 ENHANCED (Existing Code Integration)
- GravitySwitchHelper trigger logic
- Player.cs gravity management
- Camera system integration
- Existing GravitySwitch prefab compatibility

## Next Phase Prerequisites
Phase 5 (Speed Control) will require:
- Stable player-specific gravity system
- Configurable physics settings working
- Performance within target specifications  
- Multi-player gravity independence verified
- Integration with ball physics complete
