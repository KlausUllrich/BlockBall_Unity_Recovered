using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script to automatically set up UI components and replace legacy GUI.
/// Add this to an empty GameObject in your scene and click the buttons in the Inspector.
/// </summary>
public class UISetupHelper : MonoBehaviour
{
    [Header("TextMeshPro Settings")]
    public TMP_FontAsset defaultFont;
    [Header("UI References")]
    public Canvas mainCanvas;
    public GameObject mainMenuScreen;
    public GameObject gameScreen;
    public GameObject loadingScreen;
    public GameObject introScreen;
    
    [Header("Player Labels")]
    public TextMeshProUGUI playerScoreLabel;
    public TextMeshProUGUI playerInfoLabel;
    public TextMeshProUGUI playerKeysLabel;
    
    private StandardUIManager uiManager;
    
    /// <summary>
    /// Creates a Canvas with proper settings if one doesn't exist
    /// </summary>
    [ContextMenu("1. Create Canvas")]
    public void CreateCanvas()
    {
        if (mainCanvas != null)
        {
            Debug.Log("Canvas already exists!");
            return;
        }
        
        // Create Canvas
        GameObject canvasObj = new GameObject("MainCanvas");
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        // Add Canvas Scaler
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        // Add Graphic Raycaster
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create EventSystem if it doesn't exist
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        Debug.Log("Canvas created successfully!");
    }
    
    /// <summary>
    /// Creates player UI labels (score, info, keys)
    /// </summary>
    [ContextMenu("2. Create Player Labels")]
    public void CreatePlayerLabels()
    {
        if (mainCanvas == null)
        {
            Debug.LogError("Canvas not found! Please create canvas first.");
            return;
        }
        
        // Find a default font if not assigned
        EnsureDefaultFontAssigned();
        
        // Create Score Label
        if (playerScoreLabel == null)
        {
            GameObject scoreObj = new GameObject("PlayerScoreLabel");
            scoreObj.transform.SetParent(mainCanvas.transform, false);
            playerScoreLabel = scoreObj.AddComponent<TextMeshProUGUI>();
            playerScoreLabel.text = "Score: 0";
            playerScoreLabel.fontSize = 36;
            playerScoreLabel.color = Color.white;
            playerScoreLabel.font = defaultFont;
            RectTransform scoreRect = scoreObj.GetComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0, 1);
            scoreRect.anchorMax = new Vector2(0, 1);
            scoreRect.pivot = new Vector2(0, 1);
            scoreRect.anchoredPosition = new Vector2(20, -20);
            scoreRect.sizeDelta = new Vector2(300, 50);
        }
        
        // Create Info Label
        if (playerInfoLabel == null)
        {
            GameObject infoObj = new GameObject("PlayerInfoLabel");
            infoObj.transform.SetParent(mainCanvas.transform, false);
            playerInfoLabel = infoObj.AddComponent<TextMeshProUGUI>();
            playerInfoLabel.text = "";
            playerInfoLabel.fontSize = 36;
            playerInfoLabel.color = Color.white;
            playerInfoLabel.alignment = TextAlignmentOptions.Center;
            playerInfoLabel.font = defaultFont;
            RectTransform infoRect = infoObj.GetComponent<RectTransform>();
            infoRect.anchorMin = new Vector2(0.5f, 0.5f);
            infoRect.anchorMax = new Vector2(0.5f, 0.5f);
            infoRect.pivot = new Vector2(0.5f, 0.5f);
            infoRect.anchoredPosition = new Vector2(0, 0);
            infoRect.sizeDelta = new Vector2(600, 100);
        }
        
        // Create Keys Label
        if (playerKeysLabel == null)
        {
            GameObject keysObj = new GameObject("PlayerKeysLabel");
            keysObj.transform.SetParent(mainCanvas.transform, false);
            playerKeysLabel = keysObj.AddComponent<TextMeshProUGUI>();
            playerKeysLabel.text = "Keys: ";
            playerKeysLabel.fontSize = 36;
            playerKeysLabel.color = Color.white;
            playerKeysLabel.font = defaultFont;
            RectTransform keysRect = keysObj.GetComponent<RectTransform>();
            keysRect.anchorMin = new Vector2(1, 1);
            keysRect.anchorMax = new Vector2(1, 1);
            keysRect.pivot = new Vector2(1, 1);
            keysRect.anchoredPosition = new Vector2(-20, -20);
            keysRect.sizeDelta = new Vector2(300, 50);
        }
        
