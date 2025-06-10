# Phase 2: Ball Physics Overview

## Mission Statement
Implement comprehensive ball physics including rolling mechanics, jumping, state management, and camera-relative input processing. This phase builds upon Phase 1's foundation to create the complete ball control system.

## Phase Objectives
1. **Ball State Machine**: Implement grounded/airborne/sliding/transitioning states
2. **Rolling Physics**: Realistic ball rolling with friction and angular velocity
3. **Jump Mechanics**: Precise 6-Bixel (0.75 Unity unit) jumps with buffering
4. **Input Processing**: Camera-relative movement with diagonal normalization
5. **Speed Control**: Three-tier speed limiting system

## Context & Dependencies
**Requires Phase 1**: This phase depends on the core architecture from Phase 1:
- `BlockBallPhysicsManager` for object registration
- `IPhysicsObject` interface implementation
- `VelocityVerletIntegrator` for physics calculations
- Performance profiling and debugging systems

## Key Technical Specifications

### Ball Parameters
- **Radius**: 0.5 Unity units (4 Bixels)
- **Mass**: 1.0 kg (standard)
- **Jump Height**: 0.75 Unity units (6 Bixels) exactly
- **Max Input Speed**: 6 units/second
- **Max Physics Speed**: 7 units/second  
- **Max Total Speed**: 8 units/second

### State Machine
- **Grounded**: Rolling on surface, friction applied
- **Airborne**: In flight, air drag applied
- **Sliding**: On steep slopes (>45°), reduced friction
- **Transitioning**: During gravity changes, special handling

### Input Mechanics
- **Jump Buffer**: 0.1 second window before/after valid jump
- **Coyote Time**: 0.15 second grace period after leaving ground
- **Camera Relative**: Movement direction based on camera orientation
- **Diagonal Normalization**: Consistent speed in all directions

## Phase 2 Deliverables

### Core Components
1. **BallPhysics.cs**: Main ball physics component implementing IPhysicsObject
2. **BallStateMachine.cs**: State management system
3. **BallInputProcessor.cs**: Camera-relative input handling
4. **BallController.cs**: High-level ball control coordination
5. **GroundDetector.cs**: Ground contact detection system

### Integration Points
- **PhysicObject.cs**: Will be refactored to use new BallPhysics
- **Player.cs**: Integration with new ball control system
- **Camera System**: Must work with new input processing
- **Block Collision**: Ground detection with level geometry

## Success Criteria
- [ ] Ball rolls smoothly without jitter or jumping
- [ ] Jump height exactly 0.75 units (6 Bixels) consistently
- [ ] Input feels responsive and camera-relative
- [ ] State transitions are smooth and predictable
- [ ] All speed limits enforced correctly
- [ ] Ground detection works on slopes up to 45°
- [ ] Performance remains <2ms with ball physics active

## Risk Mitigation
- **Gradual Integration**: Implement alongside existing system with feature flags
- **Comprehensive Testing**: Unit tests for each component
- **Performance Monitoring**: Continuous profiling during development
- **Rollback Capability**: Can revert to Phase 1 if issues arise

## Technical Challenges

### Challenge 1: Precise Jump Height
**Issue**: Achieving exactly 6 Bixels jump height consistently
**Solution**: Calculate exact initial velocity based on gravity and target height
**Formula**: `v = sqrt(2 * g * h)` where h = 0.75 units

### Challenge 2: Rolling Constraint
**Issue**: Ball must roll realistically without sliding
**Solution**: Implement angular velocity constraint: `ω = v / r`
**Validation**: Visual wheel rotation must match movement speed

### Challenge 3: State Transitions
**Issue**: Smooth transitions between grounded/airborne states
**Solution**: Hysteresis in ground detection, gradual friction changes
**Implementation**: Different thresholds for entering/leaving states

### Challenge 4: Camera-Relative Input
**Issue**: Movement direction must account for camera orientation and gravity
**Solution**: Project camera vectors onto gravity-perpendicular plane
**Edge Cases**: Handle camera parallel to gravity direction

## Integration Strategy

### Phase 1 Dependencies
- Uses `IPhysicsObject` interface from Phase 1
- Registers with `BlockBallPhysicsManager`
- Leverages `VelocityVerletIntegrator` for physics
- Utilizes performance profiling system

### Existing System Integration
- **PhysicObject.cs**: Gradually replace with BallPhysics
- **Player.cs**: Adapt input handling to new system
- **Camera**: Ensure proper integration with input processing
- **Level System**: Maintain compatibility with existing level loading

## Documentation Requirements
- **Complete API Documentation**: XML comments for all public members
- **Usage Examples**: How to configure and use each component
- **Integration Guide**: Step-by-step integration with existing systems
- **Performance Notes**: Memory usage and optimization tips
- **Troubleshooting Guide**: Common issues and solutions

## Next Phase Prerequisites
Phase 3 (Collision System) will require:
- Stable ball physics implementation
- Reliable ground detection system
- Consistent state management
- Performance within targets
- Complete integration with existing Player component
