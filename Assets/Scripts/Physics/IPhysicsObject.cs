// File: Assets/Scripts/Physics/IPhysicsObject.cs
// Purpose: Interface for physics objects to ensure compatibility during migration
using UnityEngine;

namespace BlockBall.Physics
{
    public interface IPhysicsObject
    {
        // Core physics properties
        Vector3 Position { get; set; }
        Vector3 Velocity { get; set; }
        Vector3 AngularVelocity { get; set; }
        float Mass { get; set; }
        
        // BlockBall-specific properties
        Vector3 GravityDirection { get; set; }
        bool IsGrounded { get; }
        bool HasGroundContact();
        
        // State management
        PhysicsObjectState CurrentState { get; }
        void SetState(PhysicsObjectState state);
        
        // Integration methods
        void IntegrateVelocityVerlet(float deltaTime);
        void ApplyForce(Vector3 force, ForceMode mode = ForceMode.Force);
        void ApplyTorque(Vector3 torque, ForceMode mode = ForceMode.Force);
        
        // Collision callbacks
        void OnPhysicsCollision(Collision collision);
        void OnPhysicsTrigger(Collider other, bool isEnter);
    }

    public enum PhysicsObjectState
    {
        Grounded,
        Airborne,
        Sliding,
        Transitioning
    }
}
