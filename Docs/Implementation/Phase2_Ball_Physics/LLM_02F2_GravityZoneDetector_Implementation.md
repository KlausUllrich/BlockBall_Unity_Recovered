---
title: "Phase 2 Ball Physics - GravityZoneDetector Component Implementation Task (Part 2 of 2)"
phase: "Phase 2 - Ball Physics"
part: "2 of 2"
dependencies:
  - "LLM_02F1_GravityZoneDetector_Overview.md"
validation_steps:
  - "Test multi-zone handling by distance to pivot point."
  - "Verify OnTriggerEnter/Exit correctly manages activeZones list."
  - "Confirm gravity direction calculation uses normalized vector from zone pivot to ball position."
integration_points:
  - "Includes GravityZone component for level design use."
  - "Provides debug visualization for gravity direction and active zones."
---

# Phase 2: Ball Physics - GravityZoneDetector Component Implementation (Part 2 of 2)

## Detailed Method Implementations

```csharp
        private Vector3 CalculateGravityDirection(GravityZone zone)
        {
            // Position-based calculation: Gravity direction = normalized vector from zone pivot to ball position
            return (transform.position - zone.pivotPoint).normalized;
        }

        private void UpdateDominantZone()
        {
            if (activeZones.Count == 0)
            {
                dominantZone = null;
                return;
            }
            
            // Find zone with closest pivot point (per physics spec)
            GravityZone closest = activeZones[0];
            float closestDistance = Vector3.Distance(transform.position, closest.pivotPoint);
            
            foreach (var zone in activeZones)
            {
                float distance = Vector3.Distance(transform.position, zone.pivotPoint);
                if (distance < closestDistance)
                {
                    closest = zone;
                    closestDistance = distance;
                }
            }
            
            dominantZone = closest;
        }

        void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & gravityZoneLayer) != 0)
            {
                GravityZone zone = other.GetComponent<GravityZone>();
                if (zone != null && !activeZones.Contains(zone))
                {
                    activeZones.Add(zone);
                    Debug.Log($"Entered gravity zone: {zone.name}");
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & gravityZoneLayer) != 0)
            {
                GravityZone zone = other.GetComponent<GravityZone>();
                if (zone != null && activeZones.Contains(zone))
                {
                    activeZones.Remove(zone);
                    Debug.Log($"Exited gravity zone: {zone.name}");
                    
                    // If this was the dominant zone, find new dominant
                    if (dominantZone == zone)
                    {
                        dominantZone = null;
                        UpdateDominantZone();
                    }
                }
            }
        }

        void OnDrawGizmos()
        {
            if (!debugVisualization) return;
            
            // Draw current gravity direction
            Gizmos.color = Color.red;
            Vector3 gravityDir = ballPhysics.GetCurrentGravity().normalized;
            Gizmos.DrawRay(transform.position, gravityDir * 2f);
            
            // Draw active zones
            Gizmos.color = Color.yellow;
            foreach (var zone in activeZones)
            {
                Gizmos.DrawWireSphere(zone.pivotPoint, 0.2f);
                Gizmos.DrawLine(transform.position, zone.pivotPoint);
            }
        }

        // Public accessors
        public bool IsInGravityZone => activeZones.Count > 0;
        public GravityZone DominantZone => dominantZone;
    }

    /// <summary>
    /// Gravity zone component - attach to gravity zone trigger colliders
    /// </summary>
    [System.Serializable]
    public class GravityZone : MonoBehaviour
    {
        [Header("Gravity Zone Settings")]
        public Vector3 pivotPoint; // Point used for distance calculations
        
        void Awake()
        {
            // Default pivot to zone center if not set
            if (pivotPoint == Vector3.zero)
                pivotPoint = transform.position;
        }
        
        void OnDrawGizmos()
        {
            // Draw pivot point
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(pivotPoint, 0.3f);
        }
    }
}
```

## Context & Dependencies
**Requires Phase 1 Completion**: This component builds on:
- `PhysicsSettings` for gravity direction storage and cardinal axis snapping
- `BallStateMachine` for state transitions and priority handling
- `BallPhysics` for gravity direction application

**Level Design Integration**: Level designers use `GravityZone` components on trigger colliders to define gravity switching areas.

## Validation Instructions
1. **Zone Detection**: Verify trigger enter/exit events correctly add/remove zones from activeZones list
2. **Multi-Zone Priority**: Test that closest pivot point determines dominant zone when multiple zones overlap
3. **Instant Switching**: Confirm gravity direction updates every FixedUpdate frame while in zones
4. **Cardinal Snapping**: Validate gravity snaps to nearest axis when exiting all zones
5. **State Override**: Ensure Transitioning state is forced while in any gravity zone
6. **Debug Visualization**: Check that gizmos correctly show gravity direction and active zones

## Next Steps
After implementing both components, proceed to test integration with `LLM_03F_Phase2_Automated_Tests_GravityZoneSystem.md`. This component enables the critical missing airborne gravity transitions from the original Phase 2 design.
