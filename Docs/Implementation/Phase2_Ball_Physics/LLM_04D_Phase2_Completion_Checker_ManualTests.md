---
title: "Phase 2 Ball Physics - Completion Checker for Manual Tests and Edge Cases"
phase: "Phase 2 - Ball Physics"
dependencies:
  - "LLM_04A_Phase2_Completion_Checker_Overview.md"
  - "LLM_01B_Phase2_Technical_Specifications.md"
  - "LLM_02A_BallStateMachine_Task.md"
  - "LLM_02B_BallPhysics_Component_Task.md"
  - "LLM_02C_BallInputProcessor_Task.md"
  - "LLM_02D_BallController_Task.md"
  - "LLM_02E_GroundDetector_Task.md"
validation_steps:
  - "Verify gameplay feel and responsiveness through hands-on testing."
  - "Confirm edge-case behaviors not covered by automated tests."
  - "Ensure no visual or functional bugs in typical and extreme scenarios."
integration_points:
  - "Validates user experience for all Phase 2 components in real gameplay scenarios."
  - "Complements automated testing from LLM_03X and LLM_04C."
---

# Phase 2: Ball Physics - Completion Checker for Manual Tests and Edge Cases

## Objective
Provide a detailed guide for manual testing of Phase 2 Ball Physics components to validate gameplay feel, responsiveness, and edge-case behavior not fully covered by automated tests, ensuring a polished user experience and identifying any visual or functional bugs before Phase 2 completion.

## Overview
- **Purpose**: Ensure that Phase 2 implementations (`BallPhysics`, `BallStateMachine`, `BallInputProcessor`, `BallController`, `GroundDetector`) deliver the intended gameplay experience and handle extreme or nuanced scenarios correctly.
- **Scope**: Covers hands-on testing in the Unity Editor with real gameplay scenarios, focusing on player feedback, edge cases, and visual/functional integrity.
- **Success Criteria**: All manual test scenarios must pass with acceptable gameplay feel (subjective but guided by specific expectations) and no critical bugs. At least 90% of test cases should pass fully.

## Manual Testing Checklist
Below is a categorized checklist for manual testing. Perform these tests in a Unity scene resembling actual gameplay conditions (e.g., a test level with flat ground, slopes, platforms, and a camera setup). Record Pass/Fail for each test case, along with notes on feel or observed issues.

### 1. Gameplay Feel and Responsiveness
These tests evaluate the subjective quality of ball control and physics interaction from a player perspective.
- [ ] **Ball Movement Smoothness**: Move the ball forward/backward/left/right on flat ground. Confirm movement feels smooth, responsive, and intuitive with no jitter or lag.
  - **Expectation**: Immediate response to input, consistent speed within limits (6 u/s input cap).
  - **Notes**: Note any delay or unnatural acceleration.
- [ ] **Camera-Relative Control**: Rotate the camera to different angles (0°, 45°, 90°, 180°) and apply input. Confirm ball moves relative to camera orientation.
  - **Expectation**: Forward input always moves ball in camera’s forward direction.
  - **Notes**: Note any misalignment between input and movement.
- [ ] **Jump Feel**: Perform jumps on flat ground. Confirm jump height (0.75 units) feels appropriate and timing is responsive.
  - **Expectation**: Jump feels snappy, consistent height visible in-game (e.g., via debug UI).
  - **Notes**: Note if jump feels sluggish or inconsistent.
- [ ] **Rolling Visuals**: Move the ball on flat ground and observe rotation. Confirm ball visually rolls realistically (rotation matches movement direction).
  - **Expectation**: Visual rotation matches linear velocity direction and speed.
  - **Notes**: Note any sliding without rotation or mismatched visuals.

### 2. Jump Mechanics Edge Cases
These tests focus on jump buffering and coyote time under tricky conditions.
- [ ] **Jump Buffering Pre-Ground**: Fall from a height, press jump just before landing (within 0.1s), and confirm ball jumps immediately upon grounding.
  - **Expectation**: Jump triggers on ground contact if input was within 0.1s.
  - **Notes**: Note if jump input is ignored.
- [ ] **Coyote Time Post-Edge**: Walk off a platform edge, press jump just after (within 0.15s), and confirm ball still jumps.
  - **Expectation**: Jump triggers if input within 0.15s of leaving ground.
  - **Notes**: Note if jump fails despite timing.
- [ ] **Jump Near Ceiling**: Jump under a low ceiling (height <0.75 units). Confirm ball doesn’t clip through or get stuck, and velocity is handled gracefully.
  - **Expectation**: Ball stops upward motion on collision, no clipping.
  - **Notes**: Note any clipping or unnatural behavior.

### 3. State Transition Edge Cases
These tests verify state machine behavior in nuanced or rapid-change scenarios.
- [ ] **Rapid Ground/Air Toggle**: Move on/off a platform edge repeatedly. Confirm state doesn’t flicker (Grounded to Airborne) due to hysteresis (0.55-0.6 unit thresholds).
  - **Expectation**: State remains stable despite rapid small changes near edge.
  - **Notes**: Note any flickering or unexpected state changes.
- [ ] **Slope State Transition**: Move onto a slope just above 45° (e.g., 46°). Confirm transition to Sliding state. Then move to flat ground and confirm return to Grounded.
  - **Expectation**: Sliding triggers above 45°, Grounded returns on flat.
  - **Notes**: Note incorrect state assignments or delays.
- [ ] **Transitioning State Lock**: If in Transitioning state (e.g., during gravity shift), apply inputs or ground changes. Confirm other state changes are blocked until transition completes.
  - **Expectation**: No state change during Transitioning except to predefined exits.
  - **Notes**: Note if state changes prematurely.

