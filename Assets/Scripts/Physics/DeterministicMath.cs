// File: Assets/Scripts/Physics/DeterministicMath.cs
// Purpose: Provides deterministic math operations for consistent physics calculations
using UnityEngine;
using System;

namespace BlockBall.Physics
{
    public static class DeterministicMath
    {
        // Constants for precision control
        private const float PrecisionFactor = 10000f;
        
        // Deterministic absolute value
        public static float Abs(float value)
        {
            return Math.Abs(value);
        }
        
        // Deterministic square root with controlled precision
        public static float Sqrt(float value)
        {
            if (value < 0)
                return 0f; // Avoid NaN in case of negative input
            return (float)Math.Sqrt(value);
        }
        
        // Deterministic rounding to control floating point precision
        public static float Round(float value)
        {
            return (float)Math.Round(value * PrecisionFactor) / PrecisionFactor;
        }
        
        // Deterministic vector rounding
        public static Vector3 RoundVector(Vector3 vector)
        {
            return new Vector3(
                Round(vector.x),
                Round(vector.y),
                Round(vector.z)
            );
        }
        
        // Deterministic vector addition
        public static Vector3 Add(Vector3 a, Vector3 b)
        {
            return new Vector3(
                Round(a.x + b.x),
                Round(a.y + b.y),
                Round(a.z + b.z)
            );
        }
        
        // Deterministic vector subtraction
        public static Vector3 Subtract(Vector3 a, Vector3 b)
        {
            return new Vector3(
                Round(a.x - b.x),
                Round(a.y - b.y),
                Round(a.z - b.z)
            );
        }
        
        // Deterministic vector multiplication by scalar
        public static Vector3 Multiply(Vector3 vector, float scalar)
        {
            return new Vector3(
                Round(vector.x * scalar),
                Round(vector.y * scalar),
                Round(vector.z * scalar)
            );
        }
        
        // Deterministic dot product
        public static float Dot(Vector3 a, Vector3 b)
        {
            return Round(a.x * b.x + a.y * b.y + a.z * b.z);
        }
        
        // Deterministic cross product
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(
                Round(a.y * b.z - a.z * b.y),
                Round(a.z * b.x - a.x * b.z),
                Round(a.x * b.y - a.y * b.x)
            );
        }
        
        // Deterministic vector magnitude
        public static float Magnitude(Vector3 vector)
        {
            return Sqrt(Dot(vector, vector));
        }
        
        // Deterministic vector normalization
        public static Vector3 Normalize(Vector3 vector)
        {
            float mag = Magnitude(vector);
            if (mag > 0)
            {
                return new Vector3(
                    Round(vector.x / mag),
                    Round(vector.y / mag),
                    Round(vector.z / mag)
                );
            }
            return Vector3.zero;
        }
        
        // Deterministic linear interpolation for floats
        public static float Lerp(float a, float b, float t)
        {
            t = Mathf.Clamp01(t);
            return Round(a + t * (b - a));
        }
        
        // Deterministic linear interpolation for vectors
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector3(
                Lerp(a.x, b.x, t),
                Lerp(a.y, b.y, t),
                Lerp(a.z, b.z, t)
            );
        }
        
        // Deterministic time delta to ensure consistent physics steps
        public static float DeltaTime
        {
            get
            {
                // Use fixed delta time for physics to ensure consistency
                return Round(Time.fixedDeltaTime);
            }
        }
    }
}
