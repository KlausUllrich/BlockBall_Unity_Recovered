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
- **STATUS**: Game UI partially working (score/keys display functional), but screen transitions need completion
- **REMAINING**: Complete UI screen prefabs and fix menu→game transitions

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

## DEBUGGING SESSION FINDINGS (2025-06-03)

### ❌ **FAILED ATTEMPT: Runtime BlockMerger Creation**

**What was tried:**
- Modified `ScreenStateLoading.TryLoadLevel()` to create BlockMerger component at runtime when none exists
- Created GameObject with BlockMerger component and set `LevelToLoad = "first_roll_campain"`
- Called `runtimeMerger.LoadLevel()` to trigger level loading

**Result:** ❌ **COMPLETE FAILURE** - MainScene still not working, same issue as before

**Root Cause Analysis - What's actually wrong:**
1. **MainScene missing BlockMerger component** ✅ **CONFIRMED** - this is definitely part of the problem
2. **Level.Build() may be failing** ❓ **UNKNOWN** - runtime creation didn't help, so there may be deeper issues
3. **Level file path issues** ❓ **POSSIBLE** - need to verify if `first_roll_campain.level` loads correctly
4. **Missing dependencies in MainScene** ❓ **LIKELY** - testcamera scene may have other required components

### 🔍 **CRITICAL FINDINGS:**

#### testcamera vs MainScene Differences:
- **testcamera**: ✅ Works but now has "nasty grey appearance" (regression introduced)
- **MainScene**: ❌ Still completely broken after runtime BlockMerger fix
- **Conclusion**: Missing BlockMerger is NOT the only problem

#### What Actually Needs Investigation:
1. **Deep component comparison**: What components/GameObjects exist in testcamera that are missing in MainScene?
2. **Level.Build() debugging**: Add detailed logging to see where Level.Build() fails
3. **XML file validation**: Verify the level file actually loads and parses correctly
4. **Scene hierarchy analysis**: Compare complete scene hierarchies between working and broken scenes

### 🚨 **CRITICAL MISTAKES MADE:**

1. **Assumed BlockMerger was the only issue** - Clearly there are deeper problems
2. **No step-by-step debugging** - Should have added detailed logging to Level.Build() process
3. **No working scene analysis** - Should have studied testcamera scene components in detail
4. **Broke working testcamera** - Introduced "grey appearance" regression
5. **No incremental testing** - Should have verified each step works before moving to next

### 📋 **RECOMMENDED NEXT APPROACH:**

#### Phase 1: Understand the Working Implementation
1. **Analyze testcamera scene completely**:
   - What GameObjects exist in scene hierarchy?
   - What components are attached to each GameObject?
   - What is the BlockMerger configuration exactly?
   - How does the level loading flow work in detail?

2. **Add comprehensive Level.Build() logging**:
   - Log every step of XML parsing
   - Log GameObject creation
   - Log any exceptions or failures
   - Identify exactly where the process breaks

#### Phase 2: Incremental Replication
1. **Copy working testcamera setup to MainScene incrementally**
2. **Test each component addition separately**
3. **Verify level loading works at each step**

#### Phase 3: Root Cause Identification
1. **Find the actual missing piece(s)** between scenes
2. **Fix MainScene by adding only what's necessary**
3. **Document the complete solution**

### ⚠️ **WARNINGS FOR NEXT SESSION:**
- **Don't assume runtime component creation will work** - May need actual scene setup
- **Don't make architectural changes** - Focus on matching working testcamera setup
- **Test incrementally** - Verify each small change works before proceeding
- **Add extensive logging** - Need visibility into what's actually happening during level loading

## Recommended Next Steps (Priority Order)

### 1. Complete UI Screen Setup (HIGH PRIORITY)
**Issue**: Screen transitions fail because StandardUIManager has no screen GameObjects assigned.

**CORRECTION**: UI screen GameObjects already exist under MainCanvas with scripts attached, but StandardUIManager component needs GameObject references assigned.

**Solution Steps**:
1. **Open MainScene in Unity Editor**
2. **Find StandardUIManager component** (likely attached to a GameObject)
3. **Assign the existing UI GameObjects** to StandardUIManager's fields:
   - Drag the MainMenu GameObject to `Main Menu Screen` field
   - Drag the Game GameObject to `Game Screen` field  
   - Drag the Loading GameObject to `Loading Screen` field
   - Drag the Intro GameObject to `Intro Screen` field
4. **Test Flow**: Intro → Menu → Loading → Game

**Note**: The UI scripts (MainMenuUI.cs, GameUI.cs, etc.) are already attached to the GameObjects.

### 2. Clean Up Project Files

## 2. Feature Status

| Feature | Status | Notes |
|---------|--------|-------|
| **Core Gameplay** | ✅ Functional | Ball movement, physics, jumping all working |
| **Level Loading** | ❌ Not Working | XML-based level loading system broken |
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

