# BlockBall Evolution - Project Overview

## 1. Architecture

### CRITICAL: UI Architecture (NEVER CHANGE THIS)
**DO NOT CREATE INDIVIDUAL CANVAS COMPONENTS FOR UI SCREENS**

The UI system uses **ONE MainCanvas** with all UI screens as child GameObjects:
```
MainCanvas (Canvas, CanvasScaler, GraphicRaycaster)
├── MainMenuScreen (GameObject with UI components)
├── GameScreen (GameObject with UI components) 
├── LoadingScreen (GameObject with UI components)
├── IntroScreen (GameObject with UI components)
```

**Key Points:**
- UI screens are **GameObjects with RectTransform + CanvasGroup**
- They do **NOT** have individual Canvas components
- All rendering goes through the single MainCanvas
- Screen switching = activating/deactivating child GameObjects
- Registration happens via UIInitializer scanning MainCanvas children

**Common Mistakes to Avoid:**
- Adding Canvas components to individual screens
- Creating standalone prefabs that need instantiation
- Trying to make screens independent of MainCanvas
- Ignoring existing screen GameObjects in scene hierarchy

### Core Structure
The game follows a component-based architecture with several distinct systems:

#### State Management System
- `ScreenStateManager`: Controls game states (Menu, Game, Loading, Intro)
- `WorldStateManager`: Manages world-related states
- Implemented through a Singleton pattern for global access

#### Level System
- `Level`: Core class that loads, builds and manages level components
- `LevelSet` & `Campain`: Handle collections of levels
- XML-based level definition format stored in `.lvl` files

#### Object Hierarchy
- `BaseObject`: Root class for all interactive objects
- `BallObject`: Base for ball-related objects
- `PhysicObject`: Handles physics interactions
- Key gameplay objects:
  - `Player`: The player-controlled ball
  - `Block`: Building blocks of levels
  - `GravitySwitch`: Changes gravity direction
  - `Goal`: Level completion trigger
  - `ScoreItem`: Collectible diamonds (also function as checkpoints)
  - `KeyItem`: Colored keys to unlock doors
  - `DoorObject`: Gates requiring specific keys
  - `InfoObject`: Displays help messages

#### Camera System
- `PlayerCameraController`: Handles camera movement, rotation, zoom
- Supports full rotation and alignment with gravity direction
- Implements transparency for obstructing objects

#### UI System
- **Migrated to Unity's native UI system** (migration functionally complete)
- `StandardUIManager`: Main UI manager handling all screen states
- Uses TextMeshPro for all text rendering
- `UISetupHelper`: Utility for configuring UI components
- Single `MainCanvas` architecture with UI screens as child GameObjects
- **Clean UI Architecture**:
  - `ScreenStateManager`: Controls high-level game states (Menu, Game, Editor)
  - `StandardUIManager`: Shows/hides screen GameObjects based on current state
  - `PanelGroupManager`: Manages panels within each screen (e.g., MainMenu, LevelSelection)
  - `MainMenuUI`: Handles button events and panel navigation within MainMenuScreen
  - `LevelSelectionManager`: Handles level listing and selection functionality
- **STATUS**: UI architecture refactored and simplified; screen transitions and panel navigation working
- **REMAINING**: Complete final UI styling and refinement

### Technical Implementation
- Uses Unity's Rigidbody physics for ball movement
- Block-based level design using 8x8x8 "bixels"
- XML serialization for level data
- Object pooling for certain game elements

## Key Architecture - Level Loading System

**CRITICAL UNDERSTANDING:** BlockMerger does NOT load Unity scenes!
The project progress has been reverted to a previous state. Previous attempts:

### How Level Loading Actually Works:
- **BlockMerger.LoadLevel()** creates `new AssemblyCSharp.Level(levelFileName)`
- **Level.Build()** reads XML level files from disk and creates GameObjects:
  - Creates `"LevelCubes"` GameObject with level geometry  
  - Creates `"BlockGroups"` GameObject with game objects
  - Parses XML files from `Definitions.relLevelFolder + fileName + ".xml"`
- **MainScene approach**: Load level content dynamically, not different Unity scenes
- **testcamera works** because it has BlockMerger component with valid level file

### Level Loading Process:
1. Find BlockMerger component in scene
2. Call `merger.LoadLevel()` to parse XML and create GameObjects
3. If no BlockMerger exists, create Level directly: `new Level("levelName").Build()`

