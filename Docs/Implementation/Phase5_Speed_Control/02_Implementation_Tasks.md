# Phase 5: Speed Control System Implementation Tasks

## Task 5.1: Update PhysicsSettings with Speed Control

### Objective
Add three-tier speed control configuration to the centralized PhysicsSettings.

### Code Template
```csharp
// Add to existing PhysicsSettings.cs
[Header("Speed Control System")]
[Tooltip("Maximum input speed (player control)")]
[Range(3f, 10f)]
public float inputSpeedLimit = 6f;

[Tooltip("Maximum physics calculation speed")]
[Range(4f, 12f)]
public float physicsSpeedLimit = 6.5f;

[Tooltip("Absolute maximum total speed")]
[Range(5f, 15f)]
public float totalSpeedLimit = 7f;

[Tooltip("Exponential decay rate when exceeding 95% of total limit")]
[Range(0.5f, 5f)]
public float speedDecayRate = 2f;

// Calculated properties
public float ExponentialDecayThreshold => totalSpeedLimit * 0.95f; // 6.65f
public float MinimumSpeedThreshold => 0.01f;
```

### Validation Steps
1. Update existing PhysicsSettings asset
2. Verify all slider ranges work correctly
3. Confirm calculated properties return correct values

---

## Task 5.2: Create SpeedController Component

### Objective
Implement the core three-tier speed control system with exponential decay.

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    /// <summary>
    /// Three-tier speed control: Input (6), Physics (6.5), Total (7) with exponential decay at 6.65
    /// </summary>
    public class SpeedController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PhysicsSettings physicsSettings;
        [SerializeField] private Rigidbody ballRigidbody;
        
        private Vector3 cachedVelocity;
        
        void Start()
        {
            if (physicsSettings == null)
                physicsSettings = Resources.Load<PhysicsSettings>("PhysicsSettings");
        }
        
        void FixedUpdate()
        {
            ApplySpeedControl();
        }
        
        private void ApplySpeedControl()
        {
            cachedVelocity = ballRigidbody.velocity;
            float currentSpeed = cachedVelocity.magnitude;
            
            if (currentSpeed < physicsSettings.MinimumSpeedThreshold)
                return;
            
            float controlledSpeed = ApplyThreeTierControl(currentSpeed);
            
            if (controlledSpeed != currentSpeed)
            {
                ballRigidbody.velocity = cachedVelocity.normalized * controlledSpeed;
            }
        }
        
        private float ApplyThreeTierControl(float currentSpeed)
        {
            // Tier 3: Check for exponential decay (95% of total = 6.65)
            if (currentSpeed > physicsSettings.ExponentialDecayThreshold)
            {
                return ApplyExponentialDecay(currentSpeed);
            }
            
            // Tier 2: Hard limit at total speed (7.0)
            if (currentSpeed > physicsSettings.totalSpeedLimit)
            {
                return physicsSettings.totalSpeedLimit;
            }
            
            return currentSpeed; // No control needed
        }
        
        private float ApplyExponentialDecay(float speed)
        {
            // Exponential decay: speed *= exp(-rate * deltaTime)
            float decayFactor = Mathf.Exp(-physicsSettings.speedDecayRate * Time.fixedDeltaTime);
            return speed * decayFactor;
        }
        
        // Public methods for input limiting
        public Vector3 LimitInputVelocity(Vector3 inputVelocity)
        {
            float inputSpeed = inputVelocity.magnitude;
            if (inputSpeed > physicsSettings.inputSpeedLimit)
            {
                return inputVelocity.normalized * physicsSettings.inputSpeedLimit;
            }
            return inputVelocity;
        }
        
        public Vector3 LimitPhysicsVelocity(Vector3 physicsVelocity)
        {
            float physicsSpeed = physicsVelocity.magnitude;
            if (physicsSpeed > physicsSettings.physicsSpeedLimit)
            {
                return physicsVelocity.normalized * physicsSettings.physicsSpeedLimit;
            }
            return physicsVelocity;
        }
    }
}
```

### Validation Steps
1. Add component to Player GameObject
2. Test three-tier speed limiting
3. Verify exponential decay at 6.65 u/s threshold
4. Confirm no frame rate dependency

---

## Task 5.3: Integration with Ball Physics

### Objective
Integrate speed controller with existing BallPhysics component.

### Implementation Steps

#### Step 1: Update BallPhysics.cs
Add speed control integration:

```csharp
// Add to existing BallPhysics.cs
[Header("Speed Control")]
[SerializeField] private SpeedController speedController;