### 4. Speed and Physics Edge Cases
These tests check speed limits and physics behavior under extreme conditions.
- [ ] **Speed Limit at Max Input**: Apply maximum input (e.g., forward+right) continuously. Confirm speed caps at 6 u/s (input limit) despite diagonal input.
  - **Expectation**: Speed never exceeds 6 u/s from input alone.
  - **Notes**: Note if diagonal input causes speed exploits.
- [ ] **Speed Decay Over Limit**: Use a debug tool or external force to set speed above 6.65 u/s (e.g., 6.8 u/s). Confirm exponential decay reduces speed toward 6.65 u/s over time.
  - **Expectation**: Speed decays noticeably within a few seconds.
  - **Notes**: Note if speed remains above limit too long.
- [ ] **Friction on Slopes**: Roll down a shallow slope (<45°) without input. Confirm friction slows ball naturally per `PhysicsSettings` coefficient.
  - **Expectation**: Ball slows and stops, doesn’t slide indefinitely.
  - **Notes**: Note if friction feels too weak or strong.

### 5. Visual and Functional Bugs
These tests look for common visual or gameplay-breaking issues.
- [ ] **No Clipping on Ground**: Move ball against walls or corners at ground level. Confirm no clipping through geometry.
  - **Expectation**: Ball stops or slides along surface, no penetration.
  - **Notes**: Note any clipping or getting stuck.
- [ ] **State Visual Feedback**: If debug UI or visual effects exist for states (e.g., dust for Sliding), confirm they match current state.
  - **Expectation**: Visuals update instantly with state (e.g., Sliding effect on steep slope).
  - **Notes**: Note mismatched or delayed visuals.
- [ ] **Input Disable Scenarios**: If game logic disables input (e.g., during cutscene via `BallController`), confirm ball doesn’t respond to player input.
  - **Expectation**: No movement or jump during disabled input.
  - **Notes**: Note if input still affects ball.

## Validation Instructions
1. **Setup Test Scene**: Create or use a test level in Unity Editor with varied terrain (flat ground, slopes >45°, platforms, walls) and a controllable camera. Attach all Phase 2 components to a ball GameObject.
2. **Perform Tests**: Follow each test case step-by-step. Use keyboard/mouse or controller input to simulate player actions. Observe behavior visually or via debug UI/logs if available.
3. **Record Results**: Mark each test as Pass/Fail based on whether it meets the expectation. For subjective “feel” tests, use your judgment aligned with design goals (e.g., responsive control).
4. **Note Issues**: For any failing test or suboptimal feel, document specific observations (e.g., “jump feels delayed by ~0.2s”) in the notes section and log detailed issues in `/Status/Issues_and_Required_Cleanup.md`.
5. **Scoring**: Calculate pass rate as (Passed Tests / Total Tests) * 100%. Aim for ≥90%. Critical failures (e.g., clipping, unresponsive controls) may weigh heavier and require immediate fixes.

## Scoring
- **Total Test Cases**: 15 (as listed above, expandable if needed).
- **Pass Rate**: (Number of Passed Tests / Total Test Cases) * 100%. Must be ≥90% for this section.
- **Critical Failures**: Failures in core gameplay feel (e.g., movement smoothness, jump feel) or severe bugs (e.g., clipping) count as critical and may fail this section regardless of overall rate.
- **Subjective Adjustment**: For “feel” tests, a borderline fail can be marked as partial pass if issue is minor and noted for polish (doesn’t count fully against score).

## Result Summary Template
Use this template to summarize manual test results after completion. Fill in based on observations.

```
Phase 2 Manual Test Results Summary:
1. Gameplay Feel and Responsiveness:
   - Ball Movement Smoothness: [Pass/Fail] - [Notes]
   - Camera-Relative Control: [Pass/Fail] - [Notes]
   - Jump Feel: [Pass/Fail] - [Notes]
   - Rolling Visuals: [Pass/Fail] - [Notes]
2. Jump Mechanics Edge Cases:
   - Jump Buffering Pre-Ground: [Pass/Fail] - [Notes]
   - Coyote Time Post-Edge: [Pass/Fail] - [Notes]
   - Jump Near Ceiling: [Pass/Fail] - [Notes]
3. State Transition Edge Cases:
   - Rapid Ground/Air Toggle: [Pass/Fail] - [Notes]
   - Slope State Transition: [Pass/Fail] - [Notes]
   - Transitioning State Lock: [Pass/Fail] - [Notes]
4. Speed and Physics Edge Cases:
   - Speed Limit at Max Input: [Pass/Fail] - [Notes]
   - Speed Decay Over Limit: [Pass/Fail] - [Notes]
   - Friction on Slopes: [Pass/Fail] - [Notes]
5. Visual and Functional Bugs:
   - No Clipping on Ground: [Pass/Fail] - [Notes]
   - State Visual Feedback: [Pass/Fail] - [Notes]
   - Input Disable Scenarios: [Pass/Fail] - [Notes]

Overall Pass Rate: XX% (Threshold: 90%)
Critical Failures: [None or list specific tests]
Conclusion: [Pass/Fail] - [Ready for next steps or requires fixes in specific areas]
```

## Next Steps
After completing manual testing and edge-case validation, proceed to `LLM_04E_Phase2_Completion_Checker_Integration.md` for integration validation with Phase 1 systems. Log progress in `/Status/Project_Overview.md` and any issues in `/Status/Issues_and_Required_Cleanup.md`.
