# BlockBall Evolution - Physics Implementation Task Specifications

## Overview

This document provides detailed implementation tasks for the custom physics system in BlockBall Evolution, targeting Unity 2022.3 LTS with Velocity Verlet integration. Each task includes specific deliverables, acceptance criteria, and implementation guidance.

---

## Task 1: Core Physics Architecture & Integration System

### Objective
Implement the foundational physics architecture with Velocity Verlet integration and fixed timestep simulation.

### Deliverables

#### 1.1 Custom Physics Manager
```csharp
public class BlockBallPhysicsManager : MonoBehaviour
{
    public float FixedTimestep = 0.02f; // 50Hz
    public int MaxSubsteps = 8;
    
    private List<IPhysicsObject> physicsObjects;
    private float accumulator = 0f;
    
    // Velocity Verlet integration
    // Debug visualization system
    // Performance profiling hooks
}
```

#### 1.2 Physics Object Interface
```csharp
public interface IPhysicsObject
{
    Vector3 Position { get; set; }
    Vector3 Velocity { get; set; }
    Vector3 Acceleration { get; set; }
    float Mass { get; }
    
    void PrePhysicsStep(float deltaTime);
    void PhysicsStep(float deltaTime);
    void PostPhysicsStep(float deltaTime);
}
```

#### 1.3 Velocity Verlet Integrator
```csharp
public static class VelocityVerletIntegrator
{
    public static void Integrate(IPhysicsObject obj, float deltaTime)
    {
        // x(t+dt) = x(t) + v(t)*dt + 0.5*a(t)*dt²
        Vector3 newPosition = obj.Position + obj.Velocity * deltaTime + 
                             0.5f * obj.Acceleration * deltaTime * deltaTime;
        
        // Calculate new acceleration at new position
        Vector3 newAcceleration = CalculateAcceleration(obj, newPosition);
        
        // v(t+dt) = v(t) + 0.5*(a(t) + a(t+dt))*dt
        Vector3 newVelocity = obj.Velocity + 
                             0.5f * (obj.Acceleration + newAcceleration) * deltaTime;
        
        obj.Position = newPosition;
        obj.Velocity = newVelocity;
        obj.Acceleration = newAcceleration;
    }
}
```

### Acceptance Criteria
- [ ] Physics runs at consistent 50Hz regardless of framerate
- [ ] Velocity Verlet integration maintains energy conservation within 1% over 10 seconds
- [ ] System handles 1-8 substeps gracefully under frame drops
- [ ] Debug profiler shows physics timing breakdown
- [ ] Memory allocation during physics step is zero (object pooling)

### Implementation Notes
- Use Unity's `FixedUpdate()` for accumulator pattern
- Implement object pooling for temporary calculation vectors
- Add conditional compilation flags for debug features
- Profile memory allocation in physics hot path

---

## Task 2: Ball Physics Implementation

### Objective
Implement the core ball physics with rolling, jumping, and state management.

### Deliverables

#### 2.1 Ball State Machine
```csharp
public enum BallState
{
    Grounded,      // Rolling on surface
    Airborne,      // In flight
    Sliding,       // On steep slope
    Transitioning  // Gravity switch active
}

public class BallStateMachine
{
    private BallState currentState;
    private float stateTimer;
    
    public void UpdateState(BallPhysics ball, float deltaTime);
    public bool CanTransitionTo(BallState newState);
    public void OnStateEnter(BallState state);
    public void OnStateExit(BallState state);
}
```

