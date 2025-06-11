# Phase 3: Task 3.3 - Create CollisionResponse System

**IMPORTANT:** Phase 3 integrates with Phase 1's centralized `PhysicsSettings` - all collision material properties come from this single source of truth!

### Objective
Implement deterministic collision response algorithms that handle bounce, friction, and penetration correction.

### Implementation Steps
1. **Create CollisionResponse Class**: Define static methods for response calculations
2. **Implement Bounce Logic**: Use restitution based on material and contact angle
3. **Add Friction Handling**: Apply friction based on material properties
4. **Penetration Correction**: Resolve overlaps with iterative correction

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    /// <summary>
    /// Handles deterministic collision response calculations using PhysicsSettings
    /// </summary>
    public static class CollisionResponse
    {
        private static float PENETRATION_SLOP => PhysicsSettings.Instance.penetrationSlop;
        private static float PENETRATION_CORRECTION_PERCENT => PhysicsSettings.Instance.penetrationCorrectionPercent;
        private static int MAX_ITERATIONS => PhysicsSettings.Instance.maxCollisionIterations;
        private static float MIN_SEPARATION_VELOCITY => PhysicsSettings.Instance.minSeparationVelocity;
        private static float MIN_BOUNCE_VELOCITY => PhysicsSettings.Instance.minBounceVelocity;
        private static float MAX_BOUNCE_VELOCITY => PhysicsSettings.Instance.maxBounceVelocity;
        private static float ANGULAR_VELOCITY_FACTOR => PhysicsSettings.Instance.angularVelocityFactor;
        private static float BALL_RADIUS => PhysicsSettings.Instance.ballRadius;
        private static float BALL_MASS => PhysicsSettings.Instance.ballMass;
        
        /// <summary>
        /// Resolve a single contact between ball and environment
        /// </summary>
        public static CollisionResolution ResolveContact(ContactPoint contact, Vector3 currentVelocity, Vector3 currentAngularVelocity)
        {
            CollisionResolution resolution = new CollisionResolution();
            
            // Handle penetration correction first
            Vector3 positionCorrection = ResolvePenetration(contact);
            resolution.positionCorrection += positionCorrection;
            
            // Handle launch pad if applicable
            Vector3 launchForce;
            if (IsLaunchPadContact(contact, out launchForce))
            {
                resolution.velocityChange = launchForce / BALL_MASS;
                resolution.wasLaunchPad = true;
                return resolution;
            }
            
            // Handle bounce and friction only if significant contact
            if (Mathf.Abs(contact.normalVelocity) > MIN_SEPARATION_VELOCITY)
            {
                // Bounce response (normal direction)
                Vector3 bounceResponse = CalculateBounceResponse(contact);
                resolution.velocityChange += bounceResponse;
                
                // Friction response (tangential direction)
                Vector3 frictionResponse = CalculateFrictionResponse(contact);
                resolution.velocityChange += frictionResponse;
                
                // Angular velocity from friction
                Vector3 angularChange = CalculateAngularVelocityChange(contact, frictionResponse);
                resolution.angularVelocityChange += angularChange;
            }
            
            return resolution;
        }
        
        /// <summary>
        /// Resolve multiple contacts iteratively
        /// </summary>
        public static CollisionResolution ResolveMultipleContacts(ContactPoint[] contacts, Vector3 currentVelocity, Vector3 currentAngularVelocity)
        {
            if (contacts == null || contacts.Length == 0)
                return new CollisionResolution();
            
            CollisionResolution totalResolution = new CollisionResolution();
            
            // Sort contacts by priority (ground first, then walls, then ceiling)
            System.Array.Sort(contacts, CompareContactPriority);
            
            // Iterative resolution for stability
            for (int iteration = 0; iteration < MAX_ITERATIONS; iteration++)
            {
                bool significantChange = false;
                
                foreach (var contact in contacts)
                {
                    if (!contact.IsValid()) continue;
                    
                    CollisionResolution contactResolution = ResolveContact(contact, currentVelocity, currentAngularVelocity);
                    
                    // Accumulate changes
                    totalResolution.positionCorrection += contactResolution.positionCorrection;
                    totalResolution.velocityChange += contactResolution.velocityChange;
                    totalResolution.angularVelocityChange += contactResolution.angularVelocityChange;
                    totalResolution.wasLaunchPad |= contactResolution.wasLaunchPad;
                    
                    if (contactResolution.positionCorrection.magnitude > PENETRATION_SLOP ||
                        contactResolution.velocityChange.magnitude > MIN_SEPARATION_VELOCITY)
                    {
                        significantChange = true;
                    }
                }
                
                // Update current state for next iteration
                currentVelocity += totalResolution.velocityChange;
                currentAngularVelocity += totalResolution.angularVelocityChange;
                
                // If no significant changes, we can exit early
                if (!significantChange) break;
            }
            
            return totalResolution;
        }
        
        /// <summary>
        /// Compare contacts for resolution priority
        /// </summary>
        private static int CompareContactPriority(ContactPoint a, ContactPoint b)
        {
            // Ground contacts first
            if (a.isGroundContact && !b.isGroundContact) return -1;
            if (b.isGroundContact && !a.isGroundContact) return 1;
            
            // Then wall contacts
            if (a.isWallContact && !b.isWallContact) return -1;
            if (b.isWallContact && !a.isWallContact) return 1;
            
            // Ceiling contacts last
            if (a.isCeilingContact && !b.isCeilingContact) return 1;
            if (b.isCeilingContact && !a.isCeilingContact) return -1;
            
            // If same type, deeper penetration first
            return b.penetrationDepth.CompareTo(a.penetrationDepth);
        }
        
        /// <summary>
        /// Calculate bounce response in normal direction
        /// </summary>
        private static Vector3 CalculateBounceResponse(ContactPoint contact)
        {
            // Bounce only for approaching contacts (negative normal velocity)
            if (contact.normalVelocity >= 0f) return Vector3.zero;
            
            float bounceMagnitude = Mathf.Abs(contact.normalVelocity) * contact.restitution;
            
            // Clamp bounce velocity to min/max values
            bounceMagnitude = Mathf.Clamp(bounceMagnitude, MIN_BOUNCE_VELOCITY, MAX_BOUNCE_VELOCITY);
            
            // Invert the normal velocity component with restitution factor
            return -contact.normal * bounceMagnitude;
        }
        
        /// <summary>
        /// Calculate friction response in tangential direction
        /// </summary>
        private static Vector3 CalculateFrictionResponse(ContactPoint contact)
        {
            // Friction opposes tangential velocity
            if (contact.tangentialVelocity.sqrMagnitude < MIN_SEPARATION_VELOCITY * MIN_SEPARATION_VELOCITY)
                return Vector3.zero;
            
            Vector3 frictionDirection = -contact.tangentialVelocity.normalized;
            float frictionMagnitude = contact.tangentialVelocity.magnitude * contact.friction;
            
            // Limit friction to prevent over-correction
            float maxFriction = contact.tangentialVelocity.magnitude;
            frictionMagnitude = Mathf.Min(frictionMagnitude, maxFriction);
            
            return frictionDirection * frictionMagnitude;
        }
        
        /// <summary>
        /// Calculate angular velocity change from friction
        /// </summary>
        private static Vector3 CalculateAngularVelocityChange(ContactPoint contact, Vector3 frictionResponse)
        {
            if (frictionResponse.sqrMagnitude < MIN_SEPARATION_VELOCITY * MIN_SEPARATION_VELOCITY)
                return Vector3.zero;
            
            // Calculate torque direction
            Vector3 torqueArm = contact.position - (contact.normal * BALL_RADIUS);
            Vector3 torque = Vector3.Cross(torqueArm, frictionResponse);
            
            // Convert to angular velocity change
            float inertia = (2f/5f) * BALL_MASS * BALL_RADIUS * BALL_RADIUS; // Sphere moment of inertia
            Vector3 angularVelocityChange = torque / inertia;
            
            return angularVelocityChange * ANGULAR_VELOCITY_FACTOR;
        }
        
        /// <summary>
        /// Resolve penetration by moving ball out of geometry
        /// </summary>
        private static Vector3 ResolvePenetration(ContactPoint contact)
        {
            if (contact.penetrationDepth <= PENETRATION_SLOP) return Vector3.zero;
            
            float correctionMagnitude = (contact.penetrationDepth - PENETRATION_SLOP) * PENETRATION_CORRECTION_PERCENT;
            return contact.normal * correctionMagnitude;
        }
        
        /// <summary>
        /// Check if contact is with a launch pad and get launch force
        /// </summary>
        private static bool IsLaunchPadContact(ContactPoint contact, out Vector3 launchForce)
        {
            launchForce = Vector3.zero;
            
            if (contact.material == null || !contact.material.isLaunchPad) return false;
            
            // Only apply launch force for significant contacts
            if (contact.normalVelocity > -MIN_SEPARATION_VELOCITY) return false;
            
            launchForce = contact.material.GetLaunchForce(contact.normal, BALL_MASS);
            return true;
        }
    }
    
    /// <summary>
    /// Result of collision resolution
    /// </summary>
    public class CollisionResolution
    {
        public Vector3 positionCorrection = Vector3.zero;
        public Vector3 velocityChange = Vector3.zero;
        public Vector3 angularVelocityChange = Vector3.zero;
        public bool wasLaunchPad = false;
        
        public bool HasSignificantChange => positionCorrection.sqrMagnitude > 0.0001f ||
                                            velocityChange.sqrMagnitude > 0.0001f ||
                                            angularVelocityChange.sqrMagnitude > 0.0001f;
        
        public void Reset()
        {
            positionCorrection = Vector3.zero;
            velocityChange = Vector3.zero;
            angularVelocityChange = Vector3.zero;
            wasLaunchPad = false;
        }
    }
}
```

---

## Documentation Requirements
- **XML Documentation**: Document all public classes, methods, and fields
- **Usage Examples**: Provide examples of collision resolution
- **Performance Notes**: Highlight iterative resolution for stability

## Related Documents
- See overview in `01_Overview.md` for context
- Previous task: `Task_3.2_MaterialProperties.md`
