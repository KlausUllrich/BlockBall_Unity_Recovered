// BallPhysics.cs - Main ball physics component using modular architecture
// Part of BlockBall Evolution Core Physics Architecture
// Created: 2025-06-12 by refactoring monolithic BallPhysics.cs into modular components

using UnityEngine;
using BlockBall.Settings;
using BlockBall.Physics.Components;

namespace BlockBall.Physics
{
    /// <summary>
    /// Main ball physics component that coordinates all physics subsystems.
    /// This is a modular architecture split into focused components for better maintainability.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class BallPhysics : MonoBehaviour, IAdvancedPhysicsObject
    {
        #region Physics Properties (IPhysicsObject)
        
        // Internal velocity tracking for kinematic mode
        private Vector3 internalVelocity = Vector3.zero;
        private Vector3 internalAngularVelocity = Vector3.zero;
        
        public Vector3 Position 
        { 
            get => transform.position; 
            set 
            {
                if (rigidBody != null && rigidBody.isKinematic)
                {
                    // Use MovePosition for kinematic bodies to maintain collision detection
                    rigidBody.MovePosition(value);
                }
                else
                {
                    // Direct assignment for non-kinematic bodies
                    transform.position = value;
                }
            }
        }
        
        public Vector3 Velocity 
        { 
            get => rigidBody.isKinematic ? internalVelocity : rigidBody.velocity; 
            set 
            {
                if (rigidBody.isKinematic)
                {
                    internalVelocity = value;
                }
                else
                {
                    rigidBody.velocity = value;
                }
            }
        }
        
        public Vector3 AngularVelocity 
        { 
            get => rigidBody.isKinematic ? internalAngularVelocity : rigidBody.angularVelocity; 
            set 
            {
                if (rigidBody.isKinematic)
                {
                    internalAngularVelocity = value;
                }
                else
                {
                    rigidBody.angularVelocity = value;
                }
            }
        }
        
        public float Mass 
        { 
            get => rigidBody.mass; 
            set => rigidBody.mass = value; 
        }
        
        public Vector3 GravityDirection { get; set; } = Vector3.down;
        public bool IsGrounded => groundDetector?.IsGrounded ?? false;
        
        // Advanced physics properties (IAdvancedPhysicsObject)
        public Vector3 Acceleration { get; set; }
        public PhysicsObjectState CurrentState => stateManager?.CurrentState ?? PhysicsObjectState.Airborne;
        
        #endregion
        
        #region Component References
        
        private Rigidbody rigidBody;
        private SphereCollider sphereCollider;
        private PhysicsSettings physicsSettings;
        
        // Physics components
        private BallInputHandler inputHandler;
        private BallGroundDetector groundDetector;
        private BallStateManager stateManager;
        private BallForceCalculator forceCalculator;
        private BallCollisionHandler collisionHandler;
        private BallDebugVisualizer debugVisualizer;
        
        #endregion
        
        #region Configuration Properties (exposed in Inspector)
        
        [Header("Speed Limits")]
        [SerializeField] private float maxInputSpeed = 5f;
        [SerializeField] private float maxPhysicsSpeed = 10f;
        [SerializeField] private float maxTotalSpeed = 15f;
        
        [Header("Jump Mechanics")]
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float jumpBufferTime = 0.2f;
        [SerializeField] private float coyoteTime = 0.1f;
        
        [Header("Physics Parameters")]
        [SerializeField] private float rollingFriction = 1f;
        [SerializeField] private float slidingFriction = 2f;
        [SerializeField] private float airDrag = 0.1f;
        [SerializeField] private float slopeLimit = 45f;
        [SerializeField] private float groundCheckDistance = 0.1f;
        
        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugVisualization = true;
        [SerializeField] private bool enableEnergyConservation = false;
        
        // Properties for external access
        public float MaxInputSpeed { get => maxInputSpeed; set => maxInputSpeed = value; }
        public float MaxPhysicsSpeed { get => maxPhysicsSpeed; set => maxPhysicsSpeed = value; }
        public float MaxTotalSpeed { get => maxTotalSpeed; set => maxTotalSpeed = value; }
        public float JumpHeight { get => jumpHeight; set => jumpHeight = value; }
        public float JumpBufferTime { get => jumpBufferTime; set => jumpBufferTime = value; }
        public float CoyoteTime { get => coyoteTime; set => coyoteTime = value; }
        public float RollingFriction { get => rollingFriction; set => rollingFriction = value; }
        public float SlidingFriction { get => slidingFriction; set => slidingFriction = value; }
        public float AirDrag { get => airDrag; set => airDrag = value; }
        public float SlopeLimit { get => slopeLimit; set => slopeLimit = value; }
        public float GroundCheckDistance { get => groundCheckDistance; set => groundCheckDistance = value; }
        public bool EnableEnergyConservation { get => enableEnergyConservation; set => enableEnergyConservation = value; }
        
        #endregion
        
        #region Unity Lifecycle
        
        void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            sphereCollider = GetComponent<SphereCollider>();
            
            // Load physics settings
            LoadPhysicsSettings();
            
            // Configure Rigidbody for custom physics
            if (physicsSettings?.physicsMode == PhysicsMode.CustomPhysics)
            {
                rigidBody.isKinematic = true; // Disable Unity physics
                rigidBody.useGravity = false; // We handle gravity ourselves
                rigidBody.drag = 0f; // We handle drag ourselves
                rigidBody.angularDrag = 0f; // We handle angular drag ourselves
                UnityEngine.Debug.Log($"Configured Rigidbody for CustomPhysics mode on {gameObject.name}");
            }
            
            // Initialize physics components
            InitializeComponents();
        }
        
