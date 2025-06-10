# Phase 5: Speed Control System Overview

## Mission Statement
Implement a **three-tier speed control system** with exponential decay that enforces specific speed limits for different movement types while maintaining smooth gameplay and deterministic physics behavior. This system prevents physics instability from excessive speeds while allowing controlled high-speed gameplay.

## Phase Objectives
1. **Three-Tier Speed Limits**: Input (6 u/s), Physics (6.5 u/s), Total (7 u/s)
2. **Exponential Decay**: Smooth speed reduction starting at 95% of total limit (6.65 u/s)
3. **Deterministic Behavior**: Consistent speed control across all platforms and runs
4. **PhysicsSettings Integration**: All speed values configurable through centralized settings
5. **Performance**: Zero allocation speed control with <0.2ms processing cost

## Context & Dependencies
**Requires Phase 1**: Core physics architecture and Velocity Verlet integration
**Requires Phase 2**: Ball physics component and state machine
**Requires Phase 3**: Collision system for speed-collision interactions
**Requires Phase 4**: Gravity system for gravity-relative speed control

## Key Technical Specifications

### Three-Tier Speed Limit System
- **Input Speed Limit**: 6 u/s (player input cannot exceed this)
- **Physics Speed Limit**: 6.5 u/s (physics calculations max speed)
- **Total Speed Limit**: 7 u/s (absolute maximum velocity magnitude)
- **Exponential Decay Threshold**: 6.65 u/s (95% of total limit)
- **Minimum Speed Threshold**: 0.01 u/s (below this, treated as stationary)

### Exponential Decay System
- **Decay Start**: When speed > 6.65 u/s (95% of 7 u/s)
- **Decay Formula**: `speed *= exp(-decayRate * deltaTime)`
- **Decay Rate**: Configurable via PhysicsSettings (default: 2.0)
- **Direction Preservation**: Maintain velocity direction while reducing magnitude
- **Smooth Transition**: No sudden speed changes or jitter

### Speed Control Mechanics
- **Soft Limiting**: Gradual speed reduction rather than hard cutoff
- **Layer-Based Control**: Different limits for different movement sources
- **Gravity Relative**: Speed calculations relative to current gravity direction
- **State Aware**: Speed limits vary based on ball state (grounded/airborne)

### Performance Requirements
- **Update Frequency**: 50Hz fixed timestep alignment
- **Processing Cost**: <0.2ms additional processing per frame
- **Memory Usage**: Zero allocation during speed control updates
- **Determinism**: Identical behavior across all platforms and runs

## Phase 5 Deliverables

### Core Components
1. **SpeedController.cs**: Main speed control system coordinator
2. **SpeedLimiter.cs**: Three-tier speed limiting algorithms
3. **ExponentialDecay.cs**: Exponential speed decay implementation
4. **SpeedProfiler.cs**: Performance monitoring and analysis
5. **PhysicsSettings.cs**: Updated with speed control configuration

### Integration Components
- **BallSpeedHandler.cs**: Ball-specific speed control integration
- **SpeedDebugger.cs**: Debug visualization and analysis tools

## Success Criteria
- [ ] Input speed never exceeds 6 u/s
- [ ] Physics calculations never exceed 6.5 u/s
- [ ] Total velocity magnitude never exceeds 7 u/s
- [ ] Exponential decay activates at 6.65 u/s
- [ ] Speed control maintains deterministic behavior
- [ ] Performance: <0.2ms processing cost per frame
- [ ] Zero allocation during speed control updates
- [ ] All speed values configurable via PhysicsSettings

## Technical Challenges & Solutions

### Challenge 1: Three-Tier Speed Management
**Issue**: Managing three different speed limits without conflicts
**Solution**: Hierarchical speed control with clear precedence rules
**Implementation**: Input → Physics → Total speed limiting in sequence

### Challenge 2: Exponential Decay Precision
**Issue**: Maintaining consistent exponential decay across different framerates
**Solution**: Frame-rate independent decay using deltaTime-based calculations
**Implementation**: `speed *= Mathf.Exp(-decayRate * Time.fixedDeltaTime)`

### Challenge 3: Direction Preservation
**Issue**: Maintaining velocity direction while limiting magnitude
**Solution**: Normalize velocity direction, limit magnitude, recombine
**Implementation**: `velocity = velocity.normalized * limitedMagnitude`

### Challenge 4: Performance with Zero Allocation
**Issue**: Speed calculations without creating garbage
**Solution**: In-place vector operations and cached calculations
**Implementation**: Reuse Vector3 instances, avoid new allocations

## Integration with PhysicsSettings

### Centralized Speed Configuration
```csharp
[Header("Speed Control System")]
[Tooltip("Maximum input speed (player control)")]
[Range(3f, 10f)]
public float inputSpeedLimit = 6f;

[Tooltip("Maximum physics calculation speed")]
[Range(4f, 12f)]
public float physicsSpeedLimit = 6.5f;

[Tooltip("Absolute maximum total speed")]
[Range(5f, 15f)]
public float totalSpeedLimit = 7f;

[Tooltip("Exponential decay rate when exceeding 95% of total limit")]
[Range(0.5f, 5f)]
public float speedDecayRate = 2f;

// Calculated properties
public float ExponentialDecayThreshold => totalSpeedLimit * 0.95f; // 6.65f
public float MinimumSpeedThreshold => 0.01f;
```

## Architecture Overview

### Speed Control Pipeline
1. **Input Processing**: Limit input-derived velocity to 6 u/s
2. **Physics Calculations**: Ensure physics forces don't exceed 6.5 u/s
3. **Total Speed Check**: Apply exponential decay if total > 6.65 u/s
4. **Final Limiting**: Hard cap at 7 u/s as absolute maximum

### Component Relationships
- **SpeedController**: Central coordinator
- **SpeedLimiter**: Implements three-tier limiting
- **ExponentialDecay**: Handles smooth speed reduction
- **BallSpeedHandler**: Integrates with ball physics
- **PhysicsSettings**: Provides all configuration values

## Next Phase Dependencies
Phase 6 (Testing & Polish) depends on this speed control system being:
- Deterministic and consistent
- Performance-optimized
- Fully integrated with existing physics
- Thoroughly tested with edge cases

The speed control system is critical for gameplay balance and physics stability.
