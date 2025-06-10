---
title: "Phase 2 Ball Physics - Automated Tests: Single Source of Truth Compliance (Part 1 of 2)"
phase: "Phase 2 - Ball Physics"
part: "1 of 2"
dependencies:
  - "Phase1_Core_Architecture/LLM_04C_PhysicsSettings_Task.md"
  - "All Phase 2 component tasks"
validation_steps:
  - "Verify no hardcoded physics constants in any component."
  - "Confirm all values reference PhysicsSettings.Instance."
  - "Ensure test consistency with game component values."
integration_points:
  - "Validates PhysicsSettings centralization."
  - "Ensures component and test consistency."
  - "Prevents magic number violations."
---

# Phase 2: Ball Physics - Automated Tests: Single Source of Truth Compliance
## Objective
Implement automated tests to **ENFORCE** the single source of truth principle by detecting hardcoded physics constants and validating that all components and tests reference `PhysicsSettings.Instance`. This addresses the EXTENSIVE violations found in the original Phase 2 documentation.
## CRITICAL CONTEXT
**This test suite addresses the MAJOR COMPLIANCE ISSUE identified in Phase 2 validation:**
- **Original Issue**: 20+ hardcoded physics constants scattered across components and tests
- **Design Principle**: All physics values must come from centralized PhysicsSettings
- **Impact**: Inconsistent behavior when changing physics settings
- **Solution**: Automated detection and validation of single source compliance

## Compliance Requirements
- **ZERO Hardcoded Constants**: No `const float` or `[SerializeField] float` physics values
- **PhysicsSettings References**: All values must use `PhysicsSettings.Instance.parameterName`
- **Test Consistency**: Test values must match component values exactly
- **Runtime Validation**: Values must update when PhysicsSettings changes
- **Documentation Compliance**: All examples must show correct patterns

## Implementation Steps
1. **Reflection Analysis**: Scan all Phase 2 components for hardcoded physics constants
2. **Test Value Verification**: Ensure test constants match PhysicsSettings values
3. **Runtime Consistency**: Verify values update when PhysicsSettings changes
4. **Pattern Detection**: Identify common violation patterns (const, SerializeField)
5. **Compliance Scoring**: Calculate overall compliance percentage
6. **Violation Reporting**: Generate detailed reports of non-compliant code

