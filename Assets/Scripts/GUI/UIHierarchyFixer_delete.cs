using UnityEngine;

/// <summary>
/// One-time utility to fix UI element hierarchy by moving game UI elements to GameScreen
/// </summary>
public class UIHierarchyFixer : MonoBehaviour
{
    [ContextMenu("Fix UI Hierarchy")]
    public void FixUIHierarchy()
    {
        GameObject gameScreen = GameObject.Find("GameScreen");
        if (gameScreen == null)
        {
            Debug.LogError("GameScreen not found! Cannot fix hierarchy.");
            return;
        }

        // Find and move Score element
        GameObject scoreLabel = GameObject.Find("PlayerScoreLabel");
        if (scoreLabel != null)
        {
            scoreLabel.transform.SetParent(gameScreen.transform, false);
            scoreLabel.name = "Score"; // Rename to match expected name
            Debug.Log("Moved PlayerScoreLabel to GameScreen and renamed to 'Score'");
        }

        // Find and move Keys element  
        GameObject keysLabel = GameObject.Find("PlayerKeysLabel");
        if (keysLabel != null)
        {
            keysLabel.transform.SetParent(gameScreen.transform, false);
            keysLabel.name = "Keys"; // Rename to match expected name
            Debug.Log("Moved PlayerKeysLabel to GameScreen and renamed to 'Keys'");
        }

        // Find and move Info element
        GameObject infoLabel = GameObject.Find("PlayerInfoLabel");
        if (infoLabel != null)
        {
            infoLabel.transform.SetParent(gameScreen.transform, false);
            infoLabel.name = "InfoText"; // Rename to match expected name
            Debug.Log("Moved PlayerInfoLabel to GameScreen and renamed to 'InfoText'");
        }

        Debug.Log("UI Hierarchy fix complete! Game UI elements now properly contained in GameScreen.");
    }
}
