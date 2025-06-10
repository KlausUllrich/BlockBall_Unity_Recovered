---
title: "Phase 0A: Task 2 - IPhysicsObject Wrapper Interface (LLM)"
phase: "0A"
context_required: ["IPhysicsObject", "CompatibilityLayer"]
dependencies: ["LLM_01A_Mission_and_Precision.md", "LLM_01B_Migration_Phases.md", "LLM_02A_PhysicsSettings_Task.md"]
output_files: ["Assets/Scripts/Physics/IPhysicsObject.cs", "Assets/Scripts/Physics/PhysicsObjectWrapper.cs"]
validation_steps:
  - "Confirm IPhysicsObject interface matches provided code"
  - "Verify PhysicsObjectWrapper implementation"
  - "Check compatibility with existing PhysicObjekt"
  - "Test integration with Player and BallObject classes"
priority: 2
---

# Phase 0A: Task 2 - Implement IPhysicsObject Wrapper Interface (LLM)

## Objective
Create a compatibility layer for the existing `PhysicObjekt` class by defining an `IPhysicsObject` interface and implementing a wrapper to ensure backward compatibility during migration.

**Directive for LLM**: Follow the steps below precisely to implement `IPhysicsObject` and its wrapper. Validate each step and log progress or issues in the status files. Do not proceed to other tasks until validation is complete.

## Implementation Steps
1. **Verify Unity Editor Setup**:
   - Ensure Unity 2022.3 LTS is installed and the project is loaded at `C:\Users\Klaus\My_Game_Projects\Blockball\BlockBall_Unity_Recovered`.
   - **Validation**: If project path is incorrect, log error in `/Status/Issues_and_Required_Cleanup.md`: "Project path mismatch. Verify project location before proceeding."
2. **Create Interface Directory**:
   - Create directory `Assets/Scripts/Physics/` if it does not exist.
   - **Validation**: Check directory existence. If creation fails, log error: "Failed to create Assets/Scripts/Physics/. Check write permissions."
3. **Create IPhysicsObject Interface Script**:
   - Write the following C# script at `Assets/Scripts/Physics/IPhysicsObject.cs`.
   - **Code Block**:
     ```csharp
     // File: Assets/Scripts/Physics/IPhysicsObject.cs
     // Purpose: Interface for physics objects to ensure compatibility during migration
     using UnityEngine;
     
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
     ```
   - **Validation**: Confirm script is created and compiles without errors in Unity. If compilation fails, log error: "IPhysicsObject.cs compilation error. Check syntax and Unity version."
4. **Create PhysicsObjectWrapper Script**:
   - Write the following C# script at `Assets/Scripts/Physics/PhysicsObjectWrapper.cs`.
   - **Code Block**:
     ```csharp
     // File: Assets/Scripts/Physics/PhysicsObjectWrapper.cs
     // Purpose: Wrapper for existing PhysicObjekt to maintain compatibility
     using UnityEngine;
     
     public class PhysicsObjectWrapper : MonoBehaviour, IPhysicsObject
     {
         private PhysicObjekt physicObject;
         private Rigidbody rigidBody;
         
         void Awake()
         {
             physicObject = GetComponent<PhysicObjekt>();
             rigidBody = GetComponent<Rigidbody>();
             
             if (physicObject == null)
                 throw new System.Exception("PhysicsObjectWrapper requires PhysicObjekt component");
         }
         
         // Implement interface by delegating to existing PhysicObjekt
         public Vector3 Position 
         { 
             get => transform.position; 
             set => transform.position = value; 
         }
         
         public Vector3 Velocity 
         { 
             get => rigidBody.velocity; 
             set => rigidBody.velocity = value; 
         }
         
         public Vector3 AngularVelocity 
         { 
             get => rigidBody.angularVelocity; 
             set => rigidBody.angularVelocity = value; 
         }
         
         public float Mass 
         { 
             get => rigidBody.mass; 
             set => rigidBody.mass = value; 
         }
         
         public Vector3 GravityDirection 
         { 
             get => physicObject.GravityDirection; 
             set => physicObject.SetGravityDirection(value); 
         }
         
         public bool IsGrounded => physicObject.IsGrounded;
         
         public bool HasGroundContact() => physicObject.HasGroundContact();
         
         public PhysicsObjectState CurrentState 
         { 
             get => (PhysicsObjectState)physicObject.CurrentState; 
         }
         
         public void SetState(PhysicsObjectState state)
         {
             physicObject.SetState((int)state);
         }
         
         public void IntegrateVelocityVerlet(float deltaTime)
         {
             // Placeholder for custom physics integration
             // Initially delegates to Unity physics
             physicObject.UpdatePhysics(deltaTime);
         }
         
         public void ApplyForce(Vector3 force, ForceMode mode = ForceMode.Force)
         {
             rigidBody.AddForce(force, mode);
         }
         
         public void ApplyTorque(Vector3 torque, ForceMode mode = ForceMode.Force)
         {
             rigidBody.AddTorque(torque, mode);
         }
         
         public void OnPhysicsCollision(Collision collision)
         {
             physicObject.OnCollision(collision);
         }
         
         public void OnPhysicsTrigger(Collider other, bool isEnter)
         {
             if (isEnter)
                 physicObject.OnTriggerEnter(other);
             else
                 physicObject.OnTriggerExit(other);
         }
     }
     ```
   - **Validation**: Confirm script is created and compiles without errors in Unity. If compilation fails, log error: "PhysicsObjectWrapper.cs compilation error. Check syntax and Unity version."
5. **Attach Wrapper to PhysicObjekt GameObjects**:
   - Add `PhysicsObjectWrapper` component to all GameObjects with `PhysicObjekt` in existing prefabs and scenes.
   - **Validation**: Check a sample prefab (e.g., Player or BallObject) to confirm wrapper is attached. If not, log error: "PhysicsObjectWrapper not attached to PhysicObjekt objects. Update prefabs and scenes."
6. **Test Compatibility with Existing Classes**:
   - Test integration with `Player.cs` and `BallObject.cs` to ensure existing functionality is maintained.
   - **Validation**: Run a test scene with Player and BallObject. If behavior deviates, log error: "Compatibility issue with PhysicsObjectWrapper. Check delegation to PhysicObjekt."

## Acceptance Criteria for IPhysicsObject Wrapper
- **Interface Definition Check**: Verify `IPhysicsObject.cs` exists and contains all specified methods and properties. If incomplete, log error: "IPhysicsObject interface incomplete. Add missing methods."
- **Wrapper Implementation Check**: Confirm `PhysicsObjectWrapper.cs` implements all interface methods by delegating to `PhysicObjekt`. If not, log error: "PhysicsObjectWrapper implementation incomplete. Ensure full delegation."
- **Compatibility Check**: Test that existing `PhysicObjekt` functionality is accessible through the wrapper without breaking changes. If breaks occur, log error: "Breaking changes in PhysicObjekt compatibility. Adjust wrapper."
- **Integration Test**: Validate integration with `Player` and `BallObject` classes. If integration fails, log error: "Integration failure with Player/BallObject. Review wrapper implementation."

**Directive for LLM**: Log all validation results and issues in `/Status/Issues_and_Required_Cleanup.md`. Update `/Status/Project_Overview.md` with completion status of IPhysicsObject wrapper task.

**Next Step**: Proceed to `LLM_02C_DeterministicMath_Task.md` for the next task after completing and validating IPhysicsObject implementation.
