---
title: "Phase 0A: Task 4 - PhysicsProfiler Component (LLM)"
phase: "0A"
context_required: ["PhysicsProfiler", "PerformanceMonitoring"]
dependencies: ["LLM_01A_Mission_and_Precision.md", "LLM_01B_Migration_Phases.md", "LLM_02C_DeterministicMath_Task.md"]
output_files: ["Assets/Scripts/Physics/PhysicsProfiler.cs"]
validation_steps:
  - "Confirm PhysicsProfiler script matches provided code"
  - "Verify performance tracking accuracy"
  - "Check memory allocation monitoring"
  - "Test integration with physics components"
priority: 4
---

# Phase 0A: Task 4 - Implement PhysicsProfiler Component (LLM)

## Objective
Develop a performance monitoring component to track physics update times and memory allocations, ensuring the custom physics system meets performance targets during migration.

**Directive for LLM**: Follow the steps below precisely to implement `PhysicsProfiler`. Validate each step and log progress or issues in the status files. Do not proceed to other tasks until validation is complete.

## Implementation Steps
1. **Verify Unity Editor Setup**:
   - Ensure Unity 2022.3 LTS is installed and the project is loaded at `C:\Users\Klaus\My_Game_Projects\Blockball\BlockBall_Unity_Recovered`.
   - **Validation**: If project path is incorrect, log error in `/Status/Issues_and_Required_Cleanup.md`: "Project path mismatch. Verify project location before proceeding."
2. **Create Physics Directory**:
   - Create directory `Assets/Scripts/Physics/` if it does not exist.
   - **Validation**: Check directory existence. If creation fails, log error: "Failed to create Assets/Scripts/Physics/. Check write permissions."
3. **Create PhysicsProfiler Script**:
   - Write the following C# script at `Assets/Scripts/Physics/PhysicsProfiler.cs`.
   - **Code Block**:
     ```csharp
     // File: Assets/Scripts/Physics/PhysicsProfiler.cs
     // Purpose: Performance monitoring for physics system during migration
     using UnityEngine;
     using System.Collections.Generic;
     using System.Diagnostics;
     
     public class PhysicsProfiler : MonoBehaviour
     {
         [Header("Profiling Settings")]
         public bool enableProfiling = true;
         public bool logToConsole = false;
         public int sampleWindow = 60; // frames
         
         [Header("Performance Targets")]
         [Tooltip("Maximum physics update time in milliseconds")]
         public float maxPhysicsUpdateMs = 2.0f;
         
         [Header("Current Metrics")]
         [SerializeField]
         private float lastUpdateMs;
         [SerializeField]
         private float averageUpdateMs;
         [SerializeField]
         private float maxUpdateMs;
         [SerializeField]
         private float averageMemoryAllocations;
         [SerializeField]
         private bool targetMet;
         
         private List<float> updateTimes = new List<float>();
         private List<float> memoryAllocations = new List<float>();
         private Stopwatch stopwatch = new Stopwatch();
         private int sampleCount;
         
         void Start()
         {
             if (enableProfiling)
             {
                 updateTimes.Capacity = sampleWindow;
                 memoryAllocations.Capacity = sampleWindow;
                 StartProfiling();
             }
         }
         
         void OnDestroy()
         {
             StopProfiling();
         }
         
         public void StartProfiling()
         {
             enableProfiling = true;
             sampleCount = 0;
             updateTimes.Clear();
             memoryAllocations.Clear();
             stopwatch.Reset();
             if (logToConsole)
                 UnityEngine.Debug.Log("Physics profiling started");
         }
         
         public void StopProfiling()
         {
             enableProfiling = false;
             if (stopwatch.IsRunning)
                 stopwatch.Stop();
             if (logToConsole && sampleCount > 0)
                 LogReport();
         }
         
         public void BeginUpdate()
         {
             if (enableProfiling && !stopwatch.IsRunning)
             {
                 stopwatch.Restart();
             }
         }
         
         public void EndUpdate()
         {
             if (enableProfiling && stopwatch.IsRunning)
             {
                 stopwatch.Stop();
                 lastUpdateMs = stopwatch.ElapsedTicks * 1000f / Stopwatch.Frequency;
                 RecordSample(lastUpdateMs);
             }
         }
         
         public void RecordAllocation(float allocationBytes)
         {
             if (enableProfiling)
             {
                 memoryAllocations.Add(allocationBytes);
                 if (memoryAllocations.Count > sampleWindow)
                     memoryAllocations.RemoveAt(0);
                 ComputeMemoryStats();
             }
         }
         
         private void RecordSample(float updateMs)
         {
             updateTimes.Add(updateMs);
             if (updateTimes.Count > sampleWindow)
                 updateTimes.RemoveAt(0);
             sampleCount++;
             ComputeStats();
         }
         
         private void ComputeStats()
         {
             float total = 0f;
             maxUpdateMs = 0f;
             
             foreach (var time in updateTimes)
             {
                 total += time;
                 if (time > maxUpdateMs)
                     maxUpdateMs = time;
             }
             
             averageUpdateMs = total / updateTimes.Count;
             targetMet = averageUpdateMs <= maxPhysicsUpdateMs && maxUpdateMs <= maxPhysicsUpdateMs * 1.5f;
             
             if (logToConsole && sampleCount % sampleWindow == 0)
                 LogReport();
         }
         
         private void ComputeMemoryStats()
         {
             float total = 0f;
             foreach (var alloc in memoryAllocations)
                 total += alloc;
             averageMemoryAllocations = total / memoryAllocations.Count;
         }
         
         private void LogReport()
         {
             string report = $"Physics Profiler Report (Samples: {sampleCount})\n" +
                             $"Avg Update: {averageUpdateMs:F3}ms\n" +
                             $"Max Update: {maxUpdateMs:F3}ms\n" +
                             $"Target Met: {targetMet}\n" +
                             $"Avg Memory Allocation: {averageMemoryAllocations:F1} bytes";
             UnityEngine.Debug.Log(report);
         }
         
         public PhysicsPerformanceReport GetPerformanceReport()
         {
             return new PhysicsPerformanceReport
             {
                 lastUpdateMs = lastUpdateMs,
                 averageUpdateMs = averageUpdateMs,
                 maxPhysicsUpdateMs = maxUpdateMs,
                 targetMet = targetMet,
                 averageMemoryAllocations = averageMemoryAllocations,
                 sampleCount = sampleCount
             };
         }
     }
     
     [System.Serializable]
     public struct PhysicsPerformanceReport
     {
         public float lastUpdateMs;
         public float averageUpdateMs;
         public float maxPhysicsUpdateMs;
         public bool targetMet;
         public float averageMemoryAllocations;
         public int sampleCount;
     }
     ```
   - **Validation**: Confirm script is created and compiles without errors in Unity. If compilation fails, log error: "PhysicsProfiler.cs compilation error. Check syntax and Unity version."
