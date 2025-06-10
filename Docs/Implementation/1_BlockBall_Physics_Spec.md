# Physics Requirements for BlockBall Evolution (Custom Implementation)

## Overview

The game "BlockBall Evolution" features a ball-rolling puzzle-platformer gameplay that combines precision control, gravity-switching mechanics, and physically plausible movement. Unity's default physics (PhysX) proved insufficient in providing the required control fidelity and deterministic behavior, prompting the need for a custom physics system.

This document refines the physics requirements based on the original game design and expands them to better inform a technical implementation and potential LLM-powered development tools.

---

## Core Design Goals

- Predictable and Tailored Control: Physics must support a game-feel that is consistent across devices and frame rates.
- Smooth Transitions: Ball must roll reliably across adjacent blocks without jitter or unpredictable elevation shifts.
- Gravitational Dynamics: Support for gravity shifts is central to the puzzle and navigation mechanics.
- Determinism: All physics calculations must be deterministic to support multiplayer and replays.

---

## Measurement Units and Scale

- Unity Units: 1 Unity unit = 1 full block (1×1×1)
- Bixels: Each Unity unit subdivides into 8 Bixels (theoretical subdivision for slopes and connections)
- Conversion: 1 Unity unit = 8 Bixels
- Slope Definition: 0-8 Bixel height difference across 1 Unity unit = 45° slope
- Interchangeable Units: All measurements can be expressed in either Unity units or Bixels with fixed 1:8 ratio

---

## Control Feel Requirements

- Rolling should feel grippy, not floaty, even though friction simulation is abstracted.
- Input acceleration must be frame-rate independent
- Input acceleration must be curve-controlled (initial buildup and decreasing before reaching maximum speed).
- Jump buffering: Jump is accepted within a small timing window before and after ground contact.
- Rolling into a cliff should allow small tolerance for delayed jumps or late off-ledge inputs.
- Prevent unrealistic behavior such as edge climbing or sticking to walls.

### Knock-Out Criteria

- Transitions from one block to the next must be reliably smooth.
- Movement must be "believable" even if not physically realistic.

---

## Ball Movement and Forces

- Player movement input is interpreted relative to the camera's orientation.
- However, movement input is projected strictly onto the horizontal plane defined by the current gravity direction.
  - In other words: the ball moves only along the X/Y plane that is perpendicular to the gravity vector.
  - This ensures movement is consistent regardless of vertical camera tilt.
  - There must be no up/down motion caused by camera pitch.
- Ball responds to player input along local X/Y axes perpendicular to gravity vector.
- Acceleration profile is defined via curve (ease-in, ease-out before reaching max. speed). 
- Per-axis speed limits (forward, backward, sideways) for input based movement
- When combining forward and sideways (strafe) input, the resulting velocity vector must be normalized to maintain a consistent maximum allowed movement speed.
- Ground acceleration is influenced by surface slope.
- Gravity and slope-induced acceleration can exceed input-based speed limits
- Total movement speed vector limit (any direction)
- Ease out of acceleration before reaching total movement speed ("air drag feel")
- At 45° incline: no net acceleration possible by input control, >45° uphill = deceleration

### Speed Control System (CLARIFIED)

Speed Limits:
- Input Speed Limit: 6 Unity units/second (player input only)
- Physics Speed Limit: 6.5 Unity units/second (gravity, slopes, bounces)
- Total Speed Limit: 7 Unity units/second (absolute maximum)

Exponential Decay:
- Decay starts at 95% of total limit (6.65 Unity units/second)
- Uses exponential decay function: `speed = limit * exp(-decay_rate * excess_ratio)`
- Provides smooth speed reduction approaching the limit
- Hard clamp only above absolute maximum

Speed Combination:
- Input and physics velocities are combined additively
- Total velocity magnitude is then subject to exponential decay
- Maintains responsive feel while preventing excessive speeds

---

## Jump and Bounce Mechanics

- The player can make the ball jump.
- Fixed jump height: exactly 6 Bixels (0.75 Unity units) regardless of momentum.
- Jumping allowed from air with tight buffer (e.g., Coyote Time).
- In-air control:
  - Limited, more linear.
  - Minor directional adjustments only.
  - Only on the horizontal plane.

- Bounce response:
  - Bounce force must be less than impact force.
  - Never exceeds max jump height.
  - Influenced by angle of contact.
  - Low-angle contacts = minimal bounce.
  - High-angle contacts (e.g. 90° walls) = strong bounce.
- Collisions always apply predictable bounce impulse.
- The ball always jumps the same height: approx. 6 Bixel (1 full Block = 1x1x1 Unity units = 8x8x8 Bixels).
- Standing jump range: approx. 6 Bixel; moving jump: approx. 12 Bixel.

- **Bounce response (CLARIFIED)**:
  ```
  bounce_velocity = reflect(incoming_velocity, surface_normal) * restitution_coefficient
  restitution_coefficient = lerp(min_restitution, max_restitution, contact_angle_factor)
  
  Where:
  - min_restitution = 0.1 (low-angle contacts, minimal bounce)
  - max_restitution = 0.7 (high-angle contacts like walls)
  - contact_angle_factor = dot(velocity_direction, surface_normal)
  - Bounce force never exceeds jump height (0.75 Unity units)
  ```
