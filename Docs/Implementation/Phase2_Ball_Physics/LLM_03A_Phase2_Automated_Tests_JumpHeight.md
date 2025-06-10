---
title: "Phase 2 Ball Physics - Automated Tests for Jump Height"
phase: "Phase 2 - Ball Physics"
dependencies:
  - "LLM_01A_Phase2_Mission_and_Objectives.md"
  - "LLM_01B_Phase2_Technical_Specifications.md"
  - "LLM_02B_BallPhysics_Component_Task.md"
validation_steps:
  - "Verify that jump height achieves exactly 0.75 Unity units (6 Bixels) under standard conditions."
  - "Confirm test results are logged with pass/fail criteria and deviation metrics."
  - "Ensure test accounts for different gravity settings if applicable."
integration_points:
  - "Tests BallPhysics.cs for jump mechanics accuracy."
  - "Integrates with Unity Test Framework for automation."
---

# Phase 2: Ball Physics - Automated Tests for Jump Height

## Objective
Create an automated test script to validate that the ball's jump mechanic in `BallPhysics.cs` achieves a precise height of 0.75 Unity units (6 Bixels) under standard conditions, logging results with pass/fail criteria and deviation metrics for zero-error validation.

## Test Overview
- **Purpose**: Ensure jump height meets the design specification of exactly 0.75 Unity units, critical for gameplay consistency in BlockBall Evolution.
- **Key Metrics**:
  - Target Jump Height: 0.75 units ± 0.01 unit tolerance.
  - Test under standard gravity (from `PhysicsSettings`).
  - Measure peak height after jump initiation.
- **Environment**: Unity Test Framework (NUnit) in Editor mode for automated execution.

## Test Implementation Steps
1. **Setup Test Scene**: Create a simple test scene with a flat ground plane and a ball GameObject equipped with `BallPhysics`, `BallStateMachine`, and related components.
2. **Configure Ball**: Set ball properties (mass 1.0 kg, radius 0.5 units) and jump height target (0.75 units) as per specifications.
3. **Jump Trigger**: Simulate a jump request via `BallPhysics.RequestJump()` when the ball is in Grounded state.
4. **Height Tracking**: Record the ball’s vertical position over time to detect the peak height reached after jump.
5. **Validation Check**: Compare peak height to target (0.75 units) with a tolerance of ±0.01 units for precision.
6. **Result Logging**: Log test results including pass/fail status, actual height, deviation from target, and test conditions.
7. **Automation**: Use Unity Test Framework’s `[UnityTest]` attribute for coroutine-based testing over multiple frames.

## Test Script Template
Below is a test script template for jump height validation. Place this in a Unity test folder (e.g., `Assets/Tests/Editor/`) and ensure it runs in the Editor with the Test Runner.

```csharp
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace BlockBall.Physics.Tests
{
    public class BallJumpHeightTests
    {
        // FIXED: Reference PhysicsSettings instead of hardcoded constants
        private float ballRadius => PhysicsSettings.Instance.ballRadius;
        private float targetJumpHeight => PhysicsSettings.Instance.jumpHeight;
        private float gravityStrength => PhysicsSettings.Instance.GetCurrentGravity().magnitude;
        private float tolerance => 0.05f; // 5% tolerance for physics calculations

        private GameObject ballObject;
        private BallPhysics ballPhysics;
        private BallStateMachine stateMachine;
        private Vector3 initialPosition;
        private const float maxTestDuration = 2.0f; // Seconds before timeout

        [SetUp]
        public void Setup()
        {
            // Create ground plane
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.transform.localScale = new Vector3(10, 1, 10); // Large ground

            // Create ball with components
            ballObject = new GameObject("TestBall");
            ballObject.transform.position = new Vector3(0, ballRadius, 0); // Start just above ground
            ballPhysics = ballObject.AddComponent<BallPhysics>();
            stateMachine = new BallStateMachine(); // Assuming accessible via BallPhysics
            initialPosition = ballObject.transform.position;

            // Configure PhysicsSettings if needed (gravity, etc.)
            PhysicsSettings.Instance.Gravity = new Vector3(0, -gravityStrength, 0); // Standard gravity
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(ballObject);
            Object.DestroyImmediate(GameObject.Find("Plane"));
        }

        [UnityTest]
        public IEnumerator TestJumpHeight()
        {
            // Force Grounded state for jump eligibility
            stateMachine.TryTransitionTo(BallState.Grounded, "Test setup");

            // Trigger jump
            ballPhysics.RequestJump();
            Debug.Log("Jump requested for height test.");

            float peakHeight = initialPosition.y;
            float elapsedTime = 0f;

            // Monitor height over time until peak or timeout
            while (elapsedTime < maxTestDuration)
            {
                ballPhysics.PhysicsUpdate(Time.fixedDeltaTime); // Simulate physics step
                stateMachine.Update(Time.fixedDeltaTime);

                float currentHeight = ballObject.transform.position.y;
                peakHeight = Mathf.Max(peakHeight, currentHeight);

                // If back to ground or past peak, end test
                if (stateMachine.CurrentState == BallState.Grounded && elapsedTime > 0.5f)
                    break;

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            // Calculate relative height from starting point
            float relativePeakHeight = peakHeight - initialPosition.y;
            float deviation = Mathf.Abs(relativePeakHeight - targetJumpHeight);

            // Log results
            string resultMessage = $"Jump Height Test: Peak height = {relativePeakHeight:F3} units (Target = {targetJumpHeight} units, Deviation = {deviation:F3} units)";
            Debug.Log(resultMessage);

            // Assert with tolerance
            bool passed = deviation <= tolerance;
            Assert.IsTrue(passed, $"Jump height test failed. Expected {targetJumpHeight} ± {tolerance} units, but got {relativePeakHeight} units.");

            yield return null;
        }
    }
}
```

## Context & Dependencies
**Requires Phase 2 Components**: This test builds on:
- `BallPhysics.cs` for jump mechanics and physics updates.
- `BallStateMachine` for state determination (Grounded to Airborne transition).
- `PhysicsSettings` for gravity and environmental parameters.
- Unity Test Framework for automated test execution in Editor.

## Validation Instructions
1. **Height Precision**: Ensure the test validates jump height as specified in `PhysicsSettings.Instance.jumpHeight` with a tolerance of ±0.05 units.
2. **Test Setup**: Confirm the test scene setup (ground plane, ball position) matches standard gameplay conditions.
3. **State Handling**: Verify the test forces Grounded state initially and detects Airborne transition post-jump.
4. **Result Logging**: Check that logs include pass/fail status, actual height, deviation, and test conditions for traceability.
5. **Timeout Safety**: Ensure the test has a timeout (e.g., 2 seconds) to prevent infinite loops if jump fails.

## Next Steps
After implementing the jump height test, proceed to `LLM_03B_Phase2_Automated_Tests_StateMachine.md` for automated tests on state machine transitions. Log progress in `/Status/Project_Overview.md` and any issues in `/Status/Issues_and_Required_Cleanup.md`.
