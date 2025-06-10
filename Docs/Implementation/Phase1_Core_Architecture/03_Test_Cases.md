# Phase 1: Test Cases & Validation

## Test Framework Setup

### Test Scene Creation
1. **Create Test Scene**: `Assets/Scenes/Physics_Tests/Phase1_Core_Tests.unity`
2. **Add Test Manager**: GameObject with `PhysicsTestManager` component
3. **Add Physics Manager**: GameObject with `BlockBallPhysicsManager` component

## Test Case 1: Fixed Timestep Validation

### Test Objective
Verify that physics runs at exactly 50Hz regardless of application framerate.

### Test Setup
```csharp
[Test]
public class FixedTimestepTest : MonoBehaviour
{
    [Header("Test Configuration")]
    public float TestDuration = 10f;
    public float ExpectedTimestep = 0.02f; // 50Hz
    public float TolerancePercent = 1f; // 1% tolerance
    
    private float lastPhysicsTime = 0f;
    private List<float> timestepMeasurements = new List<float>();
    private bool testActive = false;
    
    public void StartTest()
    {
        testActive = true;
        timestepMeasurements.Clear();
        lastPhysicsTime = Time.time;
        
        // Force different application framerates
        StartCoroutine(VaryFramerate());
        
        // End test after duration
        Invoke(nameof(EndTest), TestDuration);
    }
    
    private void FixedUpdate()
    {
        if (!testActive) return;
        
        float currentTime = Time.time;
        float actualTimestep = currentTime - lastPhysicsTime;
        
        if (lastPhysicsTime > 0f) // Skip first frame
        {
            timestepMeasurements.Add(actualTimestep);
        }
        
        lastPhysicsTime = currentTime;
    }
    
    private IEnumerator VaryFramerate()
    {
        // Test at 30fps
        Application.targetFrameRate = 30;
        yield return new WaitForSeconds(TestDuration / 3f);
        
        // Test at 60fps  
        Application.targetFrameRate = 60;
        yield return new WaitForSeconds(TestDuration / 3f);
        
        // Test at 120fps
        Application.targetFrameRate = 120;
        yield return new WaitForSeconds(TestDuration / 3f);
    }
    
    private void EndTest()
    {
        testActive = false;
        Application.targetFrameRate = -1; // Reset
        
        // Analyze results
        float averageTimestep = timestepMeasurements.Average();
        float variance = CalculateVariance(timestepMeasurements, averageTimestep);
        
        bool passed = Mathf.Abs(averageTimestep - ExpectedTimestep) < (ExpectedTimestep * TolerancePercent / 100f);
        
        Debug.Log($"Fixed Timestep Test Results:");
        Debug.Log($"Expected: {ExpectedTimestep:F4}s, Actual: {averageTimestep:F4}s");
        Debug.Log($"Variance: {variance:F6}");
        Debug.Log($"Test {(passed ? "PASSED" : "FAILED")}");
        
        // Log detailed results
        LogDetailedResults(averageTimestep, variance, passed);
    }
    
    private float CalculateVariance(List<float> values, float mean)
    {
        float variance = 0f;
        foreach (float value in values)
        {
            variance += Mathf.Pow(value - mean, 2);
        }
        return variance / values.Count;
    }
    
    private void LogDetailedResults(float avgTimestep, float variance, bool passed)
    {
        string result = $"TIMESTEP_TEST_RESULT,{avgTimestep:F6},{variance:F8},{passed}";
        Debug.Log(result);
        
        // Save to file for automated testing
        System.IO.File.AppendAllText("Physics_Test_Results.csv", result + "\n");
    }
}
```

### Success Criteria
- [ ] Average timestep within 1% of 0.02s
- [ ] Timestep variance < 0.0001s
- [ ] Consistent timing across different application framerates

---

## Test Case 2: Energy Conservation Test

### Test Objective
Verify Velocity Verlet integration maintains energy conservation within 1%.

