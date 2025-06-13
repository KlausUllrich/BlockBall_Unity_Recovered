// BallDebugVisualizer.cs - Handles debug visualization and gizmos
// Part of BlockBall Evolution Core Physics Architecture
// Created: 2025-06-12 by refactoring BallPhysics.cs

using UnityEngine;
using BlockBall.Settings;

namespace BlockBall.Physics.Components
{
    /// <summary>
    /// Provides debug visualization tools for the ball physics system.
    /// Draws gizmos for velocity, acceleration, ground detection, and state visualization.
    /// </summary>
    public class BallDebugVisualizer
    {
        // Configuration
        private PhysicsSettings settings;
        private Transform transform;
        private BallStateManager stateManager;
        private BallGroundDetector groundDetector;
        private BallForceCalculator forceCalculator;
        private BallPhysics ballPhysics;
        
        // Visualization settings
        private bool showVelocity = true;
        private bool showAcceleration = true;
        private bool showGroundCheck = true;
        private bool showStateColor = true;
        
        public BallDebugVisualizer(Transform ballTransform, PhysicsSettings physicsSettings,
                                   BallStateManager states, BallGroundDetector ground, BallForceCalculator forces,
                                   BallPhysics physics)
        {
            transform = ballTransform;
            settings = physicsSettings;
            stateManager = states;
            groundDetector = ground;
            forceCalculator = forces;
            ballPhysics = physics;
        }
        
        /// <summary>
        /// Draw all debug gizmos
        /// </summary>
        public void DrawGizmos(Vector3 velocity, Vector3 acceleration)
        {
            if (!Application.isPlaying) return;
            
            Vector3 position = transform.position;
            
            // Draw velocity vector
            if (showVelocity && velocity.magnitude > 0.1f)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(position, position + velocity);
                Gizmos.DrawSphere(position + velocity, 0.1f);
            }
            
            // Draw acceleration vector
            if (showAcceleration && acceleration.magnitude > 0.1f)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(position, position + acceleration);
                Gizmos.DrawCube(position + acceleration, Vector3.one * 0.1f);
            }
            
            // Draw ground check visualization
            if (showGroundCheck)
            {
                groundDetector.DrawDebugGizmos();
            }
            
            // Draw state color sphere
            if (showStateColor)
            {
                Gizmos.color = stateManager.GetDebugColor();
                Gizmos.DrawWireSphere(position, 0.6f);
            }
            
            // Draw additional debug information
            DrawSpeedLimitVisualization(velocity);
            DrawEnergyVisualization();
        }
        
        /// <summary>
        /// Draw speed limit visualization
        /// </summary>
        private void DrawSpeedLimitVisualization(Vector3 velocity)
        {
            if (settings == null) return;
            
            Vector3 position = transform.position;
            float currentSpeed = velocity.magnitude;
            float maxSpeed = settings.maxTotalSpeed;
            
            // Draw speed percentage as colored ring
            float speedRatio = currentSpeed / maxSpeed;
            Color speedColor = Color.Lerp(Color.green, Color.red, speedRatio);
            
            Gizmos.color = speedColor;
            // Draw a ring to represent speed percentage
            for (int i = 0; i < 16; i++)
            {
                float angle1 = (i * 22.5f) * Mathf.Deg2Rad;
                float angle2 = ((i + 1) * 22.5f) * Mathf.Deg2Rad;
                
                Vector3 point1 = position + new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * 0.8f;
                Vector3 point2 = position + new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * 0.8f;
                
                if (speedRatio > (float)i / 16f)
                {
                    Gizmos.DrawLine(point1, point2);
                }
            }
        }
        
        /// <summary>
        /// Draw energy conservation visualization
        /// </summary>
        private void DrawEnergyVisualization()
        {
            if (ballPhysics.EnableEnergyConservation)
            {
                // This could show energy levels, conservation status, etc.
                Vector3 position = transform.position;
                
                // Draw energy indicator (placeholder)
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(position + Vector3.up * 2f, Vector3.one * 0.2f);
            }
        }
        
        /// <summary>
        /// Draw on-screen debug text (for GUI)
        /// </summary>
        public void DrawDebugGUI(Vector3 velocity, Vector3 position)
        {
            if (!Application.isPlaying) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"Ball Physics Debug", GUI.skin.box);
            GUILayout.Label($"State: {stateManager.CurrentState}");
            GUILayout.Label($"Velocity: {velocity:F2} ({velocity.magnitude:F2})");
            GUILayout.Label($"Position: {position:F2}");
            GUILayout.Label($"Grounded: {groundDetector.IsGrounded}");
            GUILayout.Label($"Ground Distance: {groundDetector.GroundDistance:F3}");
            GUILayout.Label($"State Timer: {stateManager.StateTimer:F2}s");
            
            if (settings != null)
            {
                float speedPercent = (velocity.magnitude / settings.maxTotalSpeed) * 100f;
                GUILayout.Label($"Speed: {speedPercent:F1}% of max");
            }
            
            GUILayout.EndArea();
        }
        
        /// <summary>
        /// Toggle visibility of specific debug elements
        /// </summary>
        public void SetDebugVisibility(bool velocity, bool acceleration, bool groundCheck, bool stateColor)
        {
            showVelocity = velocity;
            showAcceleration = acceleration;
            showGroundCheck = groundCheck;
            showStateColor = stateColor;
        }
        
        /// <summary>
        /// Log physics state information
        /// </summary>
        public void LogPhysicsState(Vector3 velocity, Vector3 acceleration)
        {
            UnityEngine.Debug.Log($"BallDebugVisualizer: State={stateManager.CurrentState}, " +
                                $"Vel={velocity:F2}({velocity.magnitude:F2}), " +
                                $"Acc={acceleration:F2}, " +
                                $"Grounded={groundDetector.IsGrounded}");
        }
        
        /// <summary>
        /// Draw performance metrics
        /// </summary>
        public void DrawPerformanceMetrics()
        {
            // This could show frame time, physics step time, etc.
            if (Application.isPlaying)
            {
                Vector3 position = transform.position + Vector3.up * 3f;
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(position, Vector3.one * 0.1f);
            }
        }
    }
}