#### 2.2 Ball Physics Component
```csharp
public class BallPhysics : MonoBehaviour, IPhysicsObject
{
    [Header("Movement Parameters")]
    public float MaxInputSpeed = 6f;
    public float MaxPhysicsSpeed = 7f;
    public float MaxTotalSpeed = 8f;
    public AnimationCurve AccelerationCurve;
    
    [Header("Jump Parameters")]
    public float JumpHeight = 0.75f; // 6 Bixels = 0.75 Unity units
    public float JumpBufferTime = 0.1f;
    public float CoyoteTime = 0.15f;
    
    [Header("Rolling Parameters")]
    public float RollingFriction = 0.8f;
    public float SlidingFriction = 0.3f;
    public float AirDrag = 0.95f;
    
    private BallStateMachine stateMachine;
    private Vector3 customGravity;
    private float jumpBufferTimer;
    private float coyoteTimer;
    private bool isGrounded;
    
    // Physics state
    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }
    public Vector3 Acceleration { get; set; }
    public float Mass => 1f;
}
```

#### 2.3 Input Processing System
```csharp
public class BallInputProcessor
{
    public Vector2 ProcessMovementInput(Vector2 rawInput, Transform cameraTransform, Vector3 gravityDirection)
    {
        // Project camera forward/right onto gravity-perpendicular plane
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        
        // Remove gravity component from camera vectors
        Vector3 projectedForward = Vector3.ProjectOnPlane(cameraForward, gravityDirection).normalized;
        Vector3 projectedRight = Vector3.ProjectOnPlane(cameraRight, gravityDirection).normalized;
        
        // Calculate movement direction
        Vector3 moveDirection = projectedForward * rawInput.y + projectedRight * rawInput.x;
        
        // Normalize diagonal movement
        if (moveDirection.magnitude > 1f)
            moveDirection = moveDirection.normalized;
            
        return new Vector2(moveDirection.x, moveDirection.z);
    }
    
    public bool ProcessJumpInput(bool jumpPressed, float jumpBufferTimer, bool isGrounded, float coyoteTimer)
    {
        return jumpPressed && (isGrounded || coyoteTimer > 0f || jumpBufferTimer > 0f);
    }
}
```

### Acceptance Criteria
- [ ] Ball rolls smoothly across flat surfaces without jitter
- [ ] Jump height is exactly 6 Bixels (0.75 Unity units) consistently
- [ ] Jump buffering works within 0.1s window before/after ground contact
- [ ] Coyote time allows jumping 0.15s after leaving ground
- [ ] Diagonal movement maintains consistent speed (normalized)
- [ ] Ball responds to input relative to camera orientation
- [ ] State transitions are smooth and predictable
- [ ] Rolling friction prevents infinite sliding

### Implementation Notes
- Use sphere collider with radius 0.5 for ball
- Implement rolling constraint: angular velocity = linear velocity / radius
- Store previous frame's grounded state for coyote time
- Use Unity's Input System for consistent input handling

---

## Task 3: Collision Detection & Response System

### Objective
Implement robust collision detection and response with proper contact handling.

### Deliverables

#### 3.1 Custom Collision System
```csharp
public class CollisionDetector
{
    public struct ContactPoint
    {
        public Vector3 point;
        public Vector3 normal;
        public float penetration;
        public Collider colliderA;
        public Collider colliderB;
        public float restitution;
    }
    
    public List<ContactPoint> DetectCollisions(SphereCollider ball, Collider[] worldColliders)
    {
        List<ContactPoint> contacts = new List<ContactPoint>();
        
        foreach (var collider in worldColliders)
        {
            if (collider is BoxCollider box)
                DetectSphereBoxCollision(ball, box, contacts);
            // Add other collider types as needed
        }
        
        return contacts;
    }
    
    private void DetectSphereBoxCollision(SphereCollider sphere, BoxCollider box, List<ContactPoint> contacts)
    {
        // Implement sphere-box collision detection
        // Calculate closest point on box to sphere center
        // Determine penetration depth and contact normal
        // Add contact point if collision detected
    }
}
```

