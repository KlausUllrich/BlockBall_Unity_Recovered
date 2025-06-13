# BlockBall Project Overview

## Introduction
BlockBall is a Unity-based game project focused on physics-driven gameplay. The project is undergoing a physics migration as part of the BlockBall Evolution initiative to enhance gameplay mechanics, improve responsiveness, and introduce custom physics features for a more tailored experience.

## Physics Migration Phases

### Phase 0: Migration Setup and Compatibility
- **Phase 0A: Setup and Initial Migration** - Completed. Basic framework for physics mode switching between Unity Physics and Custom Physics.
- **Phase 0B: Parameter Mapping and Behavioral Parity** - Completed. Ensured that legacy parameters like jump force and gravity are correctly mapped to new systems with observable behavioral distinctions.
- **Phase 0C: Core Physics Architecture** - Completed. Full custom physics implementation with modular architecture:
  - **Jump Buffering** - Completed. Frame-accurate input capture with configurable buffer times and detailed logging.
  - **Speed Limits** - Completed. Distinct directional force magnitudes with consolidated speed limit enforcement.
  - **Modular Physics** - Completed. BallPhysics refactored into 6 specialized components for maintainability.
  - **Core Systems** - Completed. 50Hz fixed timestep physics manager, Velocity Verlet integration, advanced state management.
  - **Editor Integration** - Completed. Full runtime parameter configuration via AdvancedPhysicsSetup.

### Phase 1: Testing and Validation
- **Current Focus**: Gameplay testing and performance validation of all physics modes
- Unity editor testing for compilation and functionality
- Physics behavior testing across all three modes (UnityPhysics, Hybrid, CustomPhysics)
- Performance profiling and optimization validation

## Key Features and Updates
- **Physics Mode Switching**: Three modes - UnityPhysics (default), CustomPhysics, and Hybrid - controlled via `PhysicsSettings` ScriptableObject.
- **Jump Mechanics Improvement**: Enhanced jump buffering with configurable input and ground contact buffer times for better responsiveness.
- **Debugging Tools**: Extensive logging for physics migration and specific mechanics like jump buffering, toggleable via `PhysicsSettings`.

## Project Structure
- **Scripts**: Key scripts under `Assets/Scripts/` include `PlayerCameraController.cs` for player mechanics, `PhysicsObjectWrapper.cs` for physics logic, and `PhysicsSettings.cs` for configuration.
- **Documentation**: Migration strategies and task lists in `Docs/Implementation/Phase0_Migration_Strategy/` and `Docs/Tasks/`.

## Current Focus
- Stabilize jump buffering across Unity and Hybrid physics modes (Completed).
- Debug and resolve issues with speed limit enforcement in physics modes, ensuring `physicsSpeedLimit` works in CustomPhysics mode and `totalSpeedLimit` is fully relevant (Completed).
- Continue with profiling and optimization for Phase 0C (Completed).

## How to Contribute
- Review `PhysicsSettings` asset for tuning physics parameters.
- Test gameplay in different physics modes and report discrepancies or issues.
- Check Unity console logs for detailed debugging information when toggles are enabled.

*Last Updated: June 11, 2025*