- Standing jump range: approx. 6 Bixels (0.75 Unity units)
- Moving jump: approx. 12 Bixels (1.5 Unity units)
- Holding the jump key results in continuous jumping.
- Diagonal input (e.g., forward + sideways) results in normalized movement vector, not compounded speed.
- Maximum fall height: 32.5 Bixel (approx. 4 blocks). Above 25 Bixel, a hard fall triggers visual/audio feedback.
- The level goal applies a mild attractive force to the ball when within a short range.

---
## Gravity Switching

- Within influence of a Gravity Switch Block gravity can change to a new  direction.
- Gravity magnitude remains constant
- Custom gravity vector per ball.
- Triggered when entering an invisible gravity switch effect volume.
- Volume defined via invisible colliders or geometric trigger zones.
- Gravity switches affect Ball in air and on ground.
- Change occurs relative to a specified rotation center per gravity switch block.
- Gravity directions can change smoothly, but always only for one axis.
- Leaving a gravity switch snaps the gravity direction to an axis

## Gravity Switching (CLARIFIED)

**Gravity Transition Behavior**:
- Inside Gravity Zone: Gravity transitions smoothly over 0.3 seconds using ease-in-out curve
- Leaving Gravity Zone: Gravity immediately snaps to nearest cardinal axis (±X, ±Y, ±Z)
- Ball Inertia: Ball maintains momentum during transitions, making snap feel natural
- Camera Smoothing: Camera rotation is always smoothed regardless of gravity snap

**Multi-Zone Handling**:
- When ball is within multiple overlapping gravity zones
- Only the zone with closest distance to its pivot point affects the ball
- Distance measured from ball center to zone's specified pivot point
- Prevents conflicting gravity directions

**Technical Implementation**:
- Gravity magnitude remains constant (9.81 m/s²)
- Custom gravity vector per ball
- Triggered when entering/exiting invisible collider volumes
- Smooth interpolation uses Quaternion.Slerp for rotation
- Snap uses Vector3 component comparison to find dominant axis

---

## Constraints and Limits

- Geometry is built from grid-aligned blocks.
- Ball rotation is controlled by controller input when in-air, and constrained to match ground speed when rolling.
- Rotation inertia should make rotational changes smooth.
- Rotational speed should not influence bounce direction.

---

## Multiplayer Constraints

- All physics calculations must be deterministic.
- Physics must not allow stacking (no jumping from another player's ball).

---

## Optional/Nice-to-Have Features

- Runtime changes to physics profile (e.g. low-gravity zones).
- Speed booster blocks (impulse forces in a direction).
- Jump pad blocks (on ground contact: single-time impulse).
- Specialized geometry (e.g., quarter pipes, spheres).
- Double jump support (toggleable per level or ball type).
- Ball types with varying mass, friction, bounce profiles.

---

## Unity Version Consideration

Unity 2022.3 LTS and Unity 6 may differ significantly in:

- DOTS (Unity 6): May offer performance improvements but requires architectural changes.
- Input System: Consistent across both, so input translation should work identically.
- Physics Engine (PhysX): Largely similar. But using custom physics, this doesn't matter.

Recommendation: Use Unity 2022.3 LTS for stability. Unity 6's advantages don't justify migration effort for BlockBall's scope.

---

## Implementation Notes

- Needs fixed timestep simulation (50Hz) for all ball movement.
- Velocity Verlet integrator (RECOMMENDED):
  ```
  position(t+dt) = position(t) + velocity(t)*dt + 0.5*acceleration(t)*dt²
  acceleration(t+dt) = calculateForces(position(t+dt)) / mass
  velocity(t+dt) = velocity(t) + 0.5*(acceleration(t) + acceleration(t+dt))*dt
  ```
  - More accurate than Semi-Implicit Euler, especially for gravity-shifting physics
  - Better energy conservation over time (±0.1% vs ±2-5%)
  - Smoother gravity transitions and bounce behavior
  - Worth the ~50% performance cost for superior game feel

- Clear separation between control input, physics response, and visual feedback.
- Unity's Job System or Burst Compiler can be used for performant custom physics.
- Debug HUD to visualize gravity direction, trigger zones, and contact normals.

---

## Implementation Architecture

### Core Systems Required

1. Physics Manager: Fixed timestep (50Hz) with Velocity Verlet integration
2. Ball State Machine: Grounded, Airborne, Sliding, Transitioning states
3. Collision System: Hybrid approach using Unity detection with custom response
4. Gravity Manager: Zone-based gravity with smooth transitions and snapping
5. Speed Controller: Multi-tier limits with exponential decay
6. Input Processor: Camera-relative input projected onto gravity plane
7. Debug Visualizer: Real-time physics state visualization

### Performance Targets

- Physics Frame Time: <2ms per frame (50Hz)
- Memory Allocation: <1KB per second (object pooling)
- Jump Consistency: ±0.005 Unity units variance
- Energy Conservation: ±0.1% over 60 seconds

---

## Nice to Haves

- Double jump
- Different ball types with different physical properties and settings (felt mass, bounciness)
- Dynamic modification of physical properties during runtime
- Speed-accelerator blocks (similar to gravity switches with invisible effect area, just for directional acceleration)
- Jump-pad blocks (on ground touch provides a one-time force impulse in one direction)
- Custom geometry, like half-pipes
