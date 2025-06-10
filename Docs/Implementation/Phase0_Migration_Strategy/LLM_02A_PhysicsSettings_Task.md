---
title: "Phase 0A: Task 1 - PhysicsSettings ScriptableObject (LLM)"
phase: "0A"
context_required: ["PhysicsSettings", "ParameterMapping"]
dependencies: ["LLM_01A_Mission_and_Precision.md", "LLM_01B_Migration_Phases.md", "LLM_01C_Parameter_Mapping.md"]
output_files: ["Assets/Scripts/Settings/PhysicsSettings.cs", "Assets/Settings/PhysicsSettings.asset"]
validation_steps:
  - "Confirm PhysicsSettings script matches provided code"
  - "Verify asset creation at specified path"
  - "Check parameter values match legacy defaults"
  - "Test parameter validation function"
priority: 1
---

# Phase 0A: Task 1 - Create Enhanced PhysicsSettings ScriptableObject (LLM)

## Objective
Create a centralized physics configuration system using a ScriptableObject to manage both legacy and target physics parameters, facilitating migration without breaking existing functionality.

**Directive for LLM**: Follow the steps below precisely to implement `PhysicsSettings`. Validate each step and log progress or issues in the status files. Do not proceed to other tasks until validation is complete.

## Implementation Steps
1. **Verify Unity Editor Setup**:
   - Ensure Unity 2022.3 LTS is installed and the project is loaded at `C:\Users\Klaus\My_Game_Projects\Blockball\BlockBall_Unity_Recovered`.
   - **Validation**: If project path is incorrect, log error in `/Status/Issues_and_Required_Cleanup.md`: "Project path mismatch. Verify project location before proceeding."
2. **Create Script Directory**:
   - Create directory `Assets/Scripts/Settings/` if it does not exist.
   - **Validation**: Check directory existence. If creation fails, log error: "Failed to create Assets/Scripts/Settings/. Check write permissions."
3. **Create PhysicsSettings Script**:
   - Write the following C# script at `Assets/Scripts/Settings/PhysicsSettings.cs`.
   - **Code Block**:
     ```csharp
     // File: Assets/Scripts/Settings/PhysicsSettings.cs
     // Purpose: Centralized configuration for physics parameters with migration support
     using UnityEngine;
     
     [CreateAssetMenu(fileName = "PhysicsSettings", menuName = "BlockBall/Physics Settings")]
     public class PhysicsSettings : ScriptableObject
     {
         [Header("Migration Settings")]
         public PhysicsMode physicsMode = PhysicsMode.UnityPhysics;
         public bool enableMigrationLogging = true;
         public bool validateParameterConversion = true;
         
         [Header("Legacy Unity Physics Parameters")]
         [Tooltip("Current jump force used in PlayerCameraController")]
         [Range(1f, 10f)]
         public float legacyJumpForce = 5.0f;
         
         [Tooltip("Current speed factor from PlayerCameraController")]
         [Range(0.1f, 5f)]
         public float legacySpeedFactor = 1.0f;
         
         [Tooltip("Current break factor from PlayerCameraController")]
         [Range(1f, 20f)]
         public float legacyBreakFactor = 10.0f;
         
         [Header("Target Custom Physics Parameters")]
         [Tooltip("Target jump height in Unity units (6 Bixels)")]
         [Range(0.5f, 1.5f)]
         public float targetJumpHeight = 0.75f;
         
         [Tooltip("Maximum input speed limit")]
         [Range(4f, 10f)]
         public float inputSpeedLimit = 6.0f;
         
         [Tooltip("Physics calculation speed limit")]
         [Range(5f, 12f)]
         public float physicsSpeedLimit = 6.5f;
         
         [Tooltip("Absolute maximum speed")]
         [Range(6f, 15f)]
         public float totalSpeedLimit = 7.0f;
         
         [Header("Deterministic Math Settings")]
         [Tooltip("Fixed-point scale for critical calculations")]
         public int fixedPointScale = 1000000;
         
         [Tooltip("Error accumulation threshold")]
         [Range(100f, 10000f)]
         public float accumulationThreshold = 1000f;
         
         [Header("Performance Targets")]
         [Tooltip("Maximum physics update time in milliseconds")]
         [Range(0.5f, 5f)]
         public float maxPhysicsUpdateMs = 2.0f;
         
         [Tooltip("Target physics update frequency")]
         [Range(30, 120)]
         public int physicsUpdateHz = 50;
         
         // Parameter conversion utilities
         public float ConvertJumpForceToHeight()
         {
             // Empirical conversion formula (to be calibrated)
             float gravity = Physics.gravity.magnitude;
             return (legacyJumpForce * legacyJumpForce) / (2 * gravity);
         }
         
         public float ConvertSpeedFactorToLimit()
         {
             // Convert AddForce factor to speed limit (to be calibrated)
             return legacySpeedFactor * 6.0f; // Base conversion
         }
         
         public void ValidateSettings()
         {
             // Ensure parameter consistency
             if (physicsSpeedLimit <= inputSpeedLimit)
                 physicsSpeedLimit = inputSpeedLimit + 0.5f;
             if (totalSpeedLimit <= physicsSpeedLimit)
                 totalSpeedLimit = physicsSpeedLimit + 0.5f;
         }
     }
     
     public enum PhysicsMode
     {
         UnityPhysics,      // Current system (fallback)
         HybridPhysics,     // Partial custom implementation
         CustomPhysics,     // Full custom system
         ValidationMode     // Side-by-side comparison
     }
     ```
   - **Validation**: Confirm script is created and compiles without errors in Unity. If compilation fails, log error: "PhysicsSettings.cs compilation error. Check syntax and Unity version."