        Debug.Log("Player labels created successfully!");
    }
    
    /// <summary>
    /// Makes sure a default font is assigned for TextMeshPro components
    /// </summary>
    private void EnsureDefaultFontAssigned()
    {
        if (defaultFont != null) return;
        
        // Try to find LiberationSans SDF, which is the default TMP font
#if UNITY_EDITOR
        defaultFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Packages/com.unity.textmeshpro/Editor Resources/Fonts & Materials/LiberationSans SDF.asset");
        
        if (defaultFont == null)
        {
            // If not found, try to find any TMP_FontAsset in the project
            string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                defaultFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                Debug.Log("Found font asset at: " + path);
            }
        }
#endif
        
        if (defaultFont == null)
        {
            // Fallback to Resources.FindObjectsOfTypeAll at runtime
            TMP_FontAsset[] fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
            if (fonts.Length > 0)
            {
                defaultFont = fonts[0];
                Debug.Log("Found font asset: " + defaultFont.name);
            }
            else
            {
                Debug.LogError("No TextMeshPro font assets found in the project! Please assign one manually to the UISetupHelper component.");
            }
        }
    }
    
    /// <summary>
    /// Creates UI screen prefabs required by StandardUIManager
    /// </summary>
    [ContextMenu("3. Create UI Screens")]
    public void CreateUIScreens()
    {
        if (mainCanvas == null)
        {
            Debug.LogError("Canvas not found! Please create canvas first.");
            return;
        }
        
        // Find a default font if not assigned
        EnsureDefaultFontAssigned();
        
        // Create Main Menu Screen
        if (mainMenuScreen == null)
        {
            mainMenuScreen = CreateScreen("MainMenuScreen");
            MainMenuUI menuUI = mainMenuScreen.AddComponent<MainMenuUI>();
            
            // Create Play Button
            GameObject playButton = CreateButton("PlayButton", mainMenuScreen.transform, "PLAY", new Vector2(0, 100));
            
            // Create Exit Button
            GameObject exitButton = CreateButton("ExitButton", mainMenuScreen.transform, "EXIT", new Vector2(0, 0));
            
            // Assign buttons to MainMenuUI
            menuUI.playButton = playButton.GetComponent<Button>();
            menuUI.exitButton = exitButton.GetComponent<Button>();
            
            mainMenuScreen.SetActive(false);
        }
        
        // Create Game Screen
        if (gameScreen == null)
        {
            gameScreen = CreateScreen("GameScreen");
            gameScreen.AddComponent<GameUI>();
            
            // Create game logo
            GameObject logoObj = new GameObject("GameLogo");
            logoObj.transform.SetParent(gameScreen.transform, false);
            Image logoImage = logoObj.AddComponent<Image>();
            RectTransform logoRect = logoObj.GetComponent<RectTransform>();
            logoRect.anchorMin = new Vector2(0.5f, 1);
            logoRect.anchorMax = new Vector2(0.5f, 1);
            logoRect.pivot = new Vector2(0.5f, 1);
            logoRect.anchoredPosition = new Vector2(0, -50);
            logoRect.sizeDelta = new Vector2(400, 100);
            
            gameScreen.SetActive(false);
        }
        
        // Create Loading Screen
        if (loadingScreen == null)
        {
            loadingScreen = CreateScreen("LoadingScreen");
            LoadingUI loadingUI = loadingScreen.AddComponent<LoadingUI>();
            
            // Create loading text
            GameObject loadingTextObj = new GameObject("LoadingText");
            loadingTextObj.transform.SetParent(loadingScreen.transform, false);
            TextMeshProUGUI loadingText = loadingTextObj.AddComponent<TextMeshProUGUI>();
            loadingText.text = "Loading...";
            loadingText.fontSize = 48;
            loadingText.color = Color.white;
            loadingText.alignment = TextAlignmentOptions.Center;
            loadingText.font = defaultFont;
            RectTransform textRect = loadingTextObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = new Vector2(0, 50);
            textRect.sizeDelta = new Vector2(400, 100);
            
            // Create progress bar
            GameObject progressObj = new GameObject("ProgressBar");
            progressObj.transform.SetParent(loadingScreen.transform, false);
            Slider progressBar = progressObj.AddComponent<Slider>();
            RectTransform progressRect = progressObj.GetComponent<RectTransform>();
            progressRect.anchorMin = new Vector2(0.5f, 0.5f);
            progressRect.anchorMax = new Vector2(0.5f, 0.5f);
            progressRect.pivot = new Vector2(0.5f, 0.5f);
            progressRect.anchoredPosition = new Vector2(0, -50);
            progressRect.sizeDelta = new Vector2(600, 50);
            
            // Create background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(progressObj.transform, false);
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f);
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            
            // Create fill area
            GameObject fillAreaObj = new GameObject("Fill Area");
            fillAreaObj.transform.SetParent(progressObj.transform, false);
            RectTransform fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0);
            fillAreaRect.anchorMax = new Vector2(1, 1);
            fillAreaRect.offsetMin = new Vector2(5, 5);
            fillAreaRect.offsetMax = new Vector2(-5, -5);
            
            // Create fill
            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillAreaObj.transform, false);
            Image fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0, 0.7f, 1);
            RectTransform fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0.5f, 1);
            fillRect.sizeDelta = Vector2.zero;
            
            // Configure slider
            progressBar.fillRect = fillRect;
            progressBar.targetGraphic = bgImage;
            progressBar.direction = Slider.Direction.LeftToRight;
            progressBar.value = 0.5f;
            
            // Create loading spinner
            GameObject spinnerObj = new GameObject("LoadingSpinner");
            spinnerObj.transform.SetParent(loadingScreen.transform, false);
            Image spinnerImage = spinnerObj.AddComponent<Image>();
            spinnerImage.color = Color.white;
            RectTransform spinnerRect = spinnerObj.GetComponent<RectTransform>();
            spinnerRect.anchorMin = new Vector2(0.5f, 0.5f);
            spinnerRect.anchorMax = new Vector2(0.5f, 0.5f);
            spinnerRect.pivot = new Vector2(0.5f, 0.5f);
            spinnerRect.anchoredPosition = new Vector2(0, -150);
            spinnerRect.sizeDelta = new Vector2(100, 100);
            
            // Add rotation animation component
            spinnerObj.AddComponent<LoadingSpinner>();
            
            // Assign references to LoadingUI component
            loadingUI.progressBar = progressBar;
            loadingUI.loadingText = loadingText;
            loadingUI.loadingSpinner = spinnerObj;
            
            loadingScreen.SetActive(false);
        }
        
        // Create Intro Screen
        if (introScreen == null)
        {
            introScreen = CreateScreen("IntroScreen");
            IntroScreenUI introUI = introScreen.AddComponent<IntroScreenUI>();
            
            // Create logo image
            GameObject logoObj = new GameObject("Logo");
            logoObj.transform.SetParent(introScreen.transform, false);
            Image logoImage = logoObj.AddComponent<Image>();
            logoImage.color = Color.white;
            RectTransform logoRect = logoObj.GetComponent<RectTransform>();
            logoRect.anchorMin = new Vector2(0.5f, 0.5f);
            logoRect.anchorMax = new Vector2(0.5f, 0.5f);
            logoRect.pivot = new Vector2(0.5f, 0.5f);
            logoRect.sizeDelta = new Vector2(600, 400);
            
            // Add EventTrigger
            UnityEngine.EventSystems.EventTrigger trigger = logoObj.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            
            // Assign references
            introUI.logoImage = logoImage;
            introUI.logoTrigger = trigger;
            
            introScreen.SetActive(false);
        }
        
        Debug.Log("UI screens created successfully!");
    }
    
    /// <summary>
    /// Sets up StandardUIManager and connects it to ScreenStateManager
    /// </summary>
    [ContextMenu("4. Setup UI Manager")]
    public void SetupUIManager()
    {
        // Find or create UI Manager GameObject
        GameObject uiManagerObj = GameObject.Find("StandardUIManager");
        if (uiManagerObj == null)
        {
            uiManagerObj = new GameObject("StandardUIManager");
            uiManager = uiManagerObj.AddComponent<StandardUIManager>();
        }
        else
        {
            uiManager = uiManagerObj.GetComponent<StandardUIManager>();
            if (uiManager == null)
            {
                uiManager = uiManagerObj.AddComponent<StandardUIManager>();
            }
        }
        
        // Assign UI screens to manager
        uiManager.mainMenuScreen = mainMenuScreen;
        uiManager.gameScreen = gameScreen;
        uiManager.loadingScreen = loadingScreen;
        uiManager.introScreen = introScreen;
        
        // Find ScreenStateManager and connect
        ScreenStateManager screenStateManager = FindObjectOfType<ScreenStateManager>();
        if (screenStateManager != null)
        {
            // Use reflection to set the private field
            var field = typeof(ScreenStateManager).GetField("m_xStandardUIManager", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(screenStateManager, uiManager);
                Debug.Log("Connected StandardUIManager to ScreenStateManager!");
            }
            else
            {
                Debug.LogError("Could not find m_xStandardUIManager field in ScreenStateManager!");
            }
        }
        else
        {
            Debug.LogWarning("ScreenStateManager not found in scene!");
        }
        
        Debug.Log("UI Manager setup complete!");
    }
    
    /// <summary>
    /// Updates Player object references to use the new UI
    /// </summary>
    [ContextMenu("5. Update Player References")]
    public void UpdatePlayerReferences()
    {
        Player player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogWarning("Player not found in scene!");
            return;
        }
        
        // Use reflection to set the private fields
        var scoreField = typeof(Player).GetField("pScoreText", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var keysField = typeof(Player).GetField("pKeysText", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
        if (scoreField != null && playerScoreLabel != null)
        {
            scoreField.SetValue(player, playerScoreLabel.gameObject);
            Debug.Log("Updated Player score text reference!");
        }
        
        if (keysField != null && playerKeysLabel != null)
        {
            keysField.SetValue(player, playerKeysLabel.gameObject);
            Debug.Log("Updated Player keys text reference!");
        }
        
        // Update InfoObject references
        InfoObject[] infoObjects = FindObjectsOfType<InfoObject>();
        foreach (InfoObject infoObject in infoObjects)
        {
            var labelField = typeof(InfoObject).GetField("pGUILable", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
            if (labelField != null && playerInfoLabel != null)
            {
                labelField.SetValue(infoObject, playerInfoLabel.gameObject);
            }
        }
        
        Debug.Log("Updated " + infoObjects.Length + " InfoObject references!");
    }
    
    /// <summary>
    /// Runs all setup steps in sequence
    /// </summary>
    [ContextMenu("Run All Steps")]
    public void RunAllSteps()
    {
        CreateCanvas();
        CreatePlayerLabels();
        CreateUIScreens();
        SetupUIManager();
        UpdatePlayerReferences();
        
        Debug.Log("All UI setup steps completed!");
    }
    
    /// <summary>
    /// Creates a UI screen container
    /// </summary>
    private GameObject CreateScreen(string name)
    {
        GameObject screen = new GameObject(name);
        screen.transform.SetParent(mainCanvas.transform, false);
        
        RectTransform rect = screen.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        return screen;
    }
    
    /// <summary>
    /// Creates a button with text
    /// </summary>
    private GameObject CreateButton(string name, Transform parent, string text, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        // Add image (button background)
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f);
        
        // Add button component
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = image;
        
        // Setup ColorBlock
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f);
        button.colors = colors;
        
        // Position and size
        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(300, 60);
        
        // Add text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.fontSize = 36;
        tmpText.color = Color.white;
        tmpText.font = defaultFont;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return buttonObj;
    }
}

/// <summary>
/// Simple component to rotate a loading spinner
/// </summary>
public class LoadingSpinner : MonoBehaviour
{
    public float rotationSpeed = 200f;
    
    void Update()
    {
        transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
    }
}
