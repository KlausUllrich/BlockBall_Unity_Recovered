// BallStateManager.cs - Handles ball physics state machine
// Part of BlockBall Evolution Core Physics Architecture
// Created: 2025-06-12 by refactoring BallPhysics.cs

using UnityEngine;
using BlockBall.Settings;

namespace BlockBall.Physics.Components
{
    /// <summary>
    /// Manages the physics state machine for the ball, handling transitions between
    /// Grounded, Airborne, Sliding, and Transitioning states.
    /// </summary>
    public class BallStateManager
    {
        // State management
        private PhysicsObjectState currentState = PhysicsObjectState.Airborne;
        private PhysicsObjectState previousState = PhysicsObjectState.Airborne;
        private float stateTimer = 0f;
        
        // Configuration
        private PhysicsSettings settings;
        private BallGroundDetector groundDetector;
        
        // Properties
        public PhysicsObjectState CurrentState => currentState;
        public PhysicsObjectState PreviousState => previousState;
        public float StateTimer => stateTimer;
        
        // Events
        public System.Action<PhysicsObjectState, PhysicsObjectState> OnStateChanged;
        
        public BallStateManager(PhysicsSettings physicsSettings, BallGroundDetector detector)
        {
            settings = physicsSettings;
            groundDetector = detector;
        }
        
        /// <summary>
        /// Update the state machine
        /// </summary>
        public void UpdateStateMachine(float deltaTime, Vector3 gravityDirection)
        {
            stateTimer += deltaTime;
            
            PhysicsObjectState newState = DetermineState(gravityDirection);
            SetState(newState);
        }
        
        /// <summary>
        /// Determine the appropriate state based on current conditions
        /// </summary>
        private PhysicsObjectState DetermineState(Vector3 gravityDirection)
        {
            if (groundDetector.IsGrounded)
            {
                if (groundDetector.IsOnSteepSlope(gravityDirection))
                {
                    return PhysicsObjectState.Sliding;
                }
                return PhysicsObjectState.Grounded;
            }
            
            return PhysicsObjectState.Airborne;
        }
        
        /// <summary>
        /// Set the physics state and handle transitions
        /// </summary>
        public void SetState(PhysicsObjectState newState)
        {
            if (currentState != newState)
            {
                OnStateExit(currentState);
                previousState = currentState;
                currentState = newState;
                stateTimer = 0f;
                OnStateEnter(newState);
                
                // Fire state change event
                OnStateChanged?.Invoke(previousState, currentState);
                
                UnityEngine.Debug.Log($"BallStateManager: State changed from {previousState} to {currentState}");
            }
        }
        
        /// <summary>
        /// Handle entering a new state
        /// </summary>
        private void OnStateEnter(PhysicsObjectState state)
        {
            switch (state)
            {
                case PhysicsObjectState.Grounded:
                    // Set coyote time when landing
                    if (settings != null)
                    {
                        // This will be handled by the input handler
                    }
                    break;
                    
                case PhysicsObjectState.Airborne:
                    // Could add air-specific setup here
                    break;
                    
                case PhysicsObjectState.Sliding:
                    // Could add sliding-specific setup here
                    break;
                    
                case PhysicsObjectState.Transitioning:
                    // Could add transition-specific setup here
                    break;
            }
        }
        
        /// <summary>
        /// Handle exiting a state
        /// </summary>
        private void OnStateExit(PhysicsObjectState state)
        {
            switch (state)
            {
                case PhysicsObjectState.Grounded:
                    // Could add cleanup when leaving ground
                    break;
                    
                case PhysicsObjectState.Airborne:
                    // Could add cleanup when landing
                    break;
                    
                case PhysicsObjectState.Sliding:
                    // Could add cleanup when stopping sliding
                    break;
                    
                case PhysicsObjectState.Transitioning:
                    // Could add cleanup when transition ends
                    break;
            }
        }
        
        /// <summary>
        /// Get state-specific parameters for physics calculations
        /// </summary>
        public float GetStateInputMultiplier()
        {
            switch (currentState)
            {
                case PhysicsObjectState.Grounded:
                    return 1.0f; // Full input control
                case PhysicsObjectState.Airborne:
                    return 0.5f; // Reduced air control
                case PhysicsObjectState.Sliding:
                    return 0.1f; // Very limited control when sliding
                case PhysicsObjectState.Transitioning:
                    return 0.3f; // Moderate control during transition
                default:
                    return 1.0f;
            }
        }
        
        /// <summary>
        /// Get state-specific friction coefficient
        /// </summary>
        public float GetStateFriction()
        {
            if (settings == null) return 1.0f;
            
            switch (currentState)
            {
                case PhysicsObjectState.Grounded:
                    return settings.rollingFriction;
                case PhysicsObjectState.Sliding:
                    return settings.slidingFriction;
                case PhysicsObjectState.Airborne:
                    return settings.airDrag;
                case PhysicsObjectState.Transitioning:
                    return settings.rollingFriction * 0.5f; // Reduced during transition
                default:
                    return 1.0f;
            }
        }
        
        /// <summary>
        /// Check if jumping is allowed in current state
        /// </summary>
        public bool CanJumpInCurrentState()
        {
            return currentState == PhysicsObjectState.Grounded || 
                   currentState == PhysicsObjectState.Transitioning;
        }
        
        /// <summary>
        /// Get debug color for visualization
        /// </summary>
        public Color GetDebugColor()
        {
            switch (currentState)
            {
                case PhysicsObjectState.Grounded:
                    return Color.green;
                case PhysicsObjectState.Airborne:
                    return Color.blue;
                case PhysicsObjectState.Sliding:
                    return Color.red;
                case PhysicsObjectState.Transitioning:
                    return Color.yellow;
                default:
                    return Color.white;
            }
        }
    }
}