void Start()
{
    // ... existing code ...
    if (speedController == null)
        speedController = GetComponent<SpeedController>();
}

// Modify input processing method
private void ProcessInput()
{
    Vector3 inputVelocity = CalculateInputVelocity();
    
    // Apply input speed limiting (6 u/s)
    if (speedController != null)
        inputVelocity = speedController.LimitInputVelocity(inputVelocity);
    
    ApplyInputVelocity(inputVelocity);
}

// Modify physics force application
private void ApplyPhysicsForces()
{
    Vector3 physicsForce = CalculatePhysicsForces();
    Vector3 physicsVelocity = physicsForce / rigidbody.mass;
    
    // Apply physics speed limiting (6.5 u/s)
    if (speedController != null)
        physicsVelocity = speedController.LimitPhysicsVelocity(physicsVelocity);
    
    rigidbody.AddForce(physicsVelocity * rigidbody.mass, ForceMode.Force);
}
```

### Validation Steps
1. Test input speed limited to 6 u/s
2. Verify physics calculations don't exceed 6.5 u/s
3. Confirm total speed control works with ball physics

---

## Task 5.4: Create Debug Visualization

### Objective
Add debug tools for monitoring speed control system.

### Code Template
```csharp
#if UNITY_EDITOR
using UnityEngine;

namespace BlockBall.Physics
{
    public class SpeedDebugger : MonoBehaviour
    {
        [Header("Debug Settings")]
        public bool showSpeedInfo = true;
        public bool showSpeedLimits = true;
        
        private Rigidbody ballRigidbody;
        private PhysicsSettings settings;
        
        void Start()
        {
            ballRigidbody = GetComponent<Rigidbody>();
            settings = Resources.Load<PhysicsSettings>("PhysicsSettings");
        }
        
        void OnGUI()
        {
            if (!showSpeedInfo) return;
            
            float currentSpeed = ballRigidbody.velocity.magnitude;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 150));
            GUILayout.Label("Speed Control Debug", GUI.skin.box);
            GUILayout.Label($"Current Speed: {currentSpeed:F2} u/s");
            GUILayout.Label($"Input Limit: {settings.inputSpeedLimit:F1} u/s");
            GUILayout.Label($"Physics Limit: {settings.physicsSpeedLimit:F1} u/s");
            GUILayout.Label($"Total Limit: {settings.totalSpeedLimit:F1} u/s");
            GUILayout.Label($"Decay Threshold: {settings.ExponentialDecayThreshold:F2} u/s");
            
            // Color coding for speed status
            if (currentSpeed > settings.ExponentialDecayThreshold)
                GUI.color = Color.red;
            else if (currentSpeed > settings.physicsSpeedLimit)
                GUI.color = Color.yellow;
            else
                GUI.color = Color.green;
                
            GUILayout.Label($"Status: {GetSpeedStatus(currentSpeed)}");
            GUI.color = Color.white;
            
            GUILayout.EndArea();
        }
        
        private string GetSpeedStatus(float speed)
        {
            if (speed > settings.ExponentialDecayThreshold)
                return "EXPONENTIAL DECAY";
            if (speed > settings.totalSpeedLimit)
                return "TOTAL LIMIT";
            if (speed > settings.physicsSpeedLimit)
                return "PHYSICS LIMIT";
            if (speed > settings.inputSpeedLimit)
                return "INPUT LIMIT";
            return "NORMAL";
        }
    }
}
#endif
```

### Validation Steps
1. Add to Player GameObject
2. Test debug display shows correct speed information
3. Verify color coding works for different speed ranges

---

## Integration Testing Checklist

### Basic Functionality
- [ ] Input velocity limited to 6 u/s
- [ ] Physics velocity limited to 6.5 u/s  
- [ ] Total velocity limited to 7 u/s
- [ ] Exponential decay starts at 6.65 u/s
- [ ] Speed control works at 50Hz fixed timestep

### Performance
- [ ] Zero allocation during speed control
- [ ] Processing time <0.2ms per frame
- [ ] No frame rate dependencies

### Integration
- [ ] Works with gravity system
- [ ] Integrates with ball physics component
- [ ] Compatible with collision system
- [ ] Debug visualization functions correctly

## Completion Criteria

Phase 5 is complete when:
1. All three speed tiers function correctly
2. Exponential decay activates precisely at 6.65 u/s
3. All speed values are configurable via PhysicsSettings
4. Performance targets are met consistently
5. Integration tests pass 100%

The three-tier speed control system must be deterministic and consistent across all platforms.
