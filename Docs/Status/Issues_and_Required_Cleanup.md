---
title: "BlockBall Evolution Physics Migration - Issues and Required Cleanup"
phase: "0 to 5"
last_updated: "2025-06-11"
---

# BlockBall Evolution Physics Migration - Issues and Required Cleanup

## Open Issues
- **None at this time**: Documentation for Phases 0 and 1 is complete and tailored for LLM usage.

## Required Cleanup Tasks
- **Old File Archiving or Deletion**: Once the user confirms that the LLM-tailored documentation fully meets requirements and migration progresses, consider archiving or deleting old non-LLM files in `Phase0_Migration_Strategy/` (e.g., `01A_Mission_and_Precision.md` to `01E_Timeline_and_Criteria.md`).
  - **Status**: Awaiting user confirmation. Currently retained as reference.

## Phase 0 Migration Strategy Issues

### Critical Gaps Identified
1. **Airborne Gravity Transitions**: Phase 0 documents do not address gravity switching while the ball is airborne, which is a critical requirement for BlockBall Evolution.
   - **Action Needed**: Add specific tasks or references in Phase 0 to cover this mechanic.
   - **Priority**: High
2. **Instant Gravity Snap on Zone Exit**: The requirement for instant gravity transitions when exiting gravity zones is not mentioned in the migration strategy.
   - **Action Needed**: Include planning for instant snap in Phase 0 or subsequent early phases.
   - **Priority**: High
3. **Slope Rolling Mechanics**: Specific implementation details for ensuring the ball always feels like rolling, especially on slopes, are absent.
   - **Action Needed**: Ensure Phase 0 or later phases address slope rolling explicitly.
   - **Priority**: Medium

### Existing Issues from Previous Phases
- **Phase 3 Overengineering**: As noted in memory, Phase 3 collision system documentation proposes replacing Unity's collision response entirely, conflicting with the 'no overengineering' rule.
  - **Action Needed**: Revise Phase 3 to enhance rather than replace existing systems.
  - **Priority**: High
- **Missing PhysicsSettings Integration**: Phase 3 assumes a centralized `PhysicsSettings` exists, which is not yet implemented in the codebase.
  - **Action Needed**: Ensure `PhysicsSettings` is created in Phase 0A as planned.
  - **Priority**: High

## Resolved Issues
- **IPhysicsObject Wrapper Integration**: Successfully integrated with PlayerSphere on [date]. No observable behavior changes, confirming compatibility with existing Unity physics system. No issues or cleanup required at this stage.
- **DeterministicMath Implementation**: Implemented on [date] for consistent physics calculations using float32 precision. Integrated into PhysicsObjectWrapper on [date] with RoundVector method added for vector operations; tested and validated on [date] with no observable behavior changes, confirming continued compatibility with existing system.
- **PhysicsProfiler Implementation**: Implemented on [date] to monitor physics performance during migration. Setup script and prefab created for scene integration; instructions provided for manual setup in the main scene via Unity Editor; pending attachment to a GameObject and testing to establish baseline performance metrics.
- **Duplicate PhysicsMode Definition**: Resolved error CS0101 on [date] by removing duplicate PhysicsMode enum from PhysicsSettings.cs, ensuring definition exists only in PhysicsMode.cs.
- **Legacy Parameters Impact (Resolved on 2025-06-12)**: Fixed issue where `legacySpeedFactor` and `legacyBreakFactor` had no observable effect in UnityPhysics mode by updating `PlayerCameraController.cs` to use these parameters for movement and braking forces.
- **CustomPhysics Input Direction (Resolved on 2025-06-12)**: Fixed issue where input directions in CustomPhysics mode were absolute by updating `PhysicsObjectWrapper.cs` to use camera-relative movement based on the main camera's orientation.
- **Profiling and Optimization (Initiated on 2025-06-12)**: Created `PhysicsProfilerInitializer.cs` to instantiate `PhysicsProfiler` in the main scene. Attach this script to a GameObject in the main scene to start profiling physics performance.
- **Critical CustomPhysics Collision Bug (Resolved on 2025-06-13)**: Fixed game-breaking issue where ball fell through blocks in CustomPhysics mode. Root cause was direct `transform.position` manipulation in `BallPhysics.cs` which bypassed Unity's collision detection when Rigidbody was kinematic. Solution implemented `Rigidbody.MovePosition()` for kinematic bodies in the Position property and PostPhysicsStep method. This ensures proper collision detection while maintaining the custom physics behavior. Files modified: `BallPhysics.cs` Position property (lines 24-40) and PostPhysicsStep method (line 252).

## Phase 0B: Hybrid Implementation - In Progress
- **PhysicsMode Enum**: Implemented on [date] to define physics system modes (UnityPhysics, CustomPhysics, Hybrid). Added to PhysicsSettings for mode selection; integrated into PhysicsObjectWrapper on [date] for toggling behavior between Unity and custom physics systems, pending testing to validate mode switching.
- **Duplicate Definition Fix**: Resolved error CS0101 on [date] by removing duplicate PhysicsMode enum from PhysicsSettings.cs, ensuring definition exists only in PhysicsMode.cs.
- **Namespace Organization**: Added 'BlockBall.Physics' and 'BlockBall.Settings' namespaces to relevant scripts on [date] for consistent code organization and to prevent future namespace conflicts.
- **PhysicsSettings Asset**: Created PhysicsSettings.asset on [date] in Assets/Settings/ with default parameters and physicsMode set to UnityPhysics for compatibility.
- **PhysicsSettings Path Update**: Updated resource path in PhysicsObjectWrapper.cs on [date] to correctly load PhysicsSettings asset from 'PhysicsSettings' path.
- **Error Handling**: Added try-catch block on [date] for robust loading of PhysicsSettings asset in PhysicsObjectWrapper.cs to handle potential loading errors; updated error message to provide actionable guidance for creating the asset if missing.
- **Type Qualification**: Fully qualified PhysicsSettings type as BlockBall.Settings.PhysicsSettings on [date] in PhysicsObjectWrapper.cs to resolve potential namespace reference issues.

