# Phase 3: Collision System Implementation Tasks

**IMPORTANT:** Phase 3 integrates with Phase 1's centralized `PhysicsSettings` - all collision material properties come from this single source of truth!

## Task 3.1: Create ContactPoint Data Structure

### Objective
Define a comprehensive data structure to store all collision contact information for deterministic processing.

### Implementation Steps
1. **Create ContactPoint Class**: Define all necessary collision data
2. **Add Utility Methods**: Helper functions for contact analysis
3. **Implement Object Pooling**: Reuse contact points to prevent allocation
4. **Add Validation**: Ensure contact data integrity

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    /// <summary>
    /// Represents a single contact point between the ball and environment
    /// </summary>
    public class ContactPoint
    {
        // Contact geometry
        public Vector3 position = Vector3.zero;
        public Vector3 normal = Vector3.up;
        public float penetrationDepth = 0f;
        public Vector3 relativeVelocity = Vector3.zero;
        
        // Surface properties
        public MaterialProperties material = null;
        public Collider otherCollider = null;
        public GameObject otherGameObject = null;
        
        // Contact classification
        public ContactType contactType = ContactType.Surface;
        public bool isGroundContact = false;
        public bool isWallContact = false;
        public bool isCeilingContact = false;
        
        // Calculated values
        public float normalVelocity = 0f;
        public Vector3 tangentialVelocity = Vector3.zero;
        public float friction = 0f;
        public float restitution = 0f;
        
        // Timing
        public float contactTime = 0f;
        public int frameCount = 0;
        
        /// <summary>
        /// Initialize contact point from Unity collision data
        /// </summary>
        public void Initialize(Collision collision, int contactIndex, Vector3 ballVelocity)
        {
            ContactPoint unityContact = collision.contacts[contactIndex];
            
            // Basic geometry
            position = unityContact.point;
            normal = unityContact.normal;
            penetrationDepth = unityContact.separation;
            
            // Other object info
            otherCollider = unityContact.otherCollider;
            otherGameObject = otherCollider.gameObject;
            
            // Material lookup
            material = GetMaterialProperties(otherCollider);
            
            // Velocity calculations
            relativeVelocity = ballVelocity;
            normalVelocity = Vector3.Dot(relativeVelocity, normal);
            tangentialVelocity = relativeVelocity - (normal * normalVelocity);
            
            // Contact classification
            ClassifyContact();
            
            // Material properties
            friction = material?.GetEffectiveFriction(this) ?? PhysicsSettings.Instance.friction;
            restitution = material?.GetEffectiveBounciness(this) ?? PhysicsSettings.Instance.bounciness;
            
            // Timing
            contactTime = Time.time;
            frameCount = Time.frameCount;
        }
        
        /// <summary>
        /// Calculate restitution based on contact angle and material
        /// </summary>
        private float CalculateRestitution()
        {
            if (material == null) return PhysicsSettings.Instance.bounciness;
            
            // Reduce restitution for grazing contacts
            float contactAngle = Vector3.Angle(-relativeVelocity, normal);
            float angleFactor = Mathf.Clamp01(1f - (contactAngle / 90f));
            
            return material.GetEffectiveBounciness(this) * angleFactor;
        }
        
        /// <summary>
        /// Classify contact type based on normal direction
        /// </summary>
        private void ClassifyContact()
        {
            float upDot = Vector3.Dot(normal, Vector3.up);
            
            if (upDot > 0.7f) // Normal pointing up
            {
                contactType = ContactType.Ground;
                isGroundContact = true;
            }
            else if (upDot < -0.7f) // Normal pointing down
            {
                contactType = ContactType.Ceiling;
                isCeilingContact = true;
            }
            else // Normal pointing horizontally
            {
                contactType = ContactType.Wall;
                isWallContact = true;
            }
        }
        
        /// <summary>
        /// Get material properties from collider
        /// </summary>
        private MaterialProperties GetMaterialProperties(Collider collider)
        {
            var materialComponent = collider.GetComponent<MaterialComponent>();
            if (materialComponent != null)
                return materialComponent.Properties;
            
            // Try to get from tag or name
            return MaterialProperties.GetDefaultMaterial(collider.tag);
        }
        
        /// <summary>
        /// Reset contact point for object pooling
        /// </summary>
        public void Reset()
        {
            position = Vector3.zero;
            normal = Vector3.up;
            penetrationDepth = 0f;
            relativeVelocity = Vector3.zero;
            material = null;
            otherCollider = null;
            otherGameObject = null;
            contactType = ContactType.Surface;
            isGroundContact = false;
            isWallContact = false;
            isCeilingContact = false;
            normalVelocity = 0f;
            tangentialVelocity = Vector3.zero;
            friction = 0f;
            restitution = 0f;
            contactTime = 0f;
            frameCount = 0;
        }
        
        /// <summary>
        /// Check if this contact is still valid
        /// </summary>
        public bool IsValid()
        {
            return otherCollider != null && 
                   otherGameObject != null && 
                   !float.IsNaN(penetrationDepth) &&
                   !float.IsInfinity(normalVelocity);
        }
        
        /// <summary>
        /// Get debug string representation
        /// </summary>
        public override string ToString()
        {
            return $"Contact: {contactType} at {position}, depth: {penetrationDepth:F3}, normal: {normal}, restitution: {restitution:F2}";
        }
    }
    
    /// <summary>
    /// Types of contact surfaces
    /// </summary>
    public enum ContactType
    {
        Surface,  // Generic surface contact
        Ground,   // Ground contact (normal pointing up)
        Wall,     // Wall contact (normal pointing horizontal)
        Ceiling,  // Ceiling contact (normal pointing down)
        Edge,     // Edge contact (multiple normals)
        Corner    // Corner contact (3+ normals)
    }
}

