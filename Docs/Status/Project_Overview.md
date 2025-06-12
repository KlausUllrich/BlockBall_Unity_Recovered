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
- **Phase 0A: Preparation & Compatibility Layer** - Completed
  - [x] **PhysicsSettings ScriptableObject**: Created and implemented to centralize physics parameters.
  - [x] **IPhysicsObject Interface & Wrapper**: Implemented and attached to PlayerSphere, validated with no observable behavior changes.
  - [x] **DeterministicMath**: Implemented for consistent physics calculations using float32 precision, integrated and validated with no behavior changes.
  - [x] **Physics Profiling**: Implemented to monitor performance impact during physics migration, setup instructions provided for scene integration.
- **Phase 0B: Hybrid Implementation** - Completed
  - [x] **Task 0B.1 to 0B.6**: Implemented isolation of old system forces, adapted player movement logic, controlled script execution order, enhanced mode differentiation, defined Hybrid mode behavior, and added detailed logging.
- **Phase 0C: Selective Migration** - In Progress
  - [x] **Jump Buffering Fix**: Completed with enhanced mechanics for frame-accurate input detection and physics-aligned execution. Configurable buffer times and logging added.
  - [x] **Speed Limits Debugging - Code Implementation**: Completed with configurable directional force magnitudes and input force scale in `PhysicsSettings.cs`. Movement forces implemented in CustomPhysics mode. Speed limit enforcement consolidated in `PhysicsObjectWrapper.cs`. Reorganized settings by mode with detailed tooltips.
  - [x] **Speed Limits Debugging - Validation**: Completed. User feedback confirms speed limits are working across UnityPhysics, Hybrid, and CustomPhysics modes.
  - [ ] **Legacy Parameters Impact**: Pending. User feedback indicates `legacySpeedFactor` and `legacyBreakFactor` do not have observable effects in UnityPhysics mode. Investigation and potential fixes required.
  - [ ] **CustomPhysics Input Direction**: Pending. User feedback notes that input directions in CustomPhysics mode are absolute, not camera-relative. Update needed to align movement with camera orientation.
  - [ ] **Profiling and Optimization**: Not started. Next focus area to monitor performance impacts of migrated systems.

## Next Steps
- **Address Legacy Parameters Issue**: Investigate and fix why `legacySpeedFactor` and `legacyBreakFactor` have no observable impact in UnityPhysics mode.
- **Fix CustomPhysics Input Direction**: Update movement logic in CustomPhysics mode to be relative to camera orientation.
- **Begin Profiling and Optimization**: Attach and review `PhysicsProfiler` in the main scene to establish performance metrics for Phase 0C changes.
- Address gaps in Phase 0 planning regarding airborne gravity transitions and instant gravity snap in subsequent phases.
- Continue validation through subsequent phases as per the migration plan.

## Validation Summary
- **Jump Height Consistency**: Addressed via parameter migration to `PhysicsSettings`.
- **Smooth Block Transitions**: Supported by compatibility focus.
- **Rolling Feel**: Preserved through functionality maintenance, but slope specifics missing.
- **Gravity While Airborne**: Not addressed in Phase 0 - critical gap.
- **Instant Gravity Snap**: Not covered in reviewed documents - gap identified.

**Note**: Detailed issues and gaps are logged in `/Docs/Status/Issues_and_Required_Cleanup.md`.

## Long-Term Goals
- Complete physics migration through subsequent phases, building on Phase 0's foundation.
- Enhance gameplay with deterministic physics for consistency across platforms.
- Optimize performance and maintain backward compatibility for existing players.

## Session Continuity
To ensure progress across sessions, refer to this document for the latest status of Phase 0 tasks. Updates to code, documentation, and task completion will be logged here. If resuming after a break, review `Phase0_PhysicsMigrationTasks.md` for detailed implementation steps and `OldPhysicsAnalysis.md` for context on system conflicts.

## Current Session Focus (Updated 2025-06-11)
Completed Tasks 0B.1, 0B.2, 0B.3, 0B.4, 0B.5, and 0B.6 to isolate old system forces, adapt player movement logic, control script execution order, enhance physics mode differentiation, define Hybrid mode behavior, and add detailed logging and debugging tools. Phase 0B is now fully implemented. The next focus will be on testing and refinement.