        void Start()
        {
            // Register with physics manager
            if (BlockBallPhysicsManager.Instance != null)
            {
                BlockBallPhysicsManager.Instance.RegisterObject(this);
            }
            
            // Disable old physics wrapper to prevent conflicts
            if (physicsSettings?.physicsMode == PhysicsMode.CustomPhysics)
            {
                PhysicsObjectWrapper oldWrapper = GetComponent<PhysicsObjectWrapper>();
                if (oldWrapper != null)
                {
                    oldWrapper.enabled = false;
                    UnityEngine.Debug.Log($"Disabled PhysicsObjectWrapper on {gameObject.name} to prevent conflicts with BallPhysics");
                }
            }
        }
        
        void OnDestroy()
        {
            // Unregister from physics manager
            if (BlockBallPhysicsManager.Instance != null)
            {
                BlockBallPhysicsManager.Instance.UnregisterObject(this);
            }
        }
        
        void Update()
        {
            // Handle input (only in CustomPhysics mode)
            if (physicsSettings?.physicsMode == PhysicsMode.CustomPhysics)
            {
                inputHandler?.ProcessInput();
                inputHandler?.UpdateTimers(Time.deltaTime);
            }
        }
        
        void OnDrawGizmos()
        {
            if (enableDebugVisualization)
            {
                debugVisualizer?.DrawGizmos(Velocity, Acceleration);
            }
        }
        
        void OnGUI()
        {
            if (enableDebugVisualization && Application.isPlaying)
            {
                debugVisualizer?.DrawDebugGUI(Velocity, Position);
            }
        }
        
        #endregion
        
        #region IAdvancedPhysicsObject Implementation
        
        public void PrePhysicsStep(float deltaTime)
        {
            // Update ground detection
            groundDetector?.CheckGroundContact(GravityDirection);
            
            // Update state machine
            stateManager?.UpdateStateMachine(deltaTime, GravityDirection);
            
            // Calculate forces and acceleration
            forceCalculator?.CalculateForces(Position, Velocity, GravityDirection);
            Acceleration = forceCalculator?.CurrentAcceleration ?? Vector3.zero;
        }
        
        public void PostPhysicsStep(float deltaTime)
        {
            // Apply constraints and limits
            Vector3 limitedVelocity = forceCalculator?.ApplySpeedLimits(Velocity) ?? Velocity;
            Velocity = limitedVelocity;
            
            // Update rolling constraint
            ApplyRollingConstraint();
            
            // Update position based on internal velocity (for kinematic mode)
            if (rigidBody.isKinematic)
            {
                Vector3 newPosition = Position + internalVelocity * deltaTime;
                Position = newPosition; // This now uses MovePosition() for collision detection
                
                UnityEngine.Debug.Log($"BallPhysics: Moving from {Position} to {newPosition} with velocity {internalVelocity}");
            }
        }
        
        public Vector3 CalculateAcceleration()
        {
            return forceCalculator?.CurrentAcceleration ?? Vector3.zero;
        }
        
        public Vector3 CalculateAcceleration(Vector3 position)
        {
            return forceCalculator?.CalculateAcceleration(position, Velocity, GravityDirection) ?? Vector3.zero;
        }
        
        #endregion
        
        #region IPhysicsObject Implementation
        
        public bool HasGroundContact()
        {
            return groundDetector?.IsGrounded ?? false;
        }
        
        public void SetState(PhysicsObjectState state)
        {
            stateManager?.SetState(state);
        }
        
        public void IntegrateVelocityVerlet(float deltaTime)
        {
            VelocityVerletIntegrator.Integrate(this, deltaTime);
        }
        
        public void ApplyForce(Vector3 force, ForceMode mode = ForceMode.Force)
        {
            if (!rigidBody.isKinematic)
            {
                rigidBody.AddForce(force, mode);
            }
            else
            {
                // Custom force application for kinematic mode
                switch (mode)
                {
                    case ForceMode.Force:
                        Acceleration += force / Mass;
                        break;
                    case ForceMode.Acceleration:
                        Acceleration += force;
                        break;
                    case ForceMode.Impulse:
                        Velocity += force / Mass;
                        break;
                    case ForceMode.VelocityChange:
                        Velocity += force;
                        break;
                }
            }
        }
        
