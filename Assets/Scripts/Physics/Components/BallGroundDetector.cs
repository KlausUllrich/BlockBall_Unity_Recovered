// BallGroundDetector.cs - Handles ground detection and contact management
// Part of BlockBall Evolution Core Physics Architecture
// Created: 2025-06-12 by refactoring BallPhysics.cs

using UnityEngine;
using BlockBall.Settings;

namespace BlockBall.Physics.Components
{
    /// <summary>
    /// Handles ground detection, contact management, and surface analysis for the ball physics system.
    /// Uses SphereCast for precise ground detection and provides surface normal information.
    /// </summary>
    public class BallGroundDetector
    {
        // Ground detection state
        private bool isGroundedThisFrame = false;
        private bool wasGroundedLastFrame = false;
        private Vector3 groundNormal = Vector3.up;
        private float groundDistance = float.MaxValue;
        
        // Configuration
        private PhysicsSettings settings;
        private Transform transform;
        private SphereCollider sphereCollider;
        
        // Properties
        public bool IsGrounded => isGroundedThisFrame;
        public bool WasGroundedLastFrame => wasGroundedLastFrame;
        public Vector3 GroundNormal => groundNormal;
        public float GroundDistance => groundDistance;
        public bool JustLanded => isGroundedThisFrame && !wasGroundedLastFrame;
        public bool JustLeftGround => !isGroundedThisFrame && wasGroundedLastFrame;
        
        // Events
        public System.Action OnGroundContact;
        public System.Action OnGroundLost;
        
        public BallGroundDetector(Transform ballTransform, SphereCollider collider, PhysicsSettings physicsSettings)
        {
            transform = ballTransform;
            sphereCollider = collider;
            settings = physicsSettings;
        }
        
        /// <summary>
        /// Perform ground detection check
        /// </summary>
        public void CheckGroundContact(Vector3 gravityDirection)
        {
            wasGroundedLastFrame = isGroundedThisFrame;
            
            Vector3 origin = transform.position;
            Vector3 direction = gravityDirection.normalized;
            float radius = sphereCollider.radius;
            float checkDistance = settings?.groundCheckDistance ?? 0.1f;
            
            // Sphere cast for ground detection
            if (UnityEngine.Physics.SphereCast(origin, radius * 0.9f, direction, out RaycastHit hit, checkDistance))
            {
                groundDistance = hit.distance;
                groundNormal = hit.normal;
                
                // Check if surface is ground (not wall)
                float angle = Vector3.Angle(-gravityDirection, groundNormal);
                isGroundedThisFrame = angle < 60f; // More lenient than slope limit
                
                if (isGroundedThisFrame && !wasGroundedLastFrame)
                {
                    OnGroundContact?.Invoke();
                    UnityEngine.Debug.Log("BallGroundDetector: Ground contact detected");
                }
            }
            else
            {
                if (isGroundedThisFrame && !wasGroundedLastFrame)
                {
                    OnGroundLost?.Invoke();
                    UnityEngine.Debug.Log("BallGroundDetector: Ground contact lost");
                }
                
                isGroundedThisFrame = false;
                groundDistance = float.MaxValue;
                groundNormal = -gravityDirection;
            }
        }
        
        /// <summary>
        /// Get the slope angle of the current ground surface
        /// </summary>
        public float GetSlopeAngle(Vector3 gravityDirection)
        {
            if (!isGroundedThisFrame)
                return 0f;
                
            return Vector3.Angle(groundNormal, -gravityDirection);
        }
        
        /// <summary>
        /// Check if the current surface is too steep for normal movement
        /// </summary>
        public bool IsOnSteepSlope(Vector3 gravityDirection)
        {
            if (!isGroundedThisFrame)
                return false;
                
            float slopeAngle = GetSlopeAngle(gravityDirection);
            float slopeLimit = settings?.slopeLimit ?? 45f;
            
            return slopeAngle > slopeLimit;
        }
        
        /// <summary>
        /// Get the direction parallel to the ground surface
        /// </summary>
        public Vector3 GetGroundTangent(Vector3 inputDirection)
        {
            if (!isGroundedThisFrame)
                return inputDirection;
                
            // Project input direction onto ground plane
            return Vector3.ProjectOnPlane(inputDirection, groundNormal).normalized;
        }
        
        /// <summary>
        /// Get debug information for visualization
        /// </summary>
        public void DrawDebugGizmos()
        {
            if (!Application.isPlaying) return;
            
            Vector3 position = transform.position;
            float checkDistance = settings?.groundCheckDistance ?? 0.1f;
            
            // Draw ground check sphere
            Gizmos.color = isGroundedThisFrame ? Color.green : Color.red;
            Gizmos.DrawWireSphere(position + Vector3.down * checkDistance, 0.1f);
            
            // Draw ground normal if grounded
            if (isGroundedThisFrame)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(position, position + groundNormal * 2f);
            }
        }
    }
}
