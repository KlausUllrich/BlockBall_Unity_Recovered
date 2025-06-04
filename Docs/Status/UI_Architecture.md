# BlockBall UI Architecture

## UI Component Hierarchy

```
GameManager                     # Global game state and management
└── ScreenStateManager          # Manages high-level game states (Menu, Game, Loading)
    
Canvas                          # Single UI Canvas for all UI
├── StandardUIManager           # Manages which screens are visible
│
├── MainMenuScreen              # Screen for main menu 
│   ├── PanelGroupManager       # Manages panels within MainMenuScreen
│   ├── WhiteBackgroundPanel    # Shared background
│   ├── GameLogo                # Shared logo
│   ├── MainMenuPanel           # Panel for main menu buttons
│   │   ├── PlayButton
│   │   ├── SelectLevelButton
│   │   └── ExitButton
│   ├── LevelSelectionPanel     # Panel for level selection
│   │   ├── TitleText
│   │   ├── BackButton
│   │   └── ScrollView          # For level buttons
│   └── SettingsPanel           # Panel for settings (future)
│       └── [Settings UI elements]
│
├── GameScreen                  # Screen for gameplay
│   ├── PanelGroupManager       # Manages panels within GameScreen
│   ├── GameplayPanel           # Default gameplay UI
│   │   └── [Gameplay UI elements]
│   └── PausePanel              # Panel for pause menu
│       └── [Pause UI elements]
│
└── EditorScreen                # Screen for level editor (future)
    ├── PanelGroupManager       # Manages panels within EditorScreen
    ├── EditorMainPanel         # Main editor interface
    └── EditorSettingsPanel     # Editor settings
```

## Component Responsibilities

### 1. High-Level State Management
- **ScreenStateManager**: Controls the overall game state (Menu, Game, Loading, etc.)
- **ScreenStateMenu**: Handles logic specific to the menu state
- **StandardUIManager**: Shows/hides entire screen GameObjects based on game state

### 2. Screen-Specific Panel Management
- **PanelGroupManager (MainMenuScreen)**: Controls which panel is visible within MainMenuScreen
- **PanelGroupManager (GameScreen)**: Controls which panel is visible within GameScreen
- **PanelGroupManager (EditorScreen)**: Controls which panel is visible within EditorScreen (future)
- Each screen has its own independent panel system

### 3. Panel-Specific Logic
- **MainMenuUI**: Handles buttons and events for the main menu panel
- **LevelSelectionManager**: Manages level selection functionality
- Other panel controllers handle their specific functionality

## Flow of Control

1. **ScreenStateManager** determines high-level game state
2. **StandardUIManager** activates the corresponding screen
3. **Screen-specific PanelGroupManager** shows the appropriate panel within that screen
4. Panel-specific components handle their own functionality

## Panel Navigation Guidelines

### Within a Screen
Each screen's PanelGroupManager controls its own panels:

```csharp
// In MainMenuUI.cs
private PanelGroupManager panelManager; // Reference to parent screen's panel manager

public void ShowLevelSelection()
{
    panelManager.ShowPanel("LevelSelection");
}
```

### Between Screens
Use the ScreenStateManager to switch screens:

```csharp
// In a button handler
public void StartGame()
{
    // Find the ScreenStateManager
    ScreenStateManager manager = FindObjectOfType<ScreenStateManager>();
    
    // Change to game state
    manager.ChangeToScreenState(ScreenStateManager.ScreenStates.Game);
}
```
