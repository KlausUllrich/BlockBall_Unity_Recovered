// File: Assets/Scripts/Physics/PhysicsProfiler.cs
// Purpose: Profiles physics performance to monitor impact during migration
using UnityEngine;
using System.Diagnostics;
using System.Text;

namespace BlockBall.Physics
{
    public class PhysicsProfiler : MonoBehaviour
    {
        // Singleton instance
        private static PhysicsProfiler instance;
        
        // Public accessor for singleton instance
        public static PhysicsProfiler Instance
        {
            get { return instance; }
        }
        
        // Performance metrics
        private float physicsUpdateTime = 0f;
        private int physicsUpdateCount = 0;
        private float averagePhysicsTime = 0f;
        private float peakPhysicsTime = 0f;
        
        // Stopwatch for precise timing
        private Stopwatch stopwatch = new Stopwatch();
        
        // Logging interval (seconds)
        public float logInterval = 5f;
        private float timeSinceLastLog = 0f;
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void FixedUpdate()
        {
            // Start timing
            stopwatch.Restart();
            
            // The actual physics update is handled by Unity, we just measure it
            // In later phases, custom physics updates will be profiled here
            
            // Stop timing
            stopwatch.Stop();
            float elapsedMilliseconds = stopwatch.ElapsedTicks / (float)Stopwatch.Frequency * 1000f;
            
            // Update metrics
            physicsUpdateTime += elapsedMilliseconds;
            physicsUpdateCount++;
            peakPhysicsTime = Mathf.Max(peakPhysicsTime, elapsedMilliseconds);
            
            // Log performance periodically
            timeSinceLastLog += Time.fixedDeltaTime;
            if (timeSinceLastLog >= logInterval)
            {
                UpdateAverage();
                LogPerformance();
                ResetMetrics();
                timeSinceLastLog = 0f;
            }
        }
        
        private void UpdateAverage()
        {
            if (physicsUpdateCount > 0)
            {
                averagePhysicsTime = physicsUpdateTime / physicsUpdateCount;
            }
            else
            {
                averagePhysicsTime = 0f;
            }
        }
        
        private void LogPerformance()
        {
            StringBuilder log = new StringBuilder("Physics Performance Metrics:");
            log.AppendLine();
            log.AppendLine($"Interval: {logInterval} seconds");
            log.AppendLine($"Updates: {physicsUpdateCount}");
            log.AppendLine($"Average Update Time: {averagePhysicsTime:F4} ms");
            log.AppendLine($"Peak Update Time: {peakPhysicsTime:F4} ms");
            log.AppendLine($"Total Physics Time: {physicsUpdateTime:F4} ms");
            
            UnityEngine.Debug.Log(log.ToString());
        }
        
        private void ResetMetrics()
        {
            physicsUpdateTime = 0f;
            physicsUpdateCount = 0;
            peakPhysicsTime = 0f;
        }
        
        // Public method to get current average for potential UI display or external access
        public float GetAveragePhysicsTime()
        {
            UpdateAverage();
            return averagePhysicsTime;
        }
        
        // Public method to get peak time
        public float GetPeakPhysicsTime()
        {
            return peakPhysicsTime;
        }
    }
}