2. **UI System Screen Transitions**
   - **ACHIEVEMENT**: Level loading and game UI now functional (confirmed working 2025-06-03)
   - **ACHIEVEMENT**: PowerUI HTML files removed (`Game.html`, `MainMenu.html`, etc.)
   - **ISSUE**: Menu→Game screen transitions not working properly
   - **ROOT CAUSE**: StandardUIManager expects GameObject references for screens but they're not assigned in scene
   - **NEEDED**: 
     * Create UI screen GameObjects in scene (MainMenuScreen, GameScreen, LoadingScreen, IntroScreen)
     * Assign these GameObject references to StandardUIManager component
     * Wire up UI prefab classes (GameUI.cs, MainMenuUI.cs, LoadingUI.cs, IntroScreenUI.cs) to their GameObjects
     * Test complete flow: Intro → Menu → Loading → Game

3. **Code Modernization**
   - Several deprecated Unity patterns and coding practices need updating
   - Method hiding warnings have been addressed but code quality could be improved

### UI Screen Transition Issues

The UI screen transition system has several critical architectural flaws that prevent proper functioning:

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

5. **Attempted Fixes**
   - Added null checks for `mainCanvas` references
   - Made `RegisterScreen` method public in `StandardUIManager`
   - Fixed the `Hide()` method in `ScreenStateLoading`
   - Added exception handling for screen registration
   - Replaced coroutine usage with direct method calls in non-MonoBehaviour classes
   - These fixes helped but did not resolve all issues

6. **Recommendation**
   - Consider a full rewrite of the UI screen transition system (in close collaboration with the user)
   - Either make screen state classes inherit from MonoBehaviour or remove all MonoBehaviour dependencies
   - Implement a state machine pattern for more reliable transitions
   - Ensure proper null checks and error handling throughout the system

### Required Cleanup Tasks (Priority Order)

1. **Fix or Rewrite UI Screen Transition System** *(HIGHEST PRIORITY)*
   - Redesign the `ScreenStateBase` class hierarchy to either:
     - Make all screen state classes inherit from MonoBehaviour, OR
     - Remove all MonoBehaviour dependencies (coroutines, etc.)
   - Implement a proper state machine pattern for screen transitions
   - Fix inheritance conflicts and add comprehensive error handling
   - Consider using Unity's Animation system for transitions instead of custom code
   - Ensure consistent variable naming and access patterns

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

### ❌ **CRITICAL ISSUES REMAINING:**

#### 1. **Level Loading Failure - ROOT CAUSE IDENTIFIED**
- **Root Cause**: MainScene has no BlockMerger component, so `Object.FindObjectsOfType<BlockMerger>()` returns empty
- **Attempted Fix**: Modified ScreenStateLoading to create BlockMerger component at runtime when none exists
- **Status**: ❌ **FIX NOT TESTED/VERIFIED** - May still be failing

#### 2. **Initial Screen Name Still Wrong**
- **Issue**: UIInitializer still shows `initialScreenName = 'Intro'` instead of `'IntroScreen'`
- **Expected**: Should be `'IntroScreen'` to match existing screen
- **Impact**: "Initial screen Intro not found!" error on startup
- **Note**: CreateMainScene.cs fix didn't take effect - investigation needed

#### 3. **UI Canvas Components Missing**
- **Issue**: UI Diagnostics reports "WARNING: [Screen] has no Canvas component!" for all screens
- **Expected**: Screens should have Canvas components for visibility
- **Impact**: Potential rendering issues
- **Note**: Screens are functional despite warnings

### 📊 **TECHNICAL ANALYSIS:**

#### Architecture Understanding CORRECTED:
- **BlockMerger**: Loads XML level files and creates GameObjects dynamically, NOT Unity scenes
- **MainScene**: Single scene with dynamic level content loading
- **testcamera**: Works because it HAS BlockMerger component (reference implementation)
- **Issue**: MainScene was missing BlockMerger component entirely

#### Level Loading (Still Broken):
```
ScreenStateLoading.TryLoadLevel() → No BlockMerger found → Runtime creation attempted → UNKNOWN STATUS
```

### 🎯 **IMMEDIATE PRIORITIES:**

1. **Test the BlockMerger Fix**:
   - Run MainScene and check if level loading now works
   - Observe debug logs for errors
   - Verify if level content actually appears

2. **If Still Failing - Debug Further**:
   - Check what specific error occurs with runtime BlockMerger creation
   - Verify Level.Build() process
   - Check level file paths and dependencies

3. **Fix Remaining Issues**:
   - Initial screen name persistence
   - Canvas warnings if they affect functionality

### 📁 **ARCHITECTURE CONFIRMED:**
- **UI**: Single MainCanvas with child screen GameObjects (no individual Canvas components)
- **Levels**: XML file parsing creates LevelCubes and BlockGroups GameObjects dynamically
- **States**: Screen state management controls UI transitions  
- **Loading**: BlockMerger.LoadLevel() creates new Level(fileName).Build() to parse XML and create GameObjects
- **Critical**: MainScene loads level content dynamically, does NOT load different Unity scenes