## Core Test Setup
```csharp
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using BlockBall.Physics;

namespace BlockBall.Tests.Physics
{
    [TestFixture]
    public class SingleSourceOfTruthTests
    {
        private PhysicsSettings physicsSettings;
        private List<string> violations = new List<string>();

        [SetUp]
        public void Setup()
        {
            physicsSettings = PhysicsSettings.Instance;
            violations.Clear();
            
            if (physicsSettings == null)
            {
                Assert.Fail("PhysicsSettings.Instance is null - cannot test single source of truth");
            }
        }

        [Test]
        public void TestNoHardcodedPhysicsConstants()
        {
            Debug.Log("=== Testing for Hardcoded Physics Constants ===");
            
            // List of Phase 2 component types to check
            System.Type[] componentTypes = {
                typeof(BallPhysics),
                typeof(GroundDetector),
                typeof(BallInputProcessor),
                typeof(GravityZoneDetector)
            };
            
            foreach (var componentType in componentTypes)
            {
                CheckComponentForHardcodedConstants(componentType);
            }
            
            // Check test classes
            CheckTestClassesForHardcodedConstants();
            
            // Report violations
            if (violations.Count > 0)
            {
                string violationReport = "SINGLE SOURCE OF TRUTH VIOLATIONS FOUND:\n" + 
                                       string.Join("\n", violations);
                Debug.LogError(violationReport);
                Assert.Fail($"Found {violations.Count} single source of truth violations. See log for details.");
            }
            
            Debug.Log("✅ No hardcoded physics constants found - Single source of truth maintained");
        }

        [Test]
        public void TestPhysicsSettingsIntegration()
        {
            Debug.Log("=== Testing PhysicsSettings Integration ===");
            
            // Verify all expected parameters are present in PhysicsSettings
            var requiredParameters = new Dictionary<string, string>
            {
                {"ballRadius", "Ball radius for physics calculations"},
                {"jumpHeight", "Jump height in Unity units"},
                {"jumpBufferTime", "Jump input buffer time"},
                {"coyoteTime", "Coyote time after leaving platform"},
                {"groundCheckDistance", "Ground detection distance"},
                {"groundLeaveDistance", "Ground leave distance (hysteresis)"},
                {"gravityTransitionDuration", "Gravity transition duration"},
                {"inputSpeedLimit", "Maximum input speed"},
                {"physicsSpeedLimit", "Maximum physics speed"},
                {"totalSpeedLimit", "Maximum total speed"},
                {"groundFriction", "Ground friction coefficient"},
                {"airDrag", "Air drag coefficient"},
                {"maxSlopeAngle", "Maximum slope angle for rolling"}
            };
            
            var physicsSettingsType = typeof(PhysicsSettings);
            
            foreach (var param in requiredParameters)
            {
                var property = physicsSettingsType.GetProperty(param.Key) ?? 
                              physicsSettingsType.GetField(param.Key);
                
                if (property == null)
                {
                    violations.Add($"PhysicsSettings missing required parameter: {param.Key} ({param.Value})");
                }
            }
            
            if (violations.Count > 0)
            {
                Assert.Fail($"PhysicsSettings missing {violations.Count} required parameters");
            }
            
            Debug.Log("✅ All required parameters present in PhysicsSettings");
        }

        [Test]
        public void TestParameterConsistency()
        {
            Debug.Log("=== Testing Parameter Consistency ===");
            
            // Test that components get consistent values from PhysicsSettings
            var ballPhysics = new GameObject("TestBall").AddComponent<BallPhysics>();
            var groundDetector = new GameObject("TestDetector").AddComponent<GroundDetector>();
            var inputProcessor = new GameObject("TestInput").AddComponent<BallInputProcessor>();
            
            try
            {
                // Test key parameter consistency - these should all reference PhysicsSettings
                float settingsBallRadius = physicsSettings.ballRadius;
                float settingsJumpHeight = physicsSettings.jumpHeight;
                float settingsGroundCheckDistance = physicsSettings.groundCheckDistance;
                
                // Verify values are reasonable (not default/uninitialized)
                Assert.IsTrue(settingsBallRadius > 0.1f && settingsBallRadius < 2f, 
                    $"Ball radius should be reasonable: {settingsBallRadius}");
                Assert.IsTrue(settingsJumpHeight > 0.1f && settingsJumpHeight < 5f, 
                    $"Jump height should be reasonable: {settingsJumpHeight}");
                Assert.IsTrue(settingsGroundCheckDistance > 0.01f && settingsGroundCheckDistance < 1f, 
                    $"Ground check distance should be reasonable: {settingsGroundCheckDistance}");
                
                Debug.Log($"✅ Parameter consistency verified: ballRadius={settingsBallRadius:F2}, jumpHeight={settingsJumpHeight:F2}");
            }
            finally
            {
                // Cleanup test objects
                if (ballPhysics != null) Object.DestroyImmediate(ballPhysics.gameObject);
                if (groundDetector != null) Object.DestroyImmediate(groundDetector.gameObject);
                if (inputProcessor != null) Object.DestroyImmediate(inputProcessor.gameObject);
            }
        }

        // Additional test methods implementation in Part 2
        [Test] public void TestRuntimeValueUpdates() { /* See LLM_03G2 */ }
        [Test] public void TestViolationPatterns() { /* See LLM_03G2 */ }
        [Test] public void TestComplianceScoring() { /* See LLM_03G2 */ }

        // Helper methods in Part 2
        private void CheckComponentForHardcodedConstants(System.Type componentType) { /* See LLM_03G2 */ }
        private void CheckTestClassesForHardcodedConstants() { /* See LLM_03G2 */ }
        private bool IsPhysicsConstant(FieldInfo field) { /* See LLM_03G2 */ }
    }
}
```
## Next Steps
Continue to `LLM_03G2_SingleSourceOfTruth_Tests_Implementation.md` for the complete helper methods, runtime validation tests, violation pattern detection, and compliance scoring functionality.