using UnityEngine;

/// <summary>
/// Helper script with instructions for importing TextMeshPro essentials.
/// </summary>
public class TMPEssentialsGuide : MonoBehaviour
{
    /*
    TEXTMESHPRO ESSENTIALS INSTALLATION GUIDE
    =========================================
    
    The errors you're seeing are likely because TextMeshPro essentials haven't been imported.
    This is a common issue when working with TextMeshPro for the first time in a project.
    
    To fix this issue:
    
    1. Open the TextMeshPro menu
       - Go to Window > TextMeshPro > Import TMP Essential Resources
    
    2. Click "Import" in the dialog that appears
       - This will import the default TMP fonts and materials
    
    3. Wait for the import to complete
       - You'll see a confirmation dialog when it's done
    
    4. Restart the Unity Editor
       - This ensures everything is properly initialized
    
    After completing these steps, the UISetupHelper script should work correctly.
    
    Note: You can safely delete this script after you've completed these steps.
    */
    
    private void Awake()
    {
        Debug.Log("Please import TextMeshPro essentials. See TMPEssentialsGuide.cs for instructions.");
    }
    
    private void OnEnable()
    {
        Debug.Log("TextMeshPro essentials required! See TMPEssentialsGuide.cs for instructions.");
    }
}