4. **Create Asset Folder**:
   - Create directory `Assets/Settings/` if it does not exist.
   - **Validation**: Check directory existence. If creation fails, log error: "Failed to create Assets/Settings/. Check write permissions."
5. **Create Asset Instance**:
   - In Unity Editor, create asset via menu `Assets > Create > BlockBall > Physics Settings` at `Assets/Settings/PhysicsSettings.asset`.
   - **Validation**: Confirm asset exists at specified path. If not, log error: "PhysicsSettings asset not created. Use Unity menu to create at specified path."
6. **Map Legacy Parameters**:
   - Set `legacyJumpForce = 5.0f`, `legacySpeedFactor = 1.0f`, `legacyBreakFactor = 10.0f` based on values from `PlayerCameraController.cs`.
   - **Validation**: Compare asset values with source file (`PlayerCameraController.cs`). If mismatch, log error: "Parameter mismatch in PhysicsSettings.asset. Update with values from PlayerCameraController.cs."
7. **Test Parameter Validation**:
   - Call `ValidateSettings()` method to ensure speed limits maintain hierarchy (`totalSpeedLimit > physicsSpeedLimit > inputSpeedLimit`).
   - **Validation**: If hierarchy is invalid, log error: "Speed limit hierarchy invalid in PhysicsSettings. Adjust values to maintain order."

## Acceptance Criteria for PhysicsSettings
- **Asset Creation Check**: Run script to verify `Assets/Settings/PhysicsSettings.asset` exists. If not, error message: "PhysicsSettings asset missing. Create via Unity menu: Assets > Create > BlockBall > Physics Settings."
- **Parameter Mapping Check**: Compare `legacyJumpForce` with value in `PlayerCameraController.cs`. If mismatch, log error: "Jump force mismatch. Update PhysicsSettings with value from PlayerCameraController.cs."
- **Validation Function Test**: Call `ValidateSettings()` and check if `totalSpeedLimit > physicsSpeedLimit > inputSpeedLimit`. If not, log error: "Speed limit hierarchy invalid. Adjust values in PhysicsSettings."

**Directive for LLM**: Log all validation results and issues in `/Status/Issues_and_Required_Cleanup.md`. Update `/Status/Project_Overview.md` with completion status of PhysicsSettings task.

**Next Step**: Proceed to `LLM_02B_IPhysicsObject_Task.md` for the next task after completing and validating PhysicsSettings implementation.
