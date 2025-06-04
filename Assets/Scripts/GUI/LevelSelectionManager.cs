/*---
title: LevelSelectionManager
description: Manages the level selection functionality
role: Dynamically loads and displays available game levels for selection
relationships:
  - Attached to LevelSelectionPanel GameObject
  - Communicates with MainMenuUI via SendMessageUpwards
  - Uses BlockMerger to load selected levels
components:
  - Populates level list from Resources/Levels folder
  - Creates level selection buttons dynamically
  - Handles level loading when a level is selected
  - Sends back button events to parent components
---*/

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the level selection panel within the main menu.
/// Handles loading level list, displaying level buttons, and loading selected levels.
/// Uses PanelGroupManager for panel visibility management.
/// </summary>
public class LevelSelectionManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform levelButtonContainer; // Parent transform for level buttons
    public Button backButton; // Button to return to main menu
    public GameObject levelButtonPrefab; // Button prefab for level items
    
    [Header("Settings")]
    public bool autoPopulateOnEnable = true; // Whether to automatically populate the level list when enabled
    
    [Header("Settings")]
    public string levelsFolder = "Levels"; // Folder inside Resources where levels are stored
    
    private BlockMerger blockMerger;
    private List<string> levelNames = new List<string>();

    void Awake()
    {
        // Find BlockMerger
        blockMerger = FindObjectOfType<BlockMerger>();
        
        // Set up back button
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => {
                // Send a message upwards to notify parent components that back was pressed
                // This allows MainMenuUI or other controllers to handle the navigation
                SendMessageUpwards("OnBackButtonPressed", SendMessageOptions.DontRequireReceiver);
            });
        }
    }
    
    void OnEnable()
    {
        // Populate the level list whenever this component is enabled
        PopulateLevelList();
    }
    
    // Panel navigation methods removed - panel management should be handled by MainMenuUI
    
    /// <summary>
    /// Loads the specified level by setting it in BlockMerger and triggering its LoadLevel method
    /// </summary>
    /// <param name="levelName">The name of the level to load</param>
    public void LoadLevel(string levelName)
    {
        Debug.Log($"Loading level: {levelName}");
        
        if (blockMerger != null)
        {
            // Assign level name to BlockMerger and trigger load
            blockMerger.LevelToLoad = levelName;
            blockMerger.LoadLevel();
        }
        else
        {
            Debug.LogError("BlockMerger not found! Cannot load level.");
        }
    }
    
    /// <summary>
    /// Populates the level selection panel with available levels
    /// </summary>
    public void PopulateLevelList()
    {
        CreateLevelButtons();
    }
    
    /// <summary>
    /// Creates buttons for each level file found in Resources/Levels
    /// </summary>
    private void CreateLevelButtons()
    {
        // Clear existing buttons
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Skip if no level button container assigned
        if (levelButtonContainer == null)
        {
            Debug.LogError("Level button container not assigned!");
            return;
        }
        
        Debug.Log("Looking for level files in Resources/" + levelsFolder);

        // Try loading all files from the Levels folder
        TextAsset[] levelFiles = Resources.LoadAll<TextAsset>(levelsFolder);
        Debug.Log($"Initial level files found: {levelFiles.Length}");
        
        // List all found files before filtering
        foreach (var file in levelFiles) 
        {
            Debug.Log($"Found file: {file.name}");
        }
        
        // Filter to only include campaign levels
        List<TextAsset> filteredLevelFiles = new List<TextAsset>();
        foreach (var file in levelFiles)
        {
            if (file.name.EndsWith("_campain"))
            {
                filteredLevelFiles.Add(file);
                Debug.Log($"Added campaign level: {file.name}");
            }
        }
        
        // Convert back to array
        levelFiles = filteredLevelFiles.ToArray();
        
        // Log how many levels were found
        Debug.Log($"Found {levelFiles.Length} level files in Resources/{levelsFolder}");
        
        // If no level files were found, display a message
        if (levelFiles.Length == 0)
        {
            GameObject messageObj = new GameObject("NoLevelsMessage");
            messageObj.transform.SetParent(levelButtonContainer, false);
            TextMeshProUGUI messageText = messageObj.AddComponent<TextMeshProUGUI>();
            messageText.text = $"No campaign levels found in Resources/{levelsFolder}\n\nPlease make sure you have .level files with names ending in _campain";
            messageText.fontSize = 24;
            messageText.alignment = TextAlignmentOptions.Center;
            messageText.color = Color.yellow;
            RectTransform rect = messageObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 100);
            return;
        }
        
        // Clear level names list
        levelNames.Clear();
        
        // Create a button for each level file
        foreach (TextAsset levelFile in levelFiles)
        {
            string levelName = Path.GetFileNameWithoutExtension(levelFile.name).Replace("_campain", "");
            levelNames.Add(levelName);
            
            // Skip if level button prefab is not assigned
            if (levelButtonPrefab == null)
            {
                Debug.LogError("Level button prefab not assigned!");
                continue;
            }
            
            // Create button
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);
            
            // Set up the level button
            LevelButtonUI levelButton = buttonObj.GetComponent<LevelButtonUI>();
            if (levelButton != null)
            {
                levelButton.SetLevelName(levelName);
                
                // Set up button click action
                Button button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => LoadLevel(levelName));
                }
            }
        }
    }
}
