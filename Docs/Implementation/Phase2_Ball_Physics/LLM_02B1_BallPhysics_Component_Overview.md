---
title: "Phase 2 Ball Physics - BallPhysics Component Implementation Task (Part 1 of 2)"
phase: "Phase 2 - Ball Physics"
part: "1 of 2"
dependencies:
  - "LLM_01A_Phase2_Mission_and_Objectives.md"
  - "LLM_01B_Phase2_Technical_Specifications.md"
  - "LLM_01E_Phase2_Integration_Strategy.md"
  - "LLM_02A_BallStateMachine_Task.md"
  - "Phase1_Migration_Strategy/LLM_03A_Phase1_Overview.md"
validation_steps:
  - "Verify that BallPhysics implements IPhysicsObject for Phase 1 integration."
  - "Confirm physics calculations respect speed limits (input 6 u/s, physics 7 u/s, total 8 u/s)."
  - "Ensure jump mechanics achieve exactly 0.75 units height with correct velocity."
integration_points:
  - "Integrates with BlockBallPhysicsManager and VelocityVerletIntegrator from Phase 1."
  - "Uses BallStateMachine for state-based behavior adjustments."
---

# Phase 2: Ball Physics - BallPhysics Component Implementation Task

## Objective
Implement `BallPhysics.cs` as the core physics component for the ball, integrating with Phase 1 systems (`IPhysicsObject`, `BlockBallPhysicsManager`), managing state-based physics behavior via `BallStateMachine`, applying speed limits, handling jump mechanics, and ensuring realistic rolling.

## Component Overview
- **File**: `BallPhysics.cs`
- **Purpose**: Serves as the main physics handler for the ball, implementing `IPhysicsObject` to integrate with Phase 1 architecture, calculating movement, enforcing speed constraints, and managing jump impulses.
- **Key Features**:
  - Registers with `BlockBallPhysicsManager` for physics updates.
  - Uses `VelocityVerletIntegrator` for accurate position and velocity updates.
  - Enforces three-tier speed limits: input (6 u/s), physics (7 u/s), total (8 u/s).
  - Calculates precise jump velocity for 0.75 unit (6 Bixel) height.
  - Adjusts friction and drag based on state from `BallStateMachine`.

## Implementation Steps
1. **Interface Compliance**: Implement `IPhysicsObject` to ensure integration with `BlockBallPhysicsManager` for physics updates.
2. **State Integration**: Use `BallStateMachine` to adjust physics parameters (friction, drag) based on current state (Grounded, Airborne, etc.).
3. **Physics Update**: Leverage `VelocityVerletIntegrator` for accurate physics simulation, applying forces and updating position/velocity.
4. **Speed Limiting**: Apply three-tier speed constraints:
   - Limit input-driven velocity to 6 units/second.
   - Limit physics-driven velocity (e.g., gravity, collisions) to 7 units/second.
   - Cap total velocity magnitude to 8 units/second.
5. **Jump Mechanics**: Calculate initial jump velocity using `v = sqrt(2 * g * h)` for a height of 0.75 units, triggered by input when Grounded.
6. **Rolling Physics**: For Grounded state, apply angular velocity constraint `ω = v / r` (radius = 0.5 units) to ensure realistic rolling without sliding.
7. **Debug Profiling**: Integrate with Phase 1 performance profiling to ensure physics updates stay under 2ms.

## Code Template
Below is a partial code template for `BallPhysics.cs`. Implement this in the Unity project under the appropriate namespace (e.g., `BlockBall.Physics`).

```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    public class BallPhysics : MonoBehaviour, IPhysicsObject
    {
        [SerializeField] private float mass = 1.0f; // 1.0 kg
        // FIXED: All hardcoded values now reference PhysicsSettings
        private float radius => PhysicsSettings.Instance.ballRadius;
        private float jumpHeight => PhysicsSettings.Instance.jumpHeight;
        private float maxInputSpeed => PhysicsSettings.Instance.inputSpeedLimit;
        private float maxPhysicsSpeed => PhysicsSettings.Instance.physicsSpeedLimit;
        private float maxTotalSpeed => PhysicsSettings.Instance.totalSpeedLimit;

        private Vector3 velocity = Vector3.zero;
        private Vector3 position = Vector3.zero;
        private BallStateMachine stateMachine;
        private bool isJumpRequested = false;

        // From IPhysicsObject
        public Vector3 Velocity { get => velocity; set => velocity = value; }
        public Vector3 Position { get => position; set => position = value; }
        public float Mass => mass;
        public bool IsActive => isActiveAndEnabled;

        void Awake()
        {
            stateMachine = new BallStateMachine();
            position = transform.position;
            // Register with BlockBallPhysicsManager
            BlockBallPhysicsManager.Instance?.RegisterPhysicsObject(this);
        }

        void OnDestroy()
        {
            BlockBallPhysicsManager.Instance?.UnregisterPhysicsObject(this);
        }

        public void ApplyInput(Vector3 inputVelocity)
        {
            // Limit input speed using PhysicsSettings
            if (inputVelocity.magnitude > maxInputSpeed)
            {
                inputVelocity = inputVelocity.normalized * maxInputSpeed;
            }
            // Apply input based on state
            if (stateMachine.CurrentState == BallState.Grounded)
            {
                velocity += inputVelocity * Time.deltaTime;
            }
            // Further state-based logic...
        }

        public void RequestJump()
        {
            isJumpRequested = true;
        }

        // Detailed physics implementation in Part 2
        public void PhysicsUpdate(float deltaTime) { /* See LLM_02B2 */ }
        public void PrePhysicsStep(float deltaTime) { /* See LLM_02B2 */ }
        public void PhysicsStep(float deltaTime) { /* See LLM_02B2 */ }
        public void PostPhysicsStep(float deltaTime) { /* See LLM_02B2 */ }
    }
}
```

## Next Steps
Continue to `LLM_02B2_BallPhysics_Component_Implementation.md` for the complete physics update methods, speed limiting implementation, gravity integration, and performance profiling.
