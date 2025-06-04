/*---
title: MainMenuUI
description: Handles main menu panel button events and panel navigation
role: Controls user interaction within the main menu panel and navigation to other panels
relationships:
  - Works with PanelGroupManager to show/hide panels
  - Used by ScreenStateMenu for gameplay actions
  - Parent of LevelSelectionPanel for navigation purposes
components:
  - Manages Play, Level Selection, and Exit buttons
  - Handles panel navigation between MainMenuPanel and LevelSelectionPanel
  - Receives messages from LevelSelectionManager via SendMessageUpwards
---*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the main menu panel functionality - focused only on button handling and panel navigation.
/// </summary>
public class MainMenuUI : MonoBehaviour 
{
    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button selectLevelButton;
    public Button exitButton;
    
    [Header("Panel Configuration")]
    public string mainMenuPanelId = "MainMenu";
    public string levelSelectionPanelId = "LevelSelection";
    
    private PanelGroupManager panelManager;
    
    void Awake()
    {
        // Find the panel group manager
        panelManager = GetComponentInParent<PanelGroupManager>();
        if (panelManager == null)
        {
            panelManager = FindObjectOfType<PanelGroupManager>();
        }
        
        // Make sure we have references to our buttons
        if(playButton == null)
            playButton = transform.Find("PlayButton")?.GetComponent<Button>();
            
        if(exitButton == null)
            exitButton = transform.Find("ExitButton")?.GetComponent<Button>();
            
        if(selectLevelButton == null)
            selectLevelButton = transform.Find("SelectLevelButton")?.GetComponent<Button>();
    }

    void OnEnable()
    {
        // Make sure we have the panel manager reference
        if (panelManager == null)
        {
            panelManager = GetComponentInParent<PanelGroupManager>();
            if (panelManager == null)
            {
                panelManager = FindObjectOfType<PanelGroupManager>();
            }
        }
        
        // Wire up buttons
        SetupButtonHandlers();
        
        // Wait one frame before showing panel to ensure everything is initialized
        StartCoroutine(ShowMainMenuPanelNextFrame());
    }
    
        /// <summary>
    /// Waits for one frame and then shows the main menu panel
    /// This ensures panels are properly registered before attempting to show them
    /// </summary>
    private System.Collections.IEnumerator ShowMainMenuPanelNextFrame()
    {
        yield return null; // Wait one frame
        
        if (panelManager != null)
        {
            panelManager.ShowPanel(mainMenuPanelId);
        }
        else
        {
            Debug.LogWarning("PanelGroupManager not found! Cannot show main menu panel.");
        }
    }
    
    /// <summary>
    /// Sets up button click handlers
    /// </summary>
    private void SetupButtonHandlers()
    {
        // Play button
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(() => {
                // Find the ScreenStateManager
                ScreenStateManager manager = FindObjectOfType<ScreenStateManager>();
                if (manager != null)
                {
                    // Change to game state
                    manager.ChangeToScreenState(ScreenStateManager.ScreenStates.Game);
                }
            });
        }
        
        // Select Level button
        if (selectLevelButton != null)
        {
            selectLevelButton.onClick.RemoveAllListeners();
            selectLevelButton.onClick.AddListener(ShowLevelSelection);
        }
        
        // Exit button
        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(() => {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            });
        }
    }
    
    /// <summary>
    /// Called when the back button in LevelSelectionPanel is pressed
    /// </summary>
    public void OnBackButtonPressed()
    {
        // Return to the main menu panel
        if (panelManager != null)
        {
            panelManager.ShowPanel(mainMenuPanelId);
        }
    }
    
    /// <summary>
    /// Shows the level selection panel using the PanelGroupManager
    /// </summary>
    public void ShowLevelSelection()
    {
        if (panelManager != null)
        {
            panelManager.ShowPanel(levelSelectionPanelId);
        }
        else
        {
            Debug.LogError("PanelGroupManager not found! Cannot show level selection panel.");
        }
    }
}
