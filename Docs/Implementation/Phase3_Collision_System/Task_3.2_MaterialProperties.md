# Phase 3: Task 3.2 - Create MaterialProperties ScriptableObject

**IMPORTANT:** Phase 3 integrates with Phase 1's centralized `PhysicsSettings` - all collision material properties come from this single source of truth!

### Objective
Define collision material properties that integrate with Phase 1's PhysicsSettings for consistent physics behavior, focusing on essential properties required for the game.

### Implementation Steps
1. **Create MaterialProperties Class**: Define essential collision properties
2. **Reference PhysicsSettings**: Use centralized physics configuration as base values
3. **Implement Material Component**: Attach materials to GameObjects

### Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    /// <summary>
    /// Defines material properties for collision response.
    /// Uses PhysicsSettings as base values.
    /// </summary>
    [CreateAssetMenu(fileName = "New Material Properties", menuName = "BlockBall/Material Properties")]
    public class MaterialProperties : ScriptableObject
    {
        [Header("Surface Type")]
        [SerializeField] private string materialName = "Default";
        
        [Header("Physics Modifiers (Applied to PhysicsSettings base values)")]
        [Tooltip("Multiplier for base friction from PhysicsSettings (1.0 = normal)")]
        [Range(0f, 2f)]
        public float frictionMultiplier = 1f;
        
        [Tooltip("Multiplier for base bounciness from PhysicsSettings (1.0 = normal)")]
        [Range(0f, 2f)]
        public float bouncinessMultiplier = 1f;
        
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
            
            return Mathf.Clamp01(effectiveBounciness);
        }
    }
    
    /// <summary>
    /// Component to attach material properties to GameObjects
    /// </summary>
    public class MaterialComponent : MonoBehaviour
    {
        [Header("Material Configuration")]
        [SerializeField] private MaterialProperties materialProperties;
        [SerializeField] private bool overrideCollidersOnStart = true;
        
        public MaterialProperties Properties => materialProperties;
        
        private void Start()
        {
            if (overrideCollidersOnStart)
            {
                var colliders = GetComponents<Collider>();
                if (colliders.Length == 0)
                {
                    colliders = GetComponentsInChildren<Collider>();
                }
            }
            
            if (materialProperties == null)
            {
                Debug.LogWarning($"No material properties assigned to {gameObject.name}");
                materialProperties = MaterialProperties.GetDefaultMaterial(tag);
            }
        }
        
        private void OnValidate()
        {
            if (materialProperties == null)
            {
                materialProperties = MaterialProperties.GetDefaultMaterial(tag);
            }
        }
        
        /// <summary>
        /// Get default material based on tag or name
        /// </summary>
        public static MaterialProperties GetDefaultMaterial(string tag)
        {
            // Implement material lookup logic based on tag or name
            return null; // Placeholder
        }
    }
}

---

## Documentation Requirements
- **XML Documentation**: Document all public classes, methods, and fields
- **Usage Examples**: Provide examples of material property configuration
- **Performance Notes**: Highlight ScriptableObject benefits for memory

## Related Documents
- See overview in `01_Overview.md` for context
- Previous task: `Task_3.1_ContactPoint.md`
- Next task: `Task_3.3_CollisionResponse.md`
