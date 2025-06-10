# Phase 2: Ball Physics Implementation Tasks

## Task 2.1: Create BallStateMachine

### Objective
Implement a robust state machine to manage ball behavior across different physics states.

### Implementation Steps
1. **Create Enum**: Define all possible ball states
2. **Create StateMachine Class**: Handle state transitions and validation
3. **Implement State Logic**: Enter/exit behaviors for each state
4. **Add Debugging**: Visual state display and logging

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    public enum BallState
    {
        Grounded,      // Rolling on surface
        Airborne,      // In flight
        Sliding,       // On steep slope (>45°)
        Transitioning  // Gravity switch active
    }
    
    public class BallStateMachine
    {
        private BallState currentState = BallState.Airborne; // Start airborne
        private BallState previousState = BallState.Airborne;
        private float stateTimer = 0f;
        
        // State transition validation
        private readonly bool[,] validTransitions = new bool[4, 4]
        {
            // From:      To: Grounded, Airborne, Sliding, Transitioning
            /* Grounded */    { true,     true,     true,    true  },
            /* Airborne */    { true,     true,     false,   true  },
            /* Sliding */     { true,     true,     true,    true  },
            /* Transitioning */{ true,     true,     true,    true  }
        };
        
        // Events for state changes
        public System.Action<BallState, BallState> OnStateChanged;
        
        public BallState CurrentState => currentState;
        public BallState PreviousState => previousState;
        public float StateTimer => stateTimer;
        
        public void Update(float deltaTime)
        {
            stateTimer += deltaTime;
        }
        
        public bool TryTransitionTo(BallState newState, string reason = "")
        {
            if (!CanTransitionTo(newState))
            {
                Debug.LogWarning($"Invalid state transition: {currentState} -> {newState}. Reason: {reason}");
                return false;
            }
            
            if (currentState == newState)
                return true; // Already in target state
            
            // Execute transition
            OnStateExit(currentState);
            
            previousState = currentState;
            currentState = newState;
            stateTimer = 0f;
            
            OnStateEnter(newState);
            
            // Notify listeners
            OnStateChanged?.Invoke(previousState, currentState);
            
            Debug.Log($"State transition: {previousState} -> {currentState} ({reason})");
            return true;
        }
        
        private bool CanTransitionTo(BallState newState)
        {
            return validTransitions[(int)currentState, (int)newState];
        }
        
        private void OnStateEnter(BallState state)
        {
            switch (state)
            {
                case BallState.Grounded:
                    // Enable rolling physics
                    break;
                    
                case BallState.Airborne:
                    // Enable air drag
                    break;
                    
                case BallState.Sliding:
                    // Reduce friction
                    break;
                    
                case BallState.Transitioning:
                    // Special gravity transition handling
                    break;
            }
        }
        
        private void OnStateExit(BallState state)
        {
            // Cleanup state-specific effects
            switch (state)
            {
                case BallState.Grounded:
                    // Disable ground-specific effects
                    break;
                    
                case BallState.Airborne:
                    // Clean up air effects
                    break;
                    
                case BallState.Sliding:
                    // Reset friction
                    break;
                    
                case BallState.Transitioning:
                    // Complete gravity transition
                    break;
            }
        }
        
        public void ForceState(BallState state, string reason = "Force")
        {
            // Bypass validation for emergency state changes
            OnStateExit(currentState);
            previousState = currentState;
            currentState = state;
            stateTimer = 0f;
            OnStateEnter(state);
            
            OnStateChanged?.Invoke(previousState, currentState);
            Debug.LogWarning($"Forced state change: {previousState} -> {currentState} ({reason})");
        }
    }
}
```

### Validation Steps
1. **Transition Matrix**: Verify all valid/invalid transitions work correctly
2. **Event Firing**: Confirm state change events fire properly
3. **Timer Reset**: State timer resets on transitions
4. **Debug Logging**: State changes are logged appropriately

---

## Task 2.2: Create BallPhysics Component

### Objective
Implement the main ball physics component that integrates with Phase 1's PhysicsSettings and architecture.

### Implementation Steps
1. **Create MonoBehaviour**: Inherit from MonoBehaviour and implement IPhysicsObject
2. **Reference PhysicsSettings**: Use centralized configuration from Phase 1
3. **Implement Physics Callbacks**: Pre/during/post physics step methods
4. **Add State Integration**: Connect with BallStateMachine
5. **Implement Jump Mechanics**: Use PhysicsSettings for all calculations

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    [RequireComponent(typeof(SphereCollider))]
    public class BallPhysics : MonoBehaviour, IPhysicsObject, IInterpolatable
    {
        [Header("Configuration")]
        [SerializeField] private PhysicsSettings physicsSettings;
        
        [Header("Ball Parameters")]
        [SerializeField] private float ballRadius = 0.5f;
        [SerializeField] private float ballMass = 1f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;
        
        // IPhysicsObject interface implementation
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Acceleration { get; set; }
        public float Mass => ballMass;
        
        // Physics state
        private BallStateMachine stateMachine;
        private Vector3 previousPosition;
        private Vector3 inputDirection;
        private bool jumpRequested;
        private float jumpBufferTimer;
        private float coyoteTimer;
        
        // Ground detection
        private GroundInfo groundInfo;
        private bool wasGroundedLastFrame;
        
        // Components
        private SphereCollider sphereCollider;
        
        private void Awake()
        {
            InitializeComponents();
            LoadPhysicsSettings();
        }
        
        private void InitializeComponents()
        {
            stateMachine = new BallStateMachine();
            sphereCollider = GetComponent<SphereCollider>();
            
            // Initialize physics state
            Position = transform.position;
            Velocity = Vector3.zero;
            Acceleration = Vector3.zero;
            
            // Register with physics manager
            if (BlockBallPhysicsManager.Instance != null)
            {
                BlockBallPhysicsManager.Instance.RegisterPhysicsObject(this);
            }
        }
        
        private void LoadPhysicsSettings()
        {
            if (physicsSettings == null)
            {
                physicsSettings = BlockBallPhysicsManager.Instance?.Settings;
                if (physicsSettings == null)
                {
                    Debug.LogError("No PhysicsSettings found! Ball physics will not work correctly.");
                }
            }
        }
        
        private void OnDestroy()
        {
            // Unregister from physics manager
            if (BlockBallPhysicsManager.Instance != null)
            {
                BlockBallPhysicsManager.Instance.UnregisterPhysicsObject(this);
            }
        }
        
        // IPhysicsObject implementation
        public void PrePhysicsStep(float deltaTime)
        {
            // Store previous position for interpolation
            previousPosition = Position;
            
            // Update timers
            UpdateTimers(deltaTime);
            
            // Process input
            ProcessInput();
            
            // Update ground detection
            UpdateGroundDetection();
            
            // Update state machine
            UpdateStateMachine();
            
            // Calculate acceleration from user input
            CalculateInputAcceleration();
        }
        
        public void PhysicsStep(float deltaTime)
        {
            // Apply physics forces
            ApplyPhysicsForces();
            
            // Apply speed limiting
            ApplySpeedLimiting();
            
            // Velocity Verlet integration handled by physics manager
            VelocityVerletIntegrator.Integrate(this, deltaTime);
            
            // Update Unity transform
            transform.position = Position;
        }
        
        public void PostPhysicsStep(float deltaTime)
        {
            // Handle collisions
            ProcessCollisions();
            
            // Update visual effects
            UpdateVisualEffects();
            
            // Update state-specific behaviors
            UpdateStateSpecificBehaviors();
        }
        
        // IInterpolatable implementation
        public void Interpolate(float alpha)
        {
            // Smooth visual interpolation between physics steps
            Vector3 interpolatedPosition = Vector3.Lerp(previousPosition, Position, alpha);
            transform.position = interpolatedPosition;
        }
        
        // Input processing
        public void SetInputDirection(Vector2 input)
        {
            // Convert 2D input to 3D world direction
            inputDirection = new Vector3(input.x, 0, input.y);
        }
        
        public void RequestJump()
        {
            jumpRequested = true;
            jumpBufferTimer = physicsSettings.jumpBufferTime;
        }
        
        private void UpdateTimers(float deltaTime)
        {
            jumpBufferTimer = Mathf.Max(0, jumpBufferTimer - deltaTime);
            coyoteTimer = wasGroundedLastFrame ? physicsSettings.coyoteTime : Mathf.Max(0, coyoteTimer - deltaTime);
            
            stateMachine.Update(deltaTime);
        }
        
        private void ProcessInput()
        {
            // Process jump input
            if (jumpRequested || jumpBufferTimer > 0)
            {
                TryJump();
                jumpRequested = false;
            }
        }
        
        private void TryJump()
        {
            bool canJump = stateMachine.CurrentState == BallState.Grounded || coyoteTimer > 0;
            
            if (canJump)
            {
                // Calculate jump force using PhysicsSettings
                float jumpForce = physicsSettings.GetJumpForce(ballMass);
                Vector3 jumpVector = GetJumpDirection() * jumpForce;
                
                // Apply jump
                Velocity += jumpVector;
                
                // Update state
                stateMachine.TryTransitionTo(BallState.Airborne, "Jump");
                
                // Reset timers
                jumpBufferTimer = 0;
                coyoteTimer = 0;
                
                Debug.Log($"Jump applied: force={jumpForce:F2}, height={physicsSettings.jumpHeight:F2}");
            }
        }
        
        private Vector3 GetJumpDirection()
        {
            // For now, always jump up relative to current gravity
            // This will be extended in Phase 4 for gravity switching
            return Vector3.up;
        }
        
        private void CalculateInputAcceleration()
        {
            if (inputDirection.magnitude < 0.1f)
            {
                // No input - apply deceleration
                ApplyDeceleration();
                return;
            }
            
            // Calculate target velocity from input
            Vector3 targetVelocity = inputDirection.normalized * physicsSettings.maxRollSpeed;
            
            // Get acceleration curve from PhysicsSettings
            float currentSpeed = Velocity.magnitude;
            float acceleration = physicsSettings.GetInputAccelerationCurve(currentSpeed, targetVelocity.magnitude);
            
            // Apply state-specific modifiers
            acceleration *= GetStateAccelerationModifier();
            
            // Calculate acceleration vector
            Vector3 accelerationVector = inputDirection.normalized * acceleration;
            
            // Add to total acceleration
            Acceleration += accelerationVector;
        }
        
        private void ApplyDeceleration()
        {
            float deceleration = physicsSettings.GetRollAcceleration() * GetFrictionMultiplier();
            Vector3 decelerationVector = -Velocity.normalized * deceleration;
            
            // Don't decelerate below zero
            if (decelerationVector.magnitude * Time.fixedDeltaTime > Velocity.magnitude)
            {
                Velocity = Vector3.zero;
            }
            else
            {
                Acceleration += decelerationVector;
            }
        }
        
        private float GetStateAccelerationModifier()
        {
            switch (stateMachine.CurrentState)
            {
                case BallState.Grounded:
                    return 1f;
                    
                case BallState.Airborne:
                    return physicsSettings.airControl;
                    
                case BallState.Sliding:
                    return 0.5f; // Reduced control on slopes
                    
                case BallState.Transitioning:
                    return 0.1f; // Minimal control during gravity transitions
                    
                default:
                    return 1f;
            }
        }
        
        private float GetFrictionMultiplier()
        {
            switch (stateMachine.CurrentState)
            {
                case BallState.Grounded:
                    return physicsSettings.friction;
                    
                case BallState.Airborne:
                    return 0.1f; // Minimal air resistance
                    
                case BallState.Sliding:
                    return physicsSettings.friction * 0.5f; // Reduced friction on slopes
                    
                default:
                    return physicsSettings.friction;
            }
        }
        
        private void ApplyPhysicsForces()
        {
            // Apply gravity (will be extended in Phase 4)
            Acceleration += Vector3.down * physicsSettings.gravityStrength;
            
            // Apply drag based on state
            ApplyDrag();
        }
        
        private void ApplyDrag()
        {
            float dragCoefficient = 1f;
            
            switch (stateMachine.CurrentState)
            {
                case BallState.Airborne:
                    dragCoefficient = 0.98f; // Light air resistance
                    break;
                    
                case BallState.Grounded:
                    dragCoefficient = 0.95f; // Rolling resistance
                    break;
                    
                case BallState.Sliding:
                    dragCoefficient = 0.92f; // Sliding friction
                    break;
            }
            
            Velocity *= dragCoefficient;
        }
        
        private void ApplySpeedLimiting()
        {
            float currentSpeed = Velocity.magnitude;
            
            // Apply hierarchical speed limits from PhysicsSettings
            if (currentSpeed > physicsSettings.totalSpeedLimit)
            {
                Velocity = Velocity.normalized * physicsSettings.ApplySpeedDecay(currentSpeed, physicsSettings.totalSpeedLimit);
            }
            else if (currentSpeed > physicsSettings.physicsSpeedLimit)
            {
                Velocity = Velocity.normalized * physicsSettings.ApplySpeedDecay(currentSpeed, physicsSettings.physicsSpeedLimit);
            }
        }
        
        private void UpdateGroundDetection()
        {
            // Store previous state
            wasGroundedLastFrame = stateMachine.CurrentState == BallState.Grounded;
            
            // Raycast downward to detect ground
            Vector3 rayStart = Position;
            Vector3 rayDirection = Vector3.down;
            float rayDistance = ballRadius + 0.1f; // Slightly beyond ball radius
            
            if (Physics.Raycast(rayStart, rayDirection, out RaycastHit hit, rayDistance))
            {
                groundInfo = new GroundInfo
                {
                    isGrounded = true,
                    groundNormal = hit.normal,
                    groundDistance = hit.distance - ballRadius,
                    slopeAngle = Vector3.Angle(Vector3.up, hit.normal)
                };
            }
            else
            {
                groundInfo = new GroundInfo
                {
                    isGrounded = false,
                    groundNormal = Vector3.up,
                    groundDistance = float.MaxValue,
                    slopeAngle = 0f
                };
            }
        }
        
        private void UpdateStateMachine()
        {
            BallState targetState = DetermineTargetState();
            stateMachine.TryTransitionTo(targetState, "Physics Update");
        }
        
        private BallState DetermineTargetState()
        {
            if (!groundInfo.isGrounded)
            {
                return BallState.Airborne;
            }
            
            if (groundInfo.slopeAngle > 45f) // From PhysicsSettings (could be configurable)
            {
                return BallState.Sliding;
            }
            
            return BallState.Grounded;
        }
        
        // ... rest of implementation methods ...
        
        private void OnGUI()
        {
            if (showDebugInfo)
            {
                GUILayout.BeginArea(new Rect(10, 10, 300, 200));
                GUILayout.Label($"State: {stateMachine.CurrentState}");
                GUILayout.Label($"Speed: {Velocity.magnitude:F2}/{physicsSettings.maxRollSpeed:F2}");
                GUILayout.Label($"Grounded: {groundInfo.isGrounded}");
                GUILayout.Label($"Slope: {groundInfo.slopeAngle:F1}°");
                GUILayout.Label($"Jump Buffer: {jumpBufferTimer:F2}s");
                GUILayout.Label($"Coyote Time: {coyoteTimer:F2}s");
                GUILayout.EndArea();
            }
        }
    }
    
    // Ground detection data structure
    public struct GroundInfo
    {
        public bool isGrounded;
        public Vector3 groundNormal;
        public float groundDistance;
        public float slopeAngle;
    }
}
```

