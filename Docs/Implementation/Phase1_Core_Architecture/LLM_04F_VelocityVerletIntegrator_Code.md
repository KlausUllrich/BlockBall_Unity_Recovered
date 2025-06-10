---
llm_metadata:
  title: "Phase 1 - VelocityVerletIntegrator Code Reference"
  phase: "Phase1_Core_Architecture"
  category: "Code Reference"
  id: "04F"
  version: "1.0"
  created: "2023-10-15"
  last_modified: "2023-10-15"
  author: "Cascade AI Assistant"
  purpose: "Provide complete code reference for VelocityVerletIntegrator in BlockBall Evolution."
  usage_context: "Use this document as a code reference for implementing VelocityVerletIntegrator with LLM processing."
  validation_status: "Not Validated"
---

# VelocityVerletIntegrator Code Reference

## Complete Code for VelocityVerletIntegrator

```csharp
namespace BlockBallPhysics
{
    public class VelocityVerletIntegrator
    {
        private PhysicsSettings _settings;
        private float _accumulator = 0f;

        public VelocityVerletIntegrator(PhysicsSettings settings)
        {
            _settings = settings;
        }

        public void Integrate(IPhysicsObject obj, float deltaTime)
        {
            _accumulator += deltaTime;
            int substeps = 0;

            while (_accumulator >= _settings.FixedTimestep && substeps < _settings.MaxSubsteps)
            {
                PerformIntegrationStep(obj, _settings.FixedTimestep);
                _accumulator -= _settings.FixedTimestep;
                substeps++;
            }
        }

        private void PerformIntegrationStep(IPhysicsObject obj, float dt)
        {
            // Get current state
            Vector3 position = obj.GetPosition();
            Vector3 velocity = obj.GetVelocity();
            Vector3 acceleration = obj.GetAcceleration();

            // Velocity Verlet: Update position using current velocity and acceleration
            position += velocity * dt + 0.5f * acceleration * dt * dt;

            // Calculate new acceleration (might depend on new position)
            Vector3 newAcceleration = obj.CalculateAccelerationAtPosition(position);

            // Update velocity using average of current and new acceleration
            velocity += 0.5f * (acceleration + newAcceleration) * dt;

            // Set updated state back to the object
            obj.SetPosition(position);
            obj.SetVelocity(velocity);
        }
    }
}
```

## Validation Steps for LLM
1. Confirm that `Integrate` method uses an accumulator to handle variable frame rates.
2. Verify that substeps are capped at `MaxSubsteps` from `PhysicsSettings` to prevent performance issues.
3. Ensure `PerformIntegrationStep` follows Velocity Verlet order: position update, then velocity update with averaged acceleration.
4. Test energy conservation with a falling ball over 100 steps; total energy drift must be within ±0.1%.

## Error Handling
- If energy drift exceeds ±0.1%, check if acceleration averaging is implemented correctly.
- If physics updates stutter, ensure `_accumulator` is not reset improperly between frames.
