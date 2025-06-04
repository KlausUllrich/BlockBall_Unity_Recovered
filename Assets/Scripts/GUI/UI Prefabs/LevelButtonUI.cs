using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple component to represent a level selection button.
/// </summary>
public class LevelButtonUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI levelNameText;
    
    private string levelName;
    
    /// <summary>
    /// Sets the level name displayed on the button
    /// </summary>
    public void SetLevelName(string name)
    {
        levelName = name;
        
        if (levelNameText != null)
        {
            levelNameText.text = name;
        }
    }
    
    /// <summary>
    /// Gets the level name associated with this button
    /// </summary>
    public string GetLevelName()
    {
        return levelName;
    }
}