---

## Task 3.2: Create MaterialProperties ScriptableObject

### Objective
Define collision material properties that integrate with Phase 1's PhysicsSettings for consistent physics behavior.

### Implementation Steps
1. **Create MaterialProperties Class**: Define surface-specific collision properties
2. **Reference PhysicsSettings**: Use centralized physics configuration as base values
3. **Add Surface Modifiers**: Allow per-surface adjustments to global physics settings
4. **Implement Material Component**: Attach materials to GameObjects

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    /// <summary>
    /// Defines material properties for collision response.
    /// Uses PhysicsSettings as base values with surface-specific modifiers.
    /// </summary>
    [CreateAssetMenu(fileName = "New Material Properties", menuName = "BlockBall/Material Properties")]
    public class MaterialProperties : ScriptableObject
    {
        [Header("Surface Type")]
        [SerializeField] private string materialName = "Default";
        [SerializeField] private MaterialType materialType = MaterialType.Normal;
        
        [Header("Physics Modifiers (Applied to PhysicsSettings base values)")]
        [Tooltip("Multiplier for base friction from PhysicsSettings (1.0 = normal)")]
        [Range(0f, 3f)]
        public float frictionMultiplier = 1f;
        
        [Tooltip("Multiplier for base bounciness from PhysicsSettings (1.0 = normal)")]
        [Range(0f, 3f)]
        public float bouncinessMultiplier = 1f;
        
        [Tooltip("How much this surface affects rolling speed (1.0 = normal)")]
        [Range(0.1f, 2f)]
        public float speedMultiplier = 1f;
        
        [Header("Special Properties")]
        [Tooltip("Surface makes distinctive sound effects")]
        public bool hasSpecialSounds = false;
        
        [Tooltip("Surface has particle effects on contact")]
        public bool hasParticleEffects = false;
        
        [Tooltip("This surface can be used as a launching pad")]
        public bool isLaunchPad = false;
        
        [Tooltip("Launch pad force multiplier")]
        [Range(1f, 5f)]
        public float launchForceMultiplier = 1f;
        
        // Cached reference to physics settings
        private PhysicsSettings physicsSettings;
        
        private void OnEnable()
        {
            LoadPhysicsSettings();
        }
        
        private void LoadPhysicsSettings()
        {
            if (physicsSettings == null && BlockBallPhysicsManager.Instance != null)
            {
                physicsSettings = BlockBallPhysicsManager.Instance.Settings;
            }
        }
        
        /// <summary>
        /// Get effective friction for this material
        /// </summary>
        public float GetEffectiveFriction(ContactPoint contact = null)
        {
            LoadPhysicsSettings();
            if (physicsSettings == null) return 0.5f; // Fallback
            
            float baseFriction = physicsSettings.friction;
            float effectiveFriction = baseFriction * frictionMultiplier;
            
            // Apply special material effects
            if (materialType == MaterialType.Ice)
            {
                effectiveFriction *= 0.1f; // Very slippery
            }
            else if (materialType == MaterialType.Rubber)
            {
                effectiveFriction *= 1.5f; // Extra grip
            }
            
            return Mathf.Clamp01(effectiveFriction);
        }
        
        /// <summary>
        /// Get effective bounciness for this material
        /// </summary>
        public float GetEffectiveBounciness(ContactPoint contact = null)
        {
            LoadPhysicsSettings();
            if (physicsSettings == null) return 0.3f; // Fallback
            
            float baseBounciness = physicsSettings.bounciness;
            float effectiveBounciness = baseBounciness * bouncinessMultiplier;
            
            // Apply special material effects
            if (materialType == MaterialType.Rubber)
            {
                effectiveBounciness *= 1.8f; // Extra bouncy
            }
            else if (materialType == MaterialType.Foam)
            {
                effectiveBounciness *= 0.2f; // Absorbs impact
            }
            
            return Mathf.Clamp01(effectiveBounciness);
        }
        
        /// <summary>
        /// Get effective speed multiplier (affects rolling acceleration)
        /// </summary>
        public float GetEffectiveSpeedMultiplier()
        {
            float effectiveSpeed = speedMultiplier;
            
            // Apply material type modifiers
            switch (materialType)
            {
                case MaterialType.Ice:
                    effectiveSpeed *= 1.2f; // Faster on ice
                    break;
                    
                case MaterialType.Sand:
                    effectiveSpeed *= 0.7f; // Slower on sand
                    break;
                    
                case MaterialType.Metal:
                    effectiveSpeed *= 1.1f; // Slightly faster
                    break;
            }
            
            return effectiveSpeed;
        }
        
        /// <summary>
        /// Check if this material should trigger special effects
        /// </summary>
        public bool ShouldTriggerEffects(ContactPoint contact, float impactVelocity)
        {
            // Only trigger effects for significant impacts
            const float minImpactVelocity = 2f;
            return (hasSpecialSounds || hasParticleEffects) && impactVelocity > minImpactVelocity;
        }
        
        /// <summary>
        /// Get launch force if this is a launch pad
        /// </summary>
        public Vector3 GetLaunchForce(Vector3 contactNormal, float ballMass)
        {
            if (!isLaunchPad) return Vector3.zero;
            
            LoadPhysicsSettings();
            if (physicsSettings == null) return Vector3.zero;
            
            // Calculate launch force based on jump mechanics
            float baseJumpForce = physicsSettings.GetJumpForce(ballMass);
            float launchForce = baseJumpForce * launchForceMultiplier;
            
            return contactNormal * launchForce;
        }
    }
    
    /// <summary>
    /// Types of materials with predefined properties
    /// </summary>
    public enum MaterialType
    {
        Normal,     // Standard surfaces
        Ice,        // Slippery, fast
        Rubber,     // High friction, bouncy
        Metal,      // Medium friction, slightly fast
        Sand,       // High friction, slow
        Foam,       // High friction, low bounce
        Trampoline  // Low friction, very bouncy
    }
    
    /// <summary>
    /// Component to attach material properties to GameObjects
    /// </summary>
    public class MaterialComponent : MonoBehaviour
    {
        [Header("Material Configuration")]
        [SerializeField] private MaterialProperties materialProperties;
        [SerializeField] private bool overrideCollidersOnStart = true;
        
        [Header("Debug")]
        [SerializeField] private bool showMaterialInfo = false;
        
        // Cached colliders
        private Collider[] attachedColliders;
        
        public MaterialProperties Properties => materialProperties;
        
        private void Start()
        {
            if (overrideCollidersOnStart)
            {
                SetupColliderMaterials();
            }
        }
        
        private void SetupColliderMaterials()
        {
            attachedColliders = GetComponentsInChildren<Collider>();
            
            foreach (var collider in attachedColliders)
            {
                // Store reference for collision system to find
                if (collider.gameObject.GetComponent<MaterialComponent>() == null)
                {
                    collider.gameObject.AddComponent<MaterialComponent>().materialProperties = materialProperties;
                }
            }
        }
        
        /// <summary>
        /// Get material properties, with fallback to default if none assigned
        /// </summary>
        public MaterialProperties GetMaterialProperties()
        {
            if (materialProperties != null) return materialProperties;
            
            // Try to find default material
            var defaultMaterial = Resources.Load<MaterialProperties>("DefaultMaterial");
            if (defaultMaterial != null) return defaultMaterial;
            
            // Create temporary default
            Debug.LogWarning($"No material properties found for {gameObject.name}");
            return CreateDefaultMaterial();
        }
        
        private MaterialProperties CreateDefaultMaterial()
        {
            var defaultMaterial = ScriptableObject.CreateInstance<MaterialProperties>();
            defaultMaterial.name = "Default (Generated)";
            return defaultMaterial;
        }
        
        private void OnGUI()
        {
            if (showMaterialInfo && materialProperties != null)
            {
                var rect = new Rect(Screen.width - 200, 10, 180, 100);
                GUILayout.BeginArea(rect);
                GUILayout.Label($"Material: {materialProperties.name}");
                GUILayout.Label($"Friction: {materialProperties.GetEffectiveFriction():F2}");
                GUILayout.Label($"Bounce: {materialProperties.GetEffectiveBounciness():F2}");
                GUILayout.Label($"Speed: {materialProperties.GetEffectiveSpeedMultiplier():F2}x");
                GUILayout.EndArea();
            }
        }
    }
}

