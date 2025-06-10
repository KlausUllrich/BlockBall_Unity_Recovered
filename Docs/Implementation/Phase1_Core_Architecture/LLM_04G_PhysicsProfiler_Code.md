---
llm_metadata:
  title: "Phase 1 - PhysicsProfiler Code Reference"
  phase: "Phase1_Core_Architecture"
  category: "Code Reference"
  id: "04G"
  version: "1.0"
  created: "2023-10-15"
  last_modified: "2023-10-15"
  author: "Cascade AI Assistant"
  purpose: "Provide complete code reference for PhysicsProfiler in BlockBall Evolution."
  usage_context: "Use this document as a code reference for implementing PhysicsProfiler with LLM processing."
  validation_status: "Not Validated"
---

# PhysicsProfiler Code Reference

## Complete Code for PhysicsProfiler

```csharp
using System.Diagnostics;
using UnityEngine;

namespace BlockBallPhysics
{
    public class PhysicsProfiler
    {
        private Stopwatch _stopwatch;
        private float[] _frameTimes = new float[100];
        private int _frameIndex = 0;
        private float _averageFrameTime = 0f;
        private int _peakSubsteps = 0;
        private int _totalSubsteps = 0;
        private int _substepCountFrames = 0;
        private int _allocationCount = 0;
        private float _lastLogTime = 0f;
        private const float LogInterval = 10f; // Log every 10 seconds

        public PhysicsProfiler()
        {
            _stopwatch = new Stopwatch();
        }

        public void StartTiming()
        {
            _stopwatch.Restart();
        }

        public void EndTiming()
        {
            _stopwatch.Stop();
            float frameTime = _stopwatch.ElapsedMilliseconds / 1000f; // Convert to seconds
            _frameTimes[_frameIndex] = frameTime;
            _frameIndex = (_frameIndex + 1) % _frameTimes.Length;

            // Calculate rolling average
            float sum = 0f;
            for (int i = 0; i < _frameTimes.Length; i++)
            {
                sum += _frameTimes[i];
            }
            _averageFrameTime = sum / _frameTimes.Length;
        }

        public void RecordSubsteps(int count)
        {
            if (count > _peakSubsteps)
            {
                _peakSubsteps = count;
            }
            _totalSubsteps += count;
            _substepCountFrames++;
        }

        public void RecordAllocation()
        {
            _allocationCount++;
            Debug.LogWarning($"[PhysicsProfiler] Memory allocation detected in physics hot path! Total allocations: {_allocationCount}");
        }

        public void LogPerformanceSummary()
        {
            float currentTime = Time.time;
            if (currentTime - _lastLogTime >= LogInterval)
            {
                float averageSubsteps = _substepCountFrames > 0 ? (float)_totalSubsteps / _substepCountFrames : 0f;
                Debug.Log($"[PhysicsProfiler] Performance Summary: Avg Frame Time: {_averageFrameTime:F4}s, Peak Substeps: {_peakSubsteps}, Avg Substeps: {averageSubsteps:F2}, Allocations: {_allocationCount}");

                if (_averageFrameTime > 0.02f)
                {
                    Debug.LogWarning($"[PhysicsProfiler] Physics frame time exceeds target (0.02s): {_averageFrameTime:F4}s");
                }

                if (_allocationCount > 0)
                {
                    Debug.LogError($"[PhysicsProfiler] Allocations in hot path detected: {_allocationCount}. This violates zero-allocation policy!");
                }

                _lastLogTime = currentTime;
            }
        }

        public float GetAverageFrameTime()
        {
            return _averageFrameTime;
        }

        public int GetPeakSubsteps()
        {
            return _peakSubsteps;
        }

        public int GetAllocationCount()
        {
            return _allocationCount;
        }
    }
}
```

## Validation Steps for LLM
1. Confirm that `StartTiming` and `EndTiming` use a high-precision `Stopwatch` for accurate measurements.
2. Verify that frame times are averaged over a rolling window to smooth out spikes.
3. Ensure `RecordAllocation` logs warnings for any allocations, enforcing the zero-allocation policy.
4. Check that `LogPerformanceSummary` triggers every 10 seconds and flags frame times exceeding 0.02s.
5. Test with a 1000-frame scenario to confirm no allocations are recorded and frame times are within target.

## Error Handling
- If frame times consistently exceed 0.02s, review substep counts and suggest optimization of physics calculations.
- If allocations are logged, trace the exact frame and method causing them using additional debug logs.
- Ensure `LogPerformanceSummary` does not cause performance overhead by limiting log frequency.
