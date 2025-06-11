# BlockBall Project Overview

## Introduction
BlockBall is a Unity-based game project focused on physics-driven gameplay. The project is undergoing a physics migration as part of the BlockBall Evolution initiative to enhance gameplay mechanics, improve responsiveness, and introduce custom physics features for a more tailored experience.

## Physics Migration Phases

### Phase 0: Migration Setup and Compatibility
- **Phase 0A: Setup and Initial Migration** - Completed. Basic framework for physics mode switching between Unity Physics and Custom Physics.
- **Phase 0B: Parameter Mapping and Behavioral Parity** - Completed. Ensured that legacy parameters like jump force and gravity are correctly mapped to new systems with observable behavioral distinctions.
- **Phase 0C: Selective System Migration** - In Progress. Focus on specific mechanics:
  - **Jump Buffering Fix** - **Completed**. Resolved issues with jump buffering not functioning as expected. Jump input detection is now handled in `Update()` for frame-accurate input capture, while buffering logic and jump execution are in `FixedUpdate()` to align with physics updates. Added configurable buffer times and detailed logging toggled via `PhysicsSettings`.
  - **Speed Limits Debugging** - **In Progress**. Added distinct `forwardForceMagnitude`, `sidewaysForceMagnitude`, and `backwardForceMagnitude` in `PhysicsSettings.cs` to capture original hardcoded directional limits. Renamed `inputForceMagnitude` to `inputForceScale` (1.0 = original behavior). Implemented movement in CustomPhysics mode and consolidated speed limit enforcement to `PhysicsObjectWrapper.cs`. Reorganized `PhysicsSettings.cs` by mode (Unity, Hybrid, Custom) with detailed tooltips.
  - Profiling and optimization tasks are next in line.

### Phase 1: Advanced Custom Physics
- Implementation of advanced custom physics features, potentially replacing Unity physics core.

## Key Features and Updates
- **Physics Mode Switching**: Three modes - UnityPhysics (default), CustomPhysics, and Hybrid - controlled via `PhysicsSettings` ScriptableObject.
- **Jump Mechanics Improvement**: Enhanced jump buffering with configurable input and ground contact buffer times for better responsiveness.
- **Debugging Tools**: Extensive logging for physics migration and specific mechanics like jump buffering, toggleable via `PhysicsSettings`.

## Project Structure
- **Scripts**: Key scripts under `Assets/Scripts/` include `PlayerCameraController.cs` for player mechanics, `PhysicsObjectWrapper.cs` for physics logic, and `PhysicsSettings.cs` for configuration.
- **Documentation**: Migration strategies and task lists in `Docs/Implementation/Phase0_Migration_Strategy/` and `Docs/Tasks/`.

## Current Focus
- Stabilize jump buffering across Unity and Hybrid physics modes (Completed).
- Debug and resolve issues with speed limit enforcement in physics modes, ensuring `physicsSpeedLimit` works in CustomPhysics mode and `totalSpeedLimit` is fully relevant (In Progress).
- Continue with profiling and optimization for Phase 0C.

## How to Contribute
- Review `PhysicsSettings` asset for tuning physics parameters.
- Test gameplay in different physics modes and report discrepancies or issues.
- Check Unity console logs for detailed debugging information when toggles are enabled.

*Last Updated: June 11, 2025*
