---
title: "Phase 2 Ball Physics - BallPhysics Component Implementation Task (Part 2 of 2)"
phase: "Phase 2 - Ball Physics"
part: "2 of 2"
dependencies:
  - "LLM_02B1_BallPhysics_Component_Overview.md"
validation_steps:
  - "Verify speed limiting logic enforces input (6 u/s), physics (7 u/s), and total (8 u/s) caps correctly."
  - "Confirm jump velocity calculation achieves exactly 0.75 units height using v = sqrt(2 * g * h)."
  - "Check that physics behavior (friction, drag) adjusts based on BallStateMachine state."
integration_points:
  - "Provides gravity direction methods for GravityZoneDetector integration."
  - "Uses VelocityVerletIntegrator for position updates from Phase 1."
---

# Phase 2: Ball Physics - BallPhysics Component Implementation (Part 2 of 2)

## Physics Update Implementation
Complete implementation of the physics update methods:

```csharp
        public void PhysicsUpdate(float deltaTime)
        {
            // State-based physics adjustments
            switch (stateMachine.CurrentState)
            {
                case BallState.Grounded:
                    // Apply friction
                    ApplyFriction(deltaTime);
                    // Rolling constraint
                    ApplyRollingConstraint();
                    break;
                case BallState.Airborne:
                    // Apply air drag
                    ApplyAirDrag(deltaTime);
                    break;
                case BallState.Sliding:
                    // FIXED: Use same rolling friction (maintain rolling feel per spec)
                    ApplyFriction(deltaTime); // No special sliding friction
                    break;
            }

            // Apply gravity
            ApplyGravity();

            // Limit physics speed (excluding input)
            Vector3 physicsVelocity = velocity; // Adjust based on input separation if needed
            if (physicsVelocity.magnitude > maxPhysicsSpeed)
            {
                physicsVelocity = physicsVelocity.normalized * maxPhysicsSpeed;
                velocity = physicsVelocity; // Update after limiting
            }

            // Limit total speed
            if (velocity.magnitude > maxTotalSpeed)
            {
                velocity = velocity.normalized * maxTotalSpeed;
            }

            // Update position using Velocity Verlet Integrator
            position = VelocityVerletIntegrator.UpdatePosition(position, velocity, PhysicsSettings.Instance.GetCurrentGravity(), deltaTime);
            transform.position = position;

            // Handle jump if requested and grounded
            if (isJumpRequested && stateMachine.CurrentState == BallState.Grounded)
            {
                float jumpVelocity = Mathf.Sqrt(2 * PhysicsSettings.Instance.GetCurrentGravity().magnitude * jumpHeight);
                this.velocity.y += jumpVelocity; // Apply upward impulse
                stateMachine.TryTransitionTo(BallState.Airborne, "Jump initiated");
                isJumpRequested = false;
            }

            stateMachine.Update(deltaTime);
        }

        private void ApplyFriction(float deltaTime)
        {
            // Implement friction based on PhysicsSettings
            float frictionCoefficient = PhysicsSettings.Instance.groundFriction;
            velocity *= (1 - frictionCoefficient * deltaTime);
        }

        private void ApplyAirDrag(float deltaTime)
        {
            // Implement air drag
            float dragCoefficient = PhysicsSettings.Instance.airDrag;
            velocity *= (1 - dragCoefficient * deltaTime);
        }

        private void ApplyRollingConstraint()
        {
            // Angular velocity constraint: ω = v / r
            // For a rolling ball: v = ω * r
            // Ensure ball appears to roll realistically
            float angularVelocity = velocity.magnitude / radius;
            // Apply constraint if needed
        }

        private void ApplyGravity()
        {
            // Get current gravity from PhysicsSettings (instant, no transitions)
            Vector3 currentGravity = PhysicsSettings.Instance.GetCurrentGravity();
            
            // Apply gravity directly to velocity
            velocity += currentGravity * Time.fixedDeltaTime;
            
            // Apply terminal velocity limiting if falling
            if (Vector3.Dot(velocity, currentGravity.normalized) > 0)
            {
                float terminalSpeed = PhysicsSettings.Instance.totalSpeedLimit;
                float currentSpeed = velocity.magnitude;
                if (currentSpeed > terminalSpeed)
                {
                    velocity = velocity.normalized * terminalSpeed;
                }
            }
        }

        /// <summary>
        /// Sets gravity direction instantly (called by GravityZoneDetector)
        /// </summary>
        public void SetGravityDirection(Vector3 direction)
        {
            PhysicsSettings.Instance.SetGravityDirection(direction);
        }
        
        /// <summary>
        /// Snaps gravity to cardinal axis (called when exiting gravity zones)
        /// </summary>
        public void SnapGravityToCardinalAxis()
        {
            PhysicsSettings.Instance.SnapGravityToCardinalAxis();
        }
        
        /// <summary>
        /// Gets current gravity for external components
        /// </summary>
        public Vector3 GetCurrentGravity()
        {
            return PhysicsSettings.Instance.GetCurrentGravity();
        }
    }
}
```

## Context & Dependencies
**Requires Phase 1 Completion**: This component builds on the architecture established in Phase 1:
- `IPhysicsObject` interface for integration with `BlockBallPhysicsManager`.
- `VelocityVerletIntegrator` for physics updates.
- `PhysicsSettings` ScriptableObject for physics parameters (gravity, friction, drag).
- Relies on `BallStateMachine` for state information.

## Validation Instructions
1. **Interface Compliance**: Ensure `BallPhysics` implements `IPhysicsObject` with correct property mappings (`Velocity`, `Position`, `Mass`).
2. **Speed Limits**: Verify that speed limiting logic enforces input (6 u/s), physics (7 u/s), and total (8 u/s) caps correctly.
3. **Jump Precision**: Confirm jump velocity calculation achieves exactly 0.75 units height using `v = sqrt(2 * g * h)`.
4. **State Physics**: Check that physics behavior (friction, drag) adjusts based on `BallStateMachine` state.
5. **Rolling Constraint**: Validate angular velocity `ω = v / r` for realistic rolling in Grounded state.
6. **Performance**: Ensure integration with profiling tools to keep updates under 2ms.

## Next Steps
After implementing `BallPhysics.cs`, proceed to `LLM_02C_BallInputProcessor_Task.md` for the input processing component. Log progress in `/Status/Project_Overview.md` and any issues in `/Status/Issues_and_Required_Cleanup.md`.
