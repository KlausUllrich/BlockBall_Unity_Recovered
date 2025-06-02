using UnityEngine;

/// <summary>
/// This guide explains how to upgrade legacy GUI components to modern Unity UI.
/// </summary>
public class LegacyGUIUpgradeGuide : MonoBehaviour
{
    /*
    LEGACY GUI COMPONENTS UPGRADE GUIDE
    ===================================
    
    You're seeing errors about deprecated GUI components like "GUI Text" and "GUI Layer".
    These were part of Unity's old OnGUI system and have been replaced with the new UI system.
    
    Here's how to upgrade each component:
    
    1. GUI Text → TextMeshProUGUI
       ---------------------------
       For each GameObject with a GUI Text component (DevPlayerScoreLable, DevPlayerInfoLable, DevPlayerKeysLable):
       a. Right-click in Hierarchy → UI → Text - TextMeshPro
       b. Position the new text object where the old one was
       c. Copy the text content from the old to the new
       d. Update any script references to use the new text objects
    
    2. GUI Layer → No replacement needed
       ---------------------------------
       The GUI Layer component was used by the old GUI system and has no direct replacement.
       It can be safely removed from your MainCamera.
    
    AUTOMATIC UPGRADE PROCEDURE
    ==========================
    
    For the testcamera.unity scene:
    
    1. Create a Canvas
       ---------------
       a. Right-click in Hierarchy → UI → Canvas
       b. Make sure the Canvas Scaler is set to "Scale With Screen Size"
       c. Reference Resolution: 1920 x 1080
    
    2. Replace DevPlayerScoreLable
       ---------------------------
       a. Create a new Text - TextMeshPro under the Canvas
       b. Name it "PlayerScoreLabel"
       c. Set the text to "Score: 0"
       d. Position it in the top-left corner of the screen
       e. Update the Player.cs script reference to this new object
    
    3. Replace DevPlayerInfoLable
       -------------------------
       a. Create a new Text - TextMeshPro under the Canvas
       b. Name it "PlayerInfoLabel"
       c. Set the text to "" (empty by default)
       d. Position it in the center of the screen
       e. Update the InfoObject.cs script reference to this new object
    
    4. Replace DevPlayerKeysLable
       -------------------------
       a. Create a new Text - TextMeshPro under the Canvas
       b. Name it "PlayerKeysLabel"
       c. Set the text to "Keys: "
       d. Position it in the top-right corner of the screen
       e. Update the Player.cs script reference to this new object
    
    5. Remove GUI Layer from MainCamera
       -------------------------------
       a. Select MainCamera in the Hierarchy
       b. Find the GUI Layer component in the Inspector
       c. Click the gear icon and select "Remove Component"
    
    6. Save the Scene
       -------------
       a. Save the scene with a new name (e.g., "testcamera_upgraded")
       b. This will preserve your original scene in case you need to reference it later
    
    SCRIPT REFERENCES
    ================
    
    You'll need to update the following script references:
    
    1. In Player.cs:
       - pScoreText should reference the new PlayerScoreLabel
       - pKeysText should reference the new PlayerKeysLabel
    
    2. In InfoObject.cs:
       - pGUILable should reference the new PlayerInfoLabel
    
    Note: We've already updated the scripts to use TextMeshProUGUI and Text components,
    so you just need to assign the correct GameObject references in the Inspector.
    */
}