## Recent Major Fixes

### Critical StartObject Position Fix
**Problem**: Player ball was spawning at incorrect positions in levels despite correct StartObject coordinates in level files.

**Root Cause**: Execution order issue where `Player.Start()` captured the initial position before `Level.Build()` set the StartObject position. When `Player.ResetPosition()` was called (e.g., when hitting dead zones), it reset to the wrong saved position.

**Solution**: Added reflection-based fix in `Level.cs` StartObject processing to update Player's internal saved position fields (`xLastCollectedScoreItemPosition`, `xOrientationAsLastCollectedScoreItem`) after setting StartObject position.

**Impact**: All levels now properly position the player ball at the intended starting location.

## BlockBall Physics Migration - Project Overview

## Current Status
- **Phase 0A: Preparation & Compatibility Layer** - Completed
  - [x] **PhysicsSettings ScriptableObject**: Created and implemented to centralize physics parameters.
  - [x] **IPhysicsObject Interface & Wrapper**: Implemented and attached to PlayerSphere, validated with no observable behavior changes.
  - [x] **DeterministicMath**: Implemented for consistent physics calculations using float32 precision, integrated and validated with no behavior changes.
  - [x] **Physics Profiling**: Implemented to monitor performance impact during physics migration, setup instructions provided for scene integration.

## Next Steps
- **Phase 0B: Hybrid Implementation**: Begin implementation of hybrid physics system, allowing toggling between Unity physics and custom calculations while maintaining rollback capability.
- **PhysicsProfiler Testing**: Attach PhysicsProfilerSetup to a GameObject in the main scene and test to establish baseline performance metrics.
- Address gaps in Phase 0 planning regarding airborne gravity transitions and instant gravity snap.
- Continue validation through subsequent phases as per the 6-session plan.

## Validation Summary
- **Jump Height Consistency**: Addressed via parameter migration to `PhysicsSettings`.
- **Smooth Block Transitions**: Supported by compatibility focus.
- **Rolling Feel**: Preserved through functionality maintenance, but slope specifics missing.
- **Gravity While Airborne**: Not addressed in Phase 0 - critical gap.
- **Instant Gravity Snap**: Not covered in reviewed documents - gap identified.

**Note**: Detailed issues and gaps are logged in `/Docs/Status/Issues_and_Required_Cleanup.md`.

## Long-Term Goals
- Complete physics migration through subsequent phases, building on Phase 0's foundation.
- Enhance gameplay with deterministic physics for consistency across platforms.
- Optimize performance and maintain backward compatibility for existing players.

## Session Continuity
To ensure progress across sessions, refer to this document for the latest status of Phase 0 tasks. Updates to code, documentation, and task completion will be logged here. If resuming after a break, review `Phase0_PhysicsMigrationTasks.md` for detailed implementation steps and `OldPhysicsAnalysis.md` for context on system conflicts.

## Current Session Focus (Updated 2025-06-11)
Completed Tasks 0B.1, 0B.2, 0B.3, 0B.4, and 0B.5 to isolate old system forces in `PhysicObject.cs`, adapt player movement logic in `PlayerCameraController.cs`, control script execution order with `ScriptExecutionOrderHelper.cs`, enhance physics mode differentiation, and define Hybrid mode behavior in `PhysicsObjectWrapper.cs`. Next focus is on Task 0B.6 to add detailed logging and debugging tools.

# BlockBall Evolution Project Overview

## Project Summary
BlockBall Evolution is a Unity-based game project focused on evolving gameplay mechanics through a physics migration. The primary goal is to transition from a legacy force-based physics system to a hybrid system that supports toggling between Unity's built-in physics, a custom deterministic physics system, and a hybrid mode. This migration is critical for enhancing gameplay predictability and performance.

## Current Focus: Physics Migration Phase 0

### Objective
Phase 0 (comprising Phase 0A and 0B) aims to establish a compatibility layer and implement hybrid physics mode toggling. The goal is to integrate new physics logic without disrupting existing gameplay, ensuring clear behavioral differences between modes, and maintaining rollback capability.

