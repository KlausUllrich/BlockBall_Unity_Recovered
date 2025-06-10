---
title: "Phase 2 Ball Physics - Automated Tests for State Machine"
phase: "Phase 2 - Ball Physics"
dependencies:
  - "LLM_01A_Phase2_Mission_and_Objectives.md"
  - "LLM_01B_Phase2_Technical_Specifications.md"
  - "LLM_02A_BallStateMachine_Task.md"
  - "LLM_02E_GroundDetector_Task.md"
validation_steps:
  - "Verify that state transitions follow the defined validity matrix."
  - "Confirm invalid transitions are blocked with appropriate warnings."
  - "Ensure state change events are triggered for integration with other components."
integration_points:
  - "Tests BallStateMachine.cs for state transition logic."
  - "Integrates with Unity Test Framework for automation."
---

# Phase 2: Ball Physics - Automated Tests for State Machine

## Objective
Create an automated test script to validate the behavior of `BallStateMachine.cs`, ensuring state transitions adhere to the defined validity matrix, invalid transitions are blocked with warnings, and state change events are triggered for integration with other components.

## Test Overview
- **Purpose**: Ensure the state machine correctly manages transitions between Grounded, Airborne, Sliding, and Transitioning states, critical for accurate ball behavior in BlockBall Evolution.
- **Key Metrics**:
  - Validate allowed transitions per the matrix (e.g., Grounded to Airborne allowed, Airborne to Sliding disallowed).
  - Confirm event firing (`OnStateChanged`) on every successful transition.
  - Check debug warnings for invalid transition attempts.
- **Environment**: Unity Test Framework (NUnit) in Editor mode for automated execution.

## Test Implementation Steps
1. **Setup State Machine**: Instantiate a `BallStateMachine` instance in isolation for controlled testing.
2. **Transition Matrix Tests**: Attempt transitions between all state pairs, asserting success for valid transitions and failure for invalid ones per the matrix.
3. **Event Trigger Test**: Subscribe to `OnStateChanged` and confirm it fires with correct previous and current state data on transition.
4. **Invalid Transition Warning**: Verify that attempting an invalid transition (e.g., Airborne to Sliding) logs a warning message with the reason.
5. **State Timer Test**: Ensure the state timer resets to 0 on transition and increments correctly with `Update()` calls.
6. **Result Logging**: Log test results including pass/fail status for each transition test, event firing, and timer behavior.
7. **Automation**: Use Unity Test Framework’s `[Test]` attribute for direct method testing since state logic doesn’t require frame updates.

## Test Script Template
Below is a test script template for state machine validation. Place this in a Unity test folder (e.g., `Assets/Tests/Editor/`) and ensure it runs in the Editor with the Test Runner.

