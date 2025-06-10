---
llm_metadata:
  title: "Phase 1 - Test Cases for Custom Physics System"
  phase: "Phase1_Core_Architecture"
  category: "Test Cases"
  id: "04H"
  version: "1.0"
  created: "2023-10-15"
  last_modified: "2023-10-15"
  author: "Cascade AI Assistant"
  purpose: "Define test cases for validating the custom physics system in BlockBall Evolution."
  usage_context: "Use this document to execute test cases ensuring the Phase 1 physics system meets requirements with LLM processing."
  validation_status: "Not Validated"
---

# Test Cases for Custom Physics System

## Objective
Validate the custom physics system implemented in Phase 1 of BlockBall Evolution, ensuring fixed timestep stability, energy conservation, and performance targets are met.

## Background
Testing is critical to confirm that the Velocity Verlet-based physics system resolves jitter and collision issues from Unity's default physics, maintains energy conservation, and performs efficiently at 50Hz.

## Test Case 1: Fixed Timestep Validation

- **Objective**: Confirm the physics system adheres to a fixed 50Hz timestep (0.02s) regardless of frame rate.
- **Setup**: Create a test scene with a single ball falling under gravity. Set frame rate to vary (30fps, 60fps, 120fps) using `Application.targetFrameRate`.
- **Test Code Snippet**:
  ```csharp
  void Update()
  {
      float deltaTime = Time.deltaTime;
      BlockBallPhysicsManager.Instance.UpdatePhysics(deltaTime);
      Debug.Log($"Frame Delta: {deltaTime}, Physics Steps: {BlockBallPhysicsManager.Instance.SubstepCount}");
  }
  ```
- **Success Criteria**: Physics updates occur every 0.02s, with substeps adjusting based on frame delta (e.g., 2 substeps at 30fps). Log must show consistent physics steps.
- **Error Handling**: If physics steps are inconsistent, check accumulator logic in `BlockBallPhysicsManager`.

## Test Case 2: Energy Conservation

- **Objective**: Verify that the Velocity Verlet integrator maintains energy conservation within ±0.1% over extended simulation.
- **Setup**: Set up a ball falling 100 meters under constant gravity (9.81 m/s²) with no drag or collisions. Record initial total energy (kinetic + potential).
- **Test Code Snippet**:
  ```csharp
  float initialEnergy = CalculateTotalEnergy(ball);
  for (int i = 0; i < 500; i++) // Simulate 10s at 50Hz
  {
      BlockBallPhysicsManager.Instance.UpdatePhysics(0.02f);
      if (i % 50 == 0) // Check every second
      {
          float currentEnergy = CalculateTotalEnergy(ball);
          float drift = Mathf.Abs((currentEnergy - initialEnergy) / initialEnergy * 100f);
          Debug.Log($"Time {i*0.02f}s: Energy Drift {drift:F3}%");
      }
  }
  float CalculateTotalEnergy(IPhysicsObject obj)
  {
      float kinetic = 0.5f * obj.GetMass() * obj.GetVelocity().sqrMagnitude;
      float potential = obj.GetMass() * 9.81f * obj.GetPosition().y;
      return kinetic + potential;
  }
  ```
- **Success Criteria**: Energy drift must remain below ±0.1% after 10 seconds (500 steps). Any deviation beyond this indicates integration errors.
- **Error Handling**: If drift exceeds ±0.1%, verify Velocity Verlet implementation, especially acceleration averaging.

## Test Case 3: Performance Stress Test

- **Objective**: Ensure physics system maintains performance with multiple objects without allocations in hot path.
- **Setup**: Spawn 50 physics objects (balls) with random initial velocities in a confined space with collisions enabled. Run for 1000 frames.
- **Test Code Snippet**:
  ```csharp
  void Start()
  {
      for (int i = 0; i < 50; i++)
      {
          Vector3 randomVel = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
          var ball = SpawnBall(randomVel);
          BlockBallPhysicsManager.Instance.RegisterPhysicsObject(ball);
      }
  }
  void Update()
  {
      float deltaTime = Time.deltaTime;
      BlockBallPhysicsManager.Instance.UpdatePhysics(deltaTime);
      PhysicsProfiler profiler = BlockBallPhysicsManager.Instance.Profiler;
      if (Time.frameCount % 100 == 0)
      {
          Debug.Log($"Frame {Time.frameCount}: Avg Physics Time {profiler.GetAverageFrameTime():F4}s, Allocations {profiler.GetAllocationCount()}");
      }
  }
  ```
- **Success Criteria**: Average physics update time must be under 0.02s per frame. Zero allocations must be recorded by `PhysicsProfiler`.
- **Error Handling**: If allocations occur, trace to specific methods using profiler logs. If frame time exceeds 0.02s, reduce collision checks or optimize calculations.

## Validation Steps for LLM
1. Execute all test cases in a Unity test scene, logging results for each.
2. Confirm fixed timestep test shows consistent 0.02s updates across frame rates.
3. Verify energy conservation test maintains drift below ±0.1%.
4. Ensure stress test shows no allocations and frame times under 0.02s with 50 objects.
5. Document test results in `/Status/Project_Overview.md` under Physics Migration.

## Error Handling Instructions for LLM
- If any test fails, log detailed frame-by-frame data for the failing case and reference implementation tasks for root cause analysis.
- Do not proceed to Phase 2 until all test cases pass; flag failures in `/Status/Issues_and_Required_Cleanup.md`.
- If performance issues arise, prioritize allocation fixes over frame time initially, as allocations violate a core design principle.

## Assumptions
- Test scenes can be created in Unity with minimal setup (basic physics objects and gravity).
- `PhysicsProfiler` and `BlockBallPhysicsManager` are fully implemented before testing.

## Next Steps After Completion
- Summarize test results and update project status files.
- If tests pass, prepare for Phase 2 integration with player and camera systems.
- If tests fail, revisit specific implementation tasks for components causing issues.
