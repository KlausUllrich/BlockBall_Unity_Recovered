using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Controls the in-game UI elements like score, keys, and info displays
/// </summary>
public class GameUIController : MonoBehaviour
{
    [Header("Game UI Elements")]
    public GameObject inGameUIContainer;
    public TextMeshProUGUI scoreLabel;
    public TextMeshProUGUI keysLabel;
    public TextMeshProUGUI infoLabel;
    
    private void Awake()
    {
        // Find references if needed
        if (inGameUIContainer == null)
            inGameUIContainer = transform.Find("InGameUIContainer")?.gameObject;
            
        if (scoreLabel == null)
            scoreLabel = inGameUIContainer?.transform.Find("ScoreLabel")?.GetComponent<TextMeshProUGUI>();
            
        if (keysLabel == null)
            keysLabel = inGameUIContainer?.transform.Find("KeysLabel")?.GetComponent<TextMeshProUGUI>();
            
        if (infoLabel == null)
            infoLabel = inGameUIContainer?.transform.Find("InfoLabel")?.GetComponent<TextMeshProUGUI>();
    }
    
    /// <summary>
    /// Updates the score display
    /// </summary>
    public void UpdateScore(int score)
    {
        if (scoreLabel != null)
        {
            scoreLabel.text = $"Score: {score}";
        }
    }
    
    /// <summary>
    /// Updates the keys display
    /// </summary>
    public void UpdateKeys(int keys)
    {
        if (keysLabel != null)
        {
            keysLabel.text = $"Keys: {keys}";
        }
    }
    
    /// <summary>
    /// Shows an info message
    /// </summary>
    public void ShowInfoMessage(string message, float duration = 3.0f)
    {
        if (infoLabel != null)
        {
            // Show the message
            infoLabel.text = message;
            infoLabel.gameObject.SetActive(true);
            
            // Hide it after duration
            StartCoroutine(HideInfoAfterDelay(duration));
        }
    }
    
    private IEnumerator HideInfoAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (infoLabel != null)
        {
            infoLabel.gameObject.SetActive(false);
        }
    }
}