#### 3.2 Collision Response System
```csharp
public class CollisionResolver
{
    public void ResolveCollisions(List<CollisionDetector.ContactPoint> contacts, BallPhysics ball)
    {
        foreach (var contact in contacts)
        {
            // Position correction (prevent sinking)
            CorrectPosition(ball, contact);
            
            // Velocity response (bouncing)
            ResolveVelocity(ball, contact);
        }
    }
    
    private void CorrectPosition(BallPhysics ball, CollisionDetector.ContactPoint contact)
    {
        // Move ball out of collision by penetration distance
        float correctionAmount = contact.penetration + 0.001f; // Small epsilon
        ball.Position += contact.normal * correctionAmount;
    }
    
    private void ResolveVelocity(BallPhysics ball, CollisionDetector.ContactPoint contact)
    {
        // Calculate relative velocity
        Vector3 relativeVelocity = ball.Velocity;
        float velocityAlongNormal = Vector3.Dot(relativeVelocity, contact.normal);
        
        // Don't resolve if velocities are separating
        if (velocityAlongNormal > 0) return;
        
        // Calculate bounce response
        float restitution = contact.restitution;
        float impulseScalar = -(1 + restitution) * velocityAlongNormal;
        
        // Apply impulse
        Vector3 impulse = impulseScalar * contact.normal;
        ball.Velocity += impulse;
    }
}
```

#### 3.3 Ground Detection System
```csharp
public class GroundDetector
{
    private const float GroundThreshold = 0.7f; // Dot product threshold
    private const float GroundDistance = 0.51f; // Slightly larger than ball radius
    
    public bool IsGrounded(BallPhysics ball, List<CollisionDetector.ContactPoint> contacts, Vector3 gravityDirection)
    {
        foreach (var contact in contacts)
        {
            // Check if contact normal is opposite to gravity (ground-like)
            float dot = Vector3.Dot(contact.normal, -gravityDirection);
            if (dot > GroundThreshold)
            {
                // Verify ball is close enough to surface
                float distanceToSurface = Vector3.Distance(ball.Position, contact.point);
                if (distanceToSurface <= GroundDistance)
                    return true;
            }
        }
        return false;
    }
    
    public Vector3 GetGroundNormal(List<CollisionDetector.ContactPoint> contacts, Vector3 gravityDirection)
    {
        Vector3 averageNormal = Vector3.zero;
        int groundContacts = 0;
        
        foreach (var contact in contacts)
        {
            float dot = Vector3.Dot(contact.normal, -gravityDirection);
            if (dot > GroundThreshold)
            {
                averageNormal += contact.normal;
                groundContacts++;
            }
        }
        
        return groundContacts > 0 ? (averageNormal / groundContacts).normalized : Vector3.up;
    }
}
```

### Acceptance Criteria
- [ ] Ball never penetrates block surfaces
- [ ] Collision response feels natural and consistent
- [ ] Ground detection works reliably on slopes up to 45°
- [ ] Ball doesn't stick to walls or ceilings
- [ ] Multiple simultaneous collisions are handled correctly
- [ ] Bounce behavior varies appropriately with impact angle
- [ ] No collision tunneling at high speeds
- [ ] Performance maintains 50Hz with 100+ collision checks

### Implementation Notes
- Use Unity's Physics.OverlapSphere for broad-phase collision detection
- Implement continuous collision detection for high-speed movement
- Cache collision results to avoid redundant calculations
- Use collision layers to filter relevant colliders

---

## Task 4: Gravity System Implementation

### Objective
Implement custom gravity with smooth transitions and multi-zone handling.

### Deliverables

#### 4.1 Gravity Manager
```csharp
public class GravityManager : MonoBehaviour
{
    public static GravityManager Instance { get; private set; }
    
    [Header("Gravity Settings")]
    public float GravityStrength = 9.81f;
    public Vector3 DefaultGravityDirection = Vector3.down;
    
    private List<GravityZone> activeZones = new List<GravityZone>();
    
    public Vector3 CalculateGravityForPosition(Vector3 position, BallPhysics ball)
    {
        GravityZone closestZone = FindClosestGravityZone(position);
        
        if (closestZone == null)
            return DefaultGravityDirection * GravityStrength;
            
        return closestZone.GetGravityAtPosition(position) * GravityStrength;
    }
    
    private GravityZone FindClosestGravityZone(Vector3 position)
    {
        GravityZone closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (var zone in activeZones)
        {
            if (zone.ContainsPoint(position))
            {
                float distance = Vector3.Distance(position, zone.PivotPoint);
                if (distance < closestDistance)
                {
                    closest = zone;
                    closestDistance = distance;
                }
            }
        }
        
        return closest;
    }
}
```

