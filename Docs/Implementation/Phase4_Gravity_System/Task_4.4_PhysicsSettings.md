# Task 4.4: Update PhysicsSettings for Gravity

## Objective
Extend the single source of truth for physics parameters to include gravity system settings, ensuring all constants are configurable.

## Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    /// <summary>
    /// Single source of truth for physics settings
    /// Includes gravity parameters for player-specific system
    /// </summary>
    [CreateAssetMenu(fileName = "PhysicsSettings", menuName = "BlockBall/PhysicsSettings", order = 1)]
    public class PhysicsSettings : ScriptableObject
    {
        private static PhysicsSettings instance;
        public static PhysicsSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<PhysicsSettings>("PhysicsSettings");
                    if (instance == null)
                    {
                        Debug.LogError("PhysicsSettings asset not found in Resources!");
                    }
                }
                return instance;
            }
        }
        
        [Header("Gravity Settings")]
        [Tooltip("Strength of gravity force applied to player ball")]
        public float gravityStrength = 9.81f;
        
        [Tooltip("Allow camera smoothing time after gravity snap")]
        public float cameraRotationSmoothTime = 0.2f;
        
        [Header("Collision Settings")]
        [Tooltip("Minimum restitution for low-angle contacts")]
        public float minRestitution = 0.1f;
        
        [Tooltip("Maximum restitution for high-angle contacts")]
        public float maxRestitution = 0.7f;
        
        [Tooltip("Friction coefficient for rolling behavior")]
        public float rollingFriction = 0.05f;
        
        [Header("Ball Movement")]
        [Tooltip("Maximum speed of the ball")]
        public float maxBallSpeed = 10f;
        
        [Tooltip("Jump impulse force")]
        public float jumpImpulse = 5f;
        
        [Tooltip("Maximum fall height before hard fall feedback")]
        public float maxFallHeight = 4f; // Approx 32.5 Bixels
        
        // Singleton pattern for editor scripts to access
        #if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetInstance()
        {
            instance = null;
        }
        #endif
    }
}
```

## Validation Steps
1. Verify asset loads correctly as singleton in Resources
2. Confirm gravity strength matches expected gameplay feel (9.81 default)
3. Test camera smooth time provides natural rotation after snap
4. Ensure collision parameters align with Phase 3 requirements

## Related Documents
- **Task_4.1_PlayerGravityManager.md**: References gravity strength
- **02_Implementation_Tasks_Summary.md**: Overview of implementation sequence
- **1_BlockBall_Physics_Spec.md**: Source of physics constants
