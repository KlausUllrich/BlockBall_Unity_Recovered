---
title: "Phase 2 Ball Physics - GravityZoneDetector Component Implementation Task (Part 1 of 2)"
phase: "Phase 2 - Ball Physics"
part: "1 of 2"
dependencies:
  - "LLM_01A_Phase2_Mission_and_Objectives.md"
  - "LLM_01B_Phase2_Technical_Specifications.md" 
  - "LLM_02A_BallStateMachine_Task.md"
  - "Phase1_Core_Architecture/LLM_04C_PhysicsSettings_Task.md"
validation_steps:
  - "Verify gravity zones trigger Transitioning state from ANY ball state (including Airborne)."
  - "Confirm instant gravity changes while inside zones."
  - "Ensure gravity snaps to cardinal axes when leaving zones."
integration_points:
  - "Uses PhysicsSettings gravity direction updates from Phase 1."
  - "Triggers BallStateMachine state changes with highest priority."
  - "Integrates with BallPhysics for gravity direction updates."
---

# Phase 2: Ball Physics - GravityZoneDetector Component Implementation Task

## Objective
Implement the **GravityZoneDetector** component that enables **INSTANT** gravity switching based on the ball's relative position within gravity switch trigger zones. This component addresses the critical missing functionality that prevented airborne gravity transitions in the original Phase 2 design.

## CRITICAL CONTEXT
**This component fixes the FUNDAMENTAL FLAW identified in Phase 2 validation:**
- **Original Issue**: Ball could never transition gravity while airborne due to missing gravity zone system
- **Physics Spec Requirement**: "Gravity switches affect Ball in air and on ground" 
- **Solution**: Position-based instant gravity calculation within trigger zones
- **Key Behavior**: Gravity changes INSTANTLY based on ball position relative to zone geometry

## Core Functionality
- **Instant Gravity Switching**: Gravity direction calculated instantly from ball position within zone. There is no delay or smooth transition; the change is immediate as per the project specifications.
- **Airborne Priority**: Overrides GroundDetector to enable airborne gravity transitions  
- **Multi-Zone Handling**: Manages overlapping zones with closest pivot priority
- **Cardinal Snapping**: Snaps gravity to nearest axis when exiting all zones
- **State Integration**: Works with BallStateMachine for proper state management

## Technical Requirements
- **Position-Based Calculation**: Gravity direction = normalized vector from zone pivot to ball position
- **Instant Updates**: No transition timing - gravity changes immediately
- **Trigger Detection**: Uses OnTriggerEnter/Exit for zone boundary detection
- **Multi-Zone Priority**: Closest pivot point determines active zone
- **State Override**: Forces appropriate ball states during gravity switching

## Implementation Steps
1. **Zone Detection**: Use trigger colliders to detect gravity zone entry/exit
2. **Instant Gravity Calculation**: Calculate gravity direction based on ball position within zone
3. **State Override**: Force Transitioning state with highest priority (before GroundDetector)
4. **Multi-Zone Handling**: Handle overlapping zones by distance to pivot point
5. **Cardinal Snapping**: Snap gravity to nearest axis when leaving all zones
6. **Integration**: Work with BallPhysics to apply new gravity direction

## Core Component Structure
```csharp
using UnityEngine;
using System.Collections.Generic;

namespace BlockBall.Physics
{
    /// <summary>
    /// CRITICAL COMPONENT: Handles gravity zone detection and instant gravity switching.
    /// This component fixes the FUNDAMENTAL FLAW in original Phase 2 where gravity transitions while airborne were impossible.
    /// </summary>
    public class GravityZoneDetector : MonoBehaviour
    {
        [Header("Gravity Zone Detection")]
        [SerializeField] private LayerMask gravityZoneLayer = 1 << 8; // Layer for gravity zones
        [SerializeField] private bool debugVisualization = true;
        
        private BallStateMachine stateMachine;
        private BallPhysics ballPhysics;
        private List<GravityZone> activeZones = new List<GravityZone>();
        private GravityZone dominantZone = null;

        void Awake()
        {
            stateMachine = GetComponent<BallStateMachine>();
            ballPhysics = GetComponent<BallPhysics>();
            
            if (stateMachine == null)
                Debug.LogError("GravityZoneDetector requires BallStateMachine component");
            if (ballPhysics == null)
                Debug.LogError("GravityZoneDetector requires BallPhysics component");
        }

        void FixedUpdate()
        {
            UpdateGravity();
        }

        private void UpdateGravity()
        {
            // INSTANT GRAVITY SWITCHING: Calculate gravity direction based on ball position within zone
            if (activeZones.Count > 0)
            {
                // Find dominant zone (closest pivot to ball)
                UpdateDominantZone();
                
                // Calculate gravity direction
                Vector3 gravityDirection = CalculateGravityDirection(dominantZone);
                
                // Apply gravity direction
                ballPhysics.SetGravityDirection(gravityDirection);
                
                // Force transition state (overrides GroundDetector)
                if (stateMachine.CurrentState != BallState.Transitioning)
                {
                    stateMachine.TryTransitionTo(BallState.Transitioning, "Entered gravity zone");
                }
            }
            else if (stateMachine.CurrentState == BallState.Transitioning)
            {
                // Left all gravity zones - snap and return to normal states
                ballPhysics.SnapGravityToCardinalAxis();
                
                // Allow normal state transitions to resume
                // GroundDetector will take over on next frame
            }
        }

        // Detailed methods implementation in Part 2
        private Vector3 CalculateGravityDirection(GravityZone zone) { /* See LLM_02F2 */ }
        private void UpdateDominantZone() { /* See LLM_02F2 */ }
        void OnTriggerEnter(Collider other) { /* See LLM_02F2 */ }
        void OnTriggerExit(Collider other) { /* See LLM_02F2 */ }
    }
}
```

## Next Steps
Continue to `LLM_02F2_GravityZoneDetector_Implementation.md` for the complete method implementations, zone detection logic, multi-zone handling, and the GravityZone component definition.
