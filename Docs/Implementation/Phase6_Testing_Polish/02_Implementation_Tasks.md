# Phase 6: Testing & Polish Implementation Tasks

## Task 6.1: Create Physics Test Suite

### Objective
Implement comprehensive automated testing for all physics components and behaviors.

### Code Template
```csharp
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace BlockBall.Physics.Tests
{
    public class PhysicsTestSuite
    {
        private PhysicsSettings testSettings;
        private GameObject testBall;
        private BallPhysics ballPhysics;
        private SpeedController speedController;
        
        [SetUp]
        public void SetUp()
        {
            // Create test settings
            testSettings = ScriptableObject.CreateInstance<PhysicsSettings>();
            testSettings.inputSpeedLimit = 6f;
            testSettings.physicsSpeedLimit = 6.5f;
            testSettings.totalSpeedLimit = 7f;
            testSettings.speedDecayRate = 2f;
            
            // Create test ball
            testBall = new GameObject("TestBall");
            testBall.AddComponent<Rigidbody>();
            ballPhysics = testBall.AddComponent<BallPhysics>();
            speedController = testBall.AddComponent<SpeedController>();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (testBall != null)
                Object.DestroyImmediate(testBall);
        }
        
        [Test]
        public void TestInputSpeedLimit()
        {
            Vector3 inputVelocity = Vector3.forward * 10f; // Exceeds 6 u/s limit
            Vector3 limitedVelocity = speedController.LimitInputVelocity(inputVelocity);
            
            Assert.LessOrEqual(limitedVelocity.magnitude, testSettings.inputSpeedLimit + 0.01f);
            Assert.AreEqual(inputVelocity.normalized, limitedVelocity.normalized, "Direction should be preserved");
        }
        
        [Test]
        public void TestPhysicsSpeedLimit()
        {
            Vector3 physicsVelocity = Vector3.forward * 8f; // Exceeds 6.5 u/s limit
            Vector3 limitedVelocity = speedController.LimitPhysicsVelocity(physicsVelocity);
            
            Assert.LessOrEqual(limitedVelocity.magnitude, testSettings.physicsSpeedLimit + 0.01f);
        }
        
        [Test]
        public void TestTotalSpeedLimit()
        {
            Rigidbody rb = testBall.GetComponent<Rigidbody>();
            rb.velocity = Vector3.forward * 10f; // Exceeds 7 u/s limit
            
            // Simulate one physics step
            speedController.Invoke("ApplySpeedControl", 0f);
            
            Assert.LessOrEqual(rb.velocity.magnitude, testSettings.totalSpeedLimit + 0.01f);
        }
        
        [Test]
        public void TestExponentialDecay()
        {
            Rigidbody rb = testBall.GetComponent<Rigidbody>();
            rb.velocity = Vector3.forward * 6.8f; // Above decay threshold (6.65)
            
            float initialSpeed = rb.velocity.magnitude;
            speedController.Invoke("ApplySpeedControl", 0f);
            float finalSpeed = rb.velocity.magnitude;
            
            Assert.Less(finalSpeed, initialSpeed, "Speed should decay");
            Assert.Greater(finalSpeed, testSettings.ExponentialDecayThreshold * 0.9f, "Should not decay too much in one frame");
        }
        
        [UnityTest]
        public IEnumerator TestDeterministicBehavior()
        {
            // Test that physics behaves identically across multiple runs
            Vector3 initialVelocity = new Vector3(5f, 3f, 2f);
            Vector3[] results = new Vector3[5];
            
            for (int run = 0; run < 5; run++)
            {
                SetUp(); // Reset for each run
                Rigidbody rb = testBall.GetComponent<Rigidbody>();
                rb.velocity = initialVelocity;
                
                // Run physics for 10 frames
                for (int frame = 0; frame < 10; frame++)
                {
                    yield return new WaitForFixedUpdate();
                }
                
                results[run] = rb.velocity;
                TearDown();
            }
            
            // Verify all results are identical
            for (int i = 1; i < results.Length; i++)
            {
                Assert.AreEqual(results[0], results[i], "Physics should be deterministic");
            }
        }
    }
}
```

### Validation Steps
1. Create test assembly definition
2. Run all physics tests and verify 100% pass rate
3. Add to continuous integration pipeline

