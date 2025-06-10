---
title: "Phase 2 Ball Physics - Automated Tests for Input Processing"
phase: "Phase 2 - Ball Physics"
dependencies:
  - "LLM_01A_Phase2_Mission_and_Objectives.md"
  - "LLM_01B_Phase2_Technical_Specifications.md"
  - "LLM_02C_BallInputProcessor_Task.md"
validation_steps:
  - "Verify that input is processed relative to camera orientation."
  - "Confirm diagonal input normalization prevents speed exploits."
  - "Ensure jump buffering (0.1s) and coyote time (0.15s) work as specified."
integration_points:
  - "Tests BallInputProcessor.cs for input handling accuracy."
  - "Integrates with Unity Test Framework for automation."
---

# Phase 2: Ball Physics - Automated Tests for Input Processing

## Objective
Create an automated test script to validate the behavior of `BallInputProcessor.cs`, ensuring input is processed relative to camera orientation, diagonal input is normalized to prevent speed exploits, and jump buffering (0.1s) and coyote time (0.15s) function as designed for responsive control.

## Test Overview
- **Purpose**: Ensure input handling is accurate and enhances gameplay feel in BlockBall Evolution by validating camera-relative movement, normalized input magnitude, and timing mechanics for jumps.
- **Key Metrics**:
  - Camera-relative input: Movement direction aligns with camera forward/right vectors.
  - Diagonal normalization: Input magnitude never exceeds 1.0 (e.g., forward+right input).
  - Jump buffering: Jump input within 0.1s before grounding is executed upon grounding.
  - Coyote time: Jump allowed within 0.15s after leaving ground.
- **Environment**: Unity Test Framework (NUnit) in Editor mode for automated execution.

## Test Implementation Steps
1. **Setup Test Scene**: Create a test scene with a flat ground plane, a ball GameObject with `BallInputProcessor`, `BallPhysics`, and `BallStateMachine`, and a configurable camera.
2. **Camera-Relative Input Test**: Rotate camera and apply input (e.g., forward) to confirm output velocity aligns with camera orientation.
3. **Diagonal Normalization Test**: Simulate combined inputs (e.g., forward+right) and verify the resulting velocity magnitude is capped at the same value as single-axis input.
4. **Jump Buffering Test**: Apply jump input just before grounding (within 0.1s) and confirm jump executes upon ground contact.
5. **Coyote Time Test**: Apply jump input just after leaving ground (within 0.15s) and confirm jump still executes.
6. **Result Logging**: Log test results including pass/fail status, input directions, magnitudes, and timing accuracies.
7. **Automation**: Use Unity Test Framework’s `[UnityTest]` attribute for coroutine-based testing over multiple frames to simulate timing and physics.

## Test Script Template
Below is a test script template for input processing validation. Place this in a Unity test folder (e.g., `Assets/Tests/Editor/`) and ensure it runs in the Editor with the Test Runner.

