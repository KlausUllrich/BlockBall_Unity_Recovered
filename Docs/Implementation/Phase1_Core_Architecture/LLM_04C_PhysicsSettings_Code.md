---
title: Physics Settings Conversion Functions
description: Complete conversion functions for PhysicsSettings ScriptableObject.
phase: 1A
dependencies: LLM_04C_PhysicsSettings_Task.md
validation_criteria: Functions return correct physics values based on user inputs.
---

# PhysicsSettings Conversion Functions

## Objective
Provide the complete set of conversion functions for `PhysicsSettings.cs` to transform user-friendly inputs into physics calculations.

## Code Template (Conversion Functions)
```csharp
// Inside PhysicsSettings class in namespace BlockBall.Physics

/// <summary>
/// Convert user-friendly roll responsiveness to acceleration value
/// </summary>
public float GetRollAcceleration()
{
    // Map 1-10 range to 2-20 m/s² acceleration
    return Mathf.Lerp(2f, 20f, rollResponsiveness / 10f);
}

/// <summary>
/// Calculate jump force needed for desired jump height
/// </summary>
public float GetJumpForce(float ballMass)
{
    // F = ma, where a = sqrt(2gh) for jump height h
    return ballMass * Mathf.Sqrt(2f * gravityStrength * jumpHeight);
}

/// <summary>
/// Calculate maximum angular velocity for rolling
/// </summary>
public float GetMaxAngularVelocity(float ballRadius = 0.5f)
{
    return maxRollSpeed / ballRadius;
}

/// <summary>
/// Get input acceleration curve (ease-in/ease-out feel)
/// </summary>
public float GetInputAccelerationCurve(float currentSpeed, float targetSpeed)
{
    float speedRatio = currentSpeed / maxRollSpeed;
    float acceleration = GetRollAcceleration();
    
    // Reduce acceleration as we approach max speed (ease-out feel)
    if (speedRatio > 0.8f)
    {
        float reduceRatio = (speedRatio - 0.8f) / 0.2f; // 0-1 over final 20%
        acceleration *= Mathf.Lerp(1f, 0.1f, reduceRatio);
    }
    
    return acceleration;
}

/// <summary>
/// Calculate slope effect on movement (45° = no acceleration, >45° = deceleration)
/// </summary>
public float GetSlopeAccelerationFactor(float slopeAngleDegrees)
{
    if (slopeAngleDegrees <= 45f)
    {
        // 0-45°: full to no acceleration
        return Mathf.Lerp(1f, 0f, slopeAngleDegrees / 45f);
    }
    else
    {
        // >45°: deceleration (negative factor)
        float excessAngle = slopeAngleDegrees - 45f;
        return -Mathf.Min(excessAngle / 45f, 1f); // Cap at -1 (full deceleration)
    }
}

/// <summary>
/// Apply exponential speed decay near limits
/// </summary>
public float ApplySpeedDecay(float currentSpeed, float speedLimit)
{
    if (currentSpeed <= speedLimit * 0.95f) return currentSpeed;
    
    float excessRatio = (currentSpeed - speedLimit * 0.95f) / (speedLimit * 0.05f);
    float decayFactor = Mathf.Exp(-2f * excessRatio); // Exponential decay
    
    return speedLimit * 0.95f + (speedLimit * 0.05f * decayFactor);
}

// ===== GRAVITY SYSTEM METHODS =====

/// <summary>
/// Sets gravity instantly based on direction (used by gravity zones)
/// </summary>
public void SetGravityDirection(Vector3 direction)
{
    currentGravity = direction.normalized * gravityMagnitude;
}

/// <summary>
/// Gets current gravity vector for physics calculations
/// </summary>
public Vector3 GetCurrentGravity()
{
    return currentGravity;
}

/// <summary>
/// Snaps gravity to nearest cardinal axis (for zone exit behavior)
/// </summary>
public void SnapGravityToCardinalAxis()
{
    Vector3 direction = currentGravity.normalized;
    
    // Find the axis with the largest component
    Vector3 cardinalDirection = Vector3.down; // Default
    float maxComponent = Mathf.Abs(direction.y);
    
    if (Mathf.Abs(direction.x) > maxComponent)
    {
        cardinalDirection = direction.x > 0 ? Vector3.right : Vector3.left;
        maxComponent = Mathf.Abs(direction.x);
    }
    
    if (Mathf.Abs(direction.z) > maxComponent)
    {
        cardinalDirection = direction.z > 0 ? Vector3.forward : Vector3.back;
    }
    
    SetGravityDirection(cardinalDirection);
}

// VALIDATION: Ensure values are reasonable
private void OnValidate()
{
    // Ensure speed limits are ordered correctly
    if (inputSpeedLimit > physicsSpeedLimit)
        physicsSpeedLimit = inputSpeedLimit + 0.5f;
        
    if (physicsSpeedLimit > totalSpeedLimit)
        totalSpeedLimit = physicsSpeedLimit + 0.5f;
        
    // Ensure jump distance is achievable with jump height
    float theoreticalMaxDistance = 2f * Mathf.Sqrt(jumpHeight * gravityStrength);
    if (maxJumpDistance > theoreticalMaxDistance)
    {
        Debug.LogWarning($"Jump distance {maxJumpDistance} may be unreachable with jump height {jumpHeight}. Theoretical max: {theoreticalMaxDistance:F2}");
    }
}
```

## Validation Steps
1. **Function Output Check**:
   - Action: Test `GetJumpForce(1f)` with `jumpHeight = 0.75f` and `gravityStrength = 9.81f`.
   - Expected Outcome: Returns approximately `3.84f` (based on `F = m * sqrt(2gh)`).
2. **Speed Decay Behavior**:
   - Action: Test `ApplySpeedDecay(6.8f, 7f)` with `totalSpeedLimit = 7f`.
   - Expected Outcome: Returns a value less than `6.8f`, showing decay near limit.
3. **OnValidate Correction**:
   - Action: Set `inputSpeedLimit = 8f`, `physicsSpeedLimit = 6f` in Inspector.
   - Expected Outcome: `physicsSpeedLimit` auto-corrects to `8.5f` on validation.

## Error Handling
- **If Functions Return Unexpected Values**: Verify math formulas (e.g., `GetJumpForce` uses `sqrt(2gh)`). Check for typos in variable names.
- **If OnValidate Warnings Don't Appear**: Ensure `Debug.LogWarning` is not suppressed in Unity console. Test with extreme values to trigger warnings.
