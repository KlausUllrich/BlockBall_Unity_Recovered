---
title: "Phase 2 Ball Physics - Technical Specifications"
phase: "Phase 2 - Ball Physics"
dependencies:
  - "LLM_01A_Phase2_Mission_and_Objectives.md"
  - "Phase1_Migration_Strategy/LLM_03A_Phase1_Overview.md"
validation_steps:
  - "Verify that all ball parameters (radius, mass, jump height) match design specifications."
  - "Confirm state machine definitions cover all required ball behaviors."
  - "Ensure input mechanics timings (jump buffer, coyote time) align with gameplay feel."
integration_points:
  - "Builds on Phase 1 core architecture (BlockBallPhysicsManager, IPhysicsObject)."
  - "Provides foundation for implementation tasks in LLM_02X series."
---

# Phase 2: Ball Physics - Technical Specifications

## Ball Parameters
- **Radius**: 0.5 Unity units (4 Bixels)
- **Mass**: 1.0 kg (standard)
- **Jump Height**: 0.75 Unity units (6 Bixels) exactly
- **Max Input Speed**: 6 units/second
- **Max Physics Speed**: 7 units/second
- **Max Total Speed**: 8 units/second

## State Machine
- **Grounded**: Rolling on surface, friction applied for realistic movement.
- **Airborne**: In flight, air drag applied to simulate realistic air resistance.
- **Sliding**: On steep slopes (>45°), reduced friction for sliding behavior.
- **Transitioning**: During gravity changes, special handling to ensure smooth transitions.

## Input Mechanics
- **Jump Buffer**: 0.1 second window before/after valid jump to improve responsiveness.
- **Coyote Time**: 0.15 second grace period after leaving ground to allow late jumps.
- **Camera Relative**: Movement direction based on camera orientation for intuitive control.
- **Diagonal Normalization**: Ensures consistent speed in all directions, preventing faster diagonal movement.

## Context & Dependencies
**Requires Phase 1 Completion**: This phase depends on the core architecture established in Phase 1:
- `BlockBallPhysicsManager` for object registration and management.
- `IPhysicsObject` interface for standardized physics integration.
- `VelocityVerletIntegrator` for accurate physics calculations.

## Validation Instructions
1. **Parameter Check**: Confirm that the ball's radius (0.5 units), mass (1.0 kg), and target jump height (0.75 units) are correctly documented and understood for implementation.
2. **State Behavior**: Ensure each state (Grounded, Airborne, Sliding, Transitioning) has a clear behavioral definition and transition logic.
3. **Input Timing**: Validate that jump buffer (0.1s) and coyote time (0.15s) timings are set for optimal player experience.

## Next Steps
After understanding the technical specifications, proceed to `LLM_01C_Phase2_Deliverables_and_Success_Criteria.md` for details on core components and success criteria for Phase 2.
