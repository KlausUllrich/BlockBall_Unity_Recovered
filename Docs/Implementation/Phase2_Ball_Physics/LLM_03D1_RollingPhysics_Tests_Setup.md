---
title: "Phase 2 Ball Physics - Automated Tests for Rolling Physics (Part 1 of 2)"
phase: "Phase 2 - Ball Physics"
part: "1 of 2"
dependencies:
  - "LLM_01A_Phase2_Mission_and_Objectives.md"
  - "LLM_01B_Phase2_Technical_Specifications.md"
  - "LLM_02B1_BallPhysics_Component_Overview.md"
validation_steps:
  - "Verify that rolling physics respects three-tier speed limits (6, 6.5, 7 u/s)."
  - "Confirm angular velocity matches linear velocity for realistic rolling."
  - "Ensure friction and drag coefficients affect speed decay as expected."
integration_points:
  - "Tests BallPhysics.cs for rolling mechanics and speed constraints."
  - "Integrates with Unity Test Framework for automation."
---

# Phase 2: Ball Physics - Automated Tests for Rolling Physics

## Objective
Create an automated test script to validate the rolling physics behavior in `BallPhysics.cs`, ensuring adherence to the three-tier speed limit system (Input 6 u/s, Physics 6.5 u/s, Total 7 u/s), matching angular velocity to linear velocity for realistic rolling, and correct application of friction and drag coefficients.

## Test Overview
- **Purpose**: Ensure rolling mechanics are accurate and constrained per design specifications for consistent gameplay in BlockBall Evolution.
- **Key Metrics**:
  - Speed limits: Input (6 u/s), Physics (6.5 u/s), Total (7 u/s) with exponential decay starting at 6.65 u/s.
  - Angular velocity (rad/s) matches linear velocity (u/s) based on ball radius (0.5 units).
  - Friction and drag slow the ball appropriately per `PhysicsSettings` coefficients.
- **Environment**: Unity Test Framework (NUnit) in Editor mode for automated execution.

## Test Implementation Steps
1. **Setup Test Scene**: Create a test scene with a flat ground plane and a ball GameObject equipped with `BallPhysics` and related components.
2. **Speed Limit Tests**: Apply input or external forces to test speed caps at each tier (Input, Physics, Total) and validate exponential decay above 6.65 u/s.
3. **Rolling Realism Test**: Measure linear velocity and angular velocity during rolling to ensure they correlate (angular = linear / radius).
4. **Friction and Drag Test**: Apply an initial velocity, then let the ball roll without input to observe speed decay matching friction (grounded) and drag (airborne) coefficients.
5. **Result Logging**: Log test results including pass/fail status, measured speeds, velocity correlations, and decay rates.
6. **Automation**: Use Unity Test Framework's `[UnityTest]` attribute for coroutine-based testing over multiple frames to simulate physics behavior.

## Test Script Setup
Below is the test script setup and basic structure. Place this in a Unity test folder (e.g., `Assets/Tests/Editor/`) and ensure it runs in the Editor with the Test Runner.

```csharp
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace BlockBall.Physics.Tests
{
    public class BallRollingPhysicsTests
    {
        // FIXED: Reference PhysicsSettings instead of hardcoded constants
        private float ballRadius => PhysicsSettings.Instance.ballRadius;
        private float groundFriction => PhysicsSettings.Instance.groundFriction;
        private float maxSlopeAngle => PhysicsSettings.Instance.maxSlopeAngle;
        private float inputSpeedLimit => PhysicsSettings.Instance.inputSpeedLimit;

        private GameObject ballObject;
        private BallPhysics ballPhysics;
        private BallStateMachine stateMachine;
        private GameObject groundPlane;
        private const float physicsSpeedLimit = 6.5f; // Physics cap: 6.5 u/s
        private const float totalSpeedLimit = 7.0f; // Total cap: 7 u/s
        private const float decayStartSpeed = 6.65f; // Exponential decay start
        private const float speedTolerance = 0.05f; // ±0.05 u/s tolerance
        private const float maxTestDuration = 3.0f; // Seconds before timeout

        [SetUp]
        public void Setup()
        {
            // Create ground plane
            groundPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            groundPlane.transform.localScale = new Vector3(10, 1, 10); // Large ground

            // Create ball with components
            ballObject = new GameObject("TestBall");
            ballObject.transform.position = new Vector3(0, ballRadius, 0); // On ground
            ballPhysics = ballObject.AddComponent<BallPhysics>();
            stateMachine = new BallStateMachine(); // Assuming accessible via BallPhysics

            // Configure PhysicsSettings if needed
            PhysicsSettings.Instance.Gravity = new Vector3(0, -9.81f, 0); // Standard gravity
            PhysicsSettings.Instance.FrictionCoefficient = 0.1f; // Example friction
            PhysicsSettings.Instance.DragCoefficient = 0.05f; // Example drag
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(ballObject);
            Object.DestroyImmediate(groundPlane);
        }

        [UnityTest]
        public IEnumerator TestSpeedLimits()
        {
            // Force Grounded state
            stateMachine.TryTransitionTo(BallState.Grounded, "Test setup");

            // Test 1: Input speed limit (6 u/s)
            Vector3 inputVelocity = new Vector3(inputSpeedLimit + 1.0f, 0, 0); // Exceed limit
            ballPhysics.ApplyInputVelocity(inputVelocity);
            yield return new WaitForFixedUpdate();
            ballPhysics.PhysicsUpdate(Time.fixedDeltaTime);
            float actualSpeed = ballPhysics.CurrentVelocity.magnitude;
            Assert.LessOrEqual(actualSpeed, inputSpeedLimit + speedTolerance, "Input speed should be capped at 6 u/s.");
            Debug.Log($"Input speed test: Expected <= {inputSpeedLimit}, Got {actualSpeed}");

            // Test 2: Physics speed limit (6.5 u/s) - external force
            ballPhysics.SetVelocity(new Vector3(physicsSpeedLimit + 1.0f, 0, 0)); // Exceed physics limit
            yield return new WaitForFixedUpdate();
            // Continued in Part 2...
        }

        // Additional test methods implementation in Part 2
        [UnityTest] public IEnumerator TestRollingAnglerVelocityCorrelation() { /* See LLM_03D2 */ }
        [UnityTest] public IEnumerator TestFrictionDecay() { /* See LLM_03D2 */ }
        [UnityTest] public IEnumerator TestAirDragDecay() { /* See LLM_03D2 */ }
        [UnityTest] public IEnumerator TestSlopeRolling() { /* See LLM_03D2 */ }
    }
}
```

## Next Steps
Continue to `LLM_03D2_RollingPhysics_Tests_Implementation.md` for the complete test method implementations, rolling correlation tests, friction/drag validation, and slope rolling verification.
