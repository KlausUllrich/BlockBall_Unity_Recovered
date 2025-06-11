# Task 4.3: Create GravityDirectionUtility

## Objective
Provide utility functions for calculating gravity directions, particularly for snapping to the nearest cardinal direction on trigger exit.

## Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    /// <summary>
    /// Utility class for gravity direction calculations
    /// Primarily handles snapping to cardinal directions
    /// </summary>
    public static class GravityDirectionUtility
    {
        // Cardinal directions for snapping
        private static readonly Vector3[] CardinalDirections = {
            Vector3.right, Vector3.left, Vector3.up, 
            Vector3.down, Vector3.forward, Vector3.back
        };
        
        /// <summary>
        /// Snaps a given direction to the nearest cardinal axis
        /// Used when exiting gravity trigger zones
        /// </summary>
        /// <param name="direction">Input direction to snap</param>
        /// <returns>Nearest cardinal direction</returns>
        public static Vector3 SnapToNearestCardinal(Vector3 direction)
        {
            if (direction == Vector3.zero) return Vector3.down; // Fallback
            
            Vector3 closest = Vector3.down; // Default fallback
            float maxDot = -1f;
            
            foreach (var cardinal in CardinalDirections)
            {
                float dot = Vector3.Dot(direction.normalized, cardinal);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    closest = cardinal;
                }
            }
            
            return closest;
        }
        
        /// <summary>
        /// Checks if a direction is approximately a cardinal direction
        /// Useful for validation and testing
        /// </summary>
        /// <param name="direction">Direction to test</param>
        /// <param name="threshold">Dot product threshold, default 0.99</param>
        /// <returns>True if direction aligns with a cardinal axis</returns>
        public static bool IsCardinalDirection(Vector3 direction, float threshold = 0.99f)
        {
            if (direction == Vector3.zero) return false;
            
            foreach (var cardinal in CardinalDirections)
            {
                if (Vector3.Dot(direction.normalized, cardinal) > threshold)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Calculates a debug color based on gravity direction
        /// Useful for visualization in editor
        /// </summary>
        /// <param name="direction">Gravity direction</param>
        /// <returns>Color representing the direction</returns>
        public static Color GetDirectionColor(Vector3 direction)
        {
            Vector3 absDir = new Vector3(
                Mathf.Abs(direction.x), 
                Mathf.Abs(direction.y), 
                Mathf.Abs(direction.z)
            );
            
            if (absDir.x > absDir.y && absDir.x > absDir.z)
                return direction.x > 0 ? Color.red : new Color(0.5f, 0f, 0f); // Right/Left
            else if (absDir.y > absDir.x && absDir.y > absDir.z)
                return direction.y > 0 ? Color.green : new Color(0f, 0.5f, 0f); // Up/Down
            else
                return direction.z > 0 ? Color.blue : new Color(0f, 0f, 0.5f); // Forward/Back
        }
    }
}
```

## Validation Steps
1. Test `SnapToNearestCardinal` with various input directions
2. Verify snapping prioritizes closest cardinal axis
3. Confirm `IsCardinalDirection` identifies cardinal alignments
4. Check debug color mapping for visualization

## Related Documents
- **Task_4.1_PlayerGravityManager.md**: Uses this utility for snapping logic
- **02_Implementation_Tasks_Summary.md**: Overview of implementation sequence
