# Old Physics System Analysis for BlockBall Evolution

## Overview
This document provides a concise analysis of the legacy physics system in BlockBall Evolution, focusing on its force-based mechanics and integration challenges with the new velocity-based physics system during Phase 0B of the migration. It serves as a reference for LLMs implementing physics mode toggling (Unity, Custom, Hybrid).

## Key Scripts and Components of Old Physics System

- **PhysicObject.cs** (likely typo for `PhysicObjekt.cs`):
  - **Role**: Core physics handler for game objects like the ball.
  - **Mechanism**: Applies gravity as a force via `Rigidbody.AddForce(Gravity)` in `FixedUpdate`. Manages ground contact detection and toggles between custom and Unity gravity (`Rigidbody.useGravity` typically false).
  - **Key Logic**: Custom gravity force applied per frame; orientation vectors (up, forward) maintained for directional context.

- **PlayerCameraController.cs**:
  - **Role**: Handles player input for movement and camera control.
  - **Mechanism**: Applies movement forces and torque via `Rigidbody.AddForce` and `Rigidbody.AddTorque` in `FixedUpdate` for forward, backward, left, right, and braking. Implements jumping by directly modifying `Rigidbody.velocity`.
  - **Key Logic**: Movement based on input (arrow keys, WASD); forces scaled by `SpeedFactor` and `BreakFactor`; camera follows controlled object with adjustable distance and rotation.

- **Player.cs**:
  - **Role**: Game logic for player entity (score, keys, position resetting).
  - **Mechanism**: No direct physics manipulation found; likely interacts with physics through other components or unlocated scripts.

- **BallObject.cs**:
  - **Role**: Specific to ball behavior, inherits from `PhysicObjekt`.
  - **Mechanism**: Minimal logic; relies on parent class for physics.

## Characteristics of Old Physics System

- **Force-Based Movement**: Uses `Rigidbody.AddForce` for gravity and player movement, contrasting with the new system's direct velocity updates.
- **Unity Gravity Disabled**: `Rigidbody.useGravity` is often set to false, with custom gravity forces applied instead.
- **Input-Driven Forces**: Player input translates to forces/torque, potentially overriding new system velocity changes if not isolated.
- **Update Order Risk**: Old system updates in `FixedUpdate` may conflict with new system timing, risking overwrites of velocity or position.

## Integration Challenges with New Physics System

- **Conflict of Control**: Old forces can override new velocity updates in `PhysicsObjectWrapper.cs`, making mode changes (e.g., `CustomPhysics`, `Hybrid`) ineffective.
- **Mode Interference**: Without disabling old logic, Unity and custom physics modes may apply competing changes to the same `Rigidbody`.
- **Player Input Handling**: Movement logic in `PlayerCameraController.cs` must adapt to physics modes—force for Unity, velocity for Custom, or a mix for Hybrid.
- **Behavioral Consistency**: Ensuring consistent feel across modes requires distinct parameter adjustments (e.g., gravity multipliers) and conflict resolution.

## Recommendations for Physics Migration Phase 0B

1. **Isolate Old System Forces**:
   - Update `PhysicObject.cs` to skip `AddForce(Gravity)` if `PhysicsSettings.physicsMode` is `CustomPhysics` or `Hybrid`.
   - Add a control flag in `PhysicsObjectWrapper.cs` to disable old physics when new logic is active.

2. **Adapt Player Input Logic**:
   - Modify `PlayerCameraController.cs` to check `physicsMode` and switch between force (`UnityPhysics`), velocity (`CustomPhysics`), or mixed (`Hybrid`) movement.
   - Search for additional input scripts if other components apply forces.

3. **Control Update Order**:
   - Set Unity Script Execution Order so `PhysicsObjectWrapper` updates after `PhysicObject` and `PlayerCameraController` in custom modes, ensuring new logic has final control.
   - Consider a central physics manager to orchestrate updates per mode.

4. **Enhance Mode Differences**:
   - Amplify physics differences (e.g., gravity 2.0x for `CustomPhysics`, 0.5x for `Hybrid`) in `PhysicsObjectWrapper.cs` for visible impact.
   - Add debug logs or visuals for forces/velocities to detect interference.

5. **Define Hybrid Mode**:
   - Implement `Hybrid` as custom gravity from new system with player input forces from old system, disabling `Rigidbody.useGravity`.

6. **Debugging Tools**:
   - Extend logging in key scripts to track all forces, torques, and velocity changes per frame for diagnostics.

## Additional Notes for LLM
- **Compatibility**: Default to `UnityPhysics` mode to preserve original behavior; ensure rollback capability.
- **Code Style**: Use modular, commented code for clarity in mode-specific logic.
- **Testing**: Focus on visible mode differences in test scenes (e.g., `TestCamera`) to validate integration.
- **Documentation**: Update this file with new findings on physics-related scripts or components.