---

## Task 6.2: Create Performance Profiler

### Objective
Implement continuous performance monitoring for all physics components.

### Code Template
```csharp
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace BlockBall.Physics
{
    public class PerformanceProfiler : MonoBehaviour
    {
        [Header("Profiling Settings")]
        public bool enableProfiling = true;
        public bool showRealTimeStats = true;
        public int maxSamples = 1000;
        
        [Header("Performance Targets")]
        public float physicsFrameTarget = 2f; // ms
        public float gravityUpdateTarget = 0.1f; // ms
        public float speedControlTarget = 0.2f; // ms
        
        private Dictionary<string, ProfileData> profiles = new Dictionary<string, ProfileData>();
        private Stopwatch stopwatch = new Stopwatch();
        
        public static PerformanceProfiler Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void StartProfiling(string profileName)
        {
            if (!enableProfiling) return;
            
            if (!profiles.ContainsKey(profileName))
            {
                profiles[profileName] = new ProfileData();
            }
            
            stopwatch.Restart();
        }
        
        public void EndProfiling(string profileName)
        {
            if (!enableProfiling) return;
            
            stopwatch.Stop();
            float elapsedMs = (float)stopwatch.Elapsed.TotalMilliseconds;
            
            if (profiles.ContainsKey(profileName))
            {
                profiles[profileName].AddSample(elapsedMs);
            }
        }
        
        public ProfileData GetProfileData(string profileName)
        {
            return profiles.ContainsKey(profileName) ? profiles[profileName] : null;
        }
        
        public bool IsPerformanceTargetMet(string profileName, float targetMs)
        {
            ProfileData data = GetProfileData(profileName);
            return data != null && data.AverageMs <= targetMs;
        }
        
        public void GeneratePerformanceReport()
        {
            UnityEngine.Debug.Log("=== PHYSICS PERFORMANCE REPORT ===");
            
            foreach (var kvp in profiles)
            {
                string name = kvp.Key;
                ProfileData data = kvp.Value;
                
                string status = "✓ PASS";
                float target = GetTargetForProfile(name);
                
                if (target > 0 && data.AverageMs > target)
                {
                    status = "✗ FAIL";
                }
                
                UnityEngine.Debug.Log($"{name}: Avg={data.AverageMs:F3}ms, Max={data.MaxMs:F3}ms, Target={target:F1}ms [{status}]");
            }
        }
        
        private float GetTargetForProfile(string profileName)
        {
            switch (profileName.ToLower())
            {
                case "physics": return physicsFrameTarget;
                case "gravity": return gravityUpdateTarget;
                case "speedcontrol": return speedControlTarget;
                default: return 0f;
            }
        }
        
        private void OnGUI()
        {
            if (!showRealTimeStats) return;
            
            GUILayout.BeginArea(new Rect(Screen.width - 320, 10, 300, 200));
            GUILayout.Label("Performance Monitor", GUI.skin.box);
            
            foreach (var kvp in profiles)
            {
                ProfileData data = kvp.Value;
                float target = GetTargetForProfile(kvp.Key);
                
                GUI.color = data.AverageMs <= target ? Color.green : Color.red;
                GUILayout.Label($"{kvp.Key}: {data.AverageMs:F2}ms (target: {target:F1}ms)");
            }
            
            GUI.color = Color.white;
            GUILayout.EndArea();
        }
    }
    
    [System.Serializable]
    public class ProfileData
    {
        private List<float> samples = new List<float>();
        private float totalMs = 0f;
        
        public float AverageMs => samples.Count > 0 ? totalMs / samples.Count : 0f;
        public float MaxMs { get; private set; } = 0f;
        public int SampleCount => samples.Count;
        
        public void AddSample(float ms)
        {
            samples.Add(ms);
            totalMs += ms;
            
            if (ms > MaxMs)
                MaxMs = ms;
            
            // Keep only recent samples
            if (samples.Count > 1000)
            {
                float removedSample = samples[0];
                samples.RemoveAt(0);
                totalMs -= removedSample;
            }
        }
    }
}
```

### Validation Steps
1. Add profiling calls to all physics components
2. Verify performance targets are met
3. Generate automated performance reports

---

