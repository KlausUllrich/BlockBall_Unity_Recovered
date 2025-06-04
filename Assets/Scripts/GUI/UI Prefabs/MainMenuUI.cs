using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the MainMenuScreen UI elements.
/// </summary>
public class MainMenuUI : MonoBehaviour 
{
    [Header("Logo and Title")]
    public Image logo;
    public TextMeshProUGUI title;
    
    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button selectLevelButton;
    public Button exitButton;
    
    [Header("Panel Configuration")]
    public string mainMenuPanelId = "MainMenu";
    public string levelSelectionPanelId = "LevelSelection";
    
    [Header("Game UI")]
    public GameObject inGameUIContainer;
    public TextMeshProUGUI scoreLabel;
    public TextMeshProUGUI keysLabel;
    public TextMeshProUGUI infoLabel;
    
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
        // Hide any game UI elements when main menu is shown
        HideGameUIElements();
        
        // Make sure we have the panel manager reference
        if (panelManager == null)
        {
            panelManager = GetComponentInParent<PanelGroupManager>();
            if (panelManager == null)
            {
                panelManager = FindObjectOfType<PanelGroupManager>();
            }
        }
        
        // Setup the main menu layout and logo
        SetupMainMenuLayout();
        
        // Wire up the level selection button if present
        if (selectLevelButton != null)
        {
            selectLevelButton.onClick.RemoveAllListeners();
            selectLevelButton.onClick.AddListener(ShowLevelSelection);
        }
        
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
    
    private void HideGameUIElements()
    {
        // Hide game UI elements that might be visible
        var score = GameObject.Find("Score");
        if (score != null) score.SetActive(false);
        
        var keys = GameObject.Find("Keys");
        if (keys != null) keys.SetActive(false);
        
        var timer = GameObject.Find("Timer");
        if (timer != null) timer.SetActive(false);
        
        var diamonds = GameObject.Find("Diamonds");
        if (diamonds != null) diamonds.SetActive(false);
        
        var timebar = GameObject.Find("Timebar");
        if (timebar != null) timebar.SetActive(false);
        
        var infoText = GameObject.Find("InfoText");
        if (infoText != null) infoText.SetActive(false);
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
    
    private void SetupMainMenuLayout()
    {
        // Load and apply the BlockBall evolution logo
        if (logo != null)
        {
            var logoTexture = Resources.Load<Texture2D>("UI/BlockBall_evolution_Logo");
            if (logoTexture != null)
            {
                logo.sprite = Sprite.Create(logoTexture, 
                    new Rect(0, 0, logoTexture.width, logoTexture.height), 
                    new Vector2(0.5f, 0.5f));
            }
        }
        
        // Apply custom background to PlayButton
        if (playButton != null)
        {
            var buttonImage = playButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                var backgroundTexture = Resources.Load<Texture2D>("UI/Background");
                if (backgroundTexture != null)
                {
                    buttonImage.sprite = Sprite.Create(backgroundTexture,
                        new Rect(0, 0, backgroundTexture.width, backgroundTexture.height),
                        new Vector2(0.5f, 0.5f));
                }
            }
        }
        
        // Apply same styling to Select Level button if it exists
        if (selectLevelButton != null)
        {
            var buttonImage = selectLevelButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                var backgroundTexture = Resources.Load<Texture2D>("UI/Background");
                if (backgroundTexture != null)
                {
                    buttonImage.sprite = Sprite.Create(backgroundTexture,
                        new Rect(0, 0, backgroundTexture.width, backgroundTexture.height),
                        new Vector2(0.5f, 0.5f));
                }
            }
        }
        
        // Apply custom font to menu elements if available
        var customFont = Resources.Load<Font>("UI/BlockBall");
        if (customFont != null)
        {
            ApplyFontToMenuButtons(customFont);
        }
    }
    
    private void ApplyFontToMenuButtons(Font font)
    {
        if (playButton != null)
        {
            var playText = playButton.GetComponentInChildren<TextMeshProUGUI>();
            if (playText != null)
            {
                // Note: TextMeshPro uses different font system
                playText.font = Resources.Load<TMPro.TMP_FontAsset>("UI/BlockBall SDF");
            }
        }
        
        if (selectLevelButton != null)
        {
            var selectText = selectLevelButton.GetComponentInChildren<TextMeshProUGUI>();
            if (selectText != null)
            {
                selectText.font = Resources.Load<TMPro.TMP_FontAsset>("UI/BlockBall SDF");
            }
        }
        
        if (exitButton != null)
        {
            var exitText = exitButton.GetComponentInChildren<TextMeshProUGUI>();
            if (exitText != null)
            {
                exitText.font = Resources.Load<TMPro.TMP_FontAsset>("UI/BlockBall SDF");
            }
        }
    }
}
