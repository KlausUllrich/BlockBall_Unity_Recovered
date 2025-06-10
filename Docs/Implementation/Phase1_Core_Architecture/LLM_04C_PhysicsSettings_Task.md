---
title: Physics Settings Implementation
description: Task to create a ScriptableObject for physics configuration in BlockBall.
phase: 1A
dependencies: LLM_04A_Phase1_Overview.md, LLM_04B_Phase1_Deliverables.md
validation_criteria: Asset loads via Resources.Load(), sliders work in Inspector, conversion functions return expected values.
---

# Task 1.1: Create PhysicsSettings ScriptableObject

## Objective
Create a **single source of truth** for all physics configuration with a user-friendly interface for non-technical users.

## Background
The `PhysicsSettings` ScriptableObject centralizes all physics parameters, ensuring consistency across the game. It converts user-friendly values (e.g., jump height) into physics calculations (e.g., jump force), making tuning accessible without deep technical knowledge.

## Implementation Steps
1. **Create Script File**:
   - Path: `Assets/Scripts/Physics/PhysicsSettings.cs`
   - Action: Create a new C# script in Unity under the specified path.
2. **Add Code Template**:
   - Action: Copy the provided code template into `PhysicsSettings.cs`.
   - Purpose: This template includes user-friendly sliders, tooltips, and conversion functions for physics values.
3. **Design User-Friendly Interface**:
   - Action: Ensure the script uses `[Header]`, `[Tooltip]`, and `[Range]` attributes for clarity in the Unity Inspector.
   - Expected Outcome: Non-technical users can adjust values like `maxRollSpeed` (3-15 units/second) via sliders.
4. **Setup Asset Creation**:
   - Action: Use `[CreateAssetMenu]` attribute to allow creation via Unity's Create menu.
   - Menu Path: `BlockBall/Physics Settings`
5. **Create Default Asset**:
   - Action: Right-click in Unity Project window → Create → BlockBall → Physics Settings.
   - Name: `DefaultPhysicsSettings`
   - Location: `Assets/Resources/` (for automatic loading via `Resources.Load()`).
   - Configure: Set values per `BlockBall_Physics_Spec.md` (e.g., `jumpHeight = 0.75f` for 6 Bixels).

## Code Template (Part 1)
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    /// <summary>
    /// Single source of truth for all physics settings.
    /// Provides user-friendly interface for non-technical users.
    /// </summary>
    [CreateAssetMenu(fileName = "PhysicsSettings", menuName = "BlockBall/Physics Settings")]
    public class PhysicsSettings : ScriptableObject
    {
        [Header("Basic Movement (User-Friendly)")]
        [Tooltip("How fast the ball rolls at maximum speed (Unity units/second)")]
        [Range(3f, 15f)]
        public float maxRollSpeed = 6f;
        
        [Tooltip("How quickly the ball accelerates when rolling (1=slow, 10=instant)")]
        [Range(1f, 10f)]
        public float rollResponsiveness = 5f;
        
        [Tooltip("How high the ball jumps (Unity units)")]
        [Range(0.5f, 2f)]
        public float jumpHeight = 0.75f; // 6 Bixels
        
        [Tooltip("How far the ball can jump while moving (Unity units)")]
        [Range(1f, 4f)]
        public float maxJumpDistance = 1.5f; // 12 Bixels
        
        [Header("Physics Feel")]
        [Tooltip("How bouncy surfaces are (0=no bounce, 1=perfect bounce)")]
        [Range(0f, 1f)]
        public float bounciness = 0.3f;
        
        [Tooltip("How much surfaces slow the ball down (0=ice, 1=sandpaper)")]
        [Range(0f, 2f)]
        public float friction = 0.8f;
        
        [Header("Gravity System")]
        [Tooltip("Current gravity vector applied to the ball")]
        public Vector3 currentGravity = new Vector3(0, -9.81f, 0);
        
        [Tooltip("Standard gravity magnitude for instant calculation")]
        [Range(5f, 15f)]
        public float gravityMagnitude = 9.81f;
        
        [Header("Advanced Control")]
        [Tooltip("How much control you have while airborne (0=none, 1=full)")]
        [Range(0f, 1f)]
        public float airControl = 0.3f;
        
        [Tooltip("Time window to jump after leaving a ledge (seconds)")]
        [Range(0f, 0.3f)]
        public float coyoteTime = 0.1f;
        
        [Tooltip("Time window to buffer jump inputs (seconds)")]
        [Range(0f, 0.3f)]
        public float jumpBufferTime = 0.15f;
        
        [Header("Speed Control System")]
        [Tooltip("Maximum speed from player input only")]
        [Range(4f, 12f)]
        public float inputSpeedLimit = 6f;
        
        [Tooltip("Maximum speed including physics forces")]
        [Range(5f, 15f)]
        public float physicsSpeedLimit = 6.5f;
        
        [Tooltip("Absolute maximum speed (safety limit)")]
        [Range(6f, 20f)]
        public float totalSpeedLimit = 7f;
        
        [Header("Ball Properties")]
        [Tooltip("Ball radius for physics calculations")]
        [Range(0.1f, 1f)]
        public float ballRadius = 0.5f;
        
        [Tooltip("Distance to leave grounded state (hysteresis)")]
        [Range(0.1f, 1f)]
        public float groundLeaveDistance = 0.6f;
        
        [Tooltip("Ground detection distance from ball center")]
        [Range(0.1f, 1f)]
        public float groundCheckDistance = 0.55f;
        
        // CONVERSION FUNCTIONS: User-friendly values → Physics calculations
        // See LLM_04C_PhysicsSettings_Code.md for complete conversion functions.
    }
}
```

## Validation Steps
1. **Inspector Interface**:
   - Action: Open `DefaultPhysicsSettings` asset in Unity Inspector.
   - Check: All sliders (e.g., `maxRollSpeed`, `jumpHeight`) are visible with tooltips.
   - Expected Outcome: Interface is intuitive with categorized headers.
2. **Value Ranges**:
   - Action: Attempt to set invalid values (e.g., `inputSpeedLimit` > `physicsSpeedLimit`).
   - Check: `OnValidate()` method corrects or warns about invalid combinations.
   - Expected Outcome: Cannot set inconsistent speed limits.
3. **Asset Loading**:
   - Action: Write a test script to load the asset via `Resources.Load<PhysicsSettings>("DefaultPhysicsSettings")`.
   - Check: Asset loads successfully and values match defaults.
   - Expected Outcome: Asset is accessible at runtime.

## Error Handling
- **If Script Fails to Compile**: Check for syntax errors in the code template. Ensure Unity Editor is using a compatible C# version.
- **If Asset Creation Fails**: Verify `[CreateAssetMenu]` attribute is correctly set. Restart Unity if menu item doesn't appear.
- **If Values Don't Persist**: Ensure asset is saved in `Assets/Resources/` and not modified at runtime without serialization.
