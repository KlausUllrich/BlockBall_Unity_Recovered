---
title: "Phase 0: Critical Parameter Mapping (LLM)"
phase: "0"
context_required: ["ParameterMapping", "ConversionLogic"]
dependencies: ["LLM_01B_Migration_Phases.md"]
validation_steps:
  - "Verify understanding of legacy vs. target parameters"
  - "Confirm conversion logic is implementable"
priority: 3
---

# Phase 0: Critical Parameter Mapping (LLM)

## Overview of Parameter Migration
Parameter mapping is essential to transition from Unity's physics system to the custom BlockBall physics system without breaking existing gameplay. This document details the conversion between legacy and target parameters.

**Directive for LLM**: Use the provided conversion logic to map parameters accurately. Do not assume values or conversion formulas; always reference this document or validate empirically if instructed.

## Jump System Conversion
- **Context**: The current system uses a force-based jump mechanic, while the new system targets a specific height.
- **Legacy Parameter**:
  - Variable: `legacyJumpForce`
  - Value: `5.0f`
  - Source: `PlayerCameraController.cs`
  - Meaning: Force applied to initiate a jump.
- **Target Parameter**:
  - Variable: `targetJumpHeight`
  - Value: `0.75f` (equivalent to 6 Bixels)
  - Meaning: Desired jump height in Unity units.
- **Conversion Logic**:
  ```csharp
  // File: Assets/Scripts/Settings/PhysicsSettings.cs
  // Purpose: Convert legacy jump force to target height
  float ConvertJumpForceToHeight(float force, float gravity = 9.81f)
  {
      // Approximate conversion - needs empirical testing
      return (force * force) / (2 * gravity);
  }
  ```
- **Validation Steps**:
  1. Implement conversion function in `PhysicsSettings.cs`.
  2. Test with current gravity value from `Physics.gravity.magnitude`.
  3. Compare result with target `0.75f`. If discrepancy > 0.05f, log issue for empirical calibration in `/Status/Issues_and_Required_Cleanup.md`.
- **Error Handling**: If conversion yields unexpected results (e.g., negative or zero height), default to `0.75f` and log error: "Jump height conversion failed. Using default 0.75f. Check gravity and force values."

## Speed Control Integration
- **Context**: The current system applies forces with fixed multipliers, while the new system enforces strict speed limits.
- **Legacy Parameters**:
  - Variable: `forwardFactor`
  - Value: `8.0f`
  - Meaning: Multiplier for forward movement force.
  - Variable: `backwardFactor`
  - Value: `3.0f`
  - Meaning: Multiplier for backward movement force.
  - Variable: `breakFactor`
  - Value: `10.0f`
  - Meaning: Multiplier for braking force.
  - Source: `PlayerCameraController.cs`
- **Target Parameters**:
  - Variable: `inputSpeedLimit`
  - Value: `6.0f`
  - Meaning: Maximum speed from player input.
  - Variable: `physicsSpeedLimit`
  - Value: `6.5f`
  - Meaning: Maximum speed from physics calculations.
  - Variable: `totalSpeedLimit`
  - Value: `7.0f`
  - Meaning: Absolute maximum speed allowed.
- **Conversion Logic**:
  ```csharp
  // File: Assets/Scripts/Settings/PhysicsSettings.cs
  // Purpose: Convert legacy force factors to speed limits
  float ConvertSpeedFactorToLimit(float factor)
  {
      // Convert AddForce factor to speed limit (to be calibrated)
      return factor * 0.75f; // Approximate base conversion, adjust based on empirical data
  }
  ```
- **Validation Steps**:
  1. Implement conversion function in `PhysicsSettings.cs`.
  2. Apply to `forwardFactor` (8.0f) and compare with `inputSpeedLimit` (6.0f).
  3. If converted value deviates > 0.5f from target, log issue for calibration in `/Status/Issues_and_Required_Cleanup.md`.
- **Error Handling**: If conversion results in values outside logical bounds (e.g., negative speeds), default to target values (`6.0f`, `6.5f`, `7.0f`) and log error: "Speed conversion out of bounds. Using default limits."
- **Migration Note**: Gradually transition from force-based to speed-limited system by applying limits as constraints while maintaining legacy force application during hybrid phases.

## Validation for LLM
- **Parameter Mapping Check**: Confirm understanding of legacy parameters (`legacyJumpForce=5.0f`, `forwardFactor=8.0f`) and target parameters (`targetJumpHeight=0.75f`, `totalSpeedLimit=7.0f`).
- **Conversion Logic Check**: Verify that conversion functions are clear and can be implemented in `PhysicsSettings.cs`.
- **Error Prevention**: Ensure error handling steps are followed if conversion results are invalid. Do not proceed with unvalidated conversions.

**Directive for LLM**: Implement parameter mapping as part of `PhysicsSettings` creation in Phase 0A. Log all validation results and issues in `/Status/Issues_and_Required_Cleanup.md`. Update `/Status/Project_Overview.md` with progress on parameter mapping.

**Next Step**: Proceed to `LLM_01D_Rollback_Strategy.md` for details on rollback mechanisms after confirming understanding of parameter mapping.