### Common Mistakes:
- ❌ Trying to load different Unity scenes for levels
- ❌ Looking for specific GameObject names like "BlockMerger_TEST" 
- ✅ Look for any BlockMerger component, any name
- ✅ Understand this is XML file parsing, not scene loading





## 2. Feature Status

| Feature | Status | Notes |
|---------|--------|-------|
| **Core Gameplay** | ✅ Functional | Ball movement, physics, jumping all working |
| **Level Loading** | ✅ Functional | XML-based level loading with StartObject positioning fixed |
| **Level Selection** | ✅ Functional | UI-based level selection with campaign system |
| **Camera System** | ✅ Functional | Rotation, zoom, and gravity alignment implemented |
| **Gravity System** | ✅ Functional | 90° and 180° gravity rotation working |
| **Collectibles** | ✅ Functional | Diamonds and keys implemented |
| **UI System** | ⚠️ Partial | Game UI partially working (score/keys display functional), but screen transitions need completion |
| **Checkpoints** | ✅ Functional | Last collected diamond serves as checkpoint |
| **Scoring System** | ⚠️ Partial | Basic scoring works, time-based bonus needs verification |
| **Main Menu** | ⚠️ In Progress | Being rebuilt with new UI system |
| **Level Editor** | ❌ Not Found | Editor functionality not identified in codebase |
| **Campaign Mode** | ⚠️ Partial | Basic structure exists but may need completion |
| **Visual Effects** | ⚠️ Minimal | Limited effects implemented |
| **Sound System** | ❓ Unknown | Sound implementation not identified in examined code |

## 3. Issues and Required Cleanup

### Critical Issues

1. **Unity 2022.3 Compatibility**
   - While major API incompatibilities have been addressed, some issues may remain
   - MovieTexture-dependent code has been temporarily disabled but needs proper replacement

3. **Code Modernization**
   - Several deprecated Unity patterns and coding practices need updating
   - Method hiding warnings have been addressed but code quality could be improved

### UI Screen Transition Issues

The UI screen transition system has several critical architectural flaws that prevent proper functioning (may be outdated and need re-analysis):

1. **Class Inheritance Conflicts**
   - `ScreenStateLoading` attempts to use coroutines (`StartCoroutine`) but it's not a MonoBehaviour
   - `ScreenStateBase` has abstract methods but inconsistent implementation across derived classes
   - `Hide()` method in `ScreenStateLoading` incorrectly calls the abstract base method

2. **Variable Name Inconsistencies**
   - Several variables in `ScreenStateLoading` referenced with wrong names (`m_bLoadingComplete` vs `levelLoadAttempted`)
   - Naming conventions are mixed between Unity standard and custom prefixing

3. **Canvas Management Problems**
   - `StandardUIManager` assumes a `mainCanvas` reference that might be null
   - `UIInitializer` doesn't properly ensure the canvas hierarchy is set up before registering screens
   - Screen registration access level issues (private vs public methods)

4. **State Management Flaws**
   - Inconsistent access to screen state types (`ScreenStateType` property vs direct field access)
   - Error handling is incomplete during state transitions
   - Fallback mechanisms not properly implemented for transition failures



### Required Cleanup Tasks (Priority Order)


2. **Update Game UI Components**
   - Complete implementation of:
     - Main menu screen
     - Game screen with score, time, and key indicators
     - Loading screen
     - Intro screens

3. **Fix Visual Inconsistencies**
   - Update materials for modern rendering pipeline
   - Replace any deprecated shader code
   - Ensure lighting works correctly with current Unity version

4. **Code Refactoring**
   - Address any remaining compiler warnings
   - Improve code organization and readability
   - Remove commented-out legacy code
   - Add proper documentation

5. **Physics System Bugfix**
   - Blocking issue: The ball sometimes collides with something when it should roll smoothly over blocks
   - Likely due to a bug in the physics system, needs investigation

6. **Testing & Debugging**
   - Test gameplay mechanics thoroughly
   - Verify level loading works correctly
   - Ensure scoring system functions as intended
   - Check performance on target platforms

7. **Asset Optimization**
   - Review and optimize 3D assets
   - Update textures for modern resolution standards
   - Ensure audio is properly implemented with the new Unity Audio system

8. **Feature Completion**
   - Verify or implement the bonus time system
   - Complete any missing gameplay features described in the design doc
   - Add sound effects and music if missing

## 4. Development Roadmap Suggestion

1. **Phase 1: Foundation Repair**
   - Complete UI migration from PowerUI to Unity UI
   - Fix all remaining Unity 2022.3 compatibility issues
   - Basic testing of core gameplay mechanics