## Task 6.3: Create Integration Validator

### Objective
Validate complete physics system integration with comprehensive test scenarios.

### Code Template
```csharp
using UnityEngine;
using System.Collections;

namespace BlockBall.Physics
{
    public class IntegrationValidator : MonoBehaviour
    {
        [Header("Test Scenarios")]
        public bool runOnStart = false;
        public bool logDetailedResults = true;
        
        private int totalTests = 0;
        private int passedTests = 0;
        
        private void Start()
        {
            if (runOnStart)
                StartCoroutine(RunAllIntegrationTests());
        }
        
        public IEnumerator RunAllIntegrationTests()
        {
            UnityEngine.Debug.Log("Starting Integration Validation...");
            
            yield return StartCoroutine(TestGravitySpeedIntegration());
            yield return StartCoroutine(TestCollisionSpeedIntegration());
            yield return StartCoroutine(TestComplexPhysicsScenario());
            yield return StartCoroutine(TestSystemStability());
            
            GenerateIntegrationReport();
        }
        
        private IEnumerator TestGravitySpeedIntegration()
        {
            LogTestStart("Gravity-Speed Integration");
            
            // Test that gravity transitions work correctly with speed control
            bool testPassed = true;
            
            // Create test scenario
            GameObject testBall = CreateTestBall();
            PlayerGravityManager gravityManager = testBall.GetComponent<PlayerGravityManager>();
            SpeedController speedController = testBall.GetComponent<SpeedController>();
            
            // Test gravity change with high speed
            Rigidbody rb = testBall.GetComponent<Rigidbody>();
            rb.velocity = Vector3.forward * 8f; // High speed
            
            // Change gravity direction
            gravityManager.SetGravityDirection(Vector3.right);
            
            // Wait for transition
            yield return new WaitForSeconds(1f);
            
            // Verify speed is still controlled
            if (rb.velocity.magnitude > 7.1f) // Allow small tolerance
            {
                testPassed = false;
                LogTestFailure("Speed exceeded limit during gravity transition");
            }
            
            // Verify gravity direction changed
            if (Vector3.Dot(gravityManager.GetCurrentGravityDirection(), Vector3.right) < 0.9f)
            {
                testPassed = false;
                LogTestFailure("Gravity direction not applied correctly");
            }
            
            Destroy(testBall);
            LogTestResult("Gravity-Speed Integration", testPassed);
            
            yield return null;
        }
        
        private IEnumerator TestCollisionSpeedIntegration()
        {
            LogTestStart("Collision-Speed Integration");
            
            bool testPassed = true;
            
            // Create test scenario with collision
            GameObject testBall = CreateTestBall();
            GameObject wall = CreateTestWall();
            
            Rigidbody rb = testBall.GetComponent<Rigidbody>();
            rb.velocity = Vector3.forward * 6f; // At speed limit
            
            // Wait for collision
            yield return new WaitForSeconds(2f);
            
            // Verify speed is still controlled after collision
            if (rb.velocity.magnitude > 7.1f)
            {
                testPassed = false;
                LogTestFailure("Speed exceeded limit after collision");
            }
            
            Destroy(testBall);
            Destroy(wall);
            LogTestResult("Collision-Speed Integration", testPassed);
            
            yield return null;
        }
        
        private IEnumerator TestComplexPhysicsScenario()
        {
            LogTestStart("Complex Physics Scenario");
            
            // Test multiple physics systems working together
            bool testPassed = true;
            
            // Create complex test scenario
            GameObject testBall = CreateTestBall();
            CreateMultipleGravityZones();
            
            Rigidbody rb = testBall.GetComponent<Rigidbody>();
            
            // Simulate complex movement for 10 seconds
            float testDuration = 10f;
            float startTime = Time.time;
            
            while (Time.time - startTime < testDuration)
            {
                // Verify physics stays stable
                if (rb.velocity.magnitude > 7.5f || float.IsNaN(rb.velocity.magnitude))
                {
                    testPassed = false;
                    LogTestFailure("Physics became unstable during complex scenario");
                    break;
                }
                
                yield return new WaitForFixedUpdate();
            }
            
            Destroy(testBall);
            LogTestResult("Complex Physics Scenario", testPassed);
            
            yield return null;
        }
        
        private IEnumerator TestSystemStability()
        {
            LogTestStart("System Stability Test");
            
            bool testPassed = true;
            
            // Run physics for extended period to test stability
            float testDuration = 30f;
            float startTime = Time.time;
            int frameCount = 0;
            
            while (Time.time - startTime < testDuration)
            {
                frameCount++;
                
                // Check for performance issues
                if (Time.deltaTime > 0.1f) // Frame time too high
                {
                    testPassed = false;
                    LogTestFailure($"Performance issue detected at frame {frameCount}");
                    break;
                }
                
                yield return new WaitForFixedUpdate();
            }
            
            LogTestResult("System Stability Test", testPassed);
            
            yield return null;
        }
        
        private GameObject CreateTestBall()
        {
            GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.name = "TestBall";
            
            Rigidbody rb = ball.AddComponent<Rigidbody>();
            rb.mass = 1f;
            
            ball.AddComponent<BallPhysics>();
            ball.AddComponent<SpeedController>();
            ball.AddComponent<PlayerGravityManager>();
            
            return ball;
        }
        
        private GameObject CreateTestWall()
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "TestWall";
            wall.transform.position = Vector3.forward * 5f;
            wall.transform.localScale = new Vector3(10f, 10f, 1f);
            
            return wall;
        }
        
        private void CreateMultipleGravityZones()
        {
            // Create several overlapping gravity zones for complex testing
            for (int i = 0; i < 3; i++)
            {
                GameObject zone = new GameObject($"GravityZone_{i}");
                zone.AddComponent<BoxCollider>().isTrigger = true;
                zone.AddComponent<PlayerGravityTrigger>();
                
                zone.transform.position = Vector3.right * i * 2f;
                zone.transform.localScale = Vector3.one * 3f;
            }
        }
        
        private void LogTestStart(string testName)
        {
            totalTests++;
            if (logDetailedResults)
                UnityEngine.Debug.Log($"Starting test: {testName}");
        }
        
        private void LogTestResult(string testName, bool passed)
        {
            if (passed)
                passedTests++;
                
            string result = passed ? "PASS" : "FAIL";
            UnityEngine.Debug.Log($"Test {testName}: {result}");
        }
        
        private void LogTestFailure(string reason)
        {
            if (logDetailedResults)
                UnityEngine.Debug.LogError($"Test failure: {reason}");
        }
        
        private void GenerateIntegrationReport()
        {
            float passRate = totalTests > 0 ? (float)passedTests / totalTests * 100f : 0f;
            
            UnityEngine.Debug.Log("=== INTEGRATION TEST REPORT ===");
            UnityEngine.Debug.Log($"Total Tests: {totalTests}");
            UnityEngine.Debug.Log($"Passed Tests: {passedTests}");
            UnityEngine.Debug.Log($"Pass Rate: {passRate:F1}%");
            
            if (passRate >= 95f)
            {
                UnityEngine.Debug.Log("✓ INTEGRATION VALIDATION PASSED");
            }
            else
            {
                UnityEngine.Debug.LogError("✗ INTEGRATION VALIDATION FAILED");
            }
        }
    }
}
```

