// File: Assets/Scripts/Settings/PhysicsSettings.cs
// Purpose: Centralized configuration for physics parameters with migration support
using UnityEngine;

[CreateAssetMenu(fileName = "PhysicsSettings", menuName = "BlockBall/Physics Settings")]
public class PhysicsSettings : ScriptableObject
{
    [Header("Migration Settings")]
    public PhysicsMode physicsMode = PhysicsMode.UnityPhysics;
    public bool enableMigrationLogging = true;
    public bool validateParameterConversion = true;
    
    [Header("Legacy Unity Physics Parameters")]
    [Tooltip("Current jump force used in PlayerCameraController")]
    [Range(1f, 10f)]
    public float legacyJumpForce = 5.0f;
    
    [Tooltip("Current speed factor from PlayerCameraController")]
    [Range(0.1f, 5f)]
    public float legacySpeedFactor = 1.0f;
    
    [Tooltip("Current break factor from PlayerCameraController")]
    [Range(1f, 20f)]
    public float legacyBreakFactor = 10.0f;
    
    [Header("Target Custom Physics Parameters")]
    [Tooltip("Target jump height in Unity units (6 Bixels)")]
    [Range(0.5f, 1.5f)]
    public float targetJumpHeight = 0.75f;
    
    [Tooltip("Maximum input speed limit")]
    [Range(4f, 10f)]
    public float inputSpeedLimit = 6.0f;
    
    [Tooltip("Physics calculation speed limit")]
    [Range(5f, 12f)]
    public float physicsSpeedLimit = 6.5f;
    
    [Tooltip("Absolute maximum speed")]
    [Range(6f, 15f)]
    public float totalSpeedLimit = 7.0f;
    
    [Header("Deterministic Math Settings")]
    [Tooltip("Fixed-point scale for critical calculations")]
    public int fixedPointScale = 1000000;
    
    [Tooltip("Error accumulation threshold")]
    [Range(100f, 10000f)]
    public float accumulationThreshold = 1000f;
    
    [Header("Performance Targets")]
    [Tooltip("Maximum physics update time in milliseconds")]
    [Range(0.5f, 5f)]
    public float maxPhysicsUpdateMs = 2.0f;
    
    [Tooltip("Target physics update frequency")]
    [Range(30, 120)]
    public int physicsUpdateHz = 50;
    
    // Parameter conversion utilities
    public float ConvertJumpForceToHeight()
    {
        // Empirical conversion formula (to be calibrated)
        float gravity = Physics.gravity.magnitude;
        return (legacyJumpForce * legacyJumpForce) / (2 * gravity);
    }
    
    public float ConvertSpeedFactorToLimit()
    {
        // Convert AddForce factor to speed limit (to be calibrated)
        return legacySpeedFactor * 6.0f; // Base conversion
    }
    
    public void ValidateSettings()
    {
        // Ensure parameter consistency
        if (physicsSpeedLimit <= inputSpeedLimit)
            physicsSpeedLimit = inputSpeedLimit + 0.5f;
        if (totalSpeedLimit <= physicsSpeedLimit)
            totalSpeedLimit = physicsSpeedLimit + 0.5f;
    }
}

public enum PhysicsMode
{
    UnityPhysics,      // Current system (fallback)
    HybridPhysics,     // Partial custom implementation
    CustomPhysics,     // Full custom system
    ValidationMode     // Side-by-side comparison
}
