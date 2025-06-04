# Level Selection Implementation Guide

This guide explains how to set up the temporary level selection menu for BlockBall. This implementation follows the project's existing UI architecture (single MainCanvas with child GameObjects for screens) and adds robust panel management.

## Setup Instructions

### 1. Setup in Editor

1. **Add the LevelSelectionSetupHelper Component**
   - Select the GameObject containing the `StandardUIManager` component
   - Add the `LevelSelectionSetupHelper` component
   - Assign the `MainMenuScreen` to the "Main Menu Screen Transform" field

2. **Create Level Selection Panel**
   - With the LevelSelectionSetupHelper component selected, click "1. Create Level Selection Panel" in the context menu
   - This will create a complete panel with ScrollView, Viewport, Content area, and Back button

3. **Set Up Panel Group Manager**
   - Click "2. Set Up Panel Group Manager" in the context menu
   - This will add and configure the PanelGroupManager component for better UI organization
   - It creates a container for main menu buttons and registers all panels

4. **Add Level Selection Manager**
   - Click "3. Add Level Selection Manager" in the context menu
   - This will add the LevelSelectionManager component and configure references

5. **Create Level Button Prefab**
   - Create a new Button prefab in your project (or use the provided template)
   - Add the `LevelButtonUI` component to the prefab
   - Assign a TextMeshPro component reference to the `Level Name Text` field

6. **Configure LevelSelectionManager**
   - Assign the button prefab to the "Level Button Prefab" field
   - Assign "Levels" (or your custom value) to the "Levels Folder" field

### 2. Manual Setup (Alternative)

1. **Create LevelSelectionPanel GameObject**
   - Create a new child GameObject under MainMenuScreen named "LevelSelectionPanel"
   - Add a RectTransform, Image component
   - Create child objects:
     - TitleText (TextMeshPro)
     - BackButton (Button)
     - ScrollView with Viewport and Content

2. **Add Components**
   - Add LevelSelectionManager to MainMenuScreen
   - Configure all references

## Architecture Notes

- **Panel-based Approach**: The level selection is implemented as a panel within the MainMenuScreen rather than a separate screen in StandardUIManager
- **Panel Group Management**: Uses PanelGroupManager to handle panel visibility in a centralized way
- **Level Loading**: Uses BlockMerger to load levels from XML files
- **Navigation**: Back button returns to main menu through the panel manager
- **Button Creation**: LevelSelectionManager dynamically generates buttons for each level file

## Implementation Details

1. **PanelGroupManager.cs**
   - Centralized management of UI panels
   - Ensures only one panel is visible at a time
   - Handles panel transitions through a clean API
   - Prevents visibility management errors

2. **LevelSelectionManager.cs**
   - Integrates with PanelGroupManager
   - Loads level list from Resources folder
   - Creates buttons for each level

3. **LevelButtonUI.cs**
   - Simple component for level buttons
   - Stores and displays level name

4. **MainMenuUI.cs (Modified)**
   - Added integration with PanelGroupManager
   - Updated to use panel IDs instead of direct GameObject references

5. **ScreenStateMenu.cs (Modified)**
   - Ensures proper BlockMerger creation
   - Sets default level when Play button is clicked
   
6. **LevelSelectionSetupHelper.cs**
   - Handles creation of all UI components
   - Sets up PanelGroupManager with proper panel configuration
   - Provides easy menu setup through context menu functions

## Usage

1. Click "Select Level" in the main menu
2. Level selection panel appears with scrollable list of available levels
3. Click a level to load it with BlockMerger
4. Click "Back" to return to the main menu

## Troubleshooting

- If levels are not appearing, check that they're correctly placed in `Assets/Resources/Levels/`
- If buttons don't respond, ensure click events are properly connected
- If panel doesn't appear, check that the LevelSelectionManager references are correctly assigned