### Test Setup
```csharp
[Test]
public class EnergyConservationTest : MonoBehaviour
{
    [Header("Test Configuration")]
    public float TestDuration = 10f;
    public float InitialHeight = 10f;
    public float EnergyTolerance = 1f; // 1% tolerance
    
    private TestPhysicsObject testBall;
    private float initialEnergy;
    private List<float> energyMeasurements = new List<float>();
    
    public void StartTest()
    {
        // Create test ball
        GameObject ballObj = new GameObject("TestBall");
        testBall = ballObj.AddComponent<TestPhysicsObject>();
        
        // Set initial conditions
        testBall.Position = Vector3.up * InitialHeight;
        testBall.Velocity = Vector3.zero;
        testBall.Mass = 1f;
        
        // Calculate initial energy
        initialEnergy = CalculateTotalEnergy(testBall);
        energyMeasurements.Clear();
        
        // Register with physics manager
        BlockBallPhysicsManager.Instance.RegisterPhysicsObject(testBall);
        
        // Start monitoring
        InvokeRepeating(nameof(MeasureEnergy), 0f, 0.1f); // 10Hz measurements
        Invoke(nameof(EndTest), TestDuration);
    }
    
    private void MeasureEnergy()
    {
        if (testBall != null)
        {
            float currentEnergy = CalculateTotalEnergy(testBall);
            energyMeasurements.Add(currentEnergy);
        }
    }
    
    private float CalculateTotalEnergy(TestPhysicsObject obj)
    {
        // Kinetic energy: 0.5 * m * v²
        float kineticEnergy = 0.5f * obj.Mass * obj.Velocity.sqrMagnitude;
        
        // Potential energy: m * g * h
        float potentialEnergy = obj.Mass * 9.81f * obj.Position.y;
        
        return kineticEnergy + potentialEnergy;
    }
    
    private void EndTest()
    {
        CancelInvoke(nameof(MeasureEnergy));
        
        if (testBall != null)
        {
            BlockBallPhysicsManager.Instance.UnregisterPhysicsObject(testBall);
        }
        
        // Analyze energy conservation
        float finalEnergy = energyMeasurements.LastOrDefault();
        float energyChange = Mathf.Abs(finalEnergy - initialEnergy);
        float energyChangePercent = (energyChange / initialEnergy) * 100f;
        
        bool passed = energyChangePercent <= EnergyTolerance;
        
        Debug.Log($"Energy Conservation Test Results:");
        Debug.Log($"Initial Energy: {initialEnergy:F4}J");
        Debug.Log($"Final Energy: {finalEnergy:F4}J");
        Debug.Log($"Energy Change: {energyChangePercent:F2}%");
        Debug.Log($"Test {(passed ? "PASSED" : "FAILED")}");
        
        LogEnergyResults(initialEnergy, finalEnergy, energyChangePercent, passed);
        
        // Cleanup
        if (testBall != null)
        {
            DestroyImmediate(testBall.gameObject);
        }
    }
    
    private void LogEnergyResults(float initial, float final, float changePercent, bool passed)
    {
        string result = $"ENERGY_TEST_RESULT,{initial:F6},{final:F6},{changePercent:F4},{passed}";
        Debug.Log(result);
        System.IO.File.AppendAllText("Physics_Test_Results.csv", result + "\n");
    }
}

// Test physics object for energy conservation
public class TestPhysicsObject : MonoBehaviour, IPhysicsObject
{
    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }
    public Vector3 Acceleration { get; set; }
    public float Mass { get; set; } = 1f;
    public string PhysicsObjectName => "TestBall";
    public bool IsPhysicsEnabled => true;
    
    public void PrePhysicsStep(float deltaTime)
    {
        // Apply gravity
        Acceleration = Vector3.down * 9.81f;
    }
    
    public void PhysicsStep(float deltaTime)
    {
        VelocityVerletIntegrator.Integrate(this, deltaTime);
    }
    
    public void PostPhysicsStep(float deltaTime)
    {
        // Update visual position
        transform.position = Position;
    }
}
```

### Success Criteria
- [ ] Energy change < 1% over 10 seconds
- [ ] No energy explosion (infinite values)
- [ ] Smooth energy curve without spikes

---

## Test Case 3: Performance Benchmark

### Test Objective
Verify physics system meets performance targets.

