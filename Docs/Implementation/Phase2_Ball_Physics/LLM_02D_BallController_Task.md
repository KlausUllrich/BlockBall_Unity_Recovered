---
title: "Phase 2 Ball Physics - BallController Implementation Task"
phase: "Phase 2 - Ball Physics"
dependencies:
  - "LLM_01A_Phase2_Mission_and_Objectives.md"
  - "LLM_01B_Phase2_Technical_Specifications.md"
  - "LLM_01E_Phase2_Integration_Strategy.md"
  - "LLM_02B_BallPhysics_Component_Task.md"
  - "LLM_02C_BallInputProcessor_Task.md"
validation_steps:
  - "Verify that BallController coordinates input and physics components correctly."
  - "Confirm it serves as the central hub for ball behavior logic."
  - "Ensure integration with existing Player.cs for high-level control."
integration_points:
  - "Integrates with BallPhysics.cs and BallInputProcessor.cs for behavior control."
  - "Connects to existing Player.cs for player interaction."
---

# Phase 2: Ball Physics - BallController Implementation Task

## Objective
Implement `BallController.cs` as the high-level coordinator for ball behavior, integrating input from `BallInputProcessor`, physics from `BallPhysics`, and state management, while serving as the bridge to the existing `Player.cs` for overall player control.

## Component Overview
- **File**: `BallController.cs`
- **Purpose**: Acts as the central logic hub for ball behavior, coordinating input processing, physics application, and state transitions, ensuring cohesive operation of all ball-related systems.
- **Key Features**:
  - Coordinates between `BallInputProcessor` for player commands and `BallPhysics` for movement execution.
  - Manages high-level behavior rules (e.g., when to allow input based on state).
  - Interfaces with `Player.cs` to integrate ball control into the broader player system.

## Implementation Steps
1. **Component References**: Acquire references to `BallPhysics`, `BallInputProcessor`, and `BallStateMachine` (via `BallPhysics`) during initialization.
2. **Input Coordination**: Relay input availability to `BallInputProcessor` based on game state or external conditions (e.g., disable input during cutscenes).
3. **Behavior Logic**: Define high-level rules for ball behavior, such as enabling/disabling physics effects or input based on game events.
4. **State Feedback**: Monitor state changes from `BallStateMachine` to trigger gameplay events (e.g., sound effects on state transition).
5. **Player Integration**: Connect to `Player.cs` to ensure ball control aligns with player status, health, or other game systems.
6. **Debug Tools**: Add debug visualization or logging for ball control flow to assist in troubleshooting.

## Code Template
Below is a partial code template for `BallController.cs`. Implement this in the Unity project under the appropriate namespace (e.g., `BlockBall.Physics`).

```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    public class BallController : MonoBehaviour
    {
        [SerializeField] private bool inputEnabled = true; // Toggle for input processing

        private BallPhysics ballPhysics;
        private BallInputProcessor inputProcessor;
        private BallStateMachine stateMachine; // Accessed via BallPhysics
        private Player player; // Reference to existing Player component

        void Awake()
        {
            ballPhysics = GetComponent<BallPhysics>();
            inputProcessor = GetComponent<BallInputProcessor>();
            stateMachine = ballPhysics.GetComponent<BallStateMachine>(); // Assuming access
            player = GetComponent<Player>(); // Connect to existing player system

            // Subscribe to state changes for behavior logic
            stateMachine.OnStateChanged += OnBallStateChanged;
        }

        void OnDestroy()
        {
            stateMachine.OnStateChanged -= OnBallStateChanged;
        }

        void Update()
        {
            // High-level control logic
            UpdateInputState();

            // Trigger gameplay events based on state or conditions
            HandleGameplayEvents();
        }

        private void UpdateInputState()
        {
            // Enable/disable input based on game conditions
            bool shouldInputBeEnabled = DetermineInputAvailability();
            if (inputEnabled != shouldInputBeEnabled)
            {
                inputEnabled = shouldInputBeEnabled;
                // Notify input processor or adjust directly if it has a toggle
                Debug.Log($"Ball input {(inputEnabled ? "enabled" : "disabled")}");
            }
        }

        private bool DetermineInputAvailability()
        {
            // Logic to determine if input should be processed
            if (player != null && !player.IsActive())
                return false; // Disable input if player is inactive (e.g., stunned)

            // Add other game state checks (cutscenes, menus, etc.)
            return true;
        }

        private void HandleGameplayEvents()
        {
            // Example: Play sound or trigger effect based on state
            if (stateMachine.CurrentState == BallState.Airborne && stateMachine.StateTimer < 0.1f)
            {
                // Play jump sound or trigger effect shortly after becoming airborne
                // AudioManager.PlaySound("Jump");
            }
        }

        private void OnBallStateChanged(BallState previous, BallState current)
        {
            // React to state changes with high-level logic
            Debug.Log($"Ball state changed from {previous} to {current}");

            // Trigger specific behaviors or events
            switch (current)
            {
                case BallState.Grounded:
                    // Trigger landing effects
                    break;
                case BallState.Airborne:
                    // Trigger airborne effects
                    break;
                case BallState.Sliding:
                    // Trigger sliding effects
                    break;
                case BallState.Transitioning:
                    // Handle gravity transition effects
                    break;
            }
        }

        // Public methods for external systems to interact with ball control
        public void DisableInput()
        {
            inputEnabled = false;
        }

        public void EnableInput()
        {
            inputEnabled = true;
        }

        public BallState GetCurrentState()
        {
            return stateMachine.CurrentState;
        }
    }
}
```

## Context & Dependencies
**Requires Phase 2 Components**: This component builds on:
- `BallPhysics.cs` for physics execution.
- `BallInputProcessor.cs` for processed player input.
- `BallStateMachine` for state information and transition events.
- Connects to existing `Player.cs` for integration into the broader game system.

## Validation Instructions
1. **Component Coordination**: Ensure `BallController` correctly references and interacts with `BallPhysics`, `BallInputProcessor`, and `BallStateMachine`.
2. **Input Control**: Verify that input can be enabled/disabled based on high-level game conditions (e.g., player inactive).
3. **State Reaction**: Confirm that state changes trigger appropriate gameplay events or effects (e.g., sound on landing).
4. **Player Integration**: Check that the controller interfaces with `Player.cs` to align ball behavior with player status.
5. **Debug Support**: Validate that debug logs or visualizations assist in tracking control flow and state changes.

## Next Steps
After implementing `BallController.cs`, proceed to `LLM_02E_GroundDetector_Task.md` for the ground detection component. Log progress in `/Status/Project_Overview.md` and any issues in `/Status/Issues_and_Required_Cleanup.md`.
