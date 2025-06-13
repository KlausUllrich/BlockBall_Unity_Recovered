// BallCollisionHandler.cs - Handles collision detection and response
// Part of BlockBall Evolution Core Physics Architecture
// Created: 2025-06-12 by refactoring BallPhysics.cs

using UnityEngine;
using BlockBall.Settings;

namespace BlockBall.Physics.Components
{
    /// <summary>
    /// Handles collision detection, response, and custom physics collision behavior.
    /// Provides bounce, friction, and collision event management.
    /// </summary>
    public class BallCollisionHandler
    {
        // Configuration
        private PhysicsSettings settings;
        private BallStateManager stateManager;
        
        // Collision state
        private float lastCollisionTime = 0f;
        private Vector3 lastCollisionNormal = Vector3.zero;
        
        // Properties
        public Vector3 LastCollisionNormal => lastCollisionNormal;
        public float TimeSinceLastCollision => Time.time - lastCollisionTime;
        
        // Events
        public System.Action<Collision> OnCollisionDetected;
        public System.Action<Collider, bool> OnTriggerEvent;
        
        public BallCollisionHandler(PhysicsSettings physicsSettings, BallStateManager states)
        {
            settings = physicsSettings;
            stateManager = states;
        }
        
        /// <summary>
        /// Handle physics collision event
        /// </summary>
        public void OnPhysicsCollision(Collision collision, ref Vector3 velocity)
        {
            lastCollisionTime = Time.time;
            
            if (settings?.physicsMode == PhysicsMode.CustomPhysics)
            {
                HandleCustomCollision(collision, ref velocity);
            }
            
            // Fire event for external listeners
            OnCollisionDetected?.Invoke(collision);
            
            UnityEngine.Debug.Log($"BallCollisionHandler: Collision with {collision.gameObject.name}");
        }
        
        /// <summary>
        /// Handle custom collision response for CustomPhysics mode
        /// </summary>
        private void HandleCustomCollision(Collision collision, ref Vector3 velocity)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Vector3 normal = contact.normal;
                lastCollisionNormal = normal;
                
                Vector3 relativeVelocity = velocity;
                
                // Calculate bounce response
                float bounciness = GetBounciness(collision.gameObject);
                Vector3 reflection = Vector3.Reflect(relativeVelocity, normal);
                
                // Apply state-specific collision response
                switch (stateManager.CurrentState)
                {
                    case PhysicsObjectState.Grounded:
                        // Minimal bounce when grounded
                        velocity = Vector3.Lerp(relativeVelocity, reflection, bounciness * 0.3f);
                        break;
                        
                    case PhysicsObjectState.Airborne:
                        // Full bounce response in air
                        velocity = Vector3.Lerp(relativeVelocity, reflection, bounciness);
                        break;
                        
                    case PhysicsObjectState.Sliding:
                        // Slide along surface
                        velocity = Vector3.ProjectOnPlane(relativeVelocity, normal) * 0.8f;
                        break;
                        
                    case PhysicsObjectState.Transitioning:
                        // Moderate response during transitions
                        velocity = Vector3.Lerp(relativeVelocity, reflection, bounciness * 0.6f);
                        break;
                }
                
                // Apply collision friction
                ApplyCollisionFriction(ref velocity, normal, collision.gameObject);
            }
        }
        
        /// <summary>
        /// Get bounciness factor for the collided object
        /// </summary>
        private float GetBounciness(GameObject collisionObject)
        {
            // Default bounciness
            float bounciness = 0.3f;
            
            // Check for custom physics material or tags
            if (collisionObject.CompareTag("Bouncy"))
            {
                bounciness = 0.8f;
            }
            else if (collisionObject.CompareTag("Soft"))
            {
                bounciness = 0.1f;
            }
            
            // Could also check for PhysicMaterial
            Collider collider = collisionObject.GetComponent<Collider>();
            if (collider?.material != null)
            {
                bounciness = collider.material.bounciness;
            }
            
            return bounciness;
        }
        
        /// <summary>
        /// Apply collision-specific friction
        /// </summary>
        private void ApplyCollisionFriction(ref Vector3 velocity, Vector3 normal, GameObject collisionObject)
        {
            float frictionMultiplier = 1.0f;
            
            // Get friction from material or tags
            if (collisionObject.CompareTag("Slippery"))
            {
                frictionMultiplier = 0.1f;
            }
            else if (collisionObject.CompareTag("Rough"))
            {
                frictionMultiplier = 2.0f;
            }
            
            // Could also use PhysicMaterial friction
            Collider collider = collisionObject.GetComponent<Collider>();
            if (collider?.material != null)
            {
                frictionMultiplier = collider.material.dynamicFriction;
            }
            
            // Apply friction to velocity tangent to collision normal
            Vector3 tangentVelocity = Vector3.ProjectOnPlane(velocity, normal);
            Vector3 normalVelocity = Vector3.Project(velocity, normal);
            
            // Reduce tangent velocity by friction
            tangentVelocity *= (1.0f - frictionMultiplier * 0.1f);
            
            velocity = normalVelocity + tangentVelocity;
        }
        
        /// <summary>
        /// Handle trigger events
        /// </summary>
        public void OnPhysicsTrigger(Collider other, bool isEnter)
        {
            if (isEnter)
            {
                UnityEngine.Debug.Log($"BallCollisionHandler: Entered trigger {other.name}");
            }
            else
            {
                UnityEngine.Debug.Log($"BallCollisionHandler: Exited trigger {other.name}");
            }
            
            // Fire event for external listeners
            OnTriggerEvent?.Invoke(other, isEnter);
            
            // Handle special trigger types
            HandleSpecialTriggers(other, isEnter);
        }
        
        /// <summary>
        /// Handle special trigger zones
        /// </summary>
        private void HandleSpecialTriggers(Collider other, bool isEnter)
        {
            if (other.CompareTag("SpeedBoost") && isEnter)
            {
                // Could apply speed boost effect
                UnityEngine.Debug.Log("BallCollisionHandler: Speed boost triggered");
            }
            else if (other.CompareTag("SlowZone") && isEnter)
            {
                // Could apply slow effect
                UnityEngine.Debug.Log("BallCollisionHandler: Slow zone entered");
            }
            else if (other.CompareTag("JumpPad") && isEnter)
            {
                // Could apply jump pad effect
                UnityEngine.Debug.Log("BallCollisionHandler: Jump pad activated");
            }
        }
        
        /// <summary>
        /// Check if we're colliding with a specific type of object
        /// </summary>
        public bool IsCollidingWith(string tag)
        {
            return TimeSinceLastCollision < 0.1f; // Recent collision check
        }
        
        /// <summary>
        /// Get the surface type we're in contact with
        /// </summary>
        public string GetSurfaceType()
        {
            // This could be expanded to track surface materials
            if (TimeSinceLastCollision < 0.1f)
            {
                return "Ground"; // Default surface type
            }
            
            return "Air";
        }
    }
}
