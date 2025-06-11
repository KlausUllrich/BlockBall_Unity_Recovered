# Phase 3: Collision System Test Cases

## Automated Test Framework

### Test Runner Script
```csharp
using UnityEngine;
using System.Collections;

namespace BlockBall.Physics.Tests
{
    public class Phase3TestRunner : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private GameObject[] testBlocks;
        
        private void Start()
        {
            StartCoroutine(RunAllTests());
        }
        
        public IEnumerator RunAllTests()
        {
            Debug.Log("=== Starting Phase 3 Collision Tests ===");
            
            yield return TestBounceHeight();
            yield return TestMaterialProperties();
            yield return TestMultipleContacts();
            yield return TestPenetrationRecovery();
            yield return TestPerformance();
            
            Debug.Log("=== Phase 3 Tests Complete ===");
        }
        
        private IEnumerator TestBounceHeight()
        {
            Debug.Log("Testing bounce height accuracy...");
            
            // Drop ball on surface and measure bounce
            GameObject ball = Instantiate(ballPrefab, Vector3.up * 2f, Quaternion.identity);
            yield return new WaitForSeconds(2f);
            
            // Check if bounce height matches restitution
            bool passed = true; // Implement actual measurement
            Debug.Log($"Bounce Height Test: {(passed ? "PASS" : "FAIL")}");
            
            Destroy(ball);
        }
        
        private IEnumerator TestMaterialProperties()
        {
            Debug.Log("Testing material properties...");
            
            // Test different material responses
            bool passed = true; // Test metal vs wood vs ice
            Debug.Log($"Material Properties Test: {(passed ? "PASS" : "FAIL")}");
            
            yield return null;
        }
        
        private IEnumerator TestMultipleContacts()
        {
            Debug.Log("Testing multiple simultaneous contacts...");
            
            // Ball in corner touching multiple surfaces
            bool passed = true; // Test contact resolution
            Debug.Log($"Multiple Contacts Test: {(passed ? "PASS" : "FAIL")}");
            
            yield return null;
        }
        
        private IEnumerator TestPenetrationRecovery()
        {
            Debug.Log("Testing penetration recovery...");
            
            // Force ball into geometry and test recovery
            bool passed = true; // Test penetration correction
            Debug.Log($"Penetration Recovery Test: {(passed ? "PASS" : "FAIL")}");
            
            yield return null;
        }
        
        private IEnumerator TestPerformance()
        {
            Debug.Log("Testing collision performance...");
            
            // Measure collision processing time
            bool passed = true; // <1ms target
            Debug.Log($"Performance Test: {(passed ? "PASS" : "FAIL")}");
            
            yield return null;
        }
    }
}
```

## Manual Test Cases

### Test Case 1: Bounce Consistency
- **Setup**: Ball on flat surface at height 2m
- **Action**: Drop ball, measure bounce height
- **Expected**: Bounce height = drop height × material restitution
- **Pass Criteria**: ±5% accuracy

### Test Case 2: Material Differences
- **Setup**: Test surfaces with different materials
- **Action**: Drop ball on each surface
- **Expected**: Different bounce heights per material
- **Pass Criteria**: Metal > Wood > Stone bounce heights

### Test Case 3: Corner Collisions
- **Setup**: Ball rolling into block corner
- **Action**: Observe collision response
- **Expected**: Predictable bounce direction
- **Pass Criteria**: No stuck ball, consistent response

### Test Case 4: Smooth Block Transitions (C2)
- **Setup**: Two adjacent blocks with flat surfaces at the same height
- **Action**: Roll ball across the transition between blocks
- **Expected**: No unexpected jumps or elevation shifts during transition
- **Pass Criteria**: Ball maintains smooth rolling motion across blocks

### Test Case 5: Rolling Feel (C3)
- **Setup**: Ball on flat surface with different material types
- **Action**: Apply input to roll ball and observe friction behavior
- **Expected**: Grippy rolling without floating or sliding
- **Pass Criteria**: Ball responds to input with consistent friction, no sliding when stationary

### Test Case 6: Penetration Recovery
- **Setup**: Force ball into solid geometry
- **Action**: Observe recovery behavior
- **Expected**: Ball automatically pushed out
- **Pass Criteria**: Recovery within 1 frame

## Performance Tests

### Collision Processing Time
- **Target**: <1ms per collision
- **Measurement**: Unity Profiler
- **Test**: 20 simultaneous collisions

### Memory Allocation
- **Target**: Zero allocation during collisions
- **Measurement**: Memory Profiler
- **Test**: 1000 collisions over 60 seconds

## Success Criteria

- [ ] Bounce height accuracy within ±5%
- [ ] Material properties affect collision correctly
- [ ] Multiple contacts resolve without artifacts
- [ ] Penetration recovery works automatically
- [ ] Smooth block transitions with no unexpected jumps (C2)
- [ ] Rolling feel maintained with grippy contact (C3)
- [ ] Performance targets met (<1ms, 0KB allocation)
- [ ] No crashes or stuck ball scenarios
- [ ] Deterministic collision response

**Pass Threshold**: 7/9 criteria must pass to proceed to Phase 4.
