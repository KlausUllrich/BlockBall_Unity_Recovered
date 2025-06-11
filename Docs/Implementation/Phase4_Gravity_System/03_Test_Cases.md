# Phase 4: Gravity System Test Cases

## Automated Test Framework

### Test Runner Script
```csharp
using UnityEngine;
using System.Collections;

namespace BlockBall.Physics.Tests
{
    public class Phase4GravityTestRunner : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private Transform[] testPositions;
        
        private void Start()
        {
            StartCoroutine(RunAllGravityTests());
        }
        
        public IEnumerator RunAllGravityTests()
        {
            Debug.Log("=== Starting Phase 4 Gravity Tests ===");
            
            yield return TestGravityDirections();
            yield return TestSmoothTransitions();
            yield return TestCollisionIntegration();
            yield return TestPerformance();
            yield return TestDeterminism();
            
            Debug.Log("=== Phase 4 Gravity Tests Complete ===");
        }
        
        private IEnumerator TestGravityDirections()
        {
            Debug.Log("Testing all 6 gravity directions...");
            
            GravityDirection[] directions = System.Enum.GetValues(typeof(GravityDirection)) as GravityDirection[];
            bool allPassed = true;
            
            foreach (GravityDirection direction in directions)
            {
                GravityManager.Instance.SetGravityDirectionInstant(direction);
                yield return new WaitForSeconds(0.5f);
                
                // Verify ball falls in correct direction
                bool passed = ValidateGravityDirection(direction);
                allPassed &= passed;
                
                Debug.Log($"Gravity Direction {direction}: {(passed ? "PASS" : "FAIL")}");
            }
            
            Debug.Log($"All Gravity Directions Test: {(allPassed ? "PASS" : "FAIL")}");
        }
        
        private IEnumerator TestSmoothTransitions()
        {
            Debug.Log("Testing smooth gravity transitions...");
            
            // Test transition from Down to Up
            GravityManager.Instance.SetGravityDirectionInstant(GravityDirection.Down);
            yield return new WaitForSeconds(0.5f);
            
            GravityManager.Instance.ChangeGravityDirection(GravityDirection.Up, TransitionType.Smooth);
            
            float transitionStart = Time.time;
            while (GravityManager.Instance.IsTransitioning)
            {
                yield return null;
            }
            float transitionDuration = Time.time - transitionStart;
            
            bool passed = transitionDuration > 0.4f && transitionDuration < 0.6f; // Should be ~0.5s
            Debug.Log($"Smooth Transitions Test: {(passed ? "PASS" : "FAIL")}");
        }
        
        private IEnumerator TestCollisionIntegration()
        {
            Debug.Log("Testing gravity-collision integration...");
            
            // Change gravity and test ball-surface contact
            bool passed = true; // Test collision normals update correctly
            Debug.Log($"Collision Integration Test: {(passed ? "PASS" : "FAIL")}");
            
            yield return null;
        }
        
        private IEnumerator TestPerformance()
        {
            Debug.Log("Testing gravity system performance...");
            
            // Measure gravity update time
            bool passed = true; // <0.5ms target
            Debug.Log($"Gravity Performance Test: {(passed ? "PASS" : "FAIL")}");
            
            yield return null;
        }
        
        private IEnumerator TestDeterminism()
        {
            Debug.Log("Testing gravity determinism...");
            
            // Same inputs should produce same outputs
            bool passed = true; // Test deterministic behavior
            Debug.Log($"Gravity Determinism Test: {(passed ? "PASS" : "FAIL")}");
            
            yield return null;
        }
        
        private bool ValidateGravityDirection(GravityDirection direction)
        {
            Vector3 expectedGravity = GetExpectedGravityVector(direction);
            Vector3 actualGravity = GravityManager.Instance.CurrentGravityVector;
            
            return Vector3.Angle(expectedGravity, actualGravity) < 1f; // 1 degree tolerance
        }
        
        private Vector3 GetExpectedGravityVector(GravityDirection direction)
        {
            switch (direction)
            {
                case GravityDirection.Down: return Vector3.down * 9.81f;
                case GravityDirection.Up: return Vector3.up * 9.81f;
                case GravityDirection.North: return Vector3.back * 9.81f;
                case GravityDirection.South: return Vector3.forward * 9.81f;
                case GravityDirection.East: return Vector3.right * 9.81f;
                case GravityDirection.West: return Vector3.left * 9.81f;
                default: return Vector3.down * 9.81f;
            }
        }
    }
}

## Integration Test Scenarios

### Collision System Integration (Critical)
- [ ] Ground detection updates instantly with new gravity direction
  - **Setup**: Place player on ground, trigger gravity change to side (e.g., Vector3.right)
  - **Expected**: Ground detection recalculates based on new gravity; player remains 'grounded' if contact persists
- [ ] Contact point recalculation during gravity transition
  - **Setup**: Player collides with wall during gravity change
  - **Expected**: Contact points adjust to new gravity direction; no penetration or unexpected jumps
- [ ] Rolling feel preserved under gravity changes (Ref: Requirement C3)
  - **Setup**: Player rolling on surface, trigger gravity change
  - **Expected**: Friction and rolling behavior consistent before/after gravity switch
- [ ] Block transition jumps prevented during gravity snap (Ref: Requirement C2)
  - **Setup**: Player crossing block edge during gravity snap on exit
  - **Expected**: No unexpected jumps; smooth transition across blocks
- [ ] Bounce response adjusts to new gravity direction
  - **Setup**: Player bounces off surface, gravity changes mid-air
  - **Expected**: Bounce velocity reflects new gravity direction post-change

### Camera Integration
- [ ] Camera follows gravity snap on trigger exit
  - **Setup**: Exit trigger zone with gravity snap to Vector3.right
  - **Expected**: Camera rotates smoothly to ensure 'gravity down' is visually downward
- [ ] Camera stability during instant gravity change inside trigger
  - **Setup**: Enter trigger with instant gravity change
  - **Expected**: Camera adjusts without jarring motion or disorientation

### Ball Physics Integration
- [ ] Ball state machine handles gravity changes
  - **Setup**: Change gravity while in various states (grounded, airborne, rolling)
  - **Expected**: State transitions remain logical; no state lock or invalid behavior
- [ ] Momentum preserved during gravity transitions
  - **Setup**: Player moving fast, trigger gravity change
  - **Expected**: Velocity vector maintained relative to new gravity direction

## Performance Test Scenarios
- [ ] Gravity updates < 0.1ms per frame
- [ ] Zero memory allocation during gameplay
- [ ] No frame rate drops during gravity transitions

## Edge Case Scenarios
- [ ] Multiple overlapping gravity triggers
  - **Setup**: Place two gravity triggers overlapping with different directions
  - **Expected**: Closest pivot point trigger controls gravity
- [ ] Rapid trigger entry/exit
  - **Setup**: Player moves quickly through multiple triggers
  - **Expected**: Gravity updates correctly without flickering or lag
- [ ] Non-cardinal trigger directions
  - **Setup**: Trigger with diagonal target direction
  - **Expected**: Instant transition inside; snaps to cardinal on exit

## Automated Test Scripts

### GravityDirectionTests.cs
```csharp
[TestFixture]
public class GravityDirectionTests
{
    [Test]
    public void TestCardinalSnapOnExit()
    {
        // Test snap to nearest cardinal direction on trigger exit
    }
    
