using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Unity UI implementation for the main menu screen.
/// Handles main menu layout, logo display, and ensures game UI elements are hidden.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Menu UI Elements")]
    public Image logoImage;
    public Button playButton;
    public Button exitButton;
    
    void Awake()
    {
        // Find menu elements if not assigned
        if (logoImage == null)
            logoImage = GetComponentInChildren<Image>();
        if (playButton == null)
            playButton = GameObject.Find("PlayButton")?.GetComponent<Button>();
        if (exitButton == null)
            exitButton = GameObject.Find("ExitButton")?.GetComponent<Button>();
    }
    
    void OnEnable()
    {
        // Hide any game UI elements when main menu is shown
        HideGameUIElements();
        
        // Setup the main menu layout and logo
        SetupMainMenuLayout();
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
    
    private void SetupMainMenuLayout()
    {
        // Load and apply the BlockBall evolution logo
        if (logoImage != null)
        {
            var logoTexture = Resources.Load<Texture2D>("UI/BlockBall_evolution_Logo");
            if (logoTexture != null)
            {
                logoImage.sprite = Sprite.Create(logoTexture, 
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
