# Phase 3: Task 3.1 - Create ContactPoint Data Structure

**IMPORTANT:** Phase 3 integrates with Phase 1's centralized `PhysicsSettings` - all collision material properties come from this single source of truth!

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
            restitution = CalculateRestitution();
            
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
            
            // Use lerp-based restitution as per spec
            float contactAngleFactor = Mathf.Clamp01(Vector3.Dot(-relativeVelocity.normalized, normal));
            float baseRestitution = Mathf.Lerp(PhysicsSettings.Instance.minRestitution, PhysicsSettings.Instance.maxRestitution, contactAngleFactor);
            return baseRestitution * (material?.bouncinessMultiplier ?? 1f);
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
```

---

## Documentation Requirements
- **XML Documentation**: Document all public classes, methods, and fields
- **Usage Examples**: Provide examples of contact point initialization
- **Performance Notes**: Highlight object pooling implementation

## Related Documents
- See overview in `01_Overview.md` for context
- Next task: `Task_3.2_MaterialProperties.md`
