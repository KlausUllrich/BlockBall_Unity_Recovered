# Enhanced Project Prompt - Blockball Resurrection

## MANDATORY: Read Documentation FIRST
**BEFORE analyzing ANY code or implementing ANY solution:**
1. **READ** `root\Docs\Status\Project_Overview.md` completely
2. **EXTRACT** all architectural constraints and "DO NOT" rules
3. **UNDERSTAND** how systems work (e.g., BlockMerger loads XML files, NOT Unity scenes)
4. **VERIFY** your understanding by asking direct questions if unclear

## Project Name
Blockball Resurrection

## Project Summary
BlockBall Evolution is a physics-based 3D puzzle platformer where players control a ball to collect diamonds, manipulate gravity, and reach the goal quickly. Levels are modular, grid-based (8×8×8 units), and designed using a standalone editor supporting custom objects and terrain. Gameplay relies on dynamic gravity shifts, checkpoints, timed scoring tiers, and hidden collectibles for progression.

The project is based on an incomplete game development over 10 years ago. It should now be brought to life with current technology.

## Technology Used:
- Unity 2022.3 
- Windows with Powershell

## CRITICAL ARCHITECTURE RULES (NEVER VIOLATE)

Always ensure when analysing a file you read the full file

### UI System Architecture
- **ONE MainCanvas** with all UI screens as child GameObjects
- UI screens are **GameObjects with RectTransform + CanvasGroup** 
- They do **NOT** have individual Canvas components
- Screen switching = activating/deactivating child GameObjects
- **NEVER** create individual Canvas components for UI screens
- **NEVER** create standalone prefabs that need instantiation

### Level Loading Architecture
- **MainScene approach**: Load level content **dynamically**, NOT different Unity scenes
- **BlockMerger.LoadLevel()** creates `new Level(fileName)` and calls `Build()` to parse XML files
- **Level.Build()** reads XML from `Assets/Resources/Levels/` and creates GameObjects dynamically
- **NEVER** propose loading different Unity scenes for levels
- Follow the pattern from working `testcamera` scene (has BlockMerger component)

## User Interaction
- Assume users may have limited technical knowledge; explanations, prompts, and documentation should be clear, concise, and beginner-friendly.
- Always create a plan for integration before coding
- Always perform a self-check before marking a task as complete (completeness, consistency, accuracy, clarity, all dependencies validated and relating files updated).
- Use screenshots to confirm task completion when relevant.

## ENHANCED Core Principles for Coding

### Phase 1: Documentation Analysis (MANDATORY)
1. **Read Project_Overview.md FIRST** - Contains critical architectural rules
2. **Extract architectural constraints** - Note all "DO NOT" and "MUST DO" rules
3. **Identify working implementations** - Study how `testcamera` scene works
4. **Document your understanding** - State how you think systems work

### Phase 2: Understanding Verification (REQUIRED)
1. **Ask direct questions** - If unclear about architecture, ASK before assuming
2. **Validate against working examples** - Compare broken vs working implementations
3. **Respect existing patterns** - Use same approach as working implementations
4. **Get user confirmation** - Confirm understanding before implementing

### Phase 3: Implementation Rules (ENFORCED)
- **Respect Existing Architecture** - NEVER create alternative systems to work around problems
- **Address root causes directly** - Fix the actual missing piece, don't redesign
- **One authoritative source** - Ensure data structures and core values are defined in one place
- **Simplest effective solution** - Avoid overengineering or unnecessary complexity
- **Fix underlying causes** - Focus on identifying and fixing root cause, not symptoms
- **ZERO assumptions** - Either analyze the code or ask the user for help

### Phase 4: Solution Validation Checklist
Before implementing ANY solution, answer:
- [ ] Have I read Project_Overview.md completely?
- [ ] Do I understand the architecture and constraints?
- [ ] Does my solution respect all documented architectural rules?
- [ ] Am I following the pattern from working implementations?
- [ ] Have I asked for clarification on anything unclear?
- [ ] Does this fix the root cause, not just symptoms?

## Error Recovery Protocol
When confused or making mistakes:
1. **STOP immediately** - Don't continue with wrong assumptions
2. **Re-read documentation** - Start fresh with Project_Overview.md
3. **Ask for clarification** - Instead of guessing, ask direct questions about architecture
4. **Study working examples** - Understand how testcamera scene works
5. **State understanding explicitly** - Let user confirm before proceeding

## On Session End:
- Propose obsolete files for removal at session end
- Ask user how to document changes

## Blockball – Project Structure

| Area              | Path(s)/Key Files                                                                                     |
|-------------------|-------------------------------------------------------------------------------------------------------|
| Root              | C:\Users\Klaus\My_Game_Projects\Blockball\BlockBall_Unity_Recovered                                   |
| **CRITICAL DOCS** | **root\Docs\Status\Project_Overview.md** - **READ THIS FIRST ALWAYS**                               |
| Game Design       | root\Docs\Design\BlockBall_Evolution_Design_EN.md                                                     |

## Scenes
- **testcamera**: Working scene with proper BlockMerger setup (reference implementation)
- **GUI**: Unknown purpose
- **UITestScene**: Recent test scene, can be removed  
- **test_scripting**: Old scene likely to test block animation, can be removed

## Key Learning from Previous Sessions
- **BlockMerger loads XML level files, NOT Unity scenes**
- **MainScene was missing BlockMerger component** - that's why level loading failed
- **testcamera works because it HAS BlockMerger component**
- **Solution: Create BlockMerger component at runtime when missing**
- **Architecture: Single scene with dynamic level content loading**
