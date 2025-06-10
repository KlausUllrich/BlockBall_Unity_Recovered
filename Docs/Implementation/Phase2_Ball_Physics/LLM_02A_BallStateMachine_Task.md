---
title: "Phase 2 Ball Physics - BallStateMachine Implementation Task"
phase: "Phase 2 - Ball Physics"
dependencies:
  - "LLM_01A_Phase2_Mission_and_Objectives.md"
  - "LLM_01B_Phase2_Technical_Specifications.md"
  - "LLM_01E_Phase2_Integration_Strategy.md"
  - "Phase1_Migration_Strategy/LLM_03A_Phase1_Overview.md"
validation_steps:
  - "Verify that the BallStateMachine handles all defined states (Grounded, Airborne, Sliding, Transitioning)."
  - "Confirm that state transitions follow the defined validity matrix to prevent invalid shifts."
  - "Ensure event firing on state changes for integration with other components."
integration_points:
  - "Integrates with BallPhysics.cs for state-based physics behavior."
  - "Builds on Phase 1 core architecture (IPhysicsObject)."
---

# Phase 2: Ball Physics - BallStateMachine Implementation Task

## Objective
Implement `BallStateMachine.cs` to manage the ball's physics states (Grounded, Airborne, Sliding, Transitioning) with a validation matrix for allowed transitions, event firing on state changes, and debug logging for troubleshooting.

## Component Overview
- **File**: `BallStateMachine.cs`
- **Purpose**: Manages the current state of the ball, handles transitions between states based on game conditions, and notifies other components of state changes.
- **Key Features**:
  - Defines four states: Grounded (rolling on surface), Airborne (in flight), Sliding (on steep slopes >45°), Transitioning (during gravity changes).
  - Uses a transition validity matrix to prevent invalid state shifts.
  - Fires events on state change for integration with physics and input systems.
  - Tracks state duration with a timer for time-based logic.

## Implementation Steps
1. **Define Ball States**: Create an enum `BallState` with values `Grounded`, `Airborne`, `Sliding`, and `Transitioning` to represent all possible ball states.
2. **State Transition Matrix**: Implement a 2D boolean array `validTransitions` to define allowed transitions (e.g., Grounded can transition to Airborne, but Airborne cannot directly transition to Sliding).
3. **State Management**: Track `currentState`, `previousState`, and `stateTimer` to manage the active state and duration.
4. **Transition Logic**: Implement `TryTransitionTo()` to check transition validity using the matrix, update states, reset the timer, and fire the `OnStateChanged` event.
5. **State Behavior Hooks**: Add `OnStateEnter()` and `OnStateExit()` methods to handle state-specific setup and cleanup (e.g., enabling rolling physics for Grounded).
6. **Debug Logging**: Include debug logs for state transitions with reasons for traceability during testing.

## Code Template
Below is a code template for `BallStateMachine.cs`. Implement this in the Unity project under the appropriate namespace (e.g., `BlockBall.Physics`).

```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    public enum BallState
    {
        Grounded,      // Rolling on surface
        Airborne,      // In flight
        Sliding,       // On steep slope (>45°)
        Transitioning  // Gravity switch active
    }

    public class BallStateMachine
    {
        private BallState currentState = BallState.Airborne; // Start airborne
        private BallState previousState = BallState.Airborne;
        private float stateTimer = 0f;
        
        private readonly bool[,] validTransitions = new bool[4, 4]
        {
            { true, true, true, true },   // Grounded -> *
            { true, true, false, true },  // Airborne -> *
            { true, true, true, true },   // Sliding -> *
            { true, true, true, true }    // Transitioning -> *
        };

        public System.Action<BallState, BallState> OnStateChanged;

        public BallState CurrentState => currentState;
        public BallState PreviousState => previousState;
        public float StateTimer => stateTimer;

        public void Update(float deltaTime)
        {
            stateTimer += deltaTime;
        }

        public bool TryTransitionTo(BallState newState, string reason = "")
        {
            if (!CanTransitionTo(newState))
            {
                Debug.LogWarning($"Invalid state transition: {currentState} -> {newState}. Reason: {reason}");
                return false;
            }
            
            if (currentState == newState)
                return true; // Already in target state
            
            // Execute transition
            OnStateExit(currentState);
            
            previousState = currentState;
            currentState = newState;
            stateTimer = 0f;
            
            OnStateEnter(newState);
            
            // Notify listeners
            OnStateChanged?.Invoke(previousState, currentState);
            
            Debug.Log($"State transition: {previousState} -> {currentState} ({reason})");
            return true;
        }

        private bool CanTransitionTo(BallState newState)
        {
            return validTransitions[(int)currentState, (int)newState];
        }

        private void OnStateEnter(BallState state)
        {
            switch (state)
            {
                case BallState.Grounded:
                    // Enable rolling physics
                    break;
                    
                case BallState.Airborne:
                    // Enable air drag
                    break;
                    
                case BallState.Sliding:
                    // Reduce friction
                    break;
                    
                case BallState.Transitioning:
                    // Special gravity transition handling
                    break;
            }
        }

        private void OnStateExit(BallState state)
        {
            // Cleanup state-specific effects
            switch (state)
            {
                case BallState.Grounded:
                    // Disable ground-specific effects
                    break;
                    
                case BallState.Airborne:
                    // Clean up air effects
                    break;
                    
                case BallState.Sliding:
                    // Reset friction
                    break;
                    
                case BallState.Transitioning:
                    // End special handling
                    break;
            }
        }
    }
}
```

## Context & Dependencies
**Requires Phase 1 Completion**: This component builds on the architecture established in Phase 1:
- `IPhysicsObject` interface for integration with `BallPhysics.cs`.
- Relies on `GroundDetector.cs` (upcoming) for state determination inputs.

## Validation Instructions
1. **State Coverage**: Confirm that all states (Grounded, Airborne, Sliding, Transitioning) are implemented in the `BallState` enum and handled in `OnStateEnter()` and `OnStateExit()`.
2. **Transition Matrix**: Verify that the `validTransitions` matrix prevents invalid transitions (e.g., Airborne to Sliding directly is disallowed).
3. **Event Firing**: Ensure `OnStateChanged` event is triggered on every state change for other components to react.
4. **Debug Output**: Check that debug logs are informative and include transition reasons for debugging.

## Next Steps
After implementing `BallStateMachine.cs`, proceed to `LLM_02B_BallPhysics_Component_Task.md` for the main physics component implementation. Log progress in `/Status/Project_Overview.md` and any issues in `/Status/Issues_and_Required_Cleanup.md`.