        public void ApplyTorque(Vector3 torque, ForceMode mode = ForceMode.Force)
        {
            if (!rigidBody.isKinematic)
            {
                rigidBody.AddTorque(torque, mode);
            }
            else
            {
                // Custom torque application for kinematic mode
                switch (mode)
                {
                    case ForceMode.Force:
                        // Convert torque to angular acceleration (torque = inertia * angular acceleration)
                        // For a sphere: I = (2/5) * m * r^2
                        float inertia = (2f/5f) * Mass * (sphereCollider.radius * sphereCollider.radius);
                        Vector3 angularAcceleration = torque / inertia;
                        AngularVelocity += angularAcceleration * Time.fixedDeltaTime;
                        break;
                    case ForceMode.Acceleration:
                        // Direct angular acceleration
                        AngularVelocity += torque * Time.fixedDeltaTime;
                        break;
                    case ForceMode.Impulse:
                        // Angular impulse (change in angular momentum)
                        float impulseInertia = (2f/5f) * Mass * (sphereCollider.radius * sphereCollider.radius);
                        AngularVelocity += torque / impulseInertia;
                        break;
                    case ForceMode.VelocityChange:
                        // Direct angular velocity change
                        AngularVelocity += torque;
                        break;
                }
            }
        }
        
        public void OnPhysicsCollision(Collision collision)
        {
            Vector3 velocity = Velocity;
            collisionHandler?.OnPhysicsCollision(collision, ref velocity);
            Velocity = velocity;
        }
        
        public void OnPhysicsTrigger(Collider other, bool isEnter)
        {
            collisionHandler?.OnPhysicsTrigger(other, isEnter);
        }
        
        #endregion
        
        #region Public Compatibility Methods (for PhysicsSystemMigrator)
        
        /// <summary>
        /// Public method for external systems (like PhysicsSystemMigrator) to trigger jump input.
        /// The actual jump handling is done automatically in PrePhysicsStep.
        /// </summary>
        public void HandleJumpInput()
        {
            // Jump handling is now done automatically in PrePhysicsStep
            // This method exists for compatibility with PhysicsSystemMigrator
            // The input handler will process the jump in the next physics step
            if (inputHandler != null)
            {
                // Force a jump buffer update to ensure the jump is processed
                inputHandler.OnJumpPressed?.Invoke();
            }
        }
        
        #endregion
        
        #region Private Methods
        
        private void InitializeComponents()
        {
            // Initialize physics components
            inputHandler = new BallInputHandler(physicsSettings);
            groundDetector = new BallGroundDetector(transform, sphereCollider, physicsSettings);
            stateManager = new BallStateManager(physicsSettings, groundDetector);
            forceCalculator = new BallForceCalculator(physicsSettings, inputHandler, stateManager, groundDetector);
            collisionHandler = new BallCollisionHandler(physicsSettings, stateManager);
            debugVisualizer = new BallDebugVisualizer(transform, physicsSettings, stateManager, groundDetector, forceCalculator, this);
            
            // Set up component relationships
            if (inputHandler != null && groundDetector != null)
            {
                groundDetector.OnGroundLost += () => inputHandler.SetCoyoteTimer(CoyoteTime);
            }
            
            UnityEngine.Debug.Log("BallPhysics: All components initialized");
        }
        
        private void LoadPhysicsSettings()
        {
            physicsSettings = Resources.Load<PhysicsSettings>("PhysicsSettings");
            if (physicsSettings == null)
            {
                UnityEngine.Debug.LogWarning("PhysicsSettings not found, using defaults");
                physicsSettings = ScriptableObject.CreateInstance<PhysicsSettings>();
            }
            
            // Override with settings values if available
            if (physicsSettings != null)
            {
                maxInputSpeed = physicsSettings.maxInputSpeed;
                maxPhysicsSpeed = physicsSettings.maxPhysicsSpeed;
                maxTotalSpeed = physicsSettings.maxTotalSpeed;
                jumpHeight = physicsSettings.jumpHeight;
                jumpBufferTime = physicsSettings.jumpBufferTime;
                coyoteTime = physicsSettings.coyoteTime;
                rollingFriction = physicsSettings.rollingFriction;
                slidingFriction = physicsSettings.slidingFriction;
                airDrag = physicsSettings.airDrag;
                slopeLimit = physicsSettings.slopeLimit;
                groundCheckDistance = physicsSettings.groundCheckDistance;
            }
        }
        
        private void ApplyRollingConstraint()
        {
            if (stateManager?.CurrentState == PhysicsObjectState.Grounded)
            {
                Vector3 rollingAngularVel = forceCalculator?.CalculateRollingAngularVelocity(
                    Velocity, GravityDirection, sphereCollider.radius) ?? Vector3.zero;
                AngularVelocity = rollingAngularVel;
            }
        }
        
        #endregion
        
        #region Unity Collision Events
        
        void OnCollisionEnter(Collision collision)
        {
            OnPhysicsCollision(collision);
        }
        
        void OnTriggerEnter(Collider other)
        {
            OnPhysicsTrigger(other, true);
        }
        
        void OnTriggerExit(Collider other)
        {
            OnPhysicsTrigger(other, false);
        }
        
        #endregion
    }
}
