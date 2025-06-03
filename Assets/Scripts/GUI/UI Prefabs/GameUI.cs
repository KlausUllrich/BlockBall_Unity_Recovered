using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Unity UI implementation for the game screen.
/// Handles game UI elements like score, lives, diamonds, and keys display.
/// Controls visibility of game-specific UI elements.
/// </summary>
public class GameUI : MonoBehaviour
{
    [Header("Game UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI keysText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI diamondsText;
    public GameObject timebar;
    public TextMeshProUGUI infoText;
    
    void Awake()
    {
        // Find UI elements by name if not assigned
        if (scoreText == null)
            scoreText = GameObject.Find("Score")?.GetComponent<TextMeshProUGUI>();
        if (keysText == null)
            keysText = GameObject.Find("Keys")?.GetComponent<TextMeshProUGUI>();
        if (timerText == null)
            timerText = GameObject.Find("Timer")?.GetComponent<TextMeshProUGUI>();
        if (diamondsText == null)
            diamondsText = GameObject.Find("Diamonds")?.GetComponent<TextMeshProUGUI>();
        if (infoText == null)
            infoText = GameObject.Find("InfoText")?.GetComponent<TextMeshProUGUI>();
    }
    
    void OnEnable()
    {
        // Show game UI elements when GameScreen is enabled
        SetGameUIVisibility(true);
    }
    
    void OnDisable()
    {
        // Hide game UI elements when GameScreen is disabled
        SetGameUIVisibility(false);
    }
    
    private void SetGameUIVisibility(bool visible)
    {
        if (scoreText != null && scoreText.transform.parent != this.transform)
            scoreText.gameObject.SetActive(visible);
        if (keysText != null && keysText.transform.parent != this.transform)
            keysText.gameObject.SetActive(visible);
        if (timerText != null && timerText.transform.parent != this.transform)
            timerText.gameObject.SetActive(visible);
        if (diamondsText != null && diamondsText.transform.parent != this.transform)
            diamondsText.gameObject.SetActive(visible);
        if (timebar != null && timebar.transform.parent != this.transform)
            timebar.SetActive(visible);
        if (infoText != null && infoText.transform.parent != this.transform)
            infoText.gameObject.SetActive(visible);
    }
    
    // Update methods for game state
    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
    
    public void UpdateKeys(string keys)
    {
        if (keysText != null)
            keysText.text = "Keys: " + keys;
    }
    
    public void UpdateTimer(float time)
    {
        if (timerText != null)
        {
            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    
    public void UpdateDiamonds(int diamonds)
    {
        if (diamondsText != null)
            diamondsText.text = "♦ " + diamonds;
    }
    
    public void UpdateInfo(string info)
    {
        if (infoText != null)
            infoText.text = info;
    }
}
