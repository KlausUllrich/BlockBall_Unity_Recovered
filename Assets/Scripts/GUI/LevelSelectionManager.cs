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
using AssemblyCSharp; // For Definitions, CampaignManager
using AssemblyCSharp.LevelData; // For CampaignDefinition, CampaignLevelSet, CampaignLevel

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
    
    private BlockMerger blockMerger;
    // private List<string> levelNames = new List<string>(); // No longer directly used in the same way

    void Awake()
    {
        // Initialize CampaignManager first
        CampaignManager.Initialize();

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
        if (!CampaignManager.IsInitialized)
        {
            Debug.LogWarning("CampaignManager not initialized yet. Attempting to initialize now.");
            CampaignManager.Initialize();
        }
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
        
        if (!CampaignManager.IsInitialized || CampaignManager.Campaign == null || CampaignManager.Campaign.LevelSets == null)
        {
            Debug.LogError("Campaign data is not available. Cannot populate level list.");
            ShowNoLevelsMessage("Campaign data could not be loaded.\nPlease check campaign_definition.xml and logs.");
            return;
        }

        int totalLevelsInCampaign = 0;
        foreach (var levelSet in CampaignManager.Campaign.LevelSets)
        {
            totalLevelsInCampaign += levelSet.Levels.Count;
        }

        Debug.Log($"Populating level list from CampaignManager. Found {CampaignManager.Campaign.LevelSets.Count} level sets with a total of {totalLevelsInCampaign} levels.");

        if (totalLevelsInCampaign == 0)
        {
            ShowNoLevelsMessage("No levels defined in campaign_definition.xml.");
            return;
        }

        // Create buttons for each level in the campaign definition
        foreach (CampaignLevelSet levelSet in CampaignManager.Campaign.LevelSets)
        {
            // Optional: Add a header for the level set here if desired
            // e.g., CreateHeader(levelSet.DisplayName);

            foreach (CampaignLevel campaignLevel in levelSet.Levels)
            {
                if (levelButtonPrefab == null)
                {
                    Debug.LogError("Level button prefab not assigned!");
                    continue;
                }

                GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);
                LevelButtonUI levelButtonUI = buttonObj.GetComponent<LevelButtonUI>();
                Button button = buttonObj.GetComponent<Button>();

                if (levelButtonUI != null)
                {
                    levelButtonUI.SetLevelName(campaignLevel.DisplayName); // Use DisplayName for button text
                }
                else
                {
                    // Fallback: Try to set text on a TextMeshProUGUI component if LevelButtonUI is not found or doesn't handle it
                    TextMeshProUGUI tmpText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmpText != null)
                    {
                        tmpText.text = campaignLevel.DisplayName;
                    }
                    else
                    {
                        Debug.LogWarning("Could not find LevelButtonUI or TextMeshProUGUI on level button prefab to set display name.");
                    }
                }

                if (button != null)
                {
                    // Capture the FileName for the closure
                    string fileNameToLoad = campaignLevel.FileName;
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => LoadLevel(fileNameToLoad)); // Use FileName for loading
                }
                else
                {
                    Debug.LogError("Button component not found on level button prefab.");
                }
            }
        }
    }

    private void ShowNoLevelsMessage(string message)
    {
        // Clear existing buttons before showing message
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }
        GameObject messageObj = new GameObject("NoLevelsMessage");
        messageObj.transform.SetParent(levelButtonContainer, false);
        TextMeshProUGUI messageText = messageObj.AddComponent<TextMeshProUGUI>();
        messageText.text = message;
        messageText.fontSize = 24;
        messageText.alignment = TextAlignmentOptions.Center;
        messageText.color = Color.yellow;
        RectTransform rect = messageObj.GetComponent<RectTransform>();
        float parentWidth = ((RectTransform)levelButtonContainer).rect.width;
        rect.sizeDelta = new Vector2(parentWidth > 0 ? parentWidth * 0.9f : 600, 150);
    }
}
    
