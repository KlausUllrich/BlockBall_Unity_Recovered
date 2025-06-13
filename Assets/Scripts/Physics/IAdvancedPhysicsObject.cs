// File: IAdvancedPhysicsObject.cs
// Purpose: Extended interface for advanced physics objects with lifecycle hooks
// Version: 1.0.0
// Date: 2025-06-12

using UnityEngine;

namespace BlockBall.Physics
{
    /// <summary>
    /// Extended interface for advanced physics objects that require custom lifecycle hooks
    /// for integration with BlockBallPhysicsManager
    /// </summary>
    public interface IAdvancedPhysicsObject : IPhysicsObject
    {
        /// <summary>
        /// Called before physics integration to calculate forces and accelerations
        /// </summary>
        /// <param name="deltaTime">Physics timestep</param>
        void PrePhysicsStep(float deltaTime);
        
        /// <summary>
        /// Called after physics integration to handle collisions and constraints
        /// </summary>
        /// <param name="deltaTime">Physics timestep</param>
        void PostPhysicsStep(float deltaTime);
        
        /// <summary>
        /// Current acceleration vector for Velocity Verlet integration
        /// </summary>
        Vector3 Acceleration { get; set; }
        
        /// <summary>
        /// Calculate acceleration at current position (used by Velocity Verlet)
        /// </summary>
        /// <returns>Calculated acceleration vector</returns>
        Vector3 CalculateAcceleration();
        
        /// <summary>
        /// Calculate acceleration at a specific position (used by Velocity Verlet)
        /// </summary>
        /// <param name="position">Position to calculate acceleration at</param>
        /// <returns>Calculated acceleration vector</returns>
        Vector3 CalculateAcceleration(Vector3 position);
    }
}
