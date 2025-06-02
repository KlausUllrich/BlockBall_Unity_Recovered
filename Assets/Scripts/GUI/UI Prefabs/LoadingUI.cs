using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controller for the loading screen UI.
/// This provides a loading screen when transitioning between game states.
/// </summary>
public class LoadingUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider progressBar; // UnityEngine.UI.Slider
    public TextMeshProUGUI loadingText; // TMPro.TextMeshProUGUI
    public GameObject loadingSpinner;
    
    void Awake()
    {
        // Initialize loading UI
        if (progressBar != null)
        {
            progressBar.value = 0;
        }
        
        if (loadingText != null)
        {
            loadingText.text = "Loading...";
        }
    }
    
    // Update the progress bar (0.0 to 1.0)
    public void UpdateProgress(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = Mathf.Clamp01(progress);
        }
        
        if (loadingText != null)
        {
            loadingText.text = $"Loading... {Mathf.Floor(progress * 100)}%";
        }
    }
}
