# BlockBall Evolution - Project Overview

## Project Name
Blockball Resurrection

## Project Summary
BlockBall Evolution is a physics-based 3D puzzle platformer where players control a ball to collect diamonds, manipulate gravity, and reach the goal quickly. Levels are modular, grid-based, and build out of different types of "blocks". Each block is 1x1x1 Unity units, which is divided into so called bixels(8×8×8 bixels per block). Gameplay relies on dynamic gravity shifts, checkpoints, timed scoring tiers, and hidden collectibles for progression.

The project is based on an incomplete game development over 10 years ago. It should now be brought to life with current technology.

## Technology Used:
- Unity 2022.3 
- Windows with Powershell


## Blockball – Project Structure

| Area              | Path(s)/Key Files                                                                                     |
|-------------------|-------------------------------------------------------------------------------------------------------|
| Root              | C:\Users\Klaus\My_Game_Projects\Blockball\BlockBall_Unity_Recovered                                   |
| Project Infos     | root\Docs\Status\                                                                                     |
| Game Design       | root\Docs\Design\BlockBall_Evolution_Design_EN.md                                                     |
| Project Overview  | \Status\Project_Overview.md                                  										             |


## Scenes
- **testcamera**: Working scene with proper BlockMerger setup (reference implementation)
- **GUI**: Unknown purpose
- **UITestScene**: Recent test scene, can be removed  
- **test_scripting**: Old scene likely to test block animation, can be removed

##  Architecture

### UI System Architecture
The UI system uses **ONE MainCanvas** with all UI screens as child GameObjects:
```
MainCanvas (Canvas, CanvasScaler, GraphicRaycaster)
├── MainMenuScreen (GameObject with UI components)
├── GameScreen (GameObject with UI components) 
├── LoadingScreen (GameObject with UI components)
├── IntroScreen (GameObject with UI components)
```
- **Clean UI Architecture**:
  - `WorldStateManager`: Manages world-related states
  - `ScreenStateManager`: Controls high-level game states (Menu, Game, Editor)
  - `StandardUIManager`: Shows/hides screen GameObjects based on current state
  - `PanelGroupManager`: Manages panels within each screen (e.g., MainMenu, LevelSelection)
  - `MainMenuUI`: Handles button events and panel navigation within MainMenuScreen
  - `LevelSelectionManager`: Handles level listing and selection functionality

- UI screens are GameObjects with RectTransform + CanvasGroup 
- They do NOT have individual Canvas components
- Screen switching = activating/deactivating child GameObjects


**Common Mistakes to Avoid:**
- Adding Canvas components to individual screens
- Creating standalone prefabs that need instantiation
- Trying to make screens independent of MainCanvas
- Ignoring existing screen GameObjects in scene hierarchy


### Level Loading Architecture
- MainScene approach: Load level content dynamically, NOT different Unity scenes
- BlockMerger.LoadLevel() creates `new Level(fileName)` and calls `Build()` to parse XML files
- BlockMerger.cs loads XML level files, NOT Unity scenes
- Level.Build() reads XML from `Assets/Resources/Levels/` and creates GameObjects dynamically

**Level.Build()** reads XML level files from disk and creates GameObjects:
  - Creates `"LevelCubes"` GameObject with level geometry  
  - Creates `"BlockGroups"` GameObject with game objects
  - Parses XML files from `Definitions.relLevelFolder + fileName + ".xml"`


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


### Known Issues

**Physics System Bugfix**
   - Blocking issue: The ball sometimes collides with something when it should roll smoothly over blocks
   - Likely due to a bug in the physics system, needs investigation

**Testing & Debugging**
   - Test gameplay mechanics thoroughly
   - Verify level loading works correctly
   - Ensure scoring system functions as intended

 **Asset Optimization**
   - Review and optimize 3D assets
   - Update textures for modern resolution standards
   - Ensure audio is properly implemented with the new Unity Audio system

**Feature Completion**
   - Verify or implement the bonus time system
   - Complete any missing gameplay features described in the design doc
   - Add sound effects and music if missing



## 🎨 **UI DESIGN SPECIFICATIONS** (Based on Reference Images)

### In-Game UI Layout Specification
- **KEYS DISPLAY**: Upper left corner - showing collected keys with Key_*.dds graphics
- **TIMER**: Top center - countdown timer in MM:SS format
- **DIAMONDS/SCORE**: Upper right area - diamond count display
- **TIMEBAR**: Right side of screen - vertical progress bar
- **INFO TEXT**: Lower area - contextual game information and messages
- **LAYOUT PRINCIPLE**: HUD elements positioned around screen edges, keeping center clear for gameplay

---

## BlockBall Physics Migration - Project Overview

## Current Status
- **Phase 0: Foundation and Compatibility Layer** - Documentation Complete
  - All overview and implementation task documents have been tailored for LLM usage with modular structure.
  - Files: `LLM_01A` to `LLM_01E` (overview) and `LLM_02A` to `LLM_02E` (Phase 0A tasks) in `Phase0_Migration_Strategy/`.
  - Status: Ready for implementation of Phase 0A tasks.

- **Phase 1: Hybrid Implementation** - Documentation Complete
  - Documentation for hybrid physics system and testing framework created with LLM-tailored format.
  - Files: `LLM_03A_Phase1_Overview.md` and `LLM_03B_Phase1_Tasks.md` in `Phase1_Migration_Strategy/`.
  - Status: Awaiting completion of Phase 0 before starting Phase 1 tasks.

## Next Steps
1. **Implement Phase 0A Tasks**: Begin with `PhysicsSettings` implementation as per `LLM_02A_PhysicsSettings_Task.md`.
2. **Validate Phase 0A Completion**: Use `LLM_02E_Phase0A_Checklist.md` to ensure all criteria are met.
3. **Proceed to Phase 0B and 0C**: After Phase 0A validation, continue with hybrid and selective migration steps.
4. **Start Phase 1**: Only after Phase 0 completion, implement hybrid physics system as outlined in `LLM_03B_Phase1_Tasks.md`.

## Issues and Cleanup
- No immediate issues in documentation. Any implementation issues should be logged in `Issues_and_Required_Cleanup.md`.
- Old documentation files (non-LLM prefixed) in `Phase0_Migration_Strategy/` are retained as reference until user confirms deletion.

**Directive for LLM**: Update this file with progress on implementation tasks. Log specific issues or blockers in `Issues_and_Required_Cleanup.md`. Ensure all validation steps from documentation are followed before marking tasks as complete.

## Backlog TASKS**

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
1. **Integration Verification**
   - Verify audio integration (WinSound.wav, roll.wav)

## Recent Major Fixes

### Critical StartObject Position Fix
**Problem**: Player ball was spawning at incorrect positions in levels despite correct StartObject coordinates in level files.

**Root Cause**: Execution order issue where `Player.Start()` captured the initial position before `Level.Build()` set the StartObject position. When `Player.ResetPosition()` was called (e.g., when hitting dead zones), it reset to the wrong saved position.

**Solution**: Added reflection-based fix in `Level.cs` StartObject processing to update Player's internal saved position fields (`xLastCollectedScoreItemPosition`, `xOrientationAsLastCollectedScoreItem`) after setting StartObject position.

**Impact**: All levels now properly position the player ball at the intended starting location.