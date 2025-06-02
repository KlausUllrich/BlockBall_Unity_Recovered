using UnityEngine;
using System.Reflection;

/// <summary>
/// Helper script to properly connect the ScreenStateManager to StandardUIManager.
/// Add this to an empty GameObject and click "Connect Managers" in the Inspector.
/// </summary>
public class ScreenStateSetupHelper : MonoBehaviour
{
    [Header("Manager References")]
    public ScreenStateManager screenStateManager;
    public StandardUIManager uiManager;
    
    [Header("Status")]
    [SerializeField] private bool connectionsVerified = false;
    
    private void OnEnable()
    {
        // Auto-find references if not assigned
        if (screenStateManager == null)
            screenStateManager = FindObjectOfType<ScreenStateManager>();
            
        if (uiManager == null)
            uiManager = FindObjectOfType<StandardUIManager>();
    }
    
    /// <summary>
    /// Connect the managers and verify the connections
    /// </summary>
    [ContextMenu("Connect Managers")]
    public void ConnectManagers()
    {
        // Find managers if not assigned
        if (screenStateManager == null)
            screenStateManager = FindObjectOfType<ScreenStateManager>();
            
        if (uiManager == null)
            uiManager = FindObjectOfType<StandardUIManager>();
            
        if (screenStateManager == null)
        {
            Debug.LogError("ScreenStateManager not found in scene! Add one first.");
            return;
        }
        
        if (uiManager == null)
        {
            Debug.LogError("StandardUIManager not found in scene! Add one first.");
            return;
        }
        
        // Use reflection to set the private field
        FieldInfo field = typeof(ScreenStateManager).GetField("m_xStandardUIManager", 
            BindingFlags.Public | BindingFlags.Instance);
            
        if (field != null)
        {
            field.SetValue(screenStateManager, uiManager);
            Debug.Log("Successfully connected StandardUIManager to ScreenStateManager!");
            
            // Verify UI screens are assigned
            VerifyUIScreens();
        }
        else
        {
            Debug.LogError("Could not find m_xStandardUIManager field in ScreenStateManager!");
        }
    }
    
    /// <summary>
    /// Verify all required UI screens are assigned
    /// </summary>
    [ContextMenu("Verify UI Screens")]
    public void VerifyUIScreens()
    {
        if (uiManager == null)
        {
            Debug.LogError("StandardUIManager not found!");
            return;
        }
        
        bool allScreensAssigned = true;
        
        if (uiManager.mainMenuScreen == null)
        {
            Debug.LogError("Main Menu Screen is not assigned in StandardUIManager!");
            allScreensAssigned = false;
        }
        
        if (uiManager.gameScreen == null)
        {
            Debug.LogError("Game Screen is not assigned in StandardUIManager!");
            allScreensAssigned = false;
        }
        
        if (uiManager.loadingScreen == null)
        {
            Debug.LogError("Loading Screen is not assigned in StandardUIManager!");
            allScreensAssigned = false;
        }
        
        if (uiManager.introScreen == null)
        {
            Debug.LogError("Intro Screen is not assigned in StandardUIManager!");
            allScreensAssigned = false;
        }
        
        if (allScreensAssigned)
        {
            Debug.Log("All UI screens are properly assigned in StandardUIManager!");
            connectionsVerified = true;
        }
    }
    
    /// <summary>
    /// Create the required UI screens if they don't exist
    /// </summary>
    [ContextMenu("Create Missing UI Screens")]
    public void CreateMissingUIScreens()
    {
        if (uiManager == null)
        {
            Debug.LogError("StandardUIManager not found!");
            return;
        }
        
        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("MainCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            Debug.Log("Created new Canvas");
        }
        
        // Create Main Menu Screen if needed
        if (uiManager.mainMenuScreen == null)
        {
            GameObject mainMenuScreen = new GameObject("MainMenuScreen");
            mainMenuScreen.transform.SetParent(canvas.transform, false);
            mainMenuScreen.AddComponent<RectTransform>().anchorMin = Vector2.zero;
            mainMenuScreen.GetComponent<RectTransform>().anchorMax = Vector2.one;
            mainMenuScreen.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            mainMenuScreen.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            mainMenuScreen.AddComponent<MainMenuUI>();
            mainMenuScreen.SetActive(false);
            
            uiManager.mainMenuScreen = mainMenuScreen;
            Debug.Log("Created Main Menu Screen");
        }
        
        // Create Game Screen if needed
        if (uiManager.gameScreen == null)
        {
            GameObject gameScreen = new GameObject("GameScreen");
            gameScreen.transform.SetParent(canvas.transform, false);
            gameScreen.AddComponent<RectTransform>().anchorMin = Vector2.zero;
            gameScreen.GetComponent<RectTransform>().anchorMax = Vector2.one;
            gameScreen.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            gameScreen.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            gameScreen.AddComponent<GameUI>();
            gameScreen.SetActive(false);
            
            uiManager.gameScreen = gameScreen;
            Debug.Log("Created Game Screen");
        }
        
        // Create Loading Screen if needed
        if (uiManager.loadingScreen == null)
        {
            GameObject loadingScreen = new GameObject("LoadingScreen");
            loadingScreen.transform.SetParent(canvas.transform, false);
            loadingScreen.AddComponent<RectTransform>().anchorMin = Vector2.zero;
            loadingScreen.GetComponent<RectTransform>().anchorMax = Vector2.one;
            loadingScreen.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            loadingScreen.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            loadingScreen.AddComponent<LoadingUI>();
            loadingScreen.SetActive(false);
            
            uiManager.loadingScreen = loadingScreen;
            Debug.Log("Created Loading Screen");
        }
        
        // Create Intro Screen if needed
        if (uiManager.introScreen == null)
        {
            GameObject introScreen = new GameObject("IntroScreen");
            introScreen.transform.SetParent(canvas.transform, false);
            introScreen.AddComponent<RectTransform>().anchorMin = Vector2.zero;
            introScreen.GetComponent<RectTransform>().anchorMax = Vector2.one;
            introScreen.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            introScreen.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            introScreen.AddComponent<IntroScreenUI>();
            introScreen.SetActive(false);
            
            uiManager.introScreen = introScreen;
            Debug.Log("Created Intro Screen");
        }
        
        // Create EventSystem if needed
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("Created EventSystem");
        }
        
        Debug.Log("Created all missing UI screens!");
        VerifyUIScreens();
    }
    
    /// <summary>
    /// Set up everything in one step
    /// </summary>
    [ContextMenu("Setup Everything")]
    public void SetupEverything()
    {
        // Find or create StandardUIManager
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<StandardUIManager>();
            if (uiManager == null)
            {
                GameObject uiManagerObj = new GameObject("StandardUIManager");
                uiManager = uiManagerObj.AddComponent<StandardUIManager>();
                Debug.Log("Created new StandardUIManager");
            }
        }
        
        // Create missing UI screens
        CreateMissingUIScreens();
        
        // Connect managers
        ConnectManagers();
        
        if (connectionsVerified)
        {
            Debug.Log("Everything is set up and ready to go!");
        }
    }
}
