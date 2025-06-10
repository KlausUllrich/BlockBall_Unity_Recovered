---
llm_metadata:
  title: "Phase 1 - PhysicsProfiler Implementation Task"
  phase: "Phase1_Core_Architecture"
  category: "Task"
  id: "04G"
  version: "1.0"
  created: "2023-10-15"
  last_modified: "2023-10-15"
  author: "Cascade AI Assistant"
  purpose: "Guide the implementation of PhysicsProfiler for performance monitoring in BlockBall Evolution."
  usage_context: "Use this document to implement the PhysicsProfiler class with explicit instructions for LLM processing."
  validation_status: "Not Validated"
---

# PhysicsProfiler Implementation Task

## Objective
Implement the `PhysicsProfiler` class to monitor and log performance metrics for the custom physics system in BlockBall Evolution, ensuring it meets performance targets with zero memory allocations in the hot path.

## Background
Performance monitoring is critical for a physics system running at 50Hz. The `PhysicsProfiler` will track frame times, substep counts, and memory allocations, providing insights to optimize the system and prevent performance degradation compared to Unity's default physics.

## Step-by-Step Instructions for LLM

1. **Create the Class Structure**:
   - Define a new C# class named `PhysicsProfiler` in the `BlockBallPhysics` namespace.
   - Include fields for tracking metrics like average frame time, peak substeps, and allocation counts.

2. **Implement Timing Methods**:
   - Add methods to start and end timing for each physics update cycle, using high-precision timers like `Stopwatch`.
   - Calculate and store average frame times over a rolling window of frames (e.g., last 100 frames).

3. **Track Substeps**:
   - Include a method to record the number of substeps taken per frame, updating peak and average substep counts.

4. **Monitor Allocations**:
   - Implement logic to detect any memory allocations during physics updates, flagging them as errors since the hot path must be allocation-free.

5. **Logging and Reporting**:
   - Create a method to log performance summaries to the Unity console or a file at regular intervals (e.g., every 10 seconds).
   - Ensure logs include warnings if performance metrics exceed thresholds (e.g., frame time > 0.02s).

6. **Validation Check**:
   - After implementation, validate that no allocations occur in the physics hot path by running a test scenario with 1000 frames.
   - Confirm that frame times for physics updates remain below 0.02s on target hardware.

## Partial Code Template

```csharp
namespace BlockBallPhysics
{
    public class PhysicsProfiler
    {
        private float _averageFrameTime;
        private int _peakSubsteps;
        private int _allocationCount;

        public void StartTiming()
        {
            // Start high-precision timer
        }

        public void EndTiming()
        {
            // Calculate frame time and update averages
        }

        public void RecordSubsteps(int count)
        {
            // Update peak and average substep counts
        }

        public void LogPerformanceSummary()
        {
            // Output metrics to console or file
        }
    }
}
```

## Error Handling Instructions for LLM
- If allocations are detected in the hot path, log the exact frame and method where they occur for immediate debugging.
- If frame times exceed 0.02s, flag a warning and suggest reducing substeps or optimizing `IPhysicsObject` implementations.
- Ensure logging does not itself cause allocations or significant performance overhead.

## Assumptions
- The physics system targets a 50Hz update rate (0.02s per frame).
- Unity's `Stopwatch` or equivalent high-precision timer is available for timing metrics.

## Next Steps After Completion
- Integrate `PhysicsProfiler` with `BlockBallPhysicsManager` to monitor live performance.
- Run extended tests (1000+ frames) to confirm no allocations and acceptable frame times.
- Update status in `/Status/Project_Overview.md` under Physics Migration section.