---

## Task 3.3: Create CollisionResponse System

### Objective
Implement deterministic collision response that uses PhysicsSettings for consistent behavior.

### Implementation Steps
1. **Create CollisionResponse Class**: Handle all collision calculations
2. **Integrate PhysicsSettings**: Use centralized physics configuration
3. **Implement Material Integration**: Combine base physics with material modifiers
4. **Add Deterministic Calculations**: Ensure reproducible collision results

### Code Template
```csharp
using UnityEngine;
using System.Collections.Generic;

namespace BlockBall.Physics
{
    /// <summary>
    /// Handles deterministic collision response calculations using PhysicsSettings
    /// </summary>
    public static class CollisionResponse
    {
        private const float PENETRATION_SLOP = 0.01f; // Allowable penetration
        private const float PENETRATION_CORRECTION_PERCENT = 0.8f;
        private const int MAX_ITERATIONS = 3;
        private const float MIN_SEPARATION_VELOCITY = 0.01f;
        
        // Cached physics settings reference
        private static PhysicsSettings physicsSettings;
        
        static CollisionResponse()
        {
            LoadPhysicsSettings();
        }
        
        private static void LoadPhysicsSettings()
        {
            if (physicsSettings == null && BlockBallPhysicsManager.Instance != null)
            {
                physicsSettings = BlockBallPhysicsManager.Instance.Settings;
            }
        }
        
        /// <summary>
        /// Resolve collision response for a single contact point
        /// </summary>
        public static CollisionResult ResolveCollision(ContactPoint contact, Vector3 velocity, float mass)
        {
            LoadPhysicsSettings();
            
            var result = new CollisionResult();
            result.originalVelocity = velocity;
            result.contactsProcessed = 1;
            
            // Get material properties (uses PhysicsSettings as base)
            float friction = contact.material?.GetEffectiveFriction(contact) ?? PhysicsSettings.Instance.friction;
            float restitution = contact.material?.GetEffectiveBounciness(contact) ?? PhysicsSettings.Instance.bounciness;
            
            // Calculate collision response
            Vector3 newVelocity = CalculateCollisionResponse(velocity, contact, friction, restitution);
            
            // Apply position correction if needed
            Vector3 positionCorrection = CalculatePositionCorrection(contact);
            
            result.finalVelocity = newVelocity;
            result.positionCorrection = positionCorrection;
            result.totalEnergyLoss = CalculateEnergyLoss(velocity, newVelocity);
            result.wasGroundContact = contact.isGroundContact;
            
            return result;
        }
        
        /// <summary>
        /// Calculate velocity change from collision
        /// </summary>
        private static Vector3 CalculateCollisionResponse(Vector3 velocity, ContactPoint contact, float friction, float restitution)
        {
            Vector3 normal = contact.normal;
            float relativeVelocity = Vector3.Dot(velocity, normal);
            
            // Don't resolve if velocities are separating
            if (relativeVelocity > 0) return velocity;
            
            // Calculate restitution impulse
            float restitutionImpulse = -(1 + restitution) * relativeVelocity;
            Vector3 normalImpulse = normal * restitutionImpulse;
            
            // Apply normal impulse
            Vector3 newVelocity = velocity + normalImpulse;
            
            // Calculate and apply friction impulse
            Vector3 tangentialVelocity = newVelocity - Vector3.Dot(newVelocity, normal) * normal;
            
            if (tangentialVelocity.magnitude > 0.001f)
            {
                Vector3 frictionImpulse = CalculateFrictionImpulse(tangentialVelocity, normalImpulse.magnitude, friction);
                newVelocity += frictionImpulse;
            }
            
            // Apply material-specific speed multiplier
            if (contact.material != null)
            {
                float speedMultiplier = contact.material.GetEffectiveSpeedMultiplier();
                newVelocity *= speedMultiplier;
            }
            
            return newVelocity;
        }
        
        /// <summary>
        /// Calculate friction impulse using Coulomb friction model
        /// </summary>
        private static Vector3 CalculateFrictionImpulse(Vector3 tangentialVelocity, float normalImpulseMagnitude, float friction)
        {
            if (friction <= 0f) return Vector3.zero;
            
            Vector3 tangentDirection = tangentialVelocity.normalized;
            float tangentSpeed = tangentialVelocity.magnitude;
            
            // Coulomb friction limit
            float maxFrictionImpulse = friction * normalImpulseMagnitude;
            
            // Calculate required impulse to stop tangential motion
            float requiredImpulse = tangentSpeed;
            
            // Apply friction limit
            float frictionImpulse = Mathf.Min(requiredImpulse, maxFrictionImpulse);
            
            return -tangentDirection * frictionImpulse;
        }
        
        /// <summary>
        /// Calculate position correction to resolve penetration
        /// </summary>
        private static Vector3 CalculatePositionCorrection(ContactPoint contact)
        {
            if (contact.penetrationDepth <= PENETRATION_SLOP) return Vector3.zero;
            
            float correctionMagnitude = (contact.penetrationDepth - PENETRATION_SLOP) * PENETRATION_CORRECTION_PERCENT;
            return contact.normal * correctionMagnitude;
        }
        
        /// <summary>
        /// Calculate energy loss from collision
        /// </summary>
        private static float CalculateEnergyLoss(Vector3 velocityBefore, Vector3 velocityAfter)
        {
            float energyBefore = 0.5f * velocityBefore.sqrMagnitude;
            float energyAfter = 0.5f * velocityAfter.sqrMagnitude;
            return energyBefore - energyAfter;
        }
        
        /// <summary>
        /// Resolve multiple simultaneous contacts
        /// </summary>
        public static CollisionResult ResolveMultipleCollisions(List<ContactPoint> contacts, Vector3 velocity, float mass)
        {
            if (contacts == null || contacts.Count == 0)
            {
                return new CollisionResult
                {
                    originalVelocity = velocity,
                    finalVelocity = velocity,
                    positionCorrection = Vector3.zero,
                    contactsProcessed = 0
                };
            }
            
            var result = new CollisionResult();
            result.originalVelocity = velocity;
            result.contactsProcessed = contacts.Count;
            
            Vector3 currentVelocity = velocity;
            Vector3 totalPositionCorrection = Vector3.zero;
            float totalEnergyLoss = 0f;
            
            // Process contacts iteratively for stability
            for (int iteration = 0; iteration < MAX_ITERATIONS; iteration++)
            {
                bool anyContact = false;
                
                foreach (var contact in contacts)
                {
                    float relativeVelocity = Vector3.Dot(currentVelocity, contact.normal);
                    
                    // Only process separating contacts
                    if (relativeVelocity < -MIN_SEPARATION_VELOCITY)
                    {
                        anyContact = true;
                        
                        // Get material properties
                        float friction = contact.material?.GetEffectiveFriction(contact) ?? PhysicsSettings.Instance.friction;
                        float restitution = contact.material?.GetEffectiveBounciness(contact) ?? PhysicsSettings.Instance.bounciness;
                        
                        // Calculate response for this contact
                        Vector3 responseVelocity = CalculateCollisionResponse(currentVelocity, contact, friction, restitution);
                        Vector3 positionCorrection = CalculatePositionCorrection(contact);
                        
                        // Accumulate changes
                        totalEnergyLoss += CalculateEnergyLoss(currentVelocity, responseVelocity);
                        totalPositionCorrection += positionCorrection;
                        currentVelocity = responseVelocity;
                        
                        // Check for ground contact
                        if (contact.isGroundContact)
                        {
                            result.wasGroundContact = true;
                        }
                    }
                }
                
                // If no contacts needed processing, we're done
                if (!anyContact) break;
            }
            
            result.finalVelocity = currentVelocity;
            result.positionCorrection = totalPositionCorrection;
            result.totalEnergyLoss = totalEnergyLoss;
            
            return result;
        }
        
        /// <summary>
        /// Get default friction from PhysicsSettings
        /// </summary>
        private static float GetDefaultFriction()
        {
            LoadPhysicsSettings();
            return physicsSettings?.friction ?? 0.5f;
        }
        
        /// <summary>
        /// Get default bounciness from PhysicsSettings
        /// </summary>
        private static float GetDefaultBounciness()
        {
            LoadPhysicsSettings();
            return physicsSettings?.bounciness ?? 0.3f;
        }
        
        /// <summary>
        /// Check if material should trigger launch pad effects
        /// </summary>
        public static bool CheckLaunchPadEffect(ContactPoint contact, float impactVelocity, out Vector3 launchForce)
        {
            launchForce = Vector3.zero;
            
            if (contact.material == null || !contact.material.isLaunchPad) return false;
            
            // Only trigger on significant downward impacts
            const float minLaunchVelocity = 3f;
            if (impactVelocity < minLaunchVelocity) return false;
            
            // Calculate launch force (assuming 1kg mass for force calculation)
            launchForce = contact.material.GetLaunchForce(contact.normal, 1f);
            return launchForce.magnitude > 0.1f;
        }
    }
    
    /// <summary>
    /// Result of collision resolution
    /// </summary>
    public struct CollisionResult
    {
        public Vector3 originalVelocity;
        public Vector3 finalVelocity;
        public Vector3 positionCorrection;
        public float totalEnergyLoss;
        public int contactsProcessed;
        public bool wasGroundContact;
        
        public bool HasSignificantChange => (finalVelocity - originalVelocity).magnitude > 0.01f;
        public float SpeedChange => finalVelocity.magnitude - originalVelocity.magnitude;
    }
}

---

## Documentation Requirements for Each Task
1. **Complete API Documentation**: XML comments for all public methods and properties
2. **Usage Examples**: Practical examples showing how to use each component
3. **Integration Guide**: How to integrate with Phase 1 and Phase 2 systems  
4. **Performance Notes**: Memory usage patterns and optimization recommendations
5. **Testing Procedures**: How to validate each component works correctly
6. **Troubleshooting**: Common issues and their solutions
7. **Configuration Guide**: How to set up materials and collision layers
