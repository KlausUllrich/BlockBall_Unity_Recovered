// BallInputHandler.cs - Handles player input processing and camera-relative movement
// Part of BlockBall Evolution Core Physics Architecture
// Created: 2025-06-12 by refactoring BallPhysics.cs

using UnityEngine;
using BlockBall.Settings;

namespace BlockBall.Physics.Components
{
    /// <summary>
    /// Handles input processing and camera-relative movement calculations for the ball physics system.
    /// Converts raw input into world-space movement directions relative to the camera.
    /// </summary>
    public class BallInputHandler
    {
        // Input state
        private Vector2 currentInput = Vector2.zero;
        private bool jumpInputPressed = false;
        
        // Jump mechanics
        private float jumpBufferTimer = 0f;
        private float coyoteTimer = 0f;
        
        // Configuration
        private PhysicsSettings settings;
        
        // Properties for external access
        public Vector2 CurrentInput => currentInput;
        public bool JumpInputPressed => jumpInputPressed;
        public float JumpBufferTimer => jumpBufferTimer;
        public float CoyoteTimer => coyoteTimer;
        
        // Events for input changes
        public System.Action OnJumpPressed;
        
        public BallInputHandler(PhysicsSettings physicsSettings)
        {
            settings = physicsSettings;
        }
        
        /// <summary>
        /// Process input each frame and update input state
        /// </summary>
        public void ProcessInput()
        {
            // Get input from Unity Input System or legacy input
            currentInput.x = Input.GetAxis("Horizontal");
            currentInput.y = Input.GetAxis("Vertical");
            
            jumpInputPressed = Input.GetButtonDown("Jump");
            
            // Debug logging
            if (currentInput.magnitude > 0.1f || jumpInputPressed)
            {
                UnityEngine.Debug.Log($"BallInputHandler: Input={currentInput:F2}, Jump={jumpInputPressed}");
            }
            
            // Handle jump buffering
            if (jumpInputPressed)
            {
                jumpBufferTimer = settings?.jumpBufferTime ?? 0.2f;
                OnJumpPressed?.Invoke();
            }
        }
        
        /// <summary>
        /// Update timers (call from Update or FixedUpdate)
        /// </summary>
        public void UpdateTimers(float deltaTime)
        {
            jumpBufferTimer = Mathf.Max(0f, jumpBufferTimer - deltaTime);
            coyoteTimer = Mathf.Max(0f, coyoteTimer - deltaTime);
        }
        
        /// <summary>
        /// Set coyote timer when becoming ungrounded
        /// </summary>
        public void SetCoyoteTimer(float time)
        {
            coyoteTimer = time;
        }
        
        /// <summary>
        /// Get camera-relative input direction in world space
        /// </summary>
        public Vector3 GetCameraRelativeInputDirection()
        {
            if (currentInput.magnitude < 0.1f)
                return Vector3.zero;
                
            // Get camera forward and right vectors (assuming main camera)
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
                return new Vector3(currentInput.x, 0, currentInput.y).normalized;
                
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;
            
            // Remove vertical component for movement
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
            
            // Calculate movement direction
            Vector3 inputDirection = (cameraForward * currentInput.y + cameraRight * currentInput.x).normalized;
            return inputDirection;
        }
        
        /// <summary>
        /// Calculate input acceleration based on current state
        /// </summary>
        public Vector3 CalculateInputAcceleration(PhysicsObjectState currentState, float maxInputSpeed)
        {
            if (currentInput.magnitude < 0.1f)
                return Vector3.zero;
                
            // Get camera-relative input direction
            Vector3 inputDirection = GetCameraRelativeInputDirection();
            
            // Calculate input acceleration based on state
            float inputAcceleration = 0f;
            switch (currentState)
            {
                case PhysicsObjectState.Grounded:
                    inputAcceleration = maxInputSpeed * 2f; // Ground acceleration
                    break;
                case PhysicsObjectState.Airborne:
                    inputAcceleration = maxInputSpeed * 0.5f; // Air control (reduced)
                    break;
                case PhysicsObjectState.Sliding:
                    inputAcceleration = maxInputSpeed * 0.1f; // Very limited control when sliding
                    break;
            }
            
            return inputDirection * inputAcceleration;
        }
        
        /// <summary>
        /// Check if jump conditions are met
        /// </summary>
        public bool CanJump(bool isGrounded)
        {
            return (isGrounded || coyoteTimer > 0f) && jumpBufferTimer > 0f;
        }
        
        /// <summary>
        /// Clear jump buffer after jumping
        /// </summary>
        public void ConsumeJumpBuffer()
        {
            jumpBufferTimer = 0f;
        }
    }
}
