using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;

/// <summary>
/// Helper component to easily set up the level selection panel and panel management in the Unity editor.
/// This follows the architectural principle of using a single Canvas with child GameObjects for screens,
/// but adds the PanelGroupManager for more robust panel visibility management.
/// </summary>
public class LevelSelectionSetupHelper : MonoBehaviour
{
    [Header("References")]
    public Transform mainMenuScreenTransform;
    
    [Header("Panel Configuration")]
    public string mainMenuPanelId = "MainMenu";
    public string levelSelectionPanelId = "LevelSelection";
    
    [Header("Creation Settings")]
    public Vector2 panelSize = new Vector2(500, 600);
    public float buttonHeight = 80f;
    public float buttonSpacing = 10f;
    
    /// <summary>
    /// Creates a level selection panel as a child of the MainMenuScreen
    /// </summary>
    [ContextMenu("1. Create Level Selection Panel")]
    public void CreateLevelSelectionPanel()
    {
        if (mainMenuScreenTransform == null)
        {
            Debug.LogError("MainMenuScreen transform not assigned!");
            return;
        }
        
        // Check if panel already exists
        Transform existingPanel = mainMenuScreenTransform.Find("LevelSelectionPanel");
        if (existingPanel != null)
        {
            Debug.Log("Level selection panel already exists!");
            return;
        }
        
        // Create panel GameObject
        GameObject panelObj = new GameObject("LevelSelectionPanel", typeof(RectTransform));
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelObj.transform.SetParent(mainMenuScreenTransform, false);
        
        // Set up panel RectTransform
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = panelSize;
        
        // Add panel image
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        
        // Create Panel Title
        GameObject titleObj = new GameObject("TitleText", typeof(RectTransform));
        titleObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "SELECT LEVEL";
        titleText.fontSize = 36;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.anchoredPosition = new Vector2(0, -20);
        titleRect.sizeDelta = new Vector2(0, 60);
        
        // Create Back Button
        GameObject backButtonObj = new GameObject("BackButton", typeof(RectTransform));
        backButtonObj.transform.SetParent(panelObj.transform, false);
        Image backButtonImage = backButtonObj.AddComponent<Image>();
        backButtonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        Button backButton = backButtonObj.AddComponent<Button>();
        backButton.targetGraphic = backButtonImage;
        
        RectTransform backButtonRect = backButtonObj.GetComponent<RectTransform>();
        backButtonRect.anchorMin = new Vector2(0, 0);
        backButtonRect.anchorMax = new Vector2(1, 0);
        backButtonRect.pivot = new Vector2(0.5f, 0);
        backButtonRect.anchoredPosition = new Vector2(0, 20);
        backButtonRect.sizeDelta = new Vector2(0, 60);
        
        // Add text to back button
        GameObject backTextObj = new GameObject("BackText", typeof(RectTransform));
        backTextObj.transform.SetParent(backButtonObj.transform, false);
        TextMeshProUGUI backText = backTextObj.AddComponent<TextMeshProUGUI>();
        backText.text = "BACK";
        backText.fontSize = 24;
        backText.alignment = TextAlignmentOptions.Center;
        backText.color = Color.white;
        
        RectTransform backTextRect = backTextObj.GetComponent<RectTransform>();
        backTextRect.anchorMin = Vector2.zero;
        backTextRect.anchorMax = Vector2.one;
        backTextRect.sizeDelta = Vector2.zero;
        
        // Create ScrollView
        GameObject scrollViewObj = new GameObject("ScrollView", typeof(RectTransform));
        scrollViewObj.transform.SetParent(panelObj.transform, false);
        ScrollRect scrollRect = scrollViewObj.AddComponent<ScrollRect>();
        
        RectTransform scrollRectTransform = scrollViewObj.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = new Vector2(0, 0);
        scrollRectTransform.anchorMax = new Vector2(1, 1);
        scrollRectTransform.pivot = new Vector2(0.5f, 0.5f);
        scrollRectTransform.anchoredPosition = new Vector2(0, 0);
        scrollRectTransform.sizeDelta = new Vector2(0, -160); // Adjust for title and back button
        scrollRectTransform.offsetMin = new Vector2(20, 100);
        scrollRectTransform.offsetMax = new Vector2(-20, -100);
        
        // Create Viewport
        GameObject viewportObj = new GameObject("Viewport", typeof(RectTransform));
        viewportObj.transform.SetParent(scrollViewObj.transform, false);
        Image viewportImage = viewportObj.AddComponent<Image>();
        viewportImage.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        
        // Add mask
        Mask viewportMask = viewportObj.AddComponent<Mask>();
        viewportMask.showMaskGraphic = false;
        
        RectTransform viewportRect = viewportObj.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.pivot = new Vector2(0.5f, 0.5f);
        viewportRect.sizeDelta = Vector2.zero;
        
        // Create Content
        GameObject contentObj = new GameObject("Content", typeof(RectTransform));
        contentObj.transform.SetParent(viewportObj.transform, false);
        
        // Add vertical layout group
        VerticalLayoutGroup layoutGroup = contentObj.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = buttonSpacing;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlHeight = false;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = true;
        
        // Add content size fitter
        ContentSizeFitter contentFitter = contentObj.AddComponent<ContentSizeFitter>();
        contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        RectTransform contentRect = contentObj.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(0, 0);
        contentRect.anchoredPosition = Vector2.zero;
        
        // Set up ScrollRect components
        scrollRect.content = contentRect;
        scrollRect.viewport = viewportRect;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        
        // Make panel inactive by default (will be shown via panel manager)
        panelObj.SetActive(false);
        
        Debug.Log("Level selection panel created successfully! Next, set up the PanelGroupManager.");
    }
    
    /// <summary>
    /// Sets up the PanelGroupManager for improved panel management
    /// </summary>
    [ContextMenu("2. Set Up Panel Group Manager")]
    public void SetupPanelGroupManager()
    {
        if (mainMenuScreenTransform == null)
        {
            Debug.LogError("MainMenuScreen transform not assigned!");
            return;
        }
        
        GameObject mainMenuScreenObj = mainMenuScreenTransform.gameObject;
        
        // Check if PanelGroupManager already exists
        PanelGroupManager existingManager = mainMenuScreenObj.GetComponent<PanelGroupManager>();
        if (existingManager != null)
        {
            Debug.Log("PanelGroupManager already exists! Updating configuration...");
            ConfigurePanelManager(existingManager);
            return;
        }
        
        // Add PanelGroupManager
        PanelGroupManager panelManager = mainMenuScreenObj.AddComponent<PanelGroupManager>();
        ConfigurePanelManager(panelManager);
        
        Debug.Log("PanelGroupManager added and configured successfully!");
    }
    
    /// <summary>
    /// Configures the PanelGroupManager with appropriate panels
    /// </summary>
    private void ConfigurePanelManager(PanelGroupManager panelManager)
    {
        if (panelManager == null) return;
        
        // Find main menu buttons container
        Transform buttonContainer = mainMenuScreenTransform.Find("MainMenuButtons");
        if (buttonContainer == null)
        {
            // Create a container for main menu buttons
            GameObject containerObj = new GameObject("MainMenuButtons", typeof(RectTransform));
            containerObj.transform.SetParent(mainMenuScreenTransform, false);
            RectTransform containerRect = containerObj.GetComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.sizeDelta = Vector2.zero;
            buttonContainer = containerObj.transform;
            
            // Find the play, select level and exit buttons and move them
            MainMenuUI mainMenuUI = mainMenuScreenTransform.GetComponent<MainMenuUI>();
            if (mainMenuUI != null)
            {
                if (mainMenuUI.playButton != null)
                    mainMenuUI.playButton.transform.SetParent(containerObj.transform, true);
                if (mainMenuUI.selectLevelButton != null)
                    mainMenuUI.selectLevelButton.transform.SetParent(containerObj.transform, true);
                if (mainMenuUI.exitButton != null)
                    mainMenuUI.exitButton.transform.SetParent(containerObj.transform, true);
            }
            
            Debug.Log("Created MainMenuButtons container and moved buttons into it.");
        }
        
        // Find level selection panel
        Transform levelSelectionPanel = mainMenuScreenTransform.Find("LevelSelectionPanel");
        if (levelSelectionPanel == null)
        {
            Debug.LogError("LevelSelectionPanel not found! Create it first using the 'Create Level Selection Panel' function.");
            return;
        }
        
        // Clear existing panels array
        panelManager.panels = new PanelGroupManager.PanelInfo[0];
        
        // Add main menu panel
        PanelGroupManager.PanelInfo mainMenuPanelInfo = new PanelGroupManager.PanelInfo
        {
            panelId = mainMenuPanelId,
            panelObject = buttonContainer.gameObject,
            onPanelShow = new UnityEngine.Events.UnityEvent(),
            onPanelHide = new UnityEngine.Events.UnityEvent()
        };
        
        // Add level selection panel
        PanelGroupManager.PanelInfo levelSelectionPanelInfo = new PanelGroupManager.PanelInfo
        {
            panelId = levelSelectionPanelId,
            panelObject = levelSelectionPanel.gameObject,
            onPanelShow = new UnityEngine.Events.UnityEvent(),
            onPanelHide = new UnityEngine.Events.UnityEvent()
        };
        
        // Set up panels array
        panelManager.panels = new PanelGroupManager.PanelInfo[] { mainMenuPanelInfo, levelSelectionPanelInfo };
        panelManager.defaultPanelId = mainMenuPanelId;
        
        // Initialize panels (makes sure only default is shown)
        panelManager.InitializePanels();
    }
    
