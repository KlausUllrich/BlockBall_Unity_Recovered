---
title: "Phase 2 Ball Physics - Automated Tests: Gravity Zone System (Part 1 of 2)"
phase: "Phase 2 - Ball Physics"
part: "1 of 2"
dependencies:
  - "LLM_02F1_GravityZoneDetector_Overview.md"
  - "LLM_02A_BallStateMachine_Task.md"
  - "Phase1_Core_Architecture/LLM_04C_PhysicsSettings_Task.md"
validation_steps:
  - "Verify gravity zone detection triggers Transitioning state from Airborne."
  - "Confirm instant gravity changes when entering zones."
  - "Ensure gravity snaps to cardinal axes when leaving zones."
  - "Validate multi-zone handling selects closest pivot point."
integration_points:
  - "Uses PhysicsSettings gravity transition methods."
  - "Integrates with BallStateMachine state transitions."
  - "Tests GravityZoneDetector component functionality."
---

# Phase 2: Ball Physics - Automated Tests: Gravity Zone System
## Objective
Implement automated tests for the **CRITICAL** gravity zone system that was missing from the original Phase 2 documentation. These tests validate airborne gravity transitions, instant gravity changes, cardinal axis snapping, and multi-zone handling as required by the physics specification.

## CRITICAL CONTEXT
**This test suite addresses the FUNDAMENTAL FLAW identified in Phase 2 validation:**
- **Original Issue**: Ball could never transition gravity while airborne due to missing gravity zone system
- **Physics Spec Requirement**: "Gravity switches affect Ball in air and on ground"
- **Solution**: GravityZoneDetector component with highest priority state transitions
- **These tests ensure the core missing functionality works correctly**

## Test Coverage
- **Gravity Zone Detection**: Trigger detection when entering/leaving zones
- **Airborne Transitions**: Gravity changes while ball is airborne (CRITICAL)
- **Instant Gravity Changes**: Gravity changes instantly when entering zones
- **Cardinal Snapping**: Gravity snaps to nearest axis when leaving zones
- **Multi-Zone Priority**: Closest pivot point determines dominant zone
- **State Machine Integration**: Transitioning state priority over GroundDetector

## Implementation Steps
1. **Test Setup**: Create gravity zones with different directions and pivot points
2. **Airborne Detection**: Verify gravity zone detection works while ball is airborne
3. **Instant Gravity Changes**: Validate instant gravity changes when entering zones
4. **Cardinal Snapping**: Test gravity snapping to nearest axis (X, Y, Z)
5. **Multi-Zone Logic**: Test priority handling with overlapping zones
6. **State Priority**: Ensure gravity zones override GroundDetector state changes