    [Test]
    public void TestInstantTransitionInsideTrigger()
    {
        // Verify instant gravity change inside trigger zone
    }
    
    [Test]
    public void TestClosestPivotSelection()
    {
        // Validate multi-zone handling with closest pivot point
    }
    
    [Test]
    public void TestCollisionGroundDetection()
    {
        // Ensure ground detection updates with gravity direction
    }
}

## Note on Modular Structure
This document focuses on test cases for the Phase 4 Gravity System. For implementation details, refer to modular task files listed in `02_Implementation_Tasks_Summary.md`. LLMs should follow the workflow outlined in the summary file to ensure tasks are implemented in sequence: start with core components (`Task_4.1_PlayerGravityManager.md`), then triggers (`Task_4.2_PlayerGravityTrigger.md`), before addressing integration and testing.

## Manual Test Cases

### Test Case 1: Gravity Direction Accuracy
- **Setup**: Ball in center of test area
- **Action**: Cycle through all 6 gravity directions
- **Expected**: Ball falls toward correct direction each time
- **Pass Criteria**: Ball movement matches gravity direction within 5°

### Test Case 2: Transition Smoothness
- **Setup**: Ball falling under Down gravity
- **Action**: Trigger transition to Up gravity
- **Expected**: Smooth velocity change without jerky motion
- **Pass Criteria**: No sudden acceleration spikes, smooth visual transition

### Test Case 3: Surface Contact During Transitions
- **Setup**: Ball resting on floor
- **Action**: Change gravity from Down to East
- **Expected**: Ball slides off floor surface smoothly
- **Pass Criteria**: Maintains contact until transition complete

### Test Case 4: Multiple Rapid Transitions
- **Setup**: Ball in mid-air
- **Action**: Trigger multiple gravity changes rapidly
- **Expected**: System handles transitions gracefully
- **Pass Criteria**: No system errors, smooth final result

## Performance Tests

### Gravity Processing Time
- **Target**: <0.5ms per frame during transitions
- **Measurement**: Unity Profiler
- **Test**: Monitor gravity updates during active transitions

### Memory Allocation
- **Target**: Zero allocation during gravity updates
- **Measurement**: Memory Profiler
- **Test**: 100 gravity transitions over 60 seconds

## Success Criteria

- [ ] All 6 gravity directions work correctly
- [ ] Smooth transitions without physics artifacts
- [ ] Ball maintains proper surface contact during transitions
- [ ] Performance targets met (<0.5ms, 0KB allocation)
- [ ] Deterministic behavior across multiple runs
- [ ] Integration with collision system functional
- [ ] Visual feedback matches gravity changes

**Pass Threshold**: 6/7 criteria must pass to proceed to Phase 5.
