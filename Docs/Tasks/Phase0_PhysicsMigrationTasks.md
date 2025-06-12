# Phase 0 Physics Migration Tasks for BlockBall Evolution

## Overview
This document outlines the tasks for Phase 0 (including Phase 0A and 0B) of the BlockBall Evolution physics migration. The goal is to integrate a new physics system that can toggle between Unity's built-in physics, a custom deterministic physics system, and a hybrid mode. These tasks are designed for an LLM to implement, ensuring backward compatibility, clear mode differentiation, and minimal disruption to gameplay.

## Background
- **Old System**: The existing physics system uses force-based movement. `PhysicObject.cs` applies gravity as a force via `Rigidbody.AddForce`, while `PlayerCameraController.cs` handles player input by applying forces and torque for movement and direct velocity changes for jumping.
- **New System**: The new system, implemented in `PhysicsObjectWrapper.cs`, uses velocity-based updates for custom physics calculations, aiming to toggle between Unity physics, custom physics, and hybrid modes using the `PhysicsMode` enum.
- **Conflict**: Force-based updates from the old system can override velocity-based updates from the new system, causing mode changes or parameter adjustments to be ineffective.

## Phase 0A: Compatibility Layer (Completed)
- **Task 0A.1**: Integrate `DeterministicMath` into `PhysicsObjectWrapper.cs` without disrupting Unity physics behavior. (Completed, validated with no observable changes to gameplay.)
- **Task 0A.2**: Set up `PhysicsSettings` as a `ScriptableObject` for centralized physics parameters. (Completed, asset created in `Assets/Settings/` with default values.)

## Phase 0B: Hybrid Implementation and Mode Toggling
The following tasks focus on integrating the new physics modes while addressing conflicts with the old system.

### Task 0B.1: Isolate Old System Forces in Custom Modes
- **Objective**: Prevent old system forces from interfering with new physics modes.
- **Implementation**:
  - Modify `PhysicObject.cs` to check `PhysicsSettings.physicsMode` and skip applying gravity forces (`AddForce(Gravity)`) in `FixedUpdate` when mode is `CustomPhysics` or `Hybrid`.
  - Add a flag in `PhysicsObjectWrapper.cs` to indicate when new physics logic is active, ensuring old logic is bypassed.
- **Expected Outcome**: Custom physics modes will control the `Rigidbody` without interference from old gravity forces.

### Task 0B.2: Adapt Player Movement Logic for Physics Modes
- **Objective**: Ensure player input from `PlayerCameraController.cs` respects the active physics mode.
- **Implementation**:
  - Update the `Move` method in `PlayerCameraController.cs` to check `PhysicsSettings.physicsMode`.
  - For `CustomPhysics` mode, convert force and torque applications (`AddForce`, `AddTorque`) into direct velocity updates or scaled inputs compatible with custom physics.
  - For `Hybrid` mode, define a balanced approach (e.g., use forces for input but allow custom gravity).
  - For `UnityPhysics` mode, retain original force-based movement.
- **Expected Outcome**: Player movement will behave consistently across physics modes, with clear differences (e.g., custom mode might feel more direct or predictable).

### Task 0B.3: Control Script Execution Order
- **Objective**: Ensure update order prevents conflicts between old and new physics logic.
- **Implementation**:
  - In Unity, adjust the Script Execution Order to have `PhysicsObjectWrapper` update after `PhysicObject` and `PlayerCameraController` when in `CustomPhysics` or `Hybrid` modes. This ensures new velocity updates override any old forces if necessary.
  - Alternatively, create a central physics manager script to orchestrate updates based on mode, reducing conflicts.
- **Expected Outcome**: Predictable physics behavior with the new system having priority in custom modes.

### Task 0B.4: Enhance Physics Mode Differentiation
- **Objective**: Make differences between physics modes visually and behaviorally distinct.
- **Implementation**:
  - In `PhysicsObjectWrapper.cs`, increase gravity multipliers (e.g., 2.0x for `CustomPhysics`, 0.5x for `Hybrid`) to exaggerate mode effects.
  - Add debug visualizations (e.g., Gizmos or UI text) to display active mode and forces/velocities acting on the `Rigidbody`.
- **Expected Outcome**: Players and developers can clearly see and feel mode changes during testing.

