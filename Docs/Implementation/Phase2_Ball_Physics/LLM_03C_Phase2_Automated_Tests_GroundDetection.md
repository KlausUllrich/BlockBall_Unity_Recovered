---
title: "Phase 2 Ball Physics - Automated Tests for Ground Detection"
phase: "Phase 2 - Ball Physics"
dependencies:
  - "LLM_01A_Phase2_Mission_and_Objectives.md"
  - "LLM_01B_Phase2_Technical_Specifications.md"
  - "LLM_02E_GroundDetector_Task.md"
  - "LLM_02A_BallStateMachine_Task.md"
validation_steps:
  - "Verify that ground detection accurately identifies contact within specified thresholds."
  - "Confirm slope detection correctly identifies steep slopes (>45°) for Sliding state."
  - "Ensure hysteresis logic prevents state flickering at edges."
integration_points:
  - "Tests GroundDetector.cs for contact and slope detection accuracy."
  - "Integrates with Unity Test Framework for automation."
---

# Phase 2: Ball Physics - Automated Tests for Ground Detection

## Objective
Create an automated test script to validate the behavior of `GroundDetector.cs`, ensuring accurate ground contact detection within specified thresholds (0.55-0.6 units), correct identification of steep slopes (>45°) for Sliding state, and hysteresis logic to prevent state flickering.

## Test Overview
- **Purpose**: Ensure ground detection reliably determines the ball’s state (Grounded, Airborne, Sliding), critical for correct physics behavior in BlockBall Evolution.
- **Key Metrics**:
  - Ground contact detection within PhysicsSettings.Instance.groundCheckDistance (entering Grounded) and PhysicsSettings.Instance.groundLeaveDistance (leaving Grounded) for hysteresis.
  - Slope angle detection above PhysicsSettings.Instance.maxSlopeAngle triggers Sliding state.
  - State stability with no rapid flickering at edge conditions.
- **Environment**: Unity Test Framework (NUnit) in Editor mode for automated execution.

## Test Implementation Steps
1. **Setup Test Scene**: Create a test scene with configurable ground planes (flat and sloped) and a ball GameObject with `GroundDetector`, `BallPhysics`, and `BallStateMachine`.
2. **Ground Contact Tests**: Position the ball at varying heights above a flat plane to test contact detection thresholds for entering and leaving Grounded state.
3. **Slope Detection Tests**: Place the ball on planes with different angles (e.g., 30°, 50°) to validate Sliding state transition above 45°.
4. **Hysteresis Test**: Move the ball across the threshold distances (0.55-0.6 units) multiple times to confirm no rapid state flickering occurs.
5. **State Transition Validation**: Verify `BallStateMachine` updates based on `GroundDetector` recommendations (Grounded, Sliding, Airborne).
6. **Result Logging**: Log test results including pass/fail status, detected distances, slope angles, and state stability metrics.
7. **Automation**: Use Unity Test Framework’s `[UnityTest]` attribute for coroutine-based testing over multiple frames to simulate physics behavior.

## Test Script Template
Below is a test script template for ground detection validation. Place this in a Unity test folder (e.g., `Assets/Tests/Editor/`) and ensure it runs in the Editor with the Test Runner.

