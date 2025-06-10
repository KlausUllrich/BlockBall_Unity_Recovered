---
title: "Phase 2 Ball Physics - Automated Tests for Rolling Physics (Part 2 of 2)"
phase: "Phase 2 - Ball Physics"
part: "2 of 2"
dependencies:
  - "LLM_03D1_RollingPhysics_Tests_Setup.md"
validation_steps:
  - "Verify exponential decay above 6.65 u/s threshold."
  - "Test angular velocity correlation with linear velocity (ω = v / r)."
  - "Validate slope rolling maintains rolling friction characteristics."
integration_points:
  - "Completes automated validation of all rolling physics behaviors."
  - "Provides comprehensive logging and assertion reporting."
---

# Phase 2: Ball Physics - Automated Tests for Rolling Physics (Part 2 of 2)

## Complete Test Method Implementations

```csharp
            ballPhysics.PhysicsUpdate(Time.fixedDeltaTime);
            actualSpeed = ballPhysics.CurrentVelocity.magnitude;
            Assert.LessOrEqual(actualSpeed, physicsSpeedLimit + speedTolerance, "Physics speed should be capped at 6.5 u/s.");
            Debug.Log($"Physics speed test: Expected <= {physicsSpeedLimit}, Got {actualSpeed}");

            // Test 3: Total speed limit (7 u/s) - extreme force
            ballPhysics.SetVelocity(new Vector3(totalSpeedLimit + 2.0f, 0, 0)); // Exceed total limit
            yield return new WaitForFixedUpdate();
            ballPhysics.PhysicsUpdate(Time.fixedDeltaTime);
            actualSpeed = ballPhysics.CurrentVelocity.magnitude;
            Assert.LessOrEqual(actualSpeed, totalSpeedLimit + speedTolerance, "Total speed should be capped at 7 u/s.");
            Debug.Log($"Total speed test: Expected <= {totalSpeedLimit}, Got {actualSpeed}");

            // Test 4: Exponential decay above 6.65 u/s
            ballPhysics.SetVelocity(new Vector3(decayStartSpeed + 0.5f, 0, 0)); // Start above decay threshold
            float initialSpeed = ballPhysics.CurrentVelocity.magnitude;
            yield return new WaitForFixedUpdate();
            ballPhysics.PhysicsUpdate(Time.fixedDeltaTime);
            float newSpeed = ballPhysics.CurrentVelocity.magnitude;
            Assert.Less(newSpeed, initialSpeed, "Speed should decay exponentially above 6.65 u/s.");
            Assert.Greater(newSpeed, decayStartSpeed, "Speed should not drop below decay start in one frame.");
            Debug.Log($"Exponential decay test: Started at {initialSpeed}, Decayed to {newSpeed}");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestRollingRealism()
        {
            // Force Grounded state
            stateMachine.TryTransitionTo(BallState.Grounded, "Test setup");

            // Apply a linear velocity
            float testLinearSpeed = 3.0f; // Well below limits
            ballPhysics.SetVelocity(new Vector3(testLinearSpeed, 0, 0));
            yield return new WaitForFixedUpdate();
            ballPhysics.PhysicsUpdate(Time.fixedDeltaTime);

            // Check angular velocity
            Vector3 angularVelocity = ballPhysics.CurrentAngularVelocity;
            float expectedAngularSpeed = testLinearSpeed / ballRadius; // Angular = Linear / Radius
            float actualAngularSpeed = angularVelocity.magnitude;

            Assert.AreEqual(expectedAngularSpeed, actualAngularSpeed, 0.1f, "Angular velocity should match linear velocity divided by radius.");
            Debug.Log($"Rolling realism test: Linear speed = {testLinearSpeed}, Expected angular = {expectedAngularSpeed}, Got {actualAngularSpeed}");

            // Check direction (should be perpendicular for rolling)
            Vector3 linearDir = ballPhysics.CurrentVelocity.normalized;
            Vector3 angularAxis = angularVelocity.normalized;
            float dotProduct = Vector3.Dot(linearDir, angularAxis);
            Assert.AreEqual(0f, dotProduct, 0.1f, "Angular velocity axis should be perpendicular to linear velocity direction.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestRollingFrictionOnSlopes()
        {
            // CORRECTED: Slopes should maintain rolling friction, not reduce it
            // This aligns with the physics spec requirement for consistent rolling feel
            Debug.Log("=== Testing Rolling Friction on Slopes ===");

            // Create sloped ground (30° - within rolling threshold)
            GameObject slopedGround = CreateSlopedGround(30f);
            ballObject.transform.position = new Vector3(0, ballRadius + 0.1f, 0);
            stateMachine.TryTransitionTo(BallState.Grounded, "Test setup");
            yield return new WaitForFixedUpdate();

            // Apply constant input and measure steady-state speed
            Vector3 constantInput = Vector3.forward * inputSpeedLimit;
            float steadyStateSpeed = 0f;
            int measurements = 0;

            for (int i = 0; i < 100; i++) // 2 seconds at 50Hz
            {
                ballPhysics.ApplyInput(constantInput);
                yield return new WaitForFixedUpdate();

                // Measure speed after initial acceleration (frames 50-100)
                if (i >= 50)
                {
                    steadyStateSpeed += ballPhysics.Velocity.magnitude;
                    measurements++;
                }
            }

            steadyStateSpeed /= measurements;

            // CORRECTED EXPECTATION: Rolling friction should be consistent on slopes
            // The ball should maintain similar rolling characteristics, not slide freely
            float expectedSpeed = inputSpeedLimit * (1f - groundFriction * 0.5f); // Accounting for rolling resistance
            float deviation = Mathf.Abs(steadyStateSpeed - expectedSpeed);

            string resultMessage = $"Slope Rolling Test: Expected ~{expectedSpeed:F2} u/s, Got {steadyStateSpeed:F2} u/s (deviation: {deviation:F3})";
            Debug.Log(resultMessage);

            // FIXED: Test expects consistent rolling behavior, not reduced friction sliding
            bool passed = deviation <= speedTolerance;
            Assert.IsTrue(passed, $"Rolling friction on slopes test failed. Ball should maintain rolling characteristics, not slide with reduced friction.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestFrictionAndDrag()
        {
            // Test 1: Friction on ground
            stateMachine.TryTransitionTo(BallState.Grounded, "Test setup");
            float initialSpeed = 5.0f;
            ballPhysics.SetVelocity(new Vector3(initialSpeed, 0, 0));
            yield return new WaitForFixedUpdate();

            // Simulate friction decay over time
            float timeElapsed = 0f;
            while (timeElapsed < maxTestDuration && ballPhysics.CurrentVelocity.magnitude > 0.1f)
            {
                ballPhysics.PhysicsUpdate(Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
                timeElapsed += Time.fixedDeltaTime;
            }

            float finalSpeed = ballPhysics.CurrentVelocity.magnitude;
            Assert.Less(finalSpeed, initialSpeed, "Speed should decrease due to friction.");
            Debug.Log($"Friction test: Initial speed = {initialSpeed}, Final speed = {finalSpeed}");

            // Test 2: Air drag when airborne
            stateMachine.TryTransitionTo(BallState.Airborne, "Test setup");
            ballPhysics.SetVelocity(new Vector3(initialSpeed, 0, 0));
            yield return new WaitForFixedUpdate();

            // Simulate air drag decay
            timeElapsed = 0f;
            while (timeElapsed < maxTestDuration && ballPhysics.CurrentVelocity.magnitude > 0.1f)
            {
                ballPhysics.PhysicsUpdate(Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
                timeElapsed += Time.fixedDeltaTime;
            }

            float finalAirSpeed = ballPhysics.CurrentVelocity.magnitude;
            Assert.Less(finalAirSpeed, initialSpeed, "Speed should decrease due to air drag when airborne.");
            Debug.Log($"Air drag test: Initial speed = {initialSpeed}, Final speed = {finalAirSpeed}");

            yield return null;
        }

        // Helper method to create sloped ground
        private GameObject CreateSlopedGround(float angleInDegrees)
        {
            GameObject slope = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slope.transform.localScale = new Vector3(10, 0.1f, 10);
            slope.transform.rotation = Quaternion.Euler(angleInDegrees, 0, 0);
            slope.transform.position = new Vector3(0, -0.5f, 0);
            return slope;
        }
    }
}
```

## Context & Dependencies
**Requires Phase 1 Completion**: This test suite builds on:
- `BallPhysics` component for rolling mechanics testing
- `PhysicsSettings` for speed limits and friction coefficients
- `BallStateMachine` for state-based behavior validation

## Validation Instructions
1. **Speed Limit Validation**: All three speed tiers (6, 6.5, 7 u/s) must be respected with exponential decay above 6.65 u/s
2. **Rolling Correlation**: Angular velocity must equal linear velocity divided by radius (ω = v / r)
3. **Friction Consistency**: Slopes should maintain rolling friction, not reduce it for sliding
4. **Decay Behavior**: Both ground friction and air drag should appropriately reduce speed over time
5. **Test Duration**: All tests should complete within 3 seconds to avoid timeout

## Next Steps
Run this test suite via Unity Test Runner to validate rolling physics implementation. Proceed to `LLM_03F1_GravityZoneSystem_Tests_Core.md` for gravity zone system testing after completing rolling physics validation.
