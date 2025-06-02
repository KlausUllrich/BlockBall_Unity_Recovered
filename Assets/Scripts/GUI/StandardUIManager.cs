using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StandardUIManager : MonoBehaviour 
{
    [Header("UI Screens")]
    public GameObject mainMenuScreen;
    public GameObject gameScreen;
    public GameObject loadingScreen;
    public GameObject introScreen;

    private Dictionary<string, GameObject> screens = new Dictionary<string, GameObject>();
    private GameObject currentActiveScreen;

    // Initialize UI Manager
    void Awake()
    {
        // Register all screens
        if (mainMenuScreen != null) screens["MainMenu"] = mainMenuScreen;
        if (gameScreen != null) screens["Game"] = gameScreen;
        if (loadingScreen != null) screens["Loading"] = loadingScreen;
        if (introScreen != null) screens["Intro"] = introScreen;
        
        // Hide all screens initially
        foreach (var screen in screens.Values)
        {
            if (screen != null)
            {
                screen.SetActive(false);
            }
        }
    }

    // Change the active screen
    public void ChangeScreen(string screenName)
    {
        // Hide the current active screen
        if (currentActiveScreen != null)
        {
            currentActiveScreen.SetActive(false);
        }

        // Show the requested screen
        if (screens.TryGetValue(screenName, out GameObject screen))
        {
            screen.SetActive(true);
            currentActiveScreen = screen;
        }
        else
        {
            Debug.LogError("Screen not found: " + screenName);
        }
    }

    // Get a UI element by ID from the current screen
    public GameObject GetElementById(string id)
    {
        if (currentActiveScreen == null) return null;
        
        // Find child with matching name
        Transform element = currentActiveScreen.transform.Find(id);
        return element != null ? element.gameObject : null;
    }

    // Methods to be called directly from UI buttons
    public void PlayGame()
    {
        // This will be connected to the Play button
        // Signal to ScreenStateManager
        SendMessageUpwards("OnPlayClicked", SendMessageOptions.DontRequireReceiver);
    }

    public void ExitGame()
    {
        // This will be connected to the Exit button
        // Signal to ScreenStateManager
        SendMessageUpwards("OnExitClicked", SendMessageOptions.DontRequireReceiver);
    }
}
