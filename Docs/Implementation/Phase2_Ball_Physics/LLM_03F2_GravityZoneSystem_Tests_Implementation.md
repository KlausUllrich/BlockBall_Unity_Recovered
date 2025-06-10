---
title: "Phase 2 Ball Physics - Automated Tests: Gravity Zone System (Part 2 of 2)"
phase: "Phase 2 - Ball Physics"
part: "2 of 2"
dependencies:
  - "LLM_03F1_GravityZoneSystem_Tests_Core.md"
validation_steps:
  - "Test cardinal axis snapping when leaving all gravity zones."
  - "Verify multi-zone handling selects closest pivot point as dominant."
  - "Confirm gravity zones override GroundDetector state transitions."
  - "Validate state priority ensures Transitioning state during gravity changes."
integration_points:
  - "Tests complete gravity zone system functionality."
  - "Validates critical airborne gravity transition capability."
---

# Phase 2: Ball Physics - Gravity Zone System Tests (Part 2 of 2)

## Advanced Test Implementations

```csharp
        [UnityTest]
        public IEnumerator TestCardinalAxisSnapping()
        {
            // Test gravity snapping to nearest cardinal axis when leaving zones
            Debug.Log("=== Testing Cardinal Axis Snapping ===");

            // Create gravity zone with diagonal direction
            var gravityZone = CreateGravityZone(Vector3.zero);
            
            // Enter and complete transition
            ballObject.transform.position = Vector3.zero;
            yield return new WaitForFixedUpdate();
            
            // Leave gravity zone
            ballObject.transform.position = new Vector3(10f, 0f, 0f); // Far from zone
            yield return new WaitForFixedUpdate();
            
            // Verify gravity snapped to nearest cardinal axis (should be -Y)
            Vector3 snappedGravity = PhysicsSettings.Instance.GetCurrentGravity();
            Vector3 expectedDirection = Vector3.down; // Y axis was dominant
            
            float directionDeviation = Vector3.Distance(snappedGravity.normalized, expectedDirection);
            Assert.IsTrue(directionDeviation < 0.1f, 
                $"Gravity should snap to cardinal axis. Got {snappedGravity.normalized}, expected {expectedDirection}");
            
            Debug.Log($"Cardinal snapping successful: {snappedGravity.normalized} (expected {expectedDirection})");
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestMultiZoneHandling()
        {
            // Test multiple overlapping zones - closest pivot should dominate
            Debug.Log("=== Testing Multi-Zone Handling ===");

            // Create two gravity zones with different pivot points
            var zone1 = CreateGravityZone(new Vector3(-2f, 0f, 0f));    // Farther
            var zone2 = CreateGravityZone(new Vector3(0.5f, 0f, 0f)); // Closer
            
            // Position ball to be in both zones
            ballObject.transform.position = Vector3.zero;
            yield return new WaitForFixedUpdate();
            
            // Verify closest zone dominates
            Assert.AreEqual(zone2.GetComponent<GravityZone>(), gravityDetector.DominantZone,
                "Closest pivot point should determine dominant zone");
            
            // Verify gravity direction matches dominant zone
            Vector3 finalGravity = PhysicsSettings.Instance.GetCurrentGravity();
            float deviation = Vector3.Distance(finalGravity.normalized, (Vector3.zero - zone2.transform.position).normalized);
            
            Assert.IsTrue(deviation < 0.1f, "Gravity should match dominant zone direction");
            Debug.Log("Multi-zone handling successful - closest pivot dominates");
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestStateMachinePriority()
        {
            // Test that gravity zones override GroundDetector state changes
            Debug.Log("=== Testing State Machine Priority ===");

            // Create gravity zone
            var gravityZone = CreateGravityZone(Vector3.zero);
            
            // Set up scenario where GroundDetector would normally force Airborne
            ballObject.transform.position = new Vector3(0, ballRadius + 2f, 0); // High up
            stateMachine.TryTransitionTo(BallState.Airborne, "High altitude");
            yield return new WaitForFixedUpdate();
            
            // Move into gravity zone
            ballObject.transform.position = Vector3.zero;
            yield return new WaitForFixedUpdate();
            
            // Gravity zone should override GroundDetector
            Assert.AreEqual(BallState.Transitioning, stateMachine.CurrentState,
                "Gravity zone should override GroundDetector and force Transitioning state");
            
            // Verify state remains Transitioning during gravity transition
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
                Assert.AreEqual(BallState.Transitioning, stateMachine.CurrentState,
                    "State should remain Transitioning during gravity zone transition");
            }
            
            Debug.Log("State machine priority test passed - gravity zones override GroundDetector");
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestAirborneGravityTransitions()
        {
            // CRITICAL TEST: Verify gravity changes work while ball is airborne
            Debug.Log("=== Testing Airborne Gravity Transitions (CRITICAL) ===");

            // Create gravity zone
            var gravityZone = CreateGravityZone(Vector3.zero);
            
            // Force ball into Airborne state
            ballObject.transform.position = new Vector3(0, ballRadius + 3f, 0); // High altitude
            stateMachine.TryTransitionTo(BallState.Airborne, "Test setup");
            yield return new WaitForFixedUpdate();
            
            // Verify starting state
            Assert.AreEqual(BallState.Airborne, stateMachine.CurrentState, "Ball should start airborne");
            
            // Move airborne ball into gravity zone
            ballObject.transform.position = new Vector3(1f, ballRadius + 3f, 1f); // Still high but in zone
            yield return new WaitForFixedUpdate();
            
            // CRITICAL: Gravity zone should work while airborne
            Assert.AreEqual(BallState.Transitioning, stateMachine.CurrentState,
                "CRITICAL: Gravity zone must trigger Transitioning state even when airborne");
            
            // Verify gravity direction changed
            Vector3 expectedDirection = (ballObject.transform.position - Vector3.zero).normalized;
            Vector3 actualDirection = PhysicsSettings.Instance.GetCurrentGravity().normalized;
            float deviation = Vector3.Distance(actualDirection, expectedDirection);
            
            Assert.IsTrue(deviation < 0.1f,
                $"CRITICAL: Gravity direction should change while airborne. Expected {expectedDirection}, got {actualDirection}");
            
            Debug.Log("CRITICAL TEST PASSED: Airborne gravity transitions work correctly");
            yield return null;
        }

        private void SetupTestScene()
        {
            // Create ground plane for reference
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = Vector3.one * 10f;
        }

        private GameObject CreateGravityZone(Vector3 pivotPoint)
        {
            var zoneObject = new GameObject("TestGravityZone");
            
            // Add trigger collider
            var collider = zoneObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = Vector3.one * 8f; // Large zone to contain test positions
            
            // Add gravity zone component
            var gravityZone = zoneObject.AddComponent<GravityZone>();
            gravityZone.pivotPoint = pivotPoint;
            
            // Set zone layer
            zoneObject.layer = 8; // Gravity zone layer
            
            return zoneObject;
        }
    }
}
```

## Context & Dependencies
**Requires Phase 1 Completion**: This test suite builds on:
- `GravityZoneDetector` component for zone detection and gravity switching
- `PhysicsSettings` for gravity direction management and cardinal snapping
- `BallStateMachine` for state transition priority handling

## Validation Instructions
1. **Cardinal Snapping**: Verify gravity correctly snaps to nearest axis (X, Y, Z) when leaving zones
2. **Multi-Zone Priority**: Test that closest pivot point determines dominant zone in overlapping areas
3. **State Priority**: Confirm gravity zones override GroundDetector and maintain Transitioning state
4. **Airborne Transitions**: **CRITICAL** - validate gravity changes work while ball is airborne
5. **Instant Changes**: Ensure all gravity changes are immediate without smooth transitions

## Next Steps
Run this test suite via Unity Test Runner to validate the complete gravity zone system. This completes testing for the critical missing functionality that enables airborne gravity transitions. Proceed to `LLM_03G1_SingleSourceOfTruth_Tests_Core.md` for physics parameter validation testing.
