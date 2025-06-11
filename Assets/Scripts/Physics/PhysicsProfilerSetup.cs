// File: Assets/Scripts/Physics/PhysicsProfilerSetup.cs
// Purpose: Setup script to ensure PhysicsProfiler is in the scene
using UnityEngine;

namespace BlockBall.Physics
{
    public class PhysicsProfilerSetup : MonoBehaviour
    {
        [SerializeField] private GameObject physicsProfilerPrefab;
        
        void Awake()
        {
            // Check if PhysicsProfiler instance already exists in the scene
            if (PhysicsProfiler.Instance == null)
            {
                // If no instance exists, instantiate the profiler prefab
                if (physicsProfilerPrefab != null)
                {
                    Instantiate(physicsProfilerPrefab);
                    UnityEngine.Debug.Log("PhysicsProfiler instantiated in scene.");
                }
                else
                {
                    UnityEngine.Debug.LogError("PhysicsProfilerSetup: No profiler prefab assigned.");
                }
            }
            else
            {
                UnityEngine.Debug.Log("PhysicsProfiler already exists in scene.");
            }
        }
    }
}