### Validation Steps
1. **PhysicsSettings Integration**: Verify all values come from centralized settings
2. **Physics Integration**: Confirm integration with Phase 1 physics manager
3. **State Transitions**: Test all state machine transitions work correctly
4. **Jump Mechanics**: Verify jump height matches PhysicsSettings configuration
5. **Speed Limiting**: Confirm hierarchical speed limits work as expected

---

## Task 2.3: Create GroundDetector

### Objective
Implement reliable ground detection that works with various surface types and slopes.

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    public class GroundDetector
    {
        private const float GROUND_THRESHOLD = 0.7f; // Dot product threshold for ground detection
        private const float MAX_GROUND_DISTANCE = 0.6f; // Slightly larger than ball radius
        
        public GroundInfo DetectGround(Vector3 ballPosition, float ballRadius, Vector3 gravityDirection)
        {
            GroundInfo result = new GroundInfo
            {
                isGrounded = false,
                groundNormal = Vector3.up,
                groundDistance = float.MaxValue,
                slopeAngle = 0f
            };
            
            // Perform spherecast downward
            Vector3 castDirection = gravityDirection.normalized;
            float castDistance = ballRadius + MAX_GROUND_DISTANCE;
            
            RaycastHit hit;
            if (Physics.SphereCast(ballPosition, ballRadius * 0.9f, castDirection, out hit, castDistance))
            {
                // Check if surface is ground-like (not wall/ceiling)
                float dot = Vector3.Dot(hit.normal, -castDirection);
                if (dot > GROUND_THRESHOLD)
                {
                    result.isGrounded = true;
                    result.groundNormal = hit.normal;
                    result.groundDistance = hit.distance;
                    result.slopeAngle = Vector3.Angle(hit.normal, -castDirection);
                }
            }
            
            return result;
        }
    }
}
```

## Documentation Requirements for Each Task
1. **XML Documentation**: Complete API documentation for all public members
2. **Usage Examples**: How to configure and use each component
3. **Integration Notes**: How components work together
4. **Performance Considerations**: Memory usage and optimization tips
5. **Troubleshooting**: Common issues and their solutions
