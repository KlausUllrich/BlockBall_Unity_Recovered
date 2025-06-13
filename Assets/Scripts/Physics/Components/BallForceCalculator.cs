// BallForceCalculator.cs - Handles force and acceleration calculations
// Part of BlockBall Evolution Core Physics Architecture  
// Created: 2025-06-12 by refactoring BallPhysics.cs

using UnityEngine;
using BlockBall.Settings;

namespace BlockBall.Physics.Components
{
    /// <summary>
    /// Calculates forces, accelerations, and applies physics constraints for the ball.
    /// Handles gravity, input forces, friction, and speed limiting.
    /// </summary>
    public class BallForceCalculator
    {
        // Physics state
        private Vector3 currentAcceleration = Vector3.zero;
        
        // Configuration
        private PhysicsSettings settings;
        private BallInputHandler inputHandler;
        private BallStateManager stateManager;
        private BallGroundDetector groundDetector;
        
        // Properties
        public Vector3 CurrentAcceleration => currentAcceleration;
        
        public BallForceCalculator(PhysicsSettings physicsSettings, BallInputHandler input, 
                                   BallStateManager states, BallGroundDetector ground)
        {
            settings = physicsSettings;
            inputHandler = input;
            stateManager = states;
            groundDetector = ground;
        }
        
        /// <summary>
        /// Calculate total acceleration from all forces
        /// </summary>
        public Vector3 CalculateAcceleration(Vector3 position, Vector3 velocity, Vector3 gravityDirection)
        {
            Vector3 totalAcceleration = Vector3.zero;
            
            // Gravity acceleration
            float gravityMagnitude = Mathf.Abs(UnityEngine.Physics.gravity.y);
            totalAcceleration += gravityDirection * gravityMagnitude;
            
            // Input-based acceleration (only when there's input)
            if (inputHandler.CurrentInput.magnitude > 0.1f)
            {
                Vector3 inputAcceleration = CalculateInputAcceleration();
                totalAcceleration += inputAcceleration;
            }
            
            // Friction/drag acceleration
            Vector3 frictionAcceleration = CalculateFrictionAcceleration(velocity);
            totalAcceleration += frictionAcceleration;
            
            return totalAcceleration;
        }
        
        /// <summary>
        /// Update current acceleration and store it
        /// </summary>
        public void CalculateForces(Vector3 position, Vector3 velocity, Vector3 gravityDirection)
        {
            currentAcceleration = CalculateAcceleration(position, velocity, gravityDirection);
            
            // Debug logging
            UnityEngine.Debug.Log($"BallForceCalculator: State={stateManager.CurrentState}, " +
                                $"Velocity={velocity:F2}, Acceleration={currentAcceleration:F2}");
        }
        
        /// <summary>
        /// Calculate input-based acceleration
        /// </summary>
        private Vector3 CalculateInputAcceleration()
        {
            float maxInputSpeed = settings?.maxInputSpeed ?? 5f;
            return inputHandler.CalculateInputAcceleration(stateManager.CurrentState, maxInputSpeed);
        }
        
        /// <summary>
        /// Calculate friction/drag acceleration
        /// </summary>
        private Vector3 CalculateFrictionAcceleration(Vector3 velocity)
        {
            if (velocity.magnitude < 0.01f)
                return Vector3.zero;
                
            Vector3 frictionForce = Vector3.zero;
            float frictionCoeff = stateManager.GetStateFriction();
            
            switch (stateManager.CurrentState)
            {
                case PhysicsObjectState.Grounded:
                    // Rolling friction opposes movement
                    frictionForce = -velocity.normalized * frictionCoeff;
                    break;
                case PhysicsObjectState.Sliding:
                    // Sliding friction (stronger)
                    frictionForce = -velocity.normalized * frictionCoeff;
                    break;
                case PhysicsObjectState.Airborne:
                    // Air drag (velocity-dependent)
                    frictionForce = -velocity * frictionCoeff;
                    break;
            }
            
            return frictionForce;
        }
        
        /// <summary>
        /// Apply speed limits based on configuration
        /// </summary>
        public Vector3 ApplySpeedLimits(Vector3 velocity)
        {
            if (settings == null) return velocity;
            
            float currentSpeed = velocity.magnitude;
            float speedLimit = GetCurrentSpeedLimit();
            
            if (currentSpeed > speedLimit)
            {
                velocity = velocity.normalized * speedLimit;
                UnityEngine.Debug.Log($"BallForceCalculator: Speed limited from {currentSpeed:F2} to {speedLimit:F2}");
            }
            
            return velocity;
        }
        
        /// <summary>
        /// Get the appropriate speed limit for current state
        /// </summary>
        private float GetCurrentSpeedLimit()
        {
            if (settings == null) return 10f;
            
            // Use the most restrictive applicable limit
            float limit = settings.maxTotalSpeed;
            
            // Apply state-specific limits
            switch (stateManager.CurrentState)
            {
                case PhysicsObjectState.Grounded:
                    limit = Mathf.Min(limit, settings.maxInputSpeed * 1.5f);
                    break;
                case PhysicsObjectState.Airborne:
                    limit = Mathf.Min(limit, settings.maxPhysicsSpeed);
                    break;
                case PhysicsObjectState.Sliding:
                    limit = Mathf.Min(limit, settings.maxInputSpeed * 0.5f);
                    break;
            }
            
            return limit;
        }
        
        /// <summary>
        /// Calculate jump velocity for exact height
        /// </summary>
        public Vector3 CalculateJumpVelocity(Vector3 gravityDirection)
        {
            if (settings == null) return Vector3.zero;
            
            float jumpHeight = settings.jumpHeight;
            float gravityMagnitude = Mathf.Abs(UnityEngine.Physics.gravity.y);
            float jumpVelocity = Mathf.Sqrt(2f * gravityMagnitude * jumpHeight);
            
            return -gravityDirection * jumpVelocity;
        }
        
        /// <summary>
        /// Apply rolling constraint (angular velocity = linear velocity / radius)
        /// </summary>
        public Vector3 CalculateRollingAngularVelocity(Vector3 velocity, Vector3 gravityDirection, float radius)
        {
            if (stateManager.CurrentState != PhysicsObjectState.Grounded)
                return Vector3.zero;
                
            // Rolling constraint: angular velocity = linear velocity / radius
            Vector3 rollingAxis = Vector3.Cross(gravityDirection, velocity).normalized;
            float rollingSpeed = velocity.magnitude / radius;
            
            return rollingAxis * rollingSpeed;
        }
        
        /// <summary>
        /// Check if forces are reasonable (for debugging)
        /// </summary>
        public bool ValidateForces()
        {
            float maxReasonableAcceleration = 100f; // m/s²
            if (currentAcceleration.magnitude > maxReasonableAcceleration)
            {
                UnityEngine.Debug.LogWarning($"BallForceCalculator: Unreasonable acceleration detected: {currentAcceleration.magnitude:F2}");
                return false;
            }
            
            return true;
        }
    }
}