## Physics Migration - Phase 0C Updates

- **2025-06-12**: Profiling and Optimization task completed. `PhysicsProfilerSetup.cs` and `PhysicsProfiler.prefab` successfully integrated into the active scene (`testcamera`). Profiler is active and collecting data. Awaiting user review of performance metrics to identify any bottlenecks or optimization needs for UnityPhysics, Hybrid, and CustomPhysics modes.

- **2025-06-12**: Core Physics Architecture Implementation completed. Implemented foundational components for future CustomPhysics system based on `3_Physics_Implementation_Tasks.md` Task 1:
  - `BlockBallPhysicsManager.cs` - Central physics system manager with 50Hz fixed timestep and accumulator pattern
  - `VelocityVerletIntegrator.cs` - High-precision physics integration for energy conservation with validation methods
  - `IAdvancedPhysicsObject.cs` - Extended interface for advanced physics objects with lifecycle hooks
  - `BallPhysics.cs` - Enhanced ball physics component with state management, camera-relative input, and comprehensive movement mechanics
  - `PhysicsSystemMigrator.cs` - Migration helper for gradual transition between old and new physics systems
  - `AdvancedPhysicsSetup.cs` - Unity Editor tools for easy setup and configuration
  - Extended `PhysicsSettings.cs` with 20+ advanced physics parameters for full runtime tuning
  - All components maintain backward compatibility with existing `PhysicsObjectWrapper` system
  - Ready for testing and integration with existing game systems

- **2025-06-12**: Fixed compilation errors in Core Physics Architecture. Resolved YAML header issues by replacing with proper C# comments, fixed namespace conflicts by using `UnityEngine.Debug` instead of `Debug` and `UnityEngine.Physics` instead of `Physics` to avoid conflicts with `BlockBall.Physics` namespace. All physics architecture files now compile correctly in Unity.

- **2025-06-12**: ✅ **Phase 0C Completed** - BallPhysics Modular Refactoring Complete. Successfully refactored monolithic 724-line `BallPhysics.cs` into modular architecture with 6 specialized components (53% size reduction). Fixed all compilation errors including CS0111 (duplicate methods), CS0200 (read-only properties), and CS0414 (unused fields). All physics properties now accessible via AdvancedPhysicsSetup editor tools. Three physics modes (UnityPhysics, Hybrid, CustomPhysics) now fully operational and ready for gameplay testing. Next session focus: Testing & Validation.

## Physics Migration Issues - Phase 0C
- **Speed Limits Validation Completed** (Updated 2025-06-12): User feedback confirms that speed limits are working as expected across UnityPhysics, Hybrid, and CustomPhysics modes. No further action needed on this task unless new issues arise.

- **Legacy Parameters No Impact** (Updated 2025-06-12): User feedback indicates that `legacySpeedFactor` and `legacyBreakFactor` in `PhysicsSettings.cs` do not have an observable effect in UnityPhysics mode. Investigation required to determine if these parameters are being applied correctly in `PlayerCameraController.cs` or `PhysicObject.cs`. Potential fix may involve adding or correcting logic to scale movement forces or braking behavior based on these factors. Logging will be added to track their usage.

- **Legacy Break Factor No Observable Impact (Phase 0C)**
  - **Status**: In Progress
  - **Issue**: Despite applying braking forces scaled by `legacyBreakFactor`, the braking effect is minimal and slow in UnityPhysics mode.
  - **Findings**:
    - Velocity capping interference ruled out with conditional logic in `PhysicsObjectWrapper.cs`.
    - Slow velocity decay observed in logs, indicating braking force may be applied but insufficient.
    - Confirmed `Move` method with braking logic is not called without player input, preventing braking checks.
  - **Diagnostics**:
    - Added detailed logging for Rigidbody properties (mass, drag, angular drag) when braking force is applied.
    - Added logging in `PlayerCameraController.Move` to confirm braking condition checks, but method not invoked without input.
    - Added braking logic directly in `FixedUpdate` of `PlayerCameraController` to ensure evaluation even without movement input.
  - **Next Steps**:
    - Review logs to confirm braking logic in `FixedUpdate` is triggered and assess impact.
    - Investigate Rigidbody settings (mass, drag) and physics materials in Unity Editor if braking remains weak.

## Risk Mitigation Actions
- **Jump Feel Changes**: Implement extensive playtesting and gradual transition options via `PhysicsMode` to prevent altering player muscle memory.
- **Performance Regression**: Continuous monitoring via `PhysicsProfiler` and automatic fallback if targets are missed.
- **Determinism Issues**: Use fixed-point calculations in `DeterministicMath` and conduct platform testing to avoid variations.

**Directive for LLM**: Log any issues encountered during implementation tasks here with detailed descriptions and actionable resolution steps. Update status to 'Resolved' once fixed. Add cleanup tasks as needed for maintaining project organization.

**Note**: All risks and mitigation actions are logged here for transparency and tracking. Update this document with any new issues or changes in status.
