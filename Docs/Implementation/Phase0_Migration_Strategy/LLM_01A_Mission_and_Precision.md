---
title: "Phase 0: Mission and Float Precision Strategy (LLM)"
phase: "0"
context_required: ["FloatPrecision", "Determinism"]
validation_steps:
  - "Confirm float32 decision rationale is understood"
  - "Verify deterministic math approach is clear"
priority: 1
---

# Phase 0: Mission and Float Precision Strategy (LLM)

## Mission Statement
Create a safe, incremental migration path from Unity's built-in physics to the custom BlockBall physics system, ensuring backwards compatibility and providing rollback capabilities throughout the transition.

**Directive for LLM**: Understand the goal of this migration as a non-disruptive transition to a custom physics system. All actions must preserve existing functionality unless explicitly instructed for replacement.

## Float Precision Decision: Use float32 with Enhanced Determinism

### Rationale for float32 (not float64)
- **Unity Compatibility**: Unity engine uses float32 throughout (Vector3, Transform, etc.). Using float64 would require constant conversion, leading to performance overhead.
- **Performance**: No conversion overhead between Unity's float32 and custom systems.
- **Memory Efficiency**: Half the memory usage of float64, critical for mobile and console targets.
- **Sufficient Precision**: BlockBall's scale and gameplay requirements do not necessitate float64 precision.

**Directive for LLM**: When implementing physics calculations, always use float32 unless a specific exception is documented. Avoid any assumptions about precision needs beyond Unity's default.

### Enhanced Determinism Strategy
**Purpose**: Ensure consistent physics behavior across platforms without float64.
**Approach**: Use fixed-point arithmetic for critical calculations, consistent math functions, and periodic normalization to prevent error accumulation.

```csharp
// File: Assets/Scripts/Physics/DeterministicMath.cs
// Purpose: Provide platform-consistent math operations for physics
public static class DeterministicMath
{
    // Fixed-point arithmetic for critical calculations
    private const int FIXED_SCALE = 1000000; // 6 decimal places
    
    public static int ToFixed(float value) => (int)(value * FIXED_SCALE);
    public static float FromFixed(int value) => (float)value / FIXED_SCALE;
    
    // Consistent math functions across platforms
    public static float DeterministicSqrt(float value) => (float)Math.Sqrt((double)value);
    
    // Periodic normalization to prevent error accumulation
    public static Vector3 NormalizeAccumulation(Vector3 vector, float threshold = 1000f)
    {
        return vector.magnitude > threshold ? vector.normalized * threshold : vector;
    }
}
```

**Directive for LLM**: Implement `DeterministicMath` as shown above in the specified file path when tasked with physics calculations. Use this class for all math operations to ensure determinism. Validate implementation by checking for consistent results in test cases across different platforms.

## Validation for LLM
- **Understanding Check**: Confirm that float32 is chosen for Unity compatibility and performance, not precision superiority.
- **Implementation Readiness**: Ensure the `DeterministicMath` class structure is clear and can be implemented as a single utility class.
- **Error Prevention**: Note that any deviation to float64 must be explicitly justified and documented in future tasks.

**Next Step**: Proceed to `LLM_01B_Migration_Phases.md` for an overview of the migration phases after confirming understanding of this document.
