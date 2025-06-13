// File: AdvancedPhysicsSetup.cs
// Purpose: Unity Editor tools for setting up advanced physics components
// Version: 1.0.0
// Date: 2025-06-12

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using BlockBall.Physics;

namespace BlockBall.Editor
{
    /// <summary>
    /// Unity Editor tools for setting up the advanced physics system
    /// </summary>
    public class AdvancedPhysicsSetup : EditorWindow
    {
        private GameObject selectedObject;
        private bool autoSetupRigidbody = true;
        private bool autoSetupCollider = true;
        private bool addToPhysicsManager = true;
        
        [MenuItem("BlockBall/Advanced Physics Setup")]
        public static void ShowWindow()
        {
            GetWindow<AdvancedPhysicsSetup>("Advanced Physics Setup");
        }
        
        void OnGUI()
        {
            GUILayout.Label("Advanced Physics Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            // Object selection
            selectedObject = (GameObject)EditorGUILayout.ObjectField("Target Object", selectedObject, typeof(GameObject), true);
            
            GUILayout.Space(10);
            
            // Setup options
            autoSetupRigidbody = EditorGUILayout.Toggle("Auto Setup Rigidbody", autoSetupRigidbody);
            autoSetupCollider = EditorGUILayout.Toggle("Auto Setup Sphere Collider", autoSetupCollider);
            addToPhysicsManager = EditorGUILayout.Toggle("Add to Physics Manager", addToPhysicsManager);
            
            GUILayout.Space(10);
            
            // Setup buttons
            GUI.enabled = selectedObject != null;
            
            if (GUILayout.Button("Setup BallPhysics Component"))
            {
                SetupBallPhysics();
            }
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("Setup Physics Manager"))
            {
                SetupPhysicsManager();
            }
            
            GUI.enabled = true;
            
            GUILayout.Space(20);
            
            // Information
            EditorGUILayout.HelpBox(
                "BallPhysics Component:\n" +
                "• Adds advanced physics with Velocity Verlet integration\n" +
                "• Requires Rigidbody and SphereCollider\n" +
                "• Works with CustomPhysics mode\n\n" +
                "Physics Manager:\n" +
                "• Central manager for custom physics simulation\n" +
                "• Handles fixed timestep and object registration\n" +
                "• Required for CustomPhysics mode to function",
                MessageType.Info);
        }
        
        private void SetupBallPhysics()
        {
            if (selectedObject == null) return;
            
            Undo.RegisterCompleteObjectUndo(selectedObject, "Setup BallPhysics");
            
            // Add BallPhysics component if not present
            BallPhysics ballPhysics = selectedObject.GetComponent<BallPhysics>();
            if (ballPhysics == null)
            {
                ballPhysics = selectedObject.AddComponent<BallPhysics>();
                UnityEngine.Debug.Log($"Added BallPhysics component to {selectedObject.name}");
            }
            
            // Setup Rigidbody
            if (autoSetupRigidbody)
            {
                Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = selectedObject.AddComponent<Rigidbody>();
                    UnityEngine.Debug.Log($"Added Rigidbody to {selectedObject.name}");
                }
                
                // Configure for ball physics
                rb.mass = 1f;
                rb.drag = 0f; // Custom physics handles drag
                rb.angularDrag = 0f;
                rb.useGravity = false; // Custom physics handles gravity
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
            
            // Setup SphereCollider
            if (autoSetupCollider)
            {
                SphereCollider collider = selectedObject.GetComponent<SphereCollider>();
                if (collider == null)
                {
                    collider = selectedObject.AddComponent<SphereCollider>();
                    UnityEngine.Debug.Log($"Added SphereCollider to {selectedObject.name}");
                }
                
                // Configure for ball
                collider.radius = 0.5f;
                collider.center = Vector3.zero;
            }
            
            // Set default parameters from PhysicsSettings
            var physicsSettings = Resources.Load<BlockBall.Settings.PhysicsSettings>("PhysicsSettings");
            if (physicsSettings != null)
            {
                ballPhysics.MaxInputSpeed = physicsSettings.maxInputSpeed;
                ballPhysics.MaxPhysicsSpeed = physicsSettings.maxPhysicsSpeed;
                ballPhysics.MaxTotalSpeed = physicsSettings.maxTotalSpeed;
                ballPhysics.JumpHeight = physicsSettings.jumpHeight;
                ballPhysics.JumpBufferTime = physicsSettings.jumpBufferTime;
                ballPhysics.CoyoteTime = physicsSettings.coyoteTime;
                ballPhysics.RollingFriction = physicsSettings.rollingFriction;
                ballPhysics.SlidingFriction = physicsSettings.slidingFriction;
                ballPhysics.AirDrag = physicsSettings.airDrag;
                ballPhysics.SlopeLimit = physicsSettings.slopeLimit;
                ballPhysics.GroundCheckDistance = physicsSettings.groundCheckDistance;
                
                UnityEngine.Debug.Log("Applied PhysicsSettings parameters to BallPhysics component");
            }
            
            EditorUtility.SetDirty(selectedObject);
            
            UnityEngine.Debug.Log($"Successfully set up BallPhysics on {selectedObject.name}");
        }
        