### Phase Breakdown and Progress
- **Phase 0A: Compatibility Layer** (Completed)
  - **Task 0A.1**: Integrate `DeterministicMath` into `PhysicsObjectWrapper.cs`. (Completed, validated with no gameplay disruption.)
  - **Task 0A.2**: Set up `PhysicsSettings` as a `ScriptableObject`. (Completed, asset created in `Assets/Settings/` with default values.)

- **Phase 0B: Hybrid Implementation and Mode Toggling** (In Progress)
  - **Task 0B.1: Isolate Old System Forces in Custom Modes** (Completed)
    - Objective: Prevent old system forces from interfering with new physics modes.
    - Status: Implemented in `PhysicObject.cs` to skip force application when `physicsMode` is `CustomPhysics` or `Hybrid`.
  - **Task 0B.2: Adapt Player Movement Logic for Physics Modes** (Completed)
    - Objective: Ensure player input respects active physics mode.
    - Status: Implemented in `PlayerCameraController.cs` to skip force and torque application in `CustomPhysics` mode.
  - **Task 0B.3: Control Script Execution Order** (Completed)
    - Objective: Prevent update order conflicts between old and new physics logic.
    - Status: Implemented with `ScriptExecutionOrderHelper.cs` in `Assets/Editor` to set execution order: `PhysicObjekt` (-100), `PlayerCameraController` (-50), `PhysicsObjectWrapper` (100).
  - **Task 0B.4: Enhance Physics Mode Differentiation** (Completed)
    - Objective: Make mode differences visually and behaviorally distinct.
    - Status: Implemented in `PhysicsObjectWrapper.cs` with increased gravity multipliers (1.5x for CustomPhysics, 0.7x for Hybrid) and debug visualizations (colored Gizmos: blue for UnityPhysics, green for CustomPhysics, red for Hybrid).
  - **Task 0B.5: Define and Implement Hybrid Mode Behavior** (Completed)
    - Objective: Clarify and balance Hybrid mode between old and new systems.
    - Status: Implemented in `PhysicsObjectWrapper.cs` with refined logic to allow old system input forces from Player script while applying custom gravity with a reduced multiplier (0.7x) for balance.
  - **Task 0B.6: Add Detailed Logging and Debugging** (Completed)
    - Objective: Diagnose physics interference with detailed logs.
    - Status: Implemented comprehensive logging in `PhysicObject.cs`, `PlayerCameraController.cs`, and `PhysicsObjectWrapper.cs` to track Rigidbody state, physics mode, and force/velocity application decisions for debugging.

### Implementation Plan
The physics migration will continue over multiple sessions, focusing on completing Phase 0B tasks. With Task 0B.6 completed, Phase 0B is now fully implemented. The next steps will be to review and test all changes thoroughly to ensure stability and compatibility. Documentation and any additional cleanup or refinement tasks will be addressed in subsequent sessions. Each session will update this overview with progress and blockers.

## Long-Term Goals
- Complete physics migration through subsequent phases, building on Phase 0's foundation.
- Enhance gameplay with deterministic physics for consistency across platforms.
- Optimize performance and maintain backward compatibility for existing players.

## Session Continuity
To ensure progress across sessions, refer to this document for the latest status of Phase 0 tasks. Updates to code, documentation, and task completion will be logged here. If resuming after a break, review `Phase0_PhysicsMigrationTasks.md` for detailed implementation steps and `OldPhysicsAnalysis.md` for context on system conflicts.

## Current Session Focus (Updated 2025-06-11)
Completed Tasks 0B.1, 0B.2, 0B.3, 0B.4, 0B.5, and 0B.6 to isolate old system forces in `PhysicObject.cs`, adapt player movement logic in `PlayerCameraController.cs`, control script execution order with `ScriptExecutionOrderHelper.cs`, enhance physics mode differentiation, define Hybrid mode behavior, and add detailed logging and debugging tools in relevant scripts. Phase 0B is now complete; the next focus will be on testing and refinement.