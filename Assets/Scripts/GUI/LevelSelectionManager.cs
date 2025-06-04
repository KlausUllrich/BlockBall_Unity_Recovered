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
    
    [Header("Panel Management")]
    public string mainMenuPanelId = "MainMenu"; // ID of the main menu panel in PanelGroupManager
    public string levelSelectionPanelId = "LevelSelection"; // ID of this panel in PanelGroupManager
    
    [Header("Settings")]
    public string levelsFolder = "Levels"; // Folder inside Resources where levels are stored
    
    private PanelGroupManager panelManager;
    private BlockMerger blockMerger;
    private List<string> levelNames = new List<string>();

    void Awake()
    {
        // Find BlockMerger
        blockMerger = FindObjectOfType<BlockMerger>();
        
        // Find panel manager
        panelManager = GetComponentInParent<PanelGroupManager>();
        if (panelManager == null)
        {
            panelManager = FindObjectOfType<PanelGroupManager>();
        }
        
        // Set up back button
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(ReturnToMainMenu);
        }
    }
    
    void OnEnable()
    {
        // Populate the level list whenever this component is enabled
        PopulateLevelList();
    }
    
    /// <summary>
    /// Shows the level selection panel using the panel manager
    /// </summary>
    public void ShowLevelSelection()
    {
        if (panelManager != null)
        {
            panelManager.ShowPanel(levelSelectionPanelId);
        }
        else
        {
            Debug.LogError("No PanelGroupManager found!");
        }
    }
    
    /// <summary>
    /// Returns to the main menu panel
    /// </summary>
    public void ReturnToMainMenu()
    {
        if (panelManager != null)
        {
            panelManager.ShowPanel(mainMenuPanelId);
        }
        else
        {
            Debug.LogError("No PanelGroupManager found!");
        }
    }
    
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
    private void PopulateLevelList()
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
