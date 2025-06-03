using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Unity UI implementation for the main menu screen.
/// Handles main menu navigation and UI interactions.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button playButton; // UnityEngine.UI.Button
    public Button exitButton; // UnityEngine.UI.Button
    
    private StandardUIManager uiManager;
    
    void Awake()
    {
        // Find UI Manager
        uiManager = FindObjectOfType<StandardUIManager>();
        
        // Set up button listeners
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }
        else
        {
            Debug.LogError("Play button not assigned in MainMenuUI");
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitClicked);
        }
        else
        {
            Debug.LogError("Exit button not assigned in MainMenuUI");
        }
    }
    
    void OnPlayClicked()
    {
        // Notify UI Manager that play was clicked
        uiManager.PlayGame();
    }
    
    void OnExitClicked()
    {
        // Notify UI Manager that exit was clicked
        uiManager.ExitGame();
    }
}
