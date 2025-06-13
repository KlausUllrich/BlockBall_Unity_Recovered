// File: BlockBallPhysicsManager.cs
// Purpose: Core physics system manager with Velocity Verlet integration
// Version: 1.0.0
// Date: 2025-06-12

using System.Collections.Generic;
using UnityEngine;
using BlockBall.Settings;

namespace BlockBall.Physics
{
    /// <summary>
    /// Central manager for BlockBall's custom physics system with fixed timestep simulation
    /// and Velocity Verlet integration for energy conservation and accuracy.
    /// </summary>
    public class BlockBallPhysicsManager : MonoBehaviour
    {
        [Header("Physics Configuration")]
        [Tooltip("Fixed timestep for physics simulation (50Hz = 0.02s). Ensures consistent physics regardless of framerate.")]
        [Range(0.01f, 0.05f)]
        public float FixedTimestep = 0.02f; // 50Hz
        
        [Tooltip("Maximum number of substeps to prevent spiral of death during frame drops.")]
        [Range(1, 16)]
        public int MaxSubsteps = 8;
        
        [Header("Performance Settings")]
        [Tooltip("Enable zero-allocation mode using object pooling for better performance.")]
        public bool useObjectPooling = true;
        
        [Tooltip("Enable debug profiling to monitor physics performance impact.")]
        public bool enableProfiling = false;
        
        [Header("Debug Visualization")]
        [Tooltip("Show physics debug information in scene view.")]
        public bool showDebugInfo = false;
        
        // Private fields
        private List<IPhysicsObject> physicsObjects = new List<IPhysicsObject>();
        private float accumulator = 0f;
        private PhysicsSettings physicsSettings;
        
        // Performance tracking
        private float physicsUpdateTime = 0f;
        private int physicsStepCount = 0;
        
        // Object pooling for zero allocation
        private Queue<Vector3> vector3Pool = new Queue<Vector3>();
        private const int POOL_SIZE = 100;
        
        #region Unity Lifecycle
        
        void Start()
        {
            // Load physics settings
            LoadPhysicsSettings();
            
            // Initialize object pool
            if (useObjectPooling)
            {
                InitializeObjectPool();
            }
            
            // Find and register all physics objects in scene
            RegisterPhysicsObjects();
            
            UnityEngine.Debug.Log($"BlockBallPhysicsManager initialized with {physicsObjects.Count} objects at {1f/FixedTimestep}Hz");
        }
        
        void Update()
        {
            if (physicsSettings?.physicsMode != PhysicsMode.CustomPhysics)
                return;
                
            float deltaTime = Time.deltaTime;
            accumulator += deltaTime;
            
            int stepCount = 0;
            float stepStartTime = Time.realtimeSinceStartup;
            
            // Fixed timestep with accumulator pattern
            while (accumulator >= FixedTimestep && stepCount < MaxSubsteps)
            {
                PhysicsStep(FixedTimestep);
                accumulator -= FixedTimestep;
                stepCount++;
                physicsStepCount++;
            }
            
            // Track performance
            if (enableProfiling && stepCount > 0)
            {
                physicsUpdateTime = Time.realtimeSinceStartup - stepStartTime;
            }
            
            // Prevent spiral of death
            if (stepCount >= MaxSubsteps)
            {
                UnityEngine.Debug.LogWarning($"Physics timestep overflow: {stepCount} steps, accumulator reset");
                accumulator = 0f;
            }
        }
        
        #endregion
        
        #region Physics Simulation
        
        /// <summary>
        /// Execute one physics step using Velocity Verlet integration
        /// </summary>
        private void PhysicsStep(float deltaTime)
        {
            // Pre-physics step: Calculate forces and accelerations
            foreach (var obj in physicsObjects)
            {
                if (obj is IAdvancedPhysicsObject advancedObj)
                {
                    advancedObj.PrePhysicsStep(deltaTime);
                }
            }
            
            // Main physics step: Integrate using Velocity Verlet
            foreach (var obj in physicsObjects)
            {
                VelocityVerletIntegrator.Integrate(obj, deltaTime);
            }
            
            // Post-physics step: Handle collisions and constraints
            foreach (var obj in physicsObjects)
            {
                if (obj is IAdvancedPhysicsObject advancedObj)
                {
                    advancedObj.PostPhysicsStep(deltaTime);
                }
            }
        }
        
