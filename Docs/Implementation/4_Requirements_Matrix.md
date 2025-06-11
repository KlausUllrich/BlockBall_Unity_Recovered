# BlockBall Evolution - Physics Requirements Matrix (Revised)

## Document Metadata
```yaml
target_audience: "LLM_validation"
dependencies: ["1_BlockBall_Physics_Spec.md"]
```

## Critical Requirements Matrix (Validation Focus)

### 🔴 CRITICAL (Must Pass - Game Blockers)

| ID | Requirement | Validation Criteria | Priority |
|----|-------------|---------------------|----------|
| **C1** | **Jump Height Consistency** | `±0.01 Unity units variance (0.75 units = 6 Bixels)` | HIGH |
| **C2** | **Smooth Block Transitions** | `Zero transition artifacts between blocks` | HIGH |
| **C3** | **Rolling Feel** | `No floating/sliding behavior` | HIGH |
| **C4** | **Gravity While Airborne** | `Airborne gravity switching functional` | HIGH |
| **C5** | **Instant Gravity Snap (Clarified)** | `Snap to axis on gravity zone exit (no 0.3s delay)` | HIGH |

## Validation Checklist Template

### Core Systems Required

1. Physics Manager: Fixed timestep (50Hz) with Velocity Verlet integration
2. Ball State Machine: Grounded, Airborne, Sliding, Transitioning states
3. Collision System: Hybrid approach using Unity detection with custom response
4. Gravity Manager: Zone-based gravity with smooth transitions and snapping
5. Speed Controller: Multi-tier limits with exponential decay
6. Input Processor: Camera-relative input projected onto gravity plane
7. Debug Visualizer: Real-time physics state visualization

### Performance Targets

- Physics Frame Time: <2ms per frame (50Hz)
- Memory Allocation: <1KB per second (object pooling)
- Jump Consistency: ±0.005 Unity units variance
- Energy Conservation: ±0.1% over 60 seconds

### Phase Validation Format
- [ ] **C1-C5**: All critical requirements addressed
- [ ] **Document Size**: ≤200 lines per file
- [ ] **Single Source Truth**: No duplicate physics values

## Validation Process
1. **Map to Specifications**: Cross-reference phase documents
2. **Identify Gaps**: Missing or conflicting requirements
3. **Prioritize Fixes**: Focus on critical blockers (C1-C5)

## Implementation Notes

- Needs fixed timestep simulation (50Hz) for all ball movement.
- Velocity Verlet integrator (RECOMMENDED):
  ```
  position(t+dt) = position(t) + velocity(t)*dt + 0.5*acceleration(t)*dt²
  acceleration(t+dt) = calculateForces(position(t+dt)) / mass
  velocity(t+dt) = velocity(t) + 0.5*(acceleration(t) + acceleration(t+dt))*dt
  ```
  - More accurate than Semi-Implicit Euler, especially for gravity-shifting physics
  - Better energy conservation over time (±0.1% vs ±2-5%)
  - Smoother gravity transitions and bounce behavior
  - Worth the ~50% performance cost for superior game feel

- Clear separation between control input, physics response, and visual feedback.
- Unity's Job System or Burst Compiler can be used for performant custom physics.
- Debug HUD to visualize gravity direction, trigger zones, and contact normals.


## Success Criteria
- ✅ All Critical (C1-C5) requirements specified in implementation
- ✅ All documents ≤200 lines
- ✅ Single source of truth maintained
- ✅ Implementation Notes are clear and complete
- ✅ All requirements are validated
- ✅ All documents are highly optimized for LLM usage

---
*This revised matrix focuses strictly on validation of critical game requirements.*
