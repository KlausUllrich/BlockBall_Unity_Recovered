---
title: "Phase 0: Migration Strategy Overview (Redirect)"
phase: "0"
note: "This document has been split into modular files for LLM processing. See below for references."
---

# Phase 0: Migration Strategy Overview

**Note for LLM**: This original overview has been tailored and split into modular documents to optimize for context length and zero-error processing. Refer to the following files for detailed content:

- **LLM_01A_Mission_and_Precision.md**: Covers mission statement and float precision strategy (float32 with enhanced determinism).
- **LLM_01B_Migration_Phases.md**: Details the phased approach (0A, 0B, 0C) with priorities and triggers.
- **LLM_01C_Parameter_Mapping.md**: Explains legacy-to-target parameter conversion (jump, speed).
- **LLM_01D_Rollback_Strategy.md**: Outlines rollback mechanisms and compatibility preservation.
- **LLM_01E_Timeline_and_Criteria.md**: Provides migration timeline, success criteria, and risk mitigation.

### Implementation Tasks for Phase 0A
- **LLM_02A_PhysicsSettings_Task.md**: Details implementation steps for PhysicsSettings ScriptableObject.
- **LLM_02B_IPhysicsObject_Task.md**: Covers implementation of IPhysicsObject wrapper interface.
- **LLM_02C_DeterministicMath_Task.md**: Explains creation of DeterministicMath utility library.
- **LLM_02D_PhysicsProfiler_Task.md**: Outlines implementation of PhysicsProfiler component.
- **LLM_02E_Phase0A_Checklist.md**: Provides Phase 0A completion checklist and success criteria.

### Phase 0C: Selective System Migration - Status: In Progress
- **Objective**: Migrate specific mechanics for deeper integration and testing.
- **Key Tasks**:
  - **Jump Buffering Fix**: **Completed**. Enhanced jump mechanics with frame-accurate input detection in `Update()` and physics-aligned execution in `FixedUpdate()`. Configurable buffer times and detailed logging added via `PhysicsSettings`.
  - **Speed Limits Debugging**: **In Progress**. Replaced hardcoded directional limits with configurable `forwardForceMagnitude`, `sidewaysForceMagnitude`, and `backwardForceMagnitude` in `PhysicsSettings.cs`. Renamed `inputForceMagnitude` to `inputForceScale` for clarity (1.0 = original behavior). Implemented movement forces in CustomPhysics mode to ensure `physicsSpeedLimit` has an observable effect. Consolidated speed limit enforcement to `PhysicsObjectWrapper.cs` for all modes. Reorganized `PhysicsSettings.cs` by mode (Unity, Hybrid, Custom) with detailed tooltips for each parameter. Resolved compiler errors by removing references to deprecated properties (`legacyGravity`, `useUnityGravity`) with temporary hardcoded values.
  - **Profiling and Optimization**: **Not Started**. Next focus area post-speed limits.
- **Key Files**:
  - `Assets/Scripts/Settings/PhysicsSettings.cs` - Configuration for physics parameters, now organized by mode with tooltips.
  - `Assets/Scripts/PlayerCameraController.cs` - Updated movement logic with configurable force magnitudes.
  - `Assets/Scripts/Physics/PhysicsObjectWrapper.cs` - Consolidated speed limit enforcement and CustomPhysics movement implementation.

## Directive for LLM
Start with `LLM_01A_Mission_and_Precision.md` and follow the sequence for overview. For implementation, begin with `LLM_02A_PhysicsSettings_Task.md`. Use YAML headers for context, dependencies, and validation steps. Log progress in `/Status/Project_Overview.md` and issues in `/Status/Issues_and_Required_Cleanup.md`.

**Purpose of Tailoring**: These documents are optimized for LLM usage with modular structure, explicit instructions, error handling, and machine-readable metadata to ensure complete understanding and zero-error execution.