### Test Setup
```csharp
[Test]
public class PerformanceBenchmark : MonoBehaviour
{
    [Header("Test Configuration")]
    public int NumberOfObjects = 100;
    public float TestDuration = 60f; // 1 minute stress test
    public float TargetFrameTime = 0.002f; // 2ms
    
    private List<TestPhysicsObject> testObjects = new List<TestPhysicsObject>();
    private PhysicsProfiler profiler;
    
    public void StartBenchmark()
    {
        profiler = new PhysicsProfiler();
        
        // Create test objects
        for (int i = 0; i < NumberOfObjects; i++)
        {
            CreateTestObject(i);
        }
        
        Debug.Log($"Started performance benchmark with {NumberOfObjects} objects");
        
        // Run benchmark
        StartCoroutine(RunBenchmark());
    }
    
    private void CreateTestObject(int index)
    {
        GameObject obj = new GameObject($"TestObject_{index}");
        TestPhysicsObject physicsObj = obj.AddComponent<TestPhysicsObject>();
        
        // Random initial conditions
        physicsObj.Position = new Vector3(
            Random.Range(-10f, 10f),
            Random.Range(1f, 10f),
            Random.Range(-10f, 10f)
        );
        
        physicsObj.Velocity = Random.insideUnitSphere * 5f;
        physicsObj.Mass = Random.Range(0.5f, 2f);
        
        testObjects.Add(physicsObj);
        BlockBallPhysicsManager.Instance.RegisterPhysicsObject(physicsObj);
    }
    
    private IEnumerator RunBenchmark()
    {
        float startTime = Time.time;
        int frameCount = 0;
        float totalFrameTime = 0f;
        int exceedanceCount = 0;
        
        while (Time.time - startTime < TestDuration)
        {
            profiler.BeginPhysicsFrame();
            
            // Physics update happens in FixedUpdate
            yield return new WaitForFixedUpdate();
            
            profiler.EndPhysicsFrame();
            
            float frameTime = profiler.GetLastFrameTime();
            totalFrameTime += frameTime;
            frameCount++;
            
            if (frameTime > TargetFrameTime)
            {
                exceedanceCount++;
            }
            
            // Log every 5 seconds
            if (frameCount % 250 == 0) // 50Hz * 5s = 250 frames
            {
                float avgTime = totalFrameTime / frameCount;
                Debug.Log($"Benchmark progress: {avgTime:F4}ms avg, {exceedanceCount} exceedances");
            }
        }
        
        // Final results
        float averageFrameTime = totalFrameTime / frameCount;
        float exceedancePercent = (exceedanceCount / (float)frameCount) * 100f;
        bool passed = averageFrameTime <= TargetFrameTime && exceedancePercent <= 5f;
        
        Debug.Log($"Performance Benchmark Results:");
        Debug.Log($"Average Frame Time: {averageFrameTime:F4}ms");
        Debug.Log($"Target Frame Time: {TargetFrameTime:F4}ms");
        Debug.Log($"Exceedances: {exceedancePercent:F1}%");
        Debug.Log($"Test {(passed ? "PASSED" : "FAILED")}");
        
        LogPerformanceResults(averageFrameTime, exceedancePercent, passed);
        CleanupBenchmark();
    }
    
    private void LogPerformanceResults(float avgTime, float exceedancePercent, bool passed)
    {
        string result = $"PERFORMANCE_TEST_RESULT,{avgTime:F6},{exceedancePercent:F2},{passed}";
        Debug.Log(result);
        System.IO.File.AppendAllText("Physics_Test_Results.csv", result + "\n");
    }
    
    private void CleanupBenchmark()
    {
        foreach (var obj in testObjects)
        {
            if (obj != null)
            {
                BlockBallPhysicsManager.Instance.UnregisterPhysicsObject(obj);
                DestroyImmediate(obj.gameObject);
            }
        }
        testObjects.Clear();
    }
}
```

### Success Criteria
- [ ] Average frame time ≤ 2ms with 100 objects
- [ ] Frame time exceedances < 5% of frames
- [ ] No memory allocation during physics updates
- [ ] System remains stable for full test duration

---

## Automated Test Runner

### Test Execution Script
```csharp
public class Phase1TestRunner : MonoBehaviour
{
    [Header("Test Configuration")]
    public bool RunOnStart = true;
    public bool SaveDetailedLogs = true;
    
    private void Start()
    {
        if (RunOnStart)
        {
            StartCoroutine(RunAllTests());
        }
    }
    
    private IEnumerator RunAllTests()
    {
        Debug.Log("=== PHASE 1 AUTOMATED TESTING STARTED ===");
        
        // Clear previous results
        if (SaveDetailedLogs)
        {
            System.IO.File.WriteAllText("Physics_Test_Results.csv", "TEST_TYPE,RESULT_DATA\n");
        }
        
        // Test 1: Fixed Timestep
        var timestepTest = FindObjectOfType<FixedTimestepTest>();
        if (timestepTest != null)
        {
            Debug.Log("Running Fixed Timestep Test...");
            timestepTest.StartTest();
            yield return new WaitForSeconds(timestepTest.TestDuration + 1f);
        }
        
        // Test 2: Energy Conservation
        var energyTest = FindObjectOfType<EnergyConservationTest>();
        if (energyTest != null)
        {
            Debug.Log("Running Energy Conservation Test...");
            energyTest.StartTest();
            yield return new WaitForSeconds(energyTest.TestDuration + 1f);
        }
        
        // Test 3: Performance Benchmark
        var performanceTest = FindObjectOfType<PerformanceBenchmark>();
        if (performanceTest != null)
        {
            Debug.Log("Running Performance Benchmark...");
            performanceTest.StartBenchmark();
            yield return new WaitForSeconds(performanceTest.TestDuration + 5f);
        }
        
        Debug.Log("=== PHASE 1 AUTOMATED TESTING COMPLETED ===");
        Debug.Log("Check Physics_Test_Results.csv for detailed results");
    }
}
```

## Manual Testing Checklist

### Visual Tests
- [ ] Physics objects move smoothly without jitter
- [ ] Timestep remains consistent during frame rate changes
- [ ] No visual artifacts or object teleportation
- [ ] Debug information displays correctly

### Integration Tests
- [ ] Physics manager starts correctly in all scenes
- [ ] Objects can be registered/unregistered properly
- [ ] System handles empty object lists gracefully
- [ ] Error handling works for invalid inputs

### Stress Tests
- [ ] 100+ objects maintain performance
- [ ] System remains stable under load
- [ ] Memory usage stays constant
- [ ] No performance degradation over time