4. **Attach PhysicsProfiler to GameObject**:
   - Add `PhysicsProfiler` component to a persistent GameObject in the scene (e.g., GameManager or a dedicated Profiler object).
   - **Validation**: Check a sample scene to confirm profiler is attached. If not, log error: "PhysicsProfiler not attached to any GameObject. Add to persistent object in scene."
5. **Test Performance Tracking**:
   - Test `BeginUpdate()` and `EndUpdate()` methods to ensure they accurately measure physics update times.
   - **Validation**: Run a test scene and verify `lastUpdateMs` and `averageUpdateMs` are updated. If values are zero or incorrect, log error: "Physics update time tracking failed. Check stopwatch usage in PhysicsProfiler."
6. **Test Memory Allocation Tracking**:
   - Simulate allocations with `RecordAllocation()` and verify `averageMemoryAllocations` updates correctly.
   - **Validation**: If memory stats do not update, log error: "Memory allocation tracking failed. Check RecordAllocation method."
7. **Integrate with Physics Components**:
   - Ensure `PhysicsProfiler` can be called from physics update loops in `PhysicObjekt` or future custom physics managers.
   - **Validation**: Add profiler calls to a test physics update. If integration fails, log error: "PhysicsProfiler integration issue. Check update loop calls."

## Acceptance Criteria for PhysicsProfiler
- **Script Creation Check**: Verify `Assets/Scripts/Physics/PhysicsProfiler.cs` exists and matches provided code. If not, log error: "PhysicsProfiler script missing or incorrect. Create with provided code."
- **Performance Tracking Check**: Confirm profiler tracks physics update times accurately (within 0.1ms of manual timing). If inaccurate, log error: "Update time tracking inaccurate. Test stopwatch precision."
- **Memory Allocation Check**: Validate `RecordAllocation()` updates memory stats. If not, log error: "Memory tracking not working. Check allocation recording logic."
- **Performance Target Check**: Ensure `targetMet` reflects whether performance meets `maxPhysicsUpdateMs`. If incorrect, log error: "Performance target evaluation incorrect. Review logic."
- **Integration Check**: Confirm integration with existing physics components or test loops. If fails, log error: "Integration with physics components failed. Adjust update calls."

**Directive for LLM**: Log all validation results and issues in `/Status/Issues_and_Required_Cleanup.md`. Update `/Status/Project_Overview.md` with completion status of PhysicsProfiler task.

**Next Step**: Proceed to `LLM_02E_Phase0A_Checklist.md` for the Phase 0A completion checklist after completing and validating PhysicsProfiler implementation.