```csharp
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace BlockBall.Physics.Tests
{
    public class BallInputProcessorTests
    {
        private GameObject ballObject;
        private BallInputProcessor inputProcessor;
        private BallPhysics ballPhysics;
        private BallStateMachine stateMachine;
        private GameObject groundPlane;
        private GameObject cameraObject;
        private float jumpBufferTime => PhysicsSettings.Instance.jumpBufferTime; // Reference PhysicsSettings
        private float coyoteTime => PhysicsSettings.Instance.coyoteTime; // Reference PhysicsSettings
        private float inputMagnitudeLimit => PhysicsSettings.Instance.inputSpeedLimit; // Reference PhysicsSettings
        private const float timingTolerance = 0.01f; // Timing tolerance for tests
        private const float maxTestDuration = 2.0f; // Seconds before timeout

        [SetUp]
        public void Setup()
        {
            // Create ground plane
            groundPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            groundPlane.transform.localScale = new Vector3(10, 1, 10); // Large ground

            // Create ball with components
            ballObject = new GameObject("TestBall");
            ballObject.transform.position = new Vector3(0, 0.5f, 0); // On ground (radius 0.5)
            ballPhysics = ballObject.AddComponent<BallPhysics>();
            inputProcessor = ballObject.AddComponent<BallInputProcessor>();
            stateMachine = new BallStateMachine(); // Assuming accessible via BallPhysics

            // Create camera
            cameraObject = new GameObject("TestCamera");
            cameraObject.AddComponent<Camera>();
            cameraObject.transform.position = new Vector3(0, 5, -10); // Looking down at ball
            cameraObject.transform.LookAt(ballObject.transform.position);
            inputProcessor.SetCameraReference(cameraObject.transform); // Link camera to processor

            // Configure timing settings
            inputProcessor.SetJumpBufferTime(jumpBufferTime);
            inputProcessor.SetCoyoteTime(coyoteTime);
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(ballObject);
            Object.DestroyImmediate(groundPlane);
            Object.DestroyImmediate(cameraObject);
        }

        [UnityTest]
        public IEnumerator TestCameraRelativeInput()
        {
            // Force Grounded state
            stateMachine.TryTransitionTo(BallState.Grounded, "Test setup");

            // Test 1: Camera facing forward (default), input forward
            cameraObject.transform.rotation = Quaternion.Euler(30, 0, 0); // Looking down, forward
            Vector2 input = new Vector2(0, 1); // Forward input
            inputProcessor.ProcessInput(input);
            yield return new WaitForFixedUpdate();
            Vector3 velocity = ballPhysics.CurrentVelocity;
            Assert.AreEqual(0f, velocity.x, 0.1f, "Forward input with camera at 0° yaw should have no X component.");
            Assert.Greater(velocity.z, 0f, "Forward input with camera at 0° yaw should have positive Z component.");
            Debug.Log($"Camera forward test: Input {input}, Velocity {velocity}");

            // Test 2: Camera rotated 90° right, input forward
            cameraObject.transform.rotation = Quaternion.Euler(30, 90, 0); // Camera facing right
            inputProcessor.ProcessInput(input); // Same forward input
            yield return new WaitForFixedUpdate();
            velocity = ballPhysics.CurrentVelocity;
            Assert.Greater(velocity.x, 0f, "Forward input with camera at 90° yaw should have positive X component.");
            Assert.AreEqual(0f, velocity.z, 0.1f, "Forward input with camera at 90° yaw should have no Z component.");
            Debug.Log($"Camera right test: Input {input}, Velocity {velocity}");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestDiagonalInputNormalization()
        {
            // Force Grounded state
            stateMachine.TryTransitionTo(BallState.Grounded, "Test setup");
            cameraObject.transform.rotation = Quaternion.Euler(30, 0, 0); // Forward facing

            // Test 1: Single axis input (forward)
            Vector2 singleAxisInput = new Vector2(0, 1);
            inputProcessor.ProcessInput(singleAxisInput);
            yield return new WaitForFixedUpdate();
            Vector3 singleAxisVelocity = ballPhysics.CurrentVelocity;
            float singleAxisMagnitude = singleAxisVelocity.magnitude;
            Debug.Log($"Single axis input test: Input {singleAxisInput}, Velocity magnitude {singleAxisMagnitude}");

            // Test 2: Diagonal input (forward + right)
            Vector2 diagonalInput = new Vector2(1, 1);
            inputProcessor.ProcessInput(diagonalInput);
            yield return new WaitForFixedUpdate();
            Vector3 diagonalVelocity = ballPhysics.CurrentVelocity;
            float diagonalMagnitude = diagonalVelocity.magnitude;
            Assert.AreEqual(singleAxisMagnitude, diagonalMagnitude, 0.1f, "Diagonal input magnitude should be normalized to match single axis input.");
            Assert.Greater(diagonalVelocity.x, 0f, "Diagonal input should have positive X component (right).");
            Assert.Greater(diagonalVelocity.z, 0f, "Diagonal input should have positive Z component (forward).");
            Debug.Log($"Diagonal input test: Input {diagonalInput}, Velocity magnitude {diagonalMagnitude}");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestJumpBuffering()
        {
            // Start Airborne, just above ground
            stateMachine.TryTransitionTo(BallState.Airborne, "Test setup");
            ballObject.transform.position = new Vector3(0, 0.5f + 0.1f, 0); // Slightly above ground

            // Apply jump input before grounding (within buffer time)
            float timeBeforeGrounding = jumpBufferTime - 0.02f; // Within buffer time
            inputProcessor.ProcessJumpInput(true); // Simulate jump press
            yield return new WaitForSecondsRealtime(timeBeforeGrounding);

            // Ground the ball
            ballObject.transform.position = new Vector3(0, 0.5f, 0); // On ground
            stateMachine.TryTransitionTo(BallState.Grounded, "Simulate grounding");
            yield return new WaitForFixedUpdate();
            inputProcessor.Update(Time.fixedDeltaTime);
            ballPhysics.PhysicsUpdate(Time.fixedDeltaTime);

            // Check if jump was executed
            Assert.AreEqual(BallState.Airborne, stateMachine.CurrentState, "Jump should execute upon grounding due to buffering.");
            Assert.Greater(ballPhysics.CurrentVelocity.y, 0f, "Velocity should be upward after buffered jump.");
            Debug.Log("Jump buffering test: Jump executed after grounding within buffer time.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestCoyoteTime()
        {
            // Start Grounded
            stateMachine.TryTransitionTo(BallState.Grounded, "Test setup");
            ballObject.transform.position = new Vector3(0, 0.5f, 0); // On ground

            // Leave ground
            stateMachine.TryTransitionTo(BallState.Airborne, "Simulate leaving ground");
            ballObject.transform.position = new Vector3(0, 0.5f + 0.1f, 0); // Slightly above

            // Apply jump input within coyote time
            float timeAfterLeaving = coyoteTime - 0.02f; // Within coyote time
            yield return new WaitForSecondsRealtime(timeAfterLeaving);
            inputProcessor.ProcessJumpInput(true); // Simulate jump press
            yield return new WaitForFixedUpdate();
            inputProcessor.Update(Time.fixedDeltaTime);
            ballPhysics.PhysicsUpdate(Time.fixedDeltaTime);

            // Check if jump was executed
            Assert.Greater(ballPhysics.CurrentVelocity.y, 0f, "Velocity should be upward due to coyote time jump.");
            Assert.AreEqual(BallState.Airborne, stateMachine.CurrentState, "State should remain Airborne with upward velocity after coyote jump.");
            Debug.Log("Coyote time test: Jump executed within coyote time after leaving ground.");

            yield return null;
        }
    }
}
```

## Context & Dependencies
**Requires Phase 2 Components**: This test builds on:
- `BallInputProcessor.cs` for input handling and timing mechanics.
- `BallPhysics.cs` for velocity application based on input.
- `BallStateMachine.cs` for state context affecting input processing.
- Unity Test Framework for automated test execution in Editor.

## Validation Instructions
1. **Camera-Relative Input**: Ensure tests confirm input direction adjusts based on camera orientation (e.g., forward input aligns with camera forward).
2. **Diagonal Normalization**: Verify diagonal input (forward+right) results in the same speed magnitude as single-axis input.
3. **Jump Buffering**: Confirm jump input within buffer time before grounding triggers a jump upon ground contact.
4. **Coyote Time**: Ensure jump input within coyote time after leaving ground still triggers a jump.
5. **Result Logging**: Check that logs detail pass/fail status, input directions, magnitudes, and timing accuracies for traceability.

## Next Steps
After implementing the input processing tests, you have completed the core automated test suite for Phase 2. Proceed to `LLM_04A_Phase2_Completion_Checker_Overview.md` to start creating completion checker files for final validation. Log progress in `/Status/Project_Overview.md` and any issues in `/Status/Issues_and_Required_Cleanup.md`.
