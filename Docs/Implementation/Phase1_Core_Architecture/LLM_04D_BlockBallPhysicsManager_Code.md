---
title: BlockBall Physics Manager Additional Methods
description: Additional methods for BlockBallPhysicsManager to handle physics updates and object management.
phase: 1B
dependencies: LLM_04D_BlockBallPhysicsManager_Task.md
validation_criteria: Physics updates process all objects, profiler tracks performance.
---

# BlockBallPhysicsManager Additional Methods

## Objective
Provide the complete set of methods for `BlockBallPhysicsManager.cs` to manage physics updates and object registration.

## Code Template (Additional Methods)
```csharp
// Inside BlockBallPhysicsManager class in namespace BlockBall.Physics

/// <summary>
/// Custom fixed update for physics system
/// </summary>
private void FixedUpdatePhysics()
{
    // Update all registered physics objects
    for (int i = 0; i < physicsObjects.Count; i++)
    {
        if (physicsObjects[i] != null)
        {
            physicsObjects[i].PreUpdate(FixedTimestep);
        }
    }

    // Integrate velocities and positions using Velocity Verlet
    for (int i = 0; i < physicsObjects.Count; i++)
    {
        if (physicsObjects[i] != null)
        {
            physicsObjects[i].UpdatePhysics(FixedTimestep);
        }
    }

    // Handle collisions and post-update
    for (int i = 0; i < physicsObjects.Count; i++)
    {
        if (physicsObjects[i] != null)
        {
            physicsObjects[i].PostUpdate(FixedTimestep);
        }
    }
}

/// <summary>
/// Register a physics object to be updated by the manager
/// </summary>
public void RegisterPhysicsObject(IPhysicsObject obj)
{
    if (!physicsObjects.Contains(obj))
    {
        physicsObjects.Add(obj);
        // Disable Unity physics for this object if it has a Rigidbody
        var rb = (obj as MonoBehaviour)?.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }
}

/// <summary>
/// Unregister a physics object (e.g. on destroy)
/// </summary>
public void UnregisterPhysicsObject(IPhysicsObject obj)
{
    physicsObjects.Remove(obj);
}

/// <summary>
/// Get current physics settings
/// </summary>
public PhysicsSettings GetPhysicsSettings()
{
    return settings;
}

/// <summary>
/// Get profiler for performance monitoring
/// </summary>
public PhysicsProfiler GetProfiler()
{
    return profiler;
}

private void OnDestroy()
{
    // Clean up
    if (instance == this)
    {
        instance = null;
    }
    
    // Re-enable Unity physics if shutting down
    Physics.autoSimulation = true;
}
```

## Validation Steps
1. **Object Registration**:
   - Action: Create a test `IPhysicsObject` implementation and register it.
   - Expected Outcome: Object appears in `physicsObjects` list and receives updates.
2. **Physics Update Phases**:
   - Action: Add debug logs to `PreUpdate()`, `UpdatePhysics()`, and `PostUpdate()` of a test object.
   - Expected Outcome: Logs show all three phases called in order each timestep.
3. **Unity Physics Disabling**:
   - Action: Register an object with a `Rigidbody` component.
   - Expected Outcome: `useGravity` and `isKinematic` are set correctly to disable Unity physics.

## Error Handling
- **If Objects Don't Update**: Verify `physicsObjects` list contains the object. Check update loop for null checks.
- **If Unity Physics Persists**: Ensure `Physics.autoSimulation = false` in `Awake()`. Double-check `Rigidbody` settings on registration.
- **If Profiler is Null**: Initialize `profiler` in `Awake()` if not already done.
