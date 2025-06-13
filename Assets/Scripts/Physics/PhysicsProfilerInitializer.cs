using UnityEngine;

namespace BlockBall.Physics
{
    public class PhysicsProfilerInitializer : MonoBehaviour
    {
        void Start()
        {
            // Check if PhysicsProfiler instance already exists in the scene
            if (PhysicsProfiler.Instance == null)
            {
                // Load the prefab from Resources or Assets
                GameObject profilerPrefab = Resources.Load<GameObject>("PhysicsProfiler");
                if (profilerPrefab == null)
                {
                    profilerPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PhysicsProfiler.prefab");
                }

                if (profilerPrefab != null)
                {
                    Instantiate(profilerPrefab);
                    UnityEngine.Debug.Log("PhysicsProfiler instantiated in scene by initializer.");
                }
                else
                {
                    UnityEngine.Debug.LogError("PhysicsProfilerInitializer: Could not load PhysicsProfiler prefab.");
                }
            }
            else
            {
                UnityEngine.Debug.Log("PhysicsProfiler already exists in scene.");
            }
        }
    }
}
