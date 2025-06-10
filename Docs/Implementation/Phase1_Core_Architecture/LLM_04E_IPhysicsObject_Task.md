---
title: IPhysicsObject Interface Implementation
description: Task to create the interface for standardized physics objects in BlockBall.
phase: 1C
dependencies: LLM_04A_Phase1_Overview.md, LLM_04B_Phase1_Deliverables.md, LLM_04D_BlockBallPhysicsManager_Task.md
validation_criteria: Interface defines required methods, supports backward compatibility.
---

# Task 1.3: Create IPhysicsObject Interface

## Objective
Define `IPhysicsObject` as a standardized contract for all physics objects in the game, ensuring compatibility with existing code.

## Background
This interface provides a bridge between the custom physics system and existing Unity components like `PhysicObjekt`. It allows for a gradual migration without breaking current functionality, ensuring backward compatibility while introducing new physics behaviors.

## Implementation Steps
1. **Create Script File**:
   - Path: `Assets/Scripts/Physics/IPhysicsObject.cs`
   - Action: Create a new C# script in Unity under the specified path.
2. **Add Code Template**:
   - Action: Copy the provided code template into `IPhysicsObject.cs`.
   - Purpose: Defines methods for physics updates in three phases (pre-update, update, post-update).
3. **Ensure Compatibility**:
   - Action: Include properties and methods that mirror existing `PhysicObjekt` functionality.
   - Expected Outcome: Existing classes can implement this interface with minimal changes.
4. **Document Usage**:
   - Action: Add XML comments explaining each method's purpose and when it's called.
   - Expected Outcome: Clear guidance for future implementations.

## Code Template
```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    /// <summary>
    /// Interface for objects managed by custom physics system.
    /// Provides backward compatibility with existing PhysicObjekt.
    /// </summary>
    public interface IPhysicsObject
    {
        // Physics state properties

        /// <summary>
        /// Current position in world space
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// Current velocity in world space (units/second)
        /// </summary>
        Vector3 Velocity { get; set; }

        /// <summary>
        /// Current angular velocity (radians/second)
        /// </summary>
        Vector3 AngularVelocity { get; set; }

        /// <summary>
        /// Object mass in kilograms
        /// </summary>
        float Mass { get; }

        /// <summary>
        /// Is the object currently active and simulating physics?
        /// </summary>
        bool IsActive { get; }

        // Update lifecycle methods

        /// <summary>
        /// Called before physics integration to prepare state and handle input.
        /// Use for: input processing, state updates, force accumulation.
        /// </summary>
        /// <param name="timestep">Fixed timestep for this update (e.g. 0.02s)</param>
        void PreUpdate(float timestep);

        /// <summary>
        /// Called during physics integration to update position and velocity.
        /// Use for: Velocity Verlet integration, applying forces.
        /// </summary>
        /// <param name="timestep">Fixed timestep for this update (e.g. 0.02s)</param>
        void UpdatePhysics(float timestep);

        /// <summary>
        /// Called after physics integration to handle collisions and constraints.
        /// Use for: collision detection/response, position corrections.
        /// </summary>
        /// <param name="timestep">Fixed timestep for this update (e.g. 0.02s)</param>
        void PostUpdate(float timestep);

        // Force application methods for compatibility

        /// <summary>
        /// Apply a force to the object at its center of mass.
        /// Compatible with existing AddForce() calls.
        /// </summary>
        /// <param name="force">Force vector in world space</param>
        void AddForce(Vector3 force);

        /// <summary>
        /// Apply a torque to the object.
        /// Compatible with existing AddTorque() calls.
        /// </summary>
        /// <param name="torque">Torque vector in world space</param>
        void AddTorque(Vector3 torque);

        // State management for compatibility with existing code

        /// <summary>
        /// Reset velocity to zero (for respawn, etc.)
        /// </summary>
        void ResetVelocity();

        /// <summary>
        /// Set gravity direction (for gravity switching mechanics)
        /// </summary>
        /// <param name="gravityDirection">New down direction</param>
        /// <param name="immediate">If true, snap immediately; if false, smooth transition</param>
        void SetGravityDirection(Vector3 gravityDirection, bool immediate = false);
    }
}
```

## Validation Steps
1. **Interface Compilation**:
   - Action: Save `IPhysicsObject.cs` and check for compilation errors in Unity.
   - Expected Outcome: No errors, interface is valid C#.
2. **Method Coverage Check**:
   - Action: Review the interface for methods matching `PhysicObjekt` functionality (e.g., `AddForce`, `SetGravityDirection`).
   - Expected Outcome: All critical existing methods are represented.
3. **Documentation Clarity**:
   - Action: Read XML comments for each method.
   - Expected Outcome: Purpose and usage of each method are clearly explained.

## Error Handling
- **If Compilation Fails**: Check for syntax errors or missing `UnityEngine` namespace. Ensure no reserved keywords are used.
- **If Methods Are Missing**: Compare with `PhysicObjekt` class to ensure all necessary methods are included for compatibility.
- **If Documentation is Unclear**: Add examples or use cases to XML comments for complex methods like `SetGravityDirection`.