#### 4.2 Gravity Zone Component
```csharp
public class GravityZone : MonoBehaviour
{
    [Header("Zone Configuration")]
    public Vector3 GravityDirection = Vector3.down;
    public Vector3 PivotPoint;
    public float TransitionDuration = 0.3f;
    public AnimationCurve TransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Zone Shape")]
    public BoxCollider ZoneCollider;
    
    private Dictionary<BallPhysics, GravityTransition> activeBalls = new Dictionary<BallPhysics, GravityTransition>();
    
    public bool ContainsPoint(Vector3 point)
    {
        return ZoneCollider.bounds.Contains(point);
    }
    
    public Vector3 GetGravityAtPosition(Vector3 position)
    {
        // Return the target gravity direction for this zone
        return GravityDirection.normalized;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var ball = other.GetComponent<BallPhysics>();
        if (ball != null)
        {
            StartGravityTransition(ball);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        var ball = other.GetComponent<BallPhysics>();
        if (ball != null)
        {
            SnapGravityToAxis(ball);
        }
    }
    
    private void StartGravityTransition(BallPhysics ball)
    {
        if (!activeBalls.ContainsKey(ball))
        {
            activeBalls[ball] = new GravityTransition
            {
                StartDirection = ball.CurrentGravityDirection,
                TargetDirection = GravityDirection.normalized,
                StartTime = Time.time,
                Duration = TransitionDuration
            };
        }
    }
    
    private void SnapGravityToAxis(BallPhysics ball)
    {
        if (activeBalls.ContainsKey(ball))
        {
            // Snap to nearest axis
            Vector3 currentGravity = ball.CurrentGravityDirection;
            Vector3 snappedGravity = SnapToNearestAxis(currentGravity);
            ball.SetGravityDirection(snappedGravity);
            
            activeBalls.Remove(ball);
        }
    }
    
    private Vector3 SnapToNearestAxis(Vector3 direction)
    {
        Vector3 absDir = new Vector3(Mathf.Abs(direction.x), Mathf.Abs(direction.y), Mathf.Abs(direction.z));
        
        if (absDir.x >= absDir.y && absDir.x >= absDir.z)
            return new Vector3(Mathf.Sign(direction.x), 0, 0);
        else if (absDir.y >= absDir.z)
            return new Vector3(0, Mathf.Sign(direction.y), 0);
        else
            return new Vector3(0, 0, Mathf.Sign(direction.z));
    }
}

public struct GravityTransition
{
    public Vector3 StartDirection;
    public Vector3 TargetDirection;
    public float StartTime;
    public float Duration;
}
```

### Acceptance Criteria
- [ ] Gravity transitions smoothly over 0.3 seconds when entering zones
- [ ] Gravity snaps to nearest axis when leaving zones
- [ ] Multiple overlapping zones use closest pivot point rule
- [ ] Ball maintains momentum during gravity transitions
- [ ] Gravity direction is always normalized
- [ ] System handles rapid zone entry/exit without glitches
- [ ] Visual gravity direction indicator updates smoothly
- [ ] Performance impact is minimal (< 0.1ms per frame)

### Implementation Notes
- Use Unity's Trigger colliders for gravity zones
- Implement gravity transition as separate system from physics integration
- Cache gravity calculations to avoid redundant computations
- Add debug visualization for gravity zones and transitions

---

## Task 5: Speed Control & Exponential Decay System

### Objective
Implement sophisticated speed limiting with exponential decay and proper input/physics speed separation.

### Deliverables