2. **Phase 2: Refinement**
   - Improve visuals with modern Unity features
   - Update UI design for better user experience
   - Optimize performance

3. **Phase 3: Feature Completion**
   - Implement any missing features from the original design
   - Add additional polish and feedback elements
   - Create level editor if not present

4. **Phase 4: Release Preparation**
   - Full testing across all gameplay systems
   - Final optimization
   - Platform-specific adaptations if needed

## Current Status (Session End - 2025-06-03)


### 📊 **TECHNICAL ANALYSIS:**

#### Architecture Understanding CORRECTED:
- **BlockMerger**: Loads XML level files and creates GameObjects dynamically, NOT Unity scenes
- **MainScene**: Single scene with dynamic level content loading
- **testcamera**: Works because it HAS BlockMerger component (reference implementation)



## Next Tasks

### 🚩 **Priority Task: Fix Level Loading from Level Selection Menu**
- The UI architecture is now working correctly, but level loading from the LevelSelectionManager is not functioning
- When clicking on level buttons, they should load the selected level but this is currently not working
- Verify that BlockMerger is properly connected and receiving the level name
- Check that level files are being found in Resources/Levels with proper naming (_campain suffix)
- Test the path from level button click → LoadLevel() → BlockMerger.LoadLevel()

### 📁 **ARCHITECTURE CONFIRMED:**
- **UI**: Single MainCanvas with child screen GameObjects (no individual Canvas components)
- **Levels**: XML file parsing creates LevelCubes and BlockGroups GameObjects dynamically
- **States**: Screen state management controls UI transitions  
- **Loading**: BlockMerger.LoadLevel() creates new Level(fileName).Build() to parse XML and create GameObjects
- **Critical**: MainScene loads level content dynamically, does NOT load different Unity scenes

## Critical Issues Identified

## 🎨 **UI DESIGN SPECIFICATIONS** (Based on Reference Images)

### In-Game UI Layout Specification
- **KEYS DISPLAY**: Upper left corner - showing collected keys with Key_*.dds graphics
- **TIMER**: Top center - countdown timer in MM:SS format
- **DIAMONDS/SCORE**: Upper right area - diamond count display
- **TIMEBAR**: Right side of screen - vertical progress bar
- **INFO TEXT**: Lower area - contextual game information and messages
- **LAYOUT PRINCIPLE**: HUD elements positioned around screen edges, keeping center clear for gameplay

---

## 📋 **NEXT SESSION TASKS**

### PRIORITY 1: UI Layout Implementation

2. **In-Game UI Positioning**
   - Center timer at top center of screen
   - Position diamonds/score in upper right
   - Create and position timebar on right side
   - Move info text to lower screen area

3. **Font Integration**
   - Apply custom fonts across all UI elements
   - Ensure consistent typography throughout

### PRIORITY 2: Level Loading Debugging  
1. **Diagnostic Implementation**
   - Identify missing components or configurations

2. **Integration Verification**
   - Verify audio integration (WinSound.wav, roll.wav)

## 6. Recent Major Fixes (December 2024)

### Critical StartObject Position Fix
**Problem**: Player ball was spawning at incorrect positions in levels despite correct StartObject coordinates in level files.

**Root Cause**: Execution order issue where `Player.Start()` captured the initial position before `Level.Build()` set the StartObject position. When `Player.ResetPosition()` was called (e.g., when hitting dead zones), it reset to the wrong saved position.

**Solution**: Added reflection-based fix in `Level.cs` StartObject processing to update Player's internal saved position fields (`xLastCollectedScoreItemPosition`, `xOrientationAsLastCollectedScoreItem`) after setting StartObject position.

**Impact**: All levels now properly position the player ball at the intended starting location.

### Level Loading Architecture Completion
- Fixed scene file references that still pointed to old `_campain` suffix filenames
- Completed integration between UI level selection and level loading system
- Added proper state transitions from level selection to loading screen
- Resolved duplicate key errors in level loading by clearing collections on reload

### UI System Stabilization
- Confirmed and documented the single-Canvas architecture
- Fixed level selection UI integration with campaign system
- Established proper screen state management flow

---

## 7. Next Development Phase

### Planned: Level Selection UI Enhancement
The next development session will focus on improving the level selection interface:
- Enhanced visual design for level buttons
- Level preview thumbnails
- Progress indicators and completion status
- Improved navigation and user experience
