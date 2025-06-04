using UnityEngine;
using System.Collections;

/// <summary>
/// Emergency fix for setting up the StandardUIManager connection to ScreenStateManager.
/// Add this to any GameObject in the scene to ensure proper initialization.
/// </summary>
public class UIManagerSetter : MonoBehaviour
{
    void Awake()
    {
        // Find the ScreenStateManager
        ScreenStateManager screenStateManager = FindObjectOfType<ScreenStateManager>();
        if (screenStateManager == null)
        {
            Debug.LogError("ScreenStateManager not found in scene!");
            return;
        }
        
        // Find or create a StandardUIManager
        StandardUIManager uiManager = FindObjectOfType<StandardUIManager>();
        if (uiManager == null)
        {
            // Create the UI Manager
            GameObject uiManagerObj = new GameObject("StandardUIManager");
            uiManager = uiManagerObj.AddComponent<StandardUIManager>();
            Debug.Log("Created new StandardUIManager");
            
            // Create Canvas if needed
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
            
            // Create required UI screens
            uiManager.mainMenuScreen = CreateScreen(canvas, "MainMenuScreen");
            uiManager.gameScreen = CreateScreen(canvas, "GameScreen");
            uiManager.loadingScreen = CreateScreen(canvas, "LoadingScreen");
            uiManager.introScreen = CreateScreen(canvas, "IntroScreen");
            
            // Create EventSystem if needed
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("Created EventSystem");
            }
        }
        
        // Directly set the UI Manager on the ScreenStateManager
        // This is a critical fix to prevent the assertion failure
        screenStateManager.m_xStandardUIManager = uiManager;
        
        Debug.Log("Successfully connected StandardUIManager to ScreenStateManager!");
    }
    
    /// <summary>
    /// Creates a UI screen container
    /// </summary>
    private GameObject CreateScreen(Canvas canvas, string name)
    {
        GameObject screen = new GameObject(name);
        screen.transform.SetParent(canvas.transform, false);
        
        RectTransform rect = screen.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        // Add appropriate UI component based on screen type
        if (name == "MainMenuScreen")
            screen.AddComponent<MainMenuUI>();
        else if (name == "GameScreen")
            screen.AddComponent<GameUI>();
        else if (name == "LoadingScreen")
            screen.AddComponent<LoadingUI>();
        else if (name == "IntroScreen")
            screen.AddComponent<IntroScreenUI>();
            
        screen.SetActive(false);
        
        Debug.Log("Created " + name);
        return screen;
    }
}