## Core Test Setup
```csharp
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using BlockBall.Physics;

namespace BlockBall.Tests.Physics
{
    [TestFixture]
    public class GravityZoneSystemTests
    {
        // Reference PhysicsSettings for consistent testing
        private float ballRadius => PhysicsSettings.Instance.ballRadius;
        private Vector3 standardGravity => new Vector3(0, -9.81f, 0);

        private GameObject ballObject;
        private GravityZoneDetector gravityDetector;
        private BallStateMachine stateMachine;
        private BallPhysics ballPhysics;
        private GameObject[] gravityZones;

        [SetUp]
        public void Setup()
        {
            // Create test scene
            SetupTestScene();

            // Create ball with all required components
            ballObject = new GameObject("TestBall");
            ballObject.transform.position = new Vector3(0, ballRadius, 0);
            
            // Add required components
            ballPhysics = ballObject.AddComponent<BallPhysics>();
            stateMachine = ballObject.AddComponent<BallStateMachine>();
            gravityDetector = ballObject.AddComponent<GravityZoneDetector>();
            
            // Add collider for trigger detection
            var collider = ballObject.AddComponent<SphereCollider>();
            collider.radius = ballRadius;
            collider.isTrigger = true;
            
            // Add rigidbody for physics
            var rb = ballObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; // Controlled by custom physics
            
            // Initialize PhysicsSettings
            PhysicsSettings.Instance.currentGravity = standardGravity;
            PhysicsSettings.Instance.targetGravity = standardGravity;
        }

        [TearDown]
        public void TearDown()
        {
            if (ballObject != null)
                Object.DestroyImmediate(ballObject);
            
            if (gravityZones != null)
            {
                foreach (var zone in gravityZones)
                    if (zone != null) Object.DestroyImmediate(zone);
            }
        }

        [UnityTest]
        public IEnumerator TestInstantGravityTransition()
        {
            // Test instant gravity switching based on position
            Debug.Log("=== Testing Instant Gravity Transition ===");

            // Create gravity zone with pivot at origin
            var gravityZone = CreateGravityZone(Vector3.zero);
            
            // Start with standard gravity
            PhysicsSettings.Instance.currentGravity = standardGravity;
            
            // Position ball at specific location within zone
            Vector3 ballPosition = new Vector3(3f, 0f, 4f); // 5 units from pivot
            ballObject.transform.position = ballPosition;
            yield return new WaitForFixedUpdate();
            
            // Calculate expected gravity direction (from pivot to ball position)
            Vector3 expectedDirection = (ballPosition - Vector3.zero).normalized;
            
            // Verify gravity changed instantly
            Vector3 actualGravity = PhysicsSettings.Instance.GetCurrentGravity();
            Vector3 actualDirection = actualGravity.normalized;
            
            float directionDeviation = Vector3.Distance(actualDirection, expectedDirection);
            Assert.IsTrue(directionDeviation < 0.1f, 
                $"Gravity direction should change instantly. Expected {expectedDirection}, got {actualDirection}, deviation: {directionDeviation:F3}");
            
            // Verify magnitude is preserved
            float expectedMagnitude = PhysicsSettings.Instance.gravityMagnitude;
            float actualMagnitude = actualGravity.magnitude;
            Assert.IsTrue(Mathf.Abs(actualMagnitude - expectedMagnitude) < 0.1f,
                $"Gravity magnitude should be preserved. Expected {expectedMagnitude}, got {actualMagnitude}");
            
            Debug.Log($"Instant gravity transition successful: {actualDirection} (magnitude: {actualMagnitude:F2})");
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestPositionBasedGravityCalculation()
        {
            // Test that gravity direction follows ball position within zone
            Debug.Log("=== Testing Position-Based Gravity Calculation ===");

            var gravityZone = CreateGravityZone(Vector3.zero);
            
            // Test multiple positions within the zone
            Vector3[] testPositions = {
                new Vector3(1f, 0f, 0f),   // +X direction
                new Vector3(-1f, 0f, 0f),  // -X direction
                new Vector3(0f, 1f, 0f),   // +Y direction
                new Vector3(0f, -1f, 0f),  // -Y direction
                new Vector3(0f, 0f, 1f),   // +Z direction
                new Vector3(0f, 0f, -1f)   // -Z direction
            };

            foreach (var position in testPositions)
            {
                ballObject.transform.position = position;
                yield return new WaitForFixedUpdate();
                
                Vector3 expectedDirection = (position - Vector3.zero).normalized;
                Vector3 actualDirection = PhysicsSettings.Instance.GetCurrentGravity().normalized;
                
                float deviation = Vector3.Distance(actualDirection, expectedDirection);
                Assert.IsTrue(deviation < 0.1f, 
                    $"Position {position}: Expected gravity {expectedDirection}, got {actualDirection}, deviation: {deviation:F3}");
            }

            Debug.Log("Position-based gravity calculation test passed for all directions");
            yield return null;
        }

        // Additional test methods implementation in Part 2
        [UnityTest] public IEnumerator TestAirborneGravityTransitions() { /* See LLM_03F2 */ }
        [UnityTest] public IEnumerator TestMultiZonePriority() { /* See LLM_03F2 */ }
        [UnityTest] public IEnumerator TestCardinalAxisSnapping() { /* See LLM_03F2 */ }
        [UnityTest] public IEnumerator TestStateMachineIntegration() { /* See LLM_03F2 */ }

        // Helper methods in Part 2
        private GameObject CreateGravityZone(Vector3 pivotPoint) { /* See LLM_03F2 */ }
        private void SetupTestScene() { /* See LLM_03F2 */ }
    }
}
```
## Next Steps
Continue to `LLM_03F2_GravityZoneSystem_Tests_Implementation.md` for the complete test implementations, including airborne gravity transitions, multi-zone priority handling, cardinal axis snapping, and state machine integration testing.