    /// <summary>
    /// Adds the LevelSelectionManager component to the MainMenuScreen
    /// </summary>
    [ContextMenu("3. Add Level Selection Manager")]
    public void AddLevelSelectionManager()
    {
        if (mainMenuScreenTransform == null)
        {
            Debug.LogError("MainMenuScreen transform not assigned!");
            return;
        }
        
        GameObject mainMenuScreenObj = mainMenuScreenTransform.gameObject;
        
        // Check if MainMenuUI component exists
        MainMenuUI mainMenuUI = mainMenuScreenObj.GetComponent<MainMenuUI>();
        if (mainMenuUI == null)
        {
            Debug.LogError("MainMenuUI component not found on MainMenuScreen!");
            return;
        }
        
        // Check if manager already exists
        LevelSelectionManager existingManager = mainMenuScreenObj.GetComponent<LevelSelectionManager>();
        if (existingManager != null)
        {
            Debug.Log("LevelSelectionManager already exists!");
            return;
        }
        
        // Find level selection panel
        Transform levelSelectionPanel = mainMenuScreenTransform.Find("LevelSelectionPanel");
        if (levelSelectionPanel == null)
        {
            Debug.LogError("LevelSelectionPanel not found! Create it first using the 'Create Level Selection Panel' function.");
            return;
        }
        
        // Find main menu buttons container
        // We need to create it if it doesn't exist to group the main menu buttons
        Transform buttonContainer = mainMenuScreenTransform.Find("MainMenuButtons");
        if (buttonContainer == null)
        {
            // Create a container for main menu buttons
            GameObject containerObj = new GameObject("MainMenuButtons", typeof(RectTransform));
            containerObj.transform.SetParent(mainMenuScreenTransform, false);
            RectTransform containerRect = containerObj.GetComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.sizeDelta = Vector2.zero;
            buttonContainer = containerObj.transform;
            
            // Find and move all main menu buttons to the container
            Button playButton = mainMenuUI.playButton;
            Button selectLevelButton = mainMenuUI.selectLevelButton;
            Button exitButton = mainMenuUI.exitButton;
            
            if (playButton != null)
                playButton.transform.SetParent(containerObj.transform, true);
            if (selectLevelButton != null)
                selectLevelButton.transform.SetParent(containerObj.transform, true);
            if (exitButton != null)
                exitButton.transform.SetParent(containerObj.transform, true);
            
            Debug.Log("Created MainMenuButtons container and moved buttons into it.");
        }
        
        // Add LevelSelectionManager to the LevelSelectionPanel instead of MainMenuScreen
        GameObject levelSelectionPanelObj = levelSelectionPanel.gameObject;
        LevelSelectionManager manager = levelSelectionPanelObj.AddComponent<LevelSelectionManager>();
        
        // Set up references - use panel IDs instead of direct GameObject references
        manager.mainMenuPanelId = mainMenuPanelId;
        manager.levelSelectionPanelId = levelSelectionPanelId;
        
        // Find level button container
        Transform contentTransform = levelSelectionPanel.Find("ScrollView/Viewport/Content");
        if (contentTransform != null)
        {
            manager.levelButtonContainer = contentTransform;
        }
        
        // Find back button
        Button backBtn = levelSelectionPanel.Find("BackButton")?.GetComponent<Button>();
        if (backBtn != null)
        {
            manager.backButton = backBtn;
        }
        
        // Update level selection manager references for the new panel system
        manager.mainMenuPanelId = mainMenuPanelId;
        manager.levelSelectionPanelId = levelSelectionPanelId;
        
        // Update MainMenuUI references for panel IDs if present
        if (mainMenuUI != null)
        {
            mainMenuUI.mainMenuPanelId = mainMenuPanelId;
            mainMenuUI.levelSelectionPanelId = levelSelectionPanelId;
        }
        
        Debug.Log("LevelSelectionManager added to MainMenuScreen and configured successfully!");
        Debug.Log("Next: 1. Create a level button prefab. 2. Assign it to the LevelSelectionManager.");
    }
}