        private void SetupPhysicsManager()
        {
            // Check if physics manager already exists
            BlockBallPhysicsManager existingManager = FindObjectOfType<BlockBallPhysicsManager>();
            if (existingManager != null)
            {
                UnityEngine.Debug.Log($"Physics Manager already exists on {existingManager.gameObject.name}");
                Selection.activeGameObject = existingManager.gameObject;
                return;
            }
            
            GameObject managerObject;
            
            if (selectedObject != null)
            {
                // Add to selected object
                managerObject = selectedObject;
            }
            else
            {
                // Create new GameObject
                managerObject = new GameObject("BlockBall Physics Manager");
                managerObject.transform.position = Vector3.zero;
            }
            
            Undo.RegisterCreatedObjectUndo(managerObject, "Setup Physics Manager");
            
            // Add the physics manager component
            BlockBallPhysicsManager manager = managerObject.GetComponent<BlockBallPhysicsManager>();
            if (manager == null)
            {
                manager = managerObject.AddComponent<BlockBallPhysicsManager>();
            }
            
            // Configure with settings from PhysicsSettings
            var physicsSettings = Resources.Load<BlockBall.Settings.PhysicsSettings>("PhysicsSettings");
            if (physicsSettings != null)
            {
                manager.FixedTimestep = physicsSettings.customPhysicsTimestep;
                manager.MaxSubsteps = physicsSettings.maxPhysicsSubsteps;
                manager.useObjectPooling = physicsSettings.useObjectPooling;
                manager.enableProfiling = physicsSettings.enableMigrationLogging;
                
                UnityEngine.Debug.Log("Applied PhysicsSettings parameters to Physics Manager");
            }
            
            // Select the manager object
            Selection.activeGameObject = managerObject;
            EditorUtility.SetDirty(managerObject);
            
            UnityEngine.Debug.Log($"Successfully set up Physics Manager on {managerObject.name}");
        }
        
        void OnSelectionChange()
        {
            if (Selection.activeGameObject != null)
            {
                selectedObject = Selection.activeGameObject;
                Repaint();
            }
        }
    }
    
    /// <summary>
    /// Context menu items for quick setup
    /// </summary>
    public class AdvancedPhysicsContextMenu
    {
        [MenuItem("CONTEXT/Rigidbody/Setup Ball Physics")]
        public static void SetupBallPhysicsFromRigidbody(MenuCommand command)
        {
            Rigidbody rb = (Rigidbody)command.context;
            SetupBallPhysicsOnObject(rb.gameObject);
        }
        
        [MenuItem("GameObject/BlockBall/Add Ball Physics", false, 10)]
        public static void AddBallPhysicsToSelected()
        {
            if (Selection.activeGameObject != null)
            {
                SetupBallPhysicsOnObject(Selection.activeGameObject);
            }
        }
        
        [MenuItem("GameObject/BlockBall/Create Physics Manager", false, 11)]
        public static void CreatePhysicsManager()
        {
            GameObject managerObject = new GameObject("BlockBall Physics Manager");
            BlockBallPhysicsManager manager = managerObject.AddComponent<BlockBallPhysicsManager>();
            
            // Apply default settings
            var physicsSettings = Resources.Load<BlockBall.Settings.PhysicsSettings>("PhysicsSettings");
            if (physicsSettings != null)
            {
                manager.FixedTimestep = physicsSettings.customPhysicsTimestep;
                manager.MaxSubsteps = physicsSettings.maxPhysicsSubsteps;
                manager.useObjectPooling = physicsSettings.useObjectPooling;
                manager.enableProfiling = physicsSettings.enableMigrationLogging;
            }
            
            Selection.activeGameObject = managerObject;
            Undo.RegisterCreatedObjectUndo(managerObject, "Create Physics Manager");
            
            UnityEngine.Debug.Log("Created BlockBall Physics Manager");
        }
        
        private static void SetupBallPhysicsOnObject(GameObject target)
        {
            Undo.RegisterCompleteObjectUndo(target, "Setup Ball Physics");
            
            // Add components
            if (target.GetComponent<BallPhysics>() == null)
            {
                target.AddComponent<BallPhysics>();
            }
            
            if (target.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = target.AddComponent<Rigidbody>();
                rb.mass = 1f;
                rb.useGravity = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }
            
            if (target.GetComponent<SphereCollider>() == null)
            {
                SphereCollider collider = target.AddComponent<SphereCollider>();
                collider.radius = 0.5f;
            }
            
            EditorUtility.SetDirty(target);
            UnityEngine.Debug.Log($"Set up Ball Physics on {target.name}");
        }
    }
}
#endif