#### 5.1 Speed Controller
```csharp
public class SpeedController
{
    [Header("Speed Limits")]
    public float MaxInputSpeed = 6f;        // Player input limit
    public float MaxPhysicsSpeed = 7f;      // Physics forces limit  
    public float MaxTotalSpeed = 8f;        // Absolute maximum
    
    [Header("Decay Parameters")]
    public float DecayRate = 5f;            // Exponential decay rate
    public float DecayThreshold = 0.95f;    // Start decay at 95% of limit
    
    public Vector3 ApplySpeedLimits(Vector3 velocity, Vector3 inputVelocity, Vector3 physicsVelocity)
    {
        // Separate input and physics components
        Vector3 limitedInput = LimitInputSpeed(inputVelocity);
        Vector3 limitedPhysics = LimitPhysicsSpeed(physicsVelocity);
        
        // Combine and apply total limit
        Vector3 combinedVelocity = limitedInput + limitedPhysics;
        return ApplyTotalSpeedLimit(combinedVelocity);
    }
    
    private Vector3 LimitInputSpeed(Vector3 inputVelocity)
    {
        float magnitude = inputVelocity.magnitude;
        if (magnitude <= MaxInputSpeed)
            return inputVelocity;
            
        return inputVelocity.normalized * MaxInputSpeed;
    }
    
    private Vector3 LimitPhysicsSpeed(Vector3 physicsVelocity)
    {
        float magnitude = physicsVelocity.magnitude;
        if (magnitude <= MaxPhysicsSpeed)
            return physicsVelocity;
            
        return physicsVelocity.normalized * MaxPhysicsSpeed;
    }
    
    private Vector3 ApplyTotalSpeedLimit(Vector3 velocity)
    {
        float magnitude = velocity.magnitude;
        
        if (magnitude <= MaxTotalSpeed * DecayThreshold)
            return velocity;
            
        if (magnitude <= MaxTotalSpeed)
        {
            // Apply exponential decay in the threshold zone
            float excessRatio = (magnitude - MaxTotalSpeed * DecayThreshold) / 
                               (MaxTotalSpeed * (1f - DecayThreshold));
            float decayFactor = Mathf.Exp(-DecayRate * excessRatio);
            float targetMagnitude = Mathf.Lerp(MaxTotalSpeed * DecayThreshold, MaxTotalSpeed, decayFactor);
            return velocity.normalized * targetMagnitude;
        }
        else
        {
            // Hard clamp above maximum
            return velocity.normalized * MaxTotalSpeed;
        }
    }
}
```

### Acceptance Criteria
- [ ] Input speed never exceeds 6 units/sec
- [ ] Physics speed never exceeds 7 units/sec  
- [ ] Total speed never exceeds 8 units/sec
- [ ] Exponential decay starts at 95% of total limit (7.6 units/sec)
- [ ] Speed transitions feel smooth and natural
- [ ] Acceleration curve provides good game feel
- [ ] System handles rapid input direction changes
- [ ] Performance impact is negligible

### Implementation Notes
- Update speed control every physics step
- Use separate tracking for input vs physics velocity components
- Implement smooth transitions between acceleration and deceleration
- Add debug visualization for speed components

---

## Integration Timeline

### Phase 1 (Week 1-2): Core Architecture
- Task 1: Physics Manager & Velocity Verlet Integration
- Basic debug framework setup

### Phase 2 (Week 3-4): Ball Physics
- Task 2: Ball Physics Implementation
- Task 5: Speed Control System

### Phase 3 (Week 5-6): Collision & Gravity
- Task 3: Collision Detection & Response
- Task 4: Gravity System

### Phase 4 (Week 7): Testing & Polish
- Comprehensive testing framework
- Performance optimization
- Bug fixes and refinement

## Success Metrics

- **Performance**: Maintain 50Hz physics with <2ms frame time
- **Consistency**: Jump height variance <0.5%
- **Responsiveness**: Input lag <50ms
- **Stability**: No physics glitches in 1-hour stress test
- **Determinism**: Identical replay results across 100 runs