```csharp
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace BlockBall.Physics.Tests
{
    public class GroundDetectorTests
    {
        private GameObject ballObject;
        private GroundDetector groundDetector;
        private BallStateMachine stateMachine;
        private GameObject groundPlane;
        private float ballRadius => PhysicsSettings.Instance.ballRadius;
        private float groundCheckDistance => PhysicsSettings.Instance.groundCheckDistance;
        private float groundLeaveDistance => PhysicsSettings.Instance.groundLeaveDistance;
        private float slopeThreshold => PhysicsSettings.Instance.maxSlopeAngle;

        [SetUp]
        public void Setup()
        {
            // Create configurable ground plane
            groundPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            groundPlane.transform.localScale = new Vector3(10, 1, 10); // Large ground

            // Create ball with components
            ballObject = new GameObject("TestBall");
            ballObject.transform.position = new Vector3(0, ballRadius + 0.1f, 0); // Just above ground
            groundDetector = ballObject.AddComponent<GroundDetector>();
            ballObject.AddComponent<BallPhysics>(); // Needed for radius access if applicable
            stateMachine = new BallStateMachine(); // Assuming accessible via BallPhysics or direct

            // Configure ground detector thresholds
            groundDetector.SetGroundCheckDistance(groundCheckDistance);
            groundDetector.SetGroundLeaveDistance(groundLeaveDistance);
            groundDetector.SetSlopeThreshold(slopeThreshold);

            // Set ground layer (assuming default layer is ground)
            groundPlane.layer = LayerMask.NameToLayer("Default");
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(ballObject);
            Object.DestroyImmediate(groundPlane);
        }

        [UnityTest]
        public IEnumerator TestGroundContactDetection()
        {
            // Test 1: Ball within ground check distance (should be Grounded)
            ballObject.transform.position = new Vector3(0, ballRadius + groundCheckDistance - 0.01f, 0);
            yield return new WaitForFixedUpdate();
            groundDetector.FixedUpdate(); // Simulate detection
            Assert.IsTrue(groundDetector.IsGrounded, "Ball should detect ground contact within check distance.");
            Assert.AreEqual(BallState.Grounded, stateMachine.CurrentState, "State should be Grounded within check distance.");

            // Test 2: Ball beyond leave distance (should be Airborne)
            ballObject.transform.position = new Vector3(0, ballRadius + groundLeaveDistance + 0.01f, 0);
            stateMachine.TryTransitionTo(BallState.Grounded, "Force setup");
            yield return new WaitForFixedUpdate();
            groundDetector.FixedUpdate();
            Assert.IsFalse(groundDetector.IsGrounded, "Ball should not detect ground contact beyond leave distance.");
            Assert.AreEqual(BallState.Airborne, stateMachine.CurrentState, "State should be Airborne beyond leave distance.");

            // Test 3: Hysteresis - move from grounded to just below leave distance (should stay Grounded)
            ballObject.transform.position = new Vector3(0, ballRadius + groundCheckDistance - 0.01f, 0);
            stateMachine.TryTransitionTo(BallState.Grounded, "Force setup");
            yield return new WaitForFixedUpdate();
            groundDetector.FixedUpdate();
            ballObject.transform.position = new Vector3(0, ballRadius + groundLeaveDistance - 0.01f, 0);
            yield return new WaitForFixedUpdate();
            groundDetector.FixedUpdate();
            Assert.IsTrue(groundDetector.IsGrounded, "Ball should remain grounded just below leave distance due to hysteresis.");
            Assert.AreEqual(BallState.Grounded, stateMachine.CurrentState, "State should remain Grounded due to hysteresis.");

            Debug.Log("Ground contact detection and hysteresis tests completed.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestSlopeDetection()
        {
            // Test 1: Flat surface (angle = 0°, should be Grounded)
            groundPlane.transform.rotation = Quaternion.identity; // Flat
            ballObject.transform.position = new Vector3(0, ballRadius + 0.1f, 0);
            yield return new WaitForFixedUpdate();
            groundDetector.FixedUpdate();
            Assert.IsTrue(groundDetector.IsGrounded, "Ball should detect ground on flat surface.");
            Assert.AreEqual(0f, groundDetector.SlopeAngle, 0.1f, "Slope angle should be 0 on flat surface.");
            Assert.AreEqual(BallState.Grounded, stateMachine.CurrentState, "State should be Grounded on flat surface.");

            // Test 2: Steep slope (>45°, should be Sliding)
            float steepAngle = slopeThreshold + 5f; // 50°
            groundPlane.transform.rotation = Quaternion.Euler(steepAngle, 0, 0);
            ballObject.transform.position = new Vector3(0, ballRadius + 0.1f, 0);
            yield return new WaitForFixedUpdate();
            groundDetector.FixedUpdate();
            Assert.IsTrue(groundDetector.IsGrounded, "Ball should detect ground on steep slope.");
            Assert.AreEqual(steepAngle, groundDetector.SlopeAngle, 1f, "Slope angle should match plane rotation (50°).");
            Assert.AreEqual(BallState.Sliding, stateMachine.CurrentState, "State should be Sliding on steep slope (>45°).");

            // Test 3: Shallow slope (<45°, should be Grounded)
            float shallowAngle = slopeThreshold - 10f; // 35°
            groundPlane.transform.rotation = Quaternion.Euler(shallowAngle, 0, 0);
            ballObject.transform.position = new Vector3(0, ballRadius + 0.1f, 0);
            yield return new WaitForFixedUpdate();
            groundDetector.FixedUpdate();
            Assert.IsTrue(groundDetector.IsGrounded, "Ball should detect ground on shallow slope.");
            Assert.AreEqual(shallowAngle, groundDetector.SlopeAngle, 1f, "Slope angle should match plane rotation (35°).");
            Assert.AreEqual(BallState.Grounded, stateMachine.CurrentState, "State should be Grounded on shallow slope (<45°).");

            Debug.Log("Slope detection tests completed.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestStateStability()
        {
            // Test rapid position changes to ensure no flickering
            stateMachine.TryTransitionTo(BallState.Grounded, "Force setup");
            ballObject.transform.position = new Vector3(0, ballRadius + groundCheckDistance - 0.01f, 0); // Just grounded
            yield return new WaitForFixedUpdate();
            groundDetector.FixedUpdate();
            Assert.AreEqual(BallState.Grounded, stateMachine.CurrentState, "Initial state should be Grounded.");

            // Move slightly out and back in quickly
            for (int i = 0; i < 5; i++)
            {
                ballObject.transform.position = new Vector3(0, ballRadius + groundLeaveDistance + 0.01f, 0); // Just out
                yield return new WaitForFixedUpdate();
                groundDetector.FixedUpdate();

                ballObject.transform.position = new Vector3(0, ballRadius + groundCheckDistance - 0.01f, 0); // Just in
                yield return new WaitForFixedUpdate();
                groundDetector.FixedUpdate();

                Assert.AreEqual(BallState.Grounded, stateMachine.CurrentState, $"State should remain Grounded during rapid change iteration {i} due to hysteresis.");
            }

            Debug.Log("State stability test for flickering prevention completed.");
            yield return null;
        }
    }
}
```

## Context & Dependencies
**Requires Phase 2 Components**: This test builds on:
- `GroundDetector.cs` for ground contact and slope detection.
- `BallStateMachine.cs` for state transition based on detection results.
- `BallPhysics.cs` for ball properties like radius.
- Unity Test Framework for automated test execution in Editor.

## Validation Instructions
1. **Contact Thresholds**: Ensure tests validate ground detection at specified thresholds with hysteresis.
2. **Slope Accuracy**: Confirm slope detection correctly identifies angles above 45° for Sliding state and below for Grounded.
3. **State Stability**: Verify hysteresis prevents state flickering during rapid position changes near thresholds.
4. **Scene Setup**: Check that test scenes include both flat and sloped planes for comprehensive testing.
5. **Result Logging**: Ensure logs detail pass/fail status, detected distances, slope angles, and stability results for traceability.

## Next Steps
After implementing the ground detection tests, proceed to `LLM_03D_Phase2_Automated_Tests_RollingPhysics.md` for automated tests on rolling physics behavior. Log progress in `/Status/Project_Overview.md` and any issues in `/Status/Issues_and_Required_Cleanup.md`.
