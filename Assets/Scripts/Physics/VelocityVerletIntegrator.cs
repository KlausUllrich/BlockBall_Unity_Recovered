// File: VelocityVerletIntegrator.cs
// Purpose: High-precision physics integration for energy conservation
// Version: 1.0.0
// Date: 2025-06-12

using UnityEngine;
using BlockBall.Physics;

namespace BlockBall.Physics
{
    /// <summary>
    /// Velocity Verlet integrator for high-precision physics simulation.
    /// Provides better energy conservation and stability compared to basic Euler integration.
    /// </summary>
    public static class VelocityVerletIntegrator
    {
        /// <summary>
        /// Integrate physics object using Velocity Verlet method
        /// Formula: x(t+dt) = x(t) + v(t)*dt + 0.5*a(t)*dt²
        ///          v(t+dt) = v(t) + 0.5*(a(t) + a(t+dt))*dt
        /// </summary>
        /// <param name="obj">Physics object to integrate</param>
        /// <param name="deltaTime">Integration timestep</param>
        public static void Integrate(IPhysicsObject obj, float deltaTime)
        {
            // Store current state
            Vector3 currentPosition = obj.Position;
            Vector3 currentVelocity = obj.Velocity;
            Vector3 currentAcceleration = Vector3.zero;
            
            // Get current acceleration
            if (obj is IAdvancedPhysicsObject advancedObj)
            {
                currentAcceleration = advancedObj.CalculateAcceleration();
                advancedObj.Acceleration = currentAcceleration;
            }
            
            // Calculate new position using Velocity Verlet
            // x(t+dt) = x(t) + v(t)*dt + 0.5*a(t)*dt²
            Vector3 newPosition = currentPosition + 
                                currentVelocity * deltaTime + 
                                0.5f * currentAcceleration * deltaTime * deltaTime;
            
            // Update position temporarily to calculate new acceleration
            obj.Position = newPosition;
            
            // Calculate new acceleration at new position
            Vector3 newAcceleration = Vector3.zero;
            if (obj is IAdvancedPhysicsObject advancedObjNew)
            {
                newAcceleration = advancedObjNew.CalculateAcceleration(newPosition);
            }
            
            // Calculate new velocity using average acceleration
            // v(t+dt) = v(t) + 0.5*(a(t) + a(t+dt))*dt
            Vector3 newVelocity = currentVelocity + 
                                0.5f * (currentAcceleration + newAcceleration) * deltaTime;
            
            // Apply final state
            obj.Position = newPosition;
            obj.Velocity = newVelocity;
            
            if (obj is IAdvancedPhysicsObject finalAdvancedObj)
            {
                finalAdvancedObj.Acceleration = newAcceleration;
            }
        }
        
        /// <summary>
        /// Integrate with basic Euler method as fallback for objects without advanced interface
        /// </summary>
        /// <param name="obj">Physics object to integrate</param>
        /// <param name="deltaTime">Integration timestep</param>
        /// <param name="acceleration">External acceleration to apply</param>
        public static void IntegrateEuler(IPhysicsObject obj, float deltaTime, Vector3 acceleration)
        {
            // Basic Euler integration: x = x + v*dt, v = v + a*dt
            obj.Position += obj.Velocity * deltaTime;
            obj.Velocity += acceleration * deltaTime;
        }
        
        /// <summary>
        /// Calculate kinetic energy for energy conservation monitoring
        /// </summary>
        /// <param name="obj">Physics object</param>
        /// <returns>Kinetic energy in Joules</returns>
        public static float CalculateKineticEnergy(IPhysicsObject obj)
        {
            float mass = obj.Mass;
            float velocitySquared = obj.Velocity.sqrMagnitude;
            return 0.5f * mass * velocitySquared;
        }
        
        /// <summary>
        /// Calculate potential energy (assuming gravity)
        /// </summary>
        /// <param name="obj">Physics object</param>
        /// <param name="gravityMagnitude">Gravity strength (positive value)</param>
        /// <param name="referenceHeight">Reference height for potential energy</param>
        /// <returns>Potential energy in Joules</returns>
        public static float CalculatePotentialEnergy(IPhysicsObject obj, float gravityMagnitude, float referenceHeight = 0f)
        {
            float mass = obj.Mass;
            float height = obj.Position.y - referenceHeight;
            return mass * gravityMagnitude * height;
        }
        
        /// <summary>
        /// Calculate total mechanical energy (kinetic + potential)
        /// </summary>
        /// <param name="obj">Physics object</param>
        /// <param name="gravityMagnitude">Gravity strength (positive value)</param>
        /// <param name="referenceHeight">Reference height for potential energy</param>
        /// <returns>Total mechanical energy in Joules</returns>
        public static float CalculateTotalEnergy(IPhysicsObject obj, float gravityMagnitude, float referenceHeight = 0f)
        {
            return CalculateKineticEnergy(obj) + CalculatePotentialEnergy(obj, gravityMagnitude, referenceHeight);
        }
        
        /// <summary>
        /// Validate energy conservation within tolerance
        /// </summary>
        /// <param name="initialEnergy">Energy at start of simulation</param>
        /// <param name="currentEnergy">Current energy</param>
        /// <param name="tolerance">Acceptable energy drift (0.01 = 1%)</param>
        /// <returns>True if energy is conserved within tolerance</returns>
        public static bool ValidateEnergyConservation(float initialEnergy, float currentEnergy, float tolerance = 0.01f)
        {
            if (Mathf.Abs(initialEnergy) < 0.001f) return true; // Avoid division by zero
            
            float energyDrift = Mathf.Abs(currentEnergy - initialEnergy) / Mathf.Abs(initialEnergy);
            return energyDrift <= tolerance;
        }
        
        /// <summary>
        /// Debug method to log integration state
        /// </summary>
        /// <param name="obj">Physics object</param>
        /// <param name="stepName">Name of integration step</param>
        public static void LogIntegrationState(IPhysicsObject obj, string stepName)
        {
            Vector3 acceleration = Vector3.zero;
            if (obj is IAdvancedPhysicsObject advancedObj)
            {
                acceleration = advancedObj.Acceleration;
            }
            
            UnityEngine.Debug.Log($"[{stepName}] Pos: {obj.Position:F3}, Vel: {obj.Velocity:F3}, Acc: {acceleration:F3}, KE: {CalculateKineticEnergy(obj):F3}");
        }
    }
}