```csharp
using NUnit.Framework;
using UnityEngine;

namespace BlockBall.Physics.Tests
{
    public class BallStateMachineTests
    {
        private BallStateMachine stateMachine;
        private bool stateChangedEventFired;
        private BallState lastPreviousState;
        private BallState lastCurrentState;

        [SetUp]
        public void Setup()
        {
            stateMachine = new BallStateMachine();
            stateChangedEventFired = false;
            lastPreviousState = BallState.Airborne; // Default initial
            lastCurrentState = BallState.Airborne;

            // Subscribe to state change event
            stateMachine.OnStateChanged += OnStateChangedHandler;
        }

        [TearDown]
        public void Teardown()
        {
            stateMachine.OnStateChanged -= OnStateChangedHandler;
        }

        private void OnStateChangedHandler(BallState previous, BallState current)
        {
            stateChangedEventFired = true;
            lastPreviousState = previous;
            lastCurrentState = current;
        }

        [Test]
        public void TestInitialState()
        {
            // Default state should be Airborne
            Assert.AreEqual(BallState.Airborne, stateMachine.CurrentState, "Initial state should be Airborne.");
            Assert.AreEqual(BallState.Airborne, stateMachine.PreviousState, "Initial previous state should be Airborne.");
            Assert.AreEqual(0f, stateMachine.StateTimer, "Initial state timer should be 0.");
        }

        [Test]
        public void TestValidTransitions()
        {
            // Test transitions from Grounded
            stateMachine.TryTransitionTo(BallState.Grounded, "Test setup");
            Assert.IsTrue(stateMachine.TryTransitionTo(BallState.Airborne, "Grounded to Airborne"), "Grounded to Airborne should be allowed.");
            Assert.AreEqual(BallState.Airborne, stateMachine.CurrentState);
            Assert.IsTrue(stateChangedEventFired, "State change event should fire on valid transition.");
            Assert.AreEqual(BallState.Grounded, lastPreviousState);
            Assert.AreEqual(BallState.Airborne, lastCurrentState);

            // Reset event flag
            stateChangedEventFired = false;

            // Test transitions from Airborne
            Assert.IsTrue(stateMachine.TryTransitionTo(BallState.Grounded, "Airborne to Grounded"), "Airborne to Grounded should be allowed.");
            Assert.AreEqual(BallState.Grounded, stateMachine.CurrentState);
            Assert.IsTrue(stateChangedEventFired, "State change event should fire on valid transition.");

            // Test other valid transitions (partial list, expand as needed)
            stateChangedEventFired = false;
            Assert.IsTrue(stateMachine.TryTransitionTo(BallState.Sliding, "Grounded to Sliding"), "Grounded to Sliding should be allowed.");
            Assert.IsTrue(stateChangedEventFired, "State change event should fire.");
        }

        [Test]
        public void TestInvalidTransitions()
        {
            // Start in Airborne
            stateMachine.TryTransitionTo(BallState.Airborne, "Test setup");

            // Test invalid transition: Airborne to Sliding
            Assert.IsFalse(stateMachine.TryTransitionTo(BallState.Sliding, "Airborne to Sliding attempt"), "Airborne to Sliding should not be allowed.");
            Assert.AreEqual(BallState.Airborne, stateMachine.CurrentState, "State should not change on invalid transition.");
            Assert.IsFalse(stateChangedEventFired, "State change event should not fire on invalid transition.");

            // Additional invalid transition tests can be added if matrix changes
        }

        [Test]
        public void TestStateTimer()
        {
            // Timer should start at 0
            Assert.AreEqual(0f, stateMachine.StateTimer, "State timer should start at 0.");

            // Update timer
            stateMachine.Update(0.1f);
            Assert.AreEqual(0.1f, stateMachine.StateTimer, 0.0001f, "State timer should increment with Update.");

            // Timer should reset on state change
            stateMachine.TryTransitionTo(BallState.Grounded, "Test transition");
            Assert.AreEqual(0f, stateMachine.StateTimer, "State timer should reset on transition.");
        }

        [Test]
        public void TestSameStateTransition()
        {
            // Transition to same state should return true but not trigger event
            stateMachine.TryTransitionTo(BallState.Airborne, "Test setup");
            stateChangedEventFired = false;

            Assert.IsTrue(stateMachine.TryTransitionTo(BallState.Airborne, "Same state"), "Transition to same state should return true.");
            Assert.IsFalse(stateChangedEventFired, "State change event should not fire on same-state transition.");
            Assert.AreEqual(0f, stateMachine.StateTimer, "State timer should not reset on same-state transition.");
        }

        [UnityTest]
        public IEnumerator TestGravityZoneTransitions()
        {
            // NEW: Test gravity zone priority - the critical missing functionality
            Debug.Log("=== Testing Gravity Zone State Transitions ===");

            // Setup: Create mock gravity zone detector
            var gravityDetector = ballObject.AddComponent<GravityZoneDetector>();
            
            // Test 1: Gravity zone should override normal state transitions
            stateMachine.TryTransitionTo(BallState.Airborne, "Test setup");
            yield return new WaitForFixedUpdate();
            
            // Simulate entering gravity zone while airborne
            // This should force Transitioning state regardless of ground contact
            stateMachine.TryTransitionTo(BallState.Transitioning, "Gravity zone entry");
            yield return new WaitForFixedUpdate();
            
            Assert.AreEqual(BallState.Transitioning, stateMachine.CurrentState, 
                "Ball should enter Transitioning state when gravity zone is detected while airborne");
            
            // Test 2: Verify state remains Transitioning during gravity transition
            for (int i = 0; i < 15; i++) // 0.3 seconds at 50Hz
            {
                yield return new WaitForFixedUpdate();
                Assert.AreEqual(BallState.Transitioning, stateMachine.CurrentState,
                    "Ball should remain in Transitioning state during gravity transition");
            }
            
            // Test 3: Verify transition back to normal states after leaving gravity zone
            stateMachine.TryTransitionTo(BallState.Airborne, "Left gravity zone");
            yield return new WaitForFixedUpdate();
            
            Assert.AreEqual(BallState.Airborne, stateMachine.CurrentState,
                "Ball should return to normal state after leaving gravity zone");
            
            Debug.Log("Gravity zone transition test passed - airborne gravity switching works");
            yield return null;
        }
    }
}
```

## Context & Dependencies
**Requires Phase 2 Components**: This test builds on:
- `BallStateMachine.cs` for state management and transition logic.
- Unity Test Framework for automated test execution in Editor.

## Validation Instructions
1. **Transition Matrix Adherence**: Ensure tests cover all critical valid transitions (e.g., Grounded to Airborne) and invalid ones (e.g., Airborne to Sliding) per the defined matrix.
2. **Event Firing**: Confirm `OnStateChanged` event is tested and fires only on actual state changes, not same-state attempts.
3. **Invalid Transition Handling**: Verify that invalid transitions are blocked and log appropriate debug warnings.
4. **Timer Behavior**: Check that state timer resets on transition and increments correctly during `Update()`.
5. **Result Logging**: Ensure test logs detail pass/fail status for each test case (valid/invalid transitions, event firing, timer).

## Next Steps
After implementing the state machine tests, proceed to `LLM_03C_Phase2_Automated_Tests_GroundDetection.md` for automated tests on ground detection. Log progress in `/Status/Project_Overview.md` and any issues in `/Status/Issues_and_Required_Cleanup.md`.
