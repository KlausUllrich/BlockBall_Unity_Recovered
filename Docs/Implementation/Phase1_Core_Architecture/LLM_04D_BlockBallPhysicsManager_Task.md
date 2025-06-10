---
title: BlockBall Physics Manager Implementation
description: Task to create the central physics coordinator for BlockBall.
phase: 1B
dependencies: LLM_04A_Phase1_Overview.md, LLM_04B_Phase1_Deliverables.md, LLM_04C_PhysicsSettings_Task.md
validation_criteria: Manager runs at 50Hz, handles physics updates, supports substeps.
---

# Task 1.2: Create BlockBallPhysicsManager

## Objective
Implement `BlockBallPhysicsManager` as the central coordinator for the custom physics system, managing fixed timestep updates at 50Hz.

## Background
This manager replaces Unity's physics update cycle with a deterministic, fixed timestep system using the accumulator pattern. It ensures physics updates occur at 50Hz (every 0.02 seconds), independent of framerate, to prevent jitter and maintain consistency.

## Implementation Steps
1. **Create Script File**:
   - Path: `Assets/Scripts/Physics/BlockBallPhysicsManager.cs`
   - Action: Create a new C# script in Unity under the specified path.
2. **Add Code Template**:
   - Action: Copy the provided code template into `BlockBallPhysicsManager.cs`.
   - Purpose: Implements singleton pattern, fixed timestep logic, and physics object management.
3. **Setup Singleton**:
   - Action: Ensure only one instance exists using `DontDestroyOnLoad()` and instance checks.
   - Expected Outcome: Prevents multiple managers from conflicting.
4. **Implement Accumulator Pattern**:
   - Action: Use `Update()` to accumulate time and trigger `FixedUpdatePhysics()` at 0.02s intervals.
   - Expected Outcome: Physics updates occur exactly 50 times per second.
5. **Add Substep Handling**:
   - Action: Implement logic to handle up to 8 substeps if updates lag behind.
   - Expected Outcome: Maintains stability during frame drops.
6. **Add to MainScene**:
   - Action: Add `BlockBallPhysicsManager` component to a GameObject in `MainScene`.
   - Name: `PhysicsManager`
   - Expected Outcome: Automatically starts with the game.

## Code Template (Part 1 - Core Structure)
```csharp
using UnityEngine;
using System.Collections.Generic;

namespace BlockBall.Physics
{
    /// <summary>
    /// Central coordinator for custom physics system.
    /// Manages fixed timestep updates at 50Hz.
    /// </summary>
    public class BlockBallPhysicsManager : MonoBehaviour
    {
        private static BlockBallPhysicsManager instance;
        public static BlockBallPhysicsManager Instance
        {
            get { return instance; }
        }

        [Header("Physics Configuration")]
        [Tooltip("Fixed timestep in seconds (1/50Hz = 0.02s)")]
        public float FixedTimestep = 0.02f;

        [Tooltip("Maximum number of physics substeps if framerate drops")]
        public int MaxSubsteps = 8;

        private float accumulatedTime = 0f;
        private List<IPhysicsObject> physicsObjects = new List<IPhysicsObject>();
        private PhysicsProfiler profiler;
        private PhysicsSettings settings;

        private void Awake()
        {
            // Singleton pattern
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            // Disable Unity physics for managed objects
            Physics.autoSimulation = false;

            // Load physics settings
            settings = Resources.Load<PhysicsSettings>("DefaultPhysicsSettings");
            if (settings == null)
            {
                Debug.LogError("PhysicsSettings not found! Create it in Resources folder.");
                settings = ScriptableObject.CreateInstance<PhysicsSettings>();
            }

            // Initialize profiler
            profiler = new PhysicsProfiler();
        }

        private void Update()
        {
            // Accumulator pattern for fixed timestep
            accumulatedTime += Time.deltaTime;

            // Limit substeps to prevent spiral of death
            int substeps = Mathf.Min(Mathf.FloorToInt(accumulatedTime / FixedTimestep), MaxSubsteps);

            if (substeps > 0)
            {
                profiler.BeginFrame();

                for (int i = 0; i < substeps; i++)
                {
                    profiler.BeginUpdate();
                    FixedUpdatePhysics();
                    profiler.EndUpdate();
                }

                profiler.EndFrame();
                accumulatedTime -= substeps * FixedTimestep;
            }
        }

        // See LLM_04D_BlockBallPhysicsManager_Code.md for FixedUpdatePhysics and other methods.
    }
}
```

## Validation Steps
1. **Timestep Verification**:
   - Action: Add a debug log in `FixedUpdatePhysics()` to count updates per second.
   - Check: Run game for 10 seconds, verify ~500 updates (50Hz * 10s).
   - Expected Outcome: Updates occur at 50Hz ±1%.
2. **Singleton Check**:
   - Action: Attempt to create a second `BlockBallPhysicsManager` in the scene.
   - Check: Second instance is destroyed automatically.
   - Expected Outcome: Only one instance exists.
3. **Substep Handling**:
   - Action: Simulate lag by setting `Application.targetFrameRate = 10`.
   - Check: Physics updates still occur, limited to `MaxSubsteps` per frame.
   - Expected Outcome: No more than 8 substeps per frame.

## Error Handling
- **If Physics Updates Skip**: Check `accumulatedTime` logic in `Update()`. Ensure `Time.deltaTime` is correctly added.
- **If Multiple Instances Exist**: Verify `Awake()` destroys duplicates. Check `DontDestroyOnLoad()` usage.
- **If Settings Fail to Load**: Ensure `DefaultPhysicsSettings` is in `Assets/Resources/`. Add fallback values if loading fails.
