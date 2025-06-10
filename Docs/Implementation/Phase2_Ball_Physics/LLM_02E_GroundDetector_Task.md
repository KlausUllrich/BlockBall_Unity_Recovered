---
title: "Phase 2 Ball Physics - GroundDetector Implementation Task"
phase: "Phase 2 - Ball Physics"
dependencies:
  - "LLM_01A_Phase2_Mission_and_Objectives.md"
  - "LLM_01B_Phase2_Technical_Specifications.md"
  - "LLM_01E_Phase2_Integration_Strategy.md"
  - "LLM_02B_BallPhysics_Component_Task.md"
  - "LLM_02D_BallController_Task.md"
validation_steps:
  - "Verify that GroundDetector accurately detects ground contact for state determination."
  - "Confirm slope angle detection for Sliding state (>45°)."
  - "Ensure hysteresis prevents state flickering during edge cases."
integration_points:
  - "Integrates with BallStateMachine for state transitions."
  - "Connects to BallPhysics.cs for physics interaction data."
  - "Respects GravityZoneDetector for gravity zone priority."
---

# Phase 2: Ball Physics - GroundDetector Implementation Task

## Objective
Implement `GroundDetector.cs` to detect ground contact and surface properties for the ball, determining state transitions (Grounded, Airborne, Sliding) by analyzing contact points, normals for slope angles (>45° for Sliding), and applying hysteresis to prevent state flickering. Ensure the detector respects gravity zone priority to prevent interference with airborne gravity transitions.

## Component Overview
- **File**: `GroundDetector.cs`
- **Purpose**: Analyzes collision or raycast data to determine if the ball is in contact with the ground, calculates surface slope to identify steep slopes for Sliding state, and provides input to `BallStateMachine` for state updates while respecting gravity zone priority.
- **Key Features**:
  - Detects ground contact using raycasts or sphere casts downward from the ball’s center.
  - Calculates slope angle from surface normal to determine if slope exceeds threshold for Sliding state.
  - Implements hysteresis (different thresholds for entering/leaving Grounded state) to prevent rapid state changes at edges.
  - Respects gravity zone priority to prevent interference with airborne gravity transitions.

## Implementation Steps
1. **Ground Detection Method**: Use raycasts or sphere casts downward from the ball’s center to detect ground contact within a small tolerance distance.
2. **Slope Calculation**: On contact, retrieve the surface normal and calculate the angle relative to the up vector (gravity-opposite) to determine if slope exceeds threshold for Sliding state.
3. **Hysteresis Logic**: Apply different distance thresholds for entering Grounded state versus leaving it to avoid flickering at platform edges.
4. **State Recommendation**: Based on detection and slope, recommend state transitions to `BallStateMachine` (Grounded if flat, Sliding if steep, Airborne if no contact).
5. **Gravity Zone Priority**: Ensure the detector respects gravity zone priority by checking `GravityZoneDetector` before making state changes.
6. **Debug Visualization**: Add debug drawing for raycasts and contact points to visualize detection in the Unity editor.

## Code Template
Below is a partial code template for `GroundDetector.cs`. Implement this in the Unity project under the appropriate namespace (e.g., `BlockBall.Physics`).

```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    public class GroundDetector : MonoBehaviour
    {
        [Header("Ground Detection")]
        // Reference PhysicsSettings instead of hardcoded values
        private float groundCheckDistance => PhysicsSettings.Instance.groundCheckDistance;
        private float groundLeaveDistance => PhysicsSettings.Instance.groundLeaveDistance;
        
        [SerializeField] private LayerMask groundLayer = 1 << 0; // Default layer
        [SerializeField] private bool debugVisualization = true;

        private BallStateMachine stateMachine;
        private GravityZoneDetector gravityDetector; // Check gravity zone priority
        private bool isGrounded = false;
        private float slopeAngle = 0f;
        private ContactPoint lastGroundContact;

        void Awake()
        {
            stateMachine = GetComponent<BallStateMachine>();
            gravityDetector = GetComponent<GravityZoneDetector>(); // Get gravity detector
            
            if (stateMachine == null)
                Debug.LogError("GroundDetector requires BallStateMachine component");
        }

        void FixedUpdate()
        {
            CheckGroundContact();
            UpdateBallState();
        }

        private void CheckGroundContact()
        {
            Vector3 position = transform.position;
            float ballRadius = PhysicsSettings.Instance.ballRadius; // Use PhysicsSettings
            
            // Cast downward from ball center
            RaycastHit hit;
            bool hasContact = Physics.Raycast(position, Vector3.down, out hit, 
                groundCheckDistance, groundLayer);

            if (hasContact)
            {
                // Calculate slope angle
                slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                lastGroundContact = new ContactPoint
                {
                    point = hit.point,
                    normal = hit.normal
                };

                // Hysteresis for grounded state
                if (!isGrounded && hit.distance <= groundCheckDistance)
                {
                    isGrounded = true;
                    Debug.Log("Ball grounded");
                }
            }
            else
            {
                // Use leave distance for hysteresis
                if (isGrounded)
                {
                    isGrounded = false;
                    Debug.Log("Ball left ground");
                }
            }
        }

        private void UpdateBallState()
        {
            // Respect gravity zone priority
            // Don't interfere if gravity detector is handling transitions
            if (gravityDetector != null && gravityDetector.IsInGravityZone)
            {
                // Gravity zones have highest priority - don't change states
                return;
            }
            
            // Don't interfere with gravity transitions
            if (stateMachine.CurrentState == BallState.Transitioning)
                return;

            float slopeThreshold = PhysicsSettings.Instance.maxSlopeAngle; // Use PhysicsSettings
            
            if (isGrounded)
            {
                if (slopeAngle > slopeThreshold)
                {
                    stateMachine.TryTransitionTo(BallState.Sliding, $"Steep slope: {slopeAngle:F1}°");
                }
                else
                {
                    stateMachine.TryTransitionTo(BallState.Grounded, "Ground contact");
                }
            }
            else
            {
                stateMachine.TryTransitionTo(BallState.Airborne, "No ground contact");
            }
        }
    }
}
```

## Context & Dependencies
**Requires Phase 2 Components**: This component builds on:
- `BallPhysics.cs` for ball properties (radius) and physics context.
- `BallStateMachine` for recommending and triggering state transitions.
- `GravityZoneDetector` for respecting gravity zone priority.
- Unity’s physics system for raycasting and collision detection.

## Validation Instructions
1. **Ground Detection**: Ensure `GroundDetector` accurately identifies ground contact using raycasts within the specified tolerance.
2. **Slope Threshold**: Verify that slopes steeper than the threshold trigger the Sliding state via `stateMachine.TryTransitionTo()`.
3. **Hysteresis Effect**: Confirm hysteresis logic prevents state flickering by using different thresholds for entering/leaving Grounded state.
4. **Gravity Zone Priority**: Check that the detector respects gravity zone priority by not interfering with airborne gravity transitions.
5. **State Updates**: Check that state recommendations (Grounded, Sliding, Airborne) are correctly passed to `BallStateMachine` based on detection results.
6. **Debug Visualization**: Validate that debug rays or indicators in the editor clearly show ground check results.

## Next Steps
After implementing `GroundDetector.cs`, you have completed the core implementation tasks for Phase 2 components. Proceed to `LLM_03A_Phase2_Automated_Tests_JumpHeight.md` to begin setting up automated test cases for validation. Log progress in `/Status/Project_Overview.md` and any issues in `/Status/Issues_and_Required_Cleanup.md`.