### Task 0B.5: Define and Implement Hybrid Mode Behavior
- **Objective**: Clarify and implement the behavior of the `Hybrid` mode to balance old and new systems.
- **Implementation**:
  - Define `Hybrid` mode as using custom gravity calculations from `PhysicsObjectWrapper.cs` but retaining player input forces from `PlayerCameraController.cs`.
  - Disable Unity's default gravity (`Rigidbody.useGravity = false`) in `Hybrid` mode, applying custom gravity while allowing old system forces for movement.
- **Expected Outcome**: A functional middle ground that combines aspects of both systems without conflict.

### Task 0B.6: Add Detailed Logging and Debugging
- **Objective**: Provide tools to diagnose physics interference or unexpected behavior.
- **Implementation**:
  - Extend logging in `PhysicsObjectWrapper.cs` to report all forces, torques, and velocity changes per frame.
  - Add logs in `PlayerCameraController.cs` and `PhysicObject.cs` to track when forces are applied and under what conditions.
- **Expected Outcome**: Easy identification of conflicts or overrides between systems during testing.

## Phase 0C: Speed Limits and Settings Organization
### Task 0C.1: Organize Physics Settings by Mode
- **Objective**: Reorganize `PhysicsSettings` to clearly separate parameters by physics mode.
- **Implementation**:
  - Group settings in `PhysicsSettings.cs` by mode (Unity, Hybrid, Custom) with clear headers and tooltips.
  - Ensure each parameter has a descriptive name and tooltip explaining its purpose and impact on gameplay.
- **Expected Outcome**: Improved readability and ease of use for developers adjusting physics settings.

### Task 0C.2: Speed Limits Debugging
- **Status**: In Progress
- **Objective**: Ensure `physicsSpeedLimit` has an observable effect in CustomPhysics mode and replace hardcoded directional speed limits with configurable parameters in `PhysicsSettings`.
- **Progress**:
  - Added configurable parameters `forwardForceMagnitude`, `sidewaysForceMagnitude`, and `backwardForceMagnitude` in `PhysicsSettings.cs` with ranges from 5 to 15, preserving original hardcoded values (8.0 forward/sideways, 3.0 backward).
  - Renamed `inputForceMagnitude` to `inputForceScale` (default 1.0 for original behavior) as a scaling factor for movement forces.
  - Updated `PlayerCameraController.cs` to use new directional force magnitudes and skip movement force application in CustomPhysics mode.
  - Implemented movement forces in CustomPhysics mode within `PhysicsObjectWrapper.cs` to ensure `physicsSpeedLimit` affects gameplay.
  - Consolidated speed limit enforcement to `PhysicsObjectWrapper.cs` for all physics modes, using mode-specific limits (`totalSpeedLimit`, `hybridSpeedLimit`, `physicsSpeedLimit`).
  - Reorganized `PhysicsSettings.cs` by mode (Unity, Hybrid, Custom) with detailed tooltips explaining each parameter's purpose and impact.
  - Fixed compiler errors related to removed properties (`legacyGravity`, `useUnityGravity`) by using hardcoded values or simplified logic.
- **Next Steps**:
  - Test updated directional force parameters and scaling factor in gameplay for tuning.
  - Adjust force magnitudes and speed limits as needed for desired gameplay feel.
  - Monitor logs for velocity capping and force application issues.

## Testing and Validation
- **Test Scene**: Use a dedicated test scene (e.g., `TestCamera`) to evaluate physics behavior across modes.
- **Criteria**:
  - Switching modes via `PhysicsSettings.asset` in the Inspector should produce immediate, visible changes in ball movement and gravity response.
  - Player input should work in all modes, with distinct feel (e.g., custom mode might be more precise, Unity mode more floaty).
  - No unexpected overrides or conflicts (e.g., old forces shouldn't negate new velocity updates in custom mode).
- **Rollback Capability**: Ensure the system can revert to `UnityPhysics` mode with original behavior intact.

## Documentation
- Update `Issues_and_Required_Cleanup.md` after each task to log changes and note any new issues.
- Extend `OldPhysicsAnalysis.md` with additional findings if more scripts or components are discovered to impact physics.

## Notes for LLM Implementation
- Maintain backward compatibility by defaulting to `UnityPhysics` mode and ensuring old system logic remains functional in that mode.
- Use modular, beginner-friendly code with clear comments explaining mode-specific logic.
- Avoid direct edits to Unity asset files; instead, provide scripts to adjust settings or create assets programmatically if needed.
- Prioritize visible impact of mode changes for testing and user feedback.
