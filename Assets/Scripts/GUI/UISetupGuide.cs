using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This is a reference guide file - NOT a MonoBehaviour script.
/// It provides instructions on how to set up the Unity UI prefabs that replace the PowerUI HTML-based UI.
/// </summary>
public class UISetupGuide
{
    /*
    STEP 1: CREATE THE CANVAS
    -------------------------
    - Right click in Hierarchy -> UI -> Canvas
    - Set Canvas Scaler to "Scale with Screen Size" with reference resolution 1920x1080
    - Add an EventSystem if not automatically created

    STEP 2: CREATE THE UI SCREENS
    ----------------------------
    Create the following screens as children of the Canvas:

    1. Main Menu Screen:
    - Create GameObject named "MainMenuScreen"
    - Add Image component for background (optional)
    - Create Button named "Play" with TextMeshPro text component that says "Play"
    - Create Button named "Exit" with TextMeshPro text component that says "Exit"
    - Position the buttons appropriately in the center of the screen

    2. Game Screen:
    - Create GameObject named "GameScreen"
    - Add any game UI elements needed (can be empty initially)

    3. Loading Screen:
    - Create GameObject named "LoadingScreen" 
    - Add loading text or spinning icon as desired

    4. Intro Screens:
    - Create GameObject named "IntroGamesAcademyLogo"
    - Add Image component to display the Games Academy logo
    - Add EventTrigger component to handle click events
    
    - Create GameObject named "Intro4MindsLogo"
    - Add Image component to display the 4Minds logo
    - Add EventTrigger component to handle click events

    STEP 3: SETUP THE STANDARD UI MANAGER
    ------------------------------------
    - Create an empty GameObject in the scene
    - Add the StandardUIManager component
    - Assign the UI screens to the corresponding fields in the inspector
    
    STEP 4: CONNECT TO SCREEN STATE MANAGER
    --------------------------------------
    - Find the ScreenStateManager in the scene
    - Assign the StandardUIManager to the m_xStandardUIManager field

    STEP 5: IMPORT UI ASSETS
    ----------------------
    - Import the image files used in the HTML UI into a UI folder in your project
    - Assign these images to the appropriate Image components in your UI
    */
}
