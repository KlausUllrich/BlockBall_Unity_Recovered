---
llm_metadata:
  title: "Phase 1 - VelocityVerletIntegrator Implementation Task"
  phase: "Phase1_Core_Architecture"
  category: "Task"
  id: "04F"
  version: "1.0"
  created: "2023-10-15"
  last_modified: "2023-10-15"
  author: "Cascade AI Assistant"
  purpose: "Guide the implementation of VelocityVerletIntegrator for custom physics in BlockBall Evolution."
  usage_context: "Use this document to implement the VelocityVerletIntegrator class with explicit instructions for LLM processing."
  validation_status: "Not Validated"
---

# VelocityVerletIntegrator Implementation Task

## Objective
Implement the `VelocityVerletIntegrator` class to perform physics integration using the Velocity Verlet method, ensuring superior energy conservation for the ball's movement in BlockBall Evolution.

## Background
The Velocity Verlet method is chosen for its symplectic properties, conserving energy better than simpler methods like Semi-Implicit Euler. It calculates position and velocity in a staggered manner, reducing energy drift to approximately ±0.1%.

## Step-by-Step Instructions for LLM

1. **Create the Class Structure**:
   - Define a new C# class named `VelocityVerletIntegrator` in the `BlockBallPhysics` namespace.
   - Ensure it has fields for `PhysicsSettings` to access timestep and substep configurations.

2. **Implement Core Integration Method**:
   - Create a method `Integrate` that takes an `IPhysicsObject` and a time delta as parameters.
   - Use the Velocity Verlet algorithm to update position and velocity based on current acceleration.

3. **Handle Substeps**:
   - Implement logic to divide the integration into up to 8 substeps if the delta time exceeds the fixed timestep (0.02s for 50Hz).
   - Use an accumulator pattern to manage leftover time for the next frame.

4. **Energy Conservation**:
   - Ensure the method minimizes energy drift by correctly sequencing position and velocity updates (position first with old velocity, then new velocity with new acceleration).

5. **Validation Check**:
   - After implementation, validate that the integrator maintains energy conservation within ±0.1% over 100 simulation steps under constant gravity.
   - Log any deviations beyond this threshold as errors for debugging.

## Partial Code Template

```csharp
namespace BlockBallPhysics
{
    public class VelocityVerletIntegrator
    {
        private PhysicsSettings _settings;

        public VelocityVerletIntegrator(PhysicsSettings settings)
        {
            _settings = settings;
        }

        public void Integrate(IPhysicsObject obj, float deltaTime)
        {
            // Implementation for Velocity Verlet integration with substeps
            // Use _settings.FixedTimestep for substep calculation
        }
    }
}
```

## Error Handling Instructions for LLM
- If energy conservation exceeds ±0.1%, double-check the order of position and velocity updates.
- If substep logic causes jitter, verify the accumulator pattern and ensure leftover time is carried over correctly.
- Log all integration steps with timestamps if debugging is needed.

## Assumptions
- `PhysicsSettings` provides a fixed timestep of 0.02s (50Hz).
- `IPhysicsObject` interface provides methods for getting/setting position, velocity, and acceleration.

## Next Steps After Completion
- Test the integrator with a simple falling ball scenario to confirm energy conservation.
- Integrate with `BlockBallPhysicsManager` for full system testing.
- Update status in `/Status/Project_Overview.md` under Physics Migration section.