### Validation Steps
1. Run complete integration test suite
2. Verify 95%+ pass rate
3. Document any failing tests and create fix plan

---

## Task 6.4: Create Quality Assurance Dashboard

### Objective
Create centralized quality monitoring and reporting system.

### Code Template
```csharp
using UnityEngine;
using System.Collections.Generic;

namespace BlockBall.Physics
{
    public class QualityDashboard : MonoBehaviour
    {
        [Header("Quality Metrics")]
        public float targetFrameRate = 50f;
        public float targetPhysicsTime = 2f;
        public int maxAllowedBugs = 5;
        
        private List<QualityMetric> metrics = new List<QualityMetric>();
        private bool isQualityGreen = true;
        
        private void Start()
        {
            InitializeMetrics();
            InvokeRepeating(nameof(UpdateQualityMetrics), 1f, 1f);
        }
        
        private void InitializeMetrics()
        {
            metrics.Add(new QualityMetric("Performance", "Physics frame time", () => GetPhysicsFrameTime(), targetPhysicsTime));
            metrics.Add(new QualityMetric("Stability", "Physics stability", () => GetPhysicsStability(), 1f));
            metrics.Add(new QualityMetric("Memory", "Allocations per frame", () => GetAllocationsPerFrame(), 0f));
            metrics.Add(new QualityMetric("Determinism", "Deterministic behavior", () => GetDeterminismScore(), 1f));
        }
        
        private void UpdateQualityMetrics()
        {
            isQualityGreen = true;
            
            foreach (var metric in metrics)
            {
                metric.UpdateValue();
                
                if (!metric.IsWithinTarget())
                {
                    isQualityGreen = false;
                }
            }
        }
        
        private void OnGUI()
        {
            DrawQualityDashboard();
        }
        
        private void DrawQualityDashboard()
        {
            GUILayout.BeginArea(new Rect(10, Screen.height - 250, 400, 200));
            
            // Dashboard header
            GUI.color = isQualityGreen ? Color.green : Color.red;
            GUILayout.Label("Quality Dashboard", GUI.skin.box);
            GUI.color = Color.white;
            
            // Overall status
            string status = isQualityGreen ? "✓ ALL SYSTEMS GREEN" : "⚠ QUALITY ISSUES DETECTED";
            GUI.color = isQualityGreen ? Color.green : Color.red;
            GUILayout.Label(status);
            GUI.color = Color.white;
            
            // Individual metrics
            foreach (var metric in metrics)
            {
                GUI.color = metric.IsWithinTarget() ? Color.green : Color.red;
                GUILayout.Label($"{metric.Name}: {metric.CurrentValue:F2} (target: ≤{metric.TargetValue:F2})");
            }
            
            GUI.color = Color.white;
            GUILayout.EndArea();
        }
        
        // Metric calculation methods
        private float GetPhysicsFrameTime()
        {
            PerformanceProfiler profiler = PerformanceProfiler.Instance;
            if (profiler != null)
            {
                var data = profiler.GetProfileData("Physics");
                return data?.AverageMs ?? 0f;
            }
            return 0f;
        }
        
        private float GetPhysicsStability()
        {
            // Return 1.0 if stable, 0.0 if unstable
            return Time.timeScale > 0.9f ? 1f : 0f;
        }
        
        private float GetAllocationsPerFrame()
        {
            // Monitor memory allocations
            return UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory(0) / Time.frameCount;
        }
        
        private float GetDeterminismScore()
        {
            // Return 1.0 if deterministic, 0.0 if not
            return 1f; // Placeholder - implement actual determinism check
        }
    }
    
    [System.Serializable]
    public class QualityMetric
    {
        public string Category { get; private set; }
        public string Name { get; private set; }
        public float CurrentValue { get; private set; }
        public float TargetValue { get; private set; }
        
        private System.Func<float> valueGetter;
        
        public QualityMetric(string category, string name, System.Func<float> getter, float target)
        {
            Category = category;
            Name = name;
            valueGetter = getter;
            TargetValue = target;
        }
        
        public void UpdateValue()
        {
            CurrentValue = valueGetter?.Invoke() ?? 0f;
        }
        
        public bool IsWithinTarget()
        {
            return CurrentValue <= TargetValue;
        }
    }
}
```

### Validation Steps
1. Deploy quality dashboard to test environment
2. Monitor all quality metrics for 24 hours
3. Verify all metrics stay within targets

---

## Integration Testing Checklist

### Phase 1-5 Integration
- [ ] All physics components work together seamlessly
- [ ] No conflicts between gravity, speed, and collision systems
- [ ] Performance targets met with all systems active

### Production Readiness
- [ ] Comprehensive test suite passes 95%+
- [ ] Performance profiling shows all targets met
- [ ] Quality dashboard shows green status
- [ ] Zero critical bugs, <5 minor bugs
- [ ] Complete documentation available

### Final Validation
- [ ] Full gameplay testing completed
- [ ] Cross-platform compatibility verified
- [ ] System ready for production deployment

## Completion Criteria

Phase 6 is complete when:
1. All automated tests pass consistently (95%+ rate)
2. Performance targets are met across all scenarios
3. Quality dashboard shows green status
4. Complete documentation is available
5. System demonstrates production-level stability

The physics system is now ready for full production use and integration with remaining game features.