        #endregion
        
        #region Object Management
        
        /// <summary>
        /// Register a physics object for simulation
        /// </summary>
        public void RegisterObject(IPhysicsObject obj)
        {
            if (!physicsObjects.Contains(obj))
            {
                physicsObjects.Add(obj);
                
                if (enableProfiling)
                {
                    UnityEngine.Debug.Log($"Registered physics object: {obj}");
                }
            }
        }
        
        /// <summary>
        /// Unregister a physics object from simulation
        /// </summary>
        public void UnregisterObject(IPhysicsObject obj)
        {
            if (physicsObjects.Remove(obj))
            {
                if (enableProfiling)
                {
                    UnityEngine.Debug.Log($"Unregistered physics object: {obj}");
                }
            }
        }
        
        /// <summary>
        /// Find and register all physics objects in the scene
        /// </summary>
        private void RegisterPhysicsObjects()
        {
            var objects = FindObjectsOfType<MonoBehaviour>();
            foreach (var obj in objects)
            {
                if (obj is IPhysicsObject physicsObj)
                {
                    RegisterObject(physicsObj);
                }
            }
        }
        
        #endregion
        
        #region Initialization
        
        private void LoadPhysicsSettings()
        {
            physicsSettings = Resources.Load<PhysicsSettings>("PhysicsSettings");
            if (physicsSettings == null)
            {
                UnityEngine.Debug.LogError("PhysicsSettings not found! Create it via Assets > Create > BlockBall > Physics Settings");
                physicsSettings = ScriptableObject.CreateInstance<PhysicsSettings>();
            }
        }
        
        private void InitializeObjectPool()
        {
            for (int i = 0; i < POOL_SIZE; i++)
            {
                vector3Pool.Enqueue(Vector3.zero);
            }
        }
        
        #endregion
        
        #region Object Pooling
        
        /// <summary>
        /// Get a Vector3 from the pool for zero-allocation calculations
        /// </summary>
        public Vector3 GetPooledVector3()
        {
            if (useObjectPooling && vector3Pool.Count > 0)
            {
                return vector3Pool.Dequeue();
            }
            return Vector3.zero;
        }
        
        /// <summary>
        /// Return a Vector3 to the pool
        /// </summary>
        public void ReturnVector3(Vector3 vector)
        {
            if (useObjectPooling)
            {
                vector3Pool.Enqueue(vector);
            }
        }
        
        #endregion
        
        #region Debug and Profiling
        
        void OnGUI()
        {
            if (!enableProfiling || !showDebugInfo)
                return;
                
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("BlockBall Physics Manager");
            GUILayout.Label($"Physics Mode: {physicsSettings?.physicsMode}");
            GUILayout.Label($"Objects: {physicsObjects.Count}");
            GUILayout.Label($"Timestep: {FixedTimestep:F3}s ({1f/FixedTimestep:F0}Hz)");
            GUILayout.Label($"Update Time: {physicsUpdateTime * 1000:F2}ms");
            GUILayout.Label($"Steps/Frame: {physicsStepCount}");
            GUILayout.Label($"Accumulator: {accumulator:F3}s");
            GUILayout.EndArea();
        }
        
        #endregion
        
        #region Singleton Pattern (Optional)
        
        private static BlockBallPhysicsManager instance;
        public static BlockBallPhysicsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<BlockBallPhysicsManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("BlockBallPhysicsManager");
                        instance = go.AddComponent<BlockBallPhysicsManager>();
                    }
                }
                return instance;
            }
        }
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        #endregion
    }
}
