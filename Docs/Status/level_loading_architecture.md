# Level Loading Architecture Documentation

## Overview

The Blockball level loading system is designed around dynamic content loading within a single Unity scene (`MainScene`). This architecture avoids scene switching and instead loads level content as GameObjects at runtime by parsing XML `.level` files.

## Core Components

### 1. BlockMerger Component
- **Location**: Must be present on a GameObject in the scene
- **Primary Method**: `LoadLevel(string levelName)`
- **Function**: Creates a new `Level` instance and triggers the level building process
- **Critical Note**: The `testcamera` scene works because it has a `BlockMerger` component; `MainScene` initially failed because it was missing this component

### 2. Level Class
- **File**: `Assets/Scripts/Level/Level.cs`
- **Constructor**: `new Level(fileName)` where fileName includes the `_campain` suffix
- **Primary Method**: `Build()` - parses XML and instantiates GameObjects
- **Properties**:
  - `IntrinsicData`: Contains level metadata (LevelIntrinsicData)
  - Collection of BaseObjects representing all game objects in the level

### 3. Level File Structure
- **Location**: `Assets/Resources/Levels/`
- **Extension**: `.level`
- **Naming Convention**: `{levelname}_campain.level`
- **Format**: XML with specific schema (see XML Schema section)

## XML Schema Structure

All `.level` files must conform to the following XML structure:

```xml
<BlockBallLevel>
  <MetaData>
    <InternalName>level_internal_name</InternalName>
    <Creator>BlockBall Team</Creator>
    <TextureSet>Default</TextureSet>
    <Skybox>DefaultSky</Skybox>
    <Music>DefaultTrack</Music>
    <TimeTiers Mad_Genius_Time="30" Grand_Master_Time="60" Journeyman_Time="90" Apprentice_Time="120" />
    <Thumbnail>Textures/UI/DefaultThumbnail</Thumbnail>
  </MetaData>
  <BB3Level>
    <!-- All game objects go here -->
    <StartObject pos="0 0 0" ori="0 0 0 1" color="0.35 0.667 1" />
    <Block type="8to8" pos="1 0 0" ori="0 0 0 1" color="0.35 0.667 1" />
    <InfoObject pos="2 0 0" ori="0 0 0 1" color="0.35 0.667 1" Text="Level information" />
    <ScoreItem pos="3 0 0" ori="0 0 0 1" color="0.35 0.667 1" />
    <!-- ... more game objects ... -->
  </BB3Level>
</BlockBallLevel>
```

### Metadata Fields

The `<MetaData>` section is deserialized into `LevelIntrinsicData` class:

- **InternalName**: Unique identifier for the level (usually matches filename without extension)
- **Creator**: Level creator name
- **TextureSet**: Texture set to use for rendering
- **Skybox**: Skybox identifier
- **Music**: Background music track identifier
- **TimeTiers**: Performance time thresholds for different skill levels
  - `Mad_Genius_Time`: Expert completion time
  - `Grand_Master_Time`: Advanced completion time
  - `Journeyman_Time`: Intermediate completion time
  - `Apprentice_Time`: Beginner completion time
- **Thumbnail**: Path to level thumbnail image

### Game Object Types

The `<BB3Level>` section contains various game object types:

- **StartObject**: Player spawn point
- **Block**: Physical blocks with different types (8to8, etc.)
- **InfoObject**: Text information displays
- **ScoreItem**: Collectible items
- **EnviromentObject**: Environmental elements

Each object has standard attributes:
- `pos`: Position in 3D space (x y z)
- `ori`: Orientation as quaternion (x y z w)
- `color`: RGB color values (r g b)
- Additional type-specific attributes (e.g., `Text` for InfoObject)

## Loading Process Flow

1. **Trigger**: `BlockMerger.LoadLevel(levelName)` is called
2. **Instantiation**: New `Level(levelName)` object is created
3. **File Loading**: Level constructor loads XML from `Assets/Resources/Levels/{levelName}.level`
4. **Parsing**: `Level.Build()` method parses the XML:
   - Validates root `<BlockBallLevel>` node
   - Deserializes `<MetaData>` section into `LevelIntrinsicData`
   - Processes `<BB3Level>` section to create game objects
5. **GameObject Creation**: Each XML element becomes a Unity GameObject with appropriate components
6. **Completion**: Level is fully loaded and playable

## Architectural Constraints

### Critical Rules (NEVER VIOLATE)

1. **Single Scene Architecture**: 
   - All level loading happens within `MainScene`
   - DO NOT load different Unity scenes for different levels
   - Level content is loaded as GameObjects dynamically

2. **BlockMerger Requirement**:
   - Every scene that needs to load levels MUST have a `BlockMerger` component
   - `testcamera` works because it has this component
   - Missing `BlockMerger` causes complete level loading failure

3. **XML Schema Compliance**:
   - All `.level` files MUST use the `<BlockBallLevel>` root structure
   - `<MetaData>` and `<BB3Level>` must be siblings under the root
   - Field names in `<MetaData>` must match `LevelIntrinsicData` properties exactly

## File Conversion History

### Original State (Before Conversion)
- **Reference File**: `first_roll_campain.level` (already correct)
- **Old Format Files**: 21 files starting with `<BB3Level>` (missing wrapper structure)
- **Incorrect Structure Files**: 2 files with `<MetaData>` inside `<BB3Level>`
- **Test Files**: 1 file with completely different schema

### Conversion Process (Completed)
All `.level` files have been successfully converted to the standard schema:

1. **BB3Level Format Conversion**: 21 files wrapped with `<BlockBallLevel>` root and proper `<MetaData>` section
2. **Structure Fixes**: 2 placeholder files had `<MetaData>` moved from inside `<BB3Level>` to sibling position
3. **Test File Conversion**: 1 test file completely restructured to match schema
4. **Backup Created**: All original files backed up to `backup_20250605_145905/`

### Converted Files
- ancient_key_campain.level ✓
- a_maze_thing_campain.level ✓
- big_jump_campain.level ✓
- blockjump_campain.level ✓
- blue_way_campain.level ✓
- bonus_level_1_campain.level ✓
- bonus_level_2_campain.level ✓
- bonus_level_3_campain.level ✓
- clearlevel_campain.level ✓
- crossroads_campain.level ✓
- easy_loop_campain.level ✓
- just_speed_campain.level ✓
- late_jumps_campain.level ✓
- marble_run_campain.level ✓
- small_detour_campain.level ✓
- speed_circut_campain.level ✓
- the_box_campain.level ✓
- the_bridge_campain.level ✓
- thin_grids_campain.level ✓
- wall_ride_campain.level ✓
- another_tutorial_level_placeholder.level ✓ (structure fixed)
- ruin_puzzle_placeholder.level ✓ (structure fixed)
- Level1_campain.level ✓ (completely restructured)
- first_roll_campain.level ✓ (already correct)

## Debugging and Troubleshooting

### Common Issues

1. **Level Not Loading**:
   - Check if `BlockMerger` component exists in the scene
   - Verify level file exists in `Assets/Resources/Levels/`
   - Check Unity Console for XML parsing errors

2. **XML Parsing Errors**:
   - Ensure `<BlockBallLevel>` is the root element
   - Verify `<MetaData>` and `<BB3Level>` are siblings
   - Check that all required metadata fields are present

3. **Missing Game Objects**:
   - Verify XML structure in `<BB3Level>` section
   - Check for proper attribute formatting (pos, ori, color)
   - Ensure game object types are recognized by the parser

### Debug Logging
The `Level.Build()` method includes comprehensive debug logging:
- File loading confirmation
- XML structure validation
- Metadata parsing status
- Game object creation progress

## Best Practices

1. **Level Creation**:
   - Always use the reference file `first_roll_campain.level` as a template
   - Maintain consistent metadata structure
   - Use meaningful InternalName values

2. **Testing**:
   - Test level loading in `testcamera` scene first (known working environment)
   - Verify `BlockMerger` component exists before testing in other scenes
   - Check Unity Console for any parsing warnings

3. **Maintenance**:
   - Keep backup copies of level files before making changes
   - Validate XML structure after manual edits
   - Use consistent naming conventions

## Integration Points

### UI System Integration
- Level Selection Menu should call `BlockMerger.LoadLevel()` with proper level names
- Level names should match the filename without the `.level` extension
- UI system respects the single-scene architecture

### Game State Management
- Loading state should transition to Game state after successful level loading
- Failed level loading should be handled gracefully with appropriate error messages

## Future Considerations

1. **Level Editor Integration**: Any level editor should generate files conforming to this schema
2. **Performance Optimization**: Consider object pooling for frequently loaded/unloaded levels
3. **Validation Tools**: Implement automated XML schema validation for level files
4. **Metadata Extensions**: New metadata fields should be added to `LevelIntrinsicData` class first

---

**Document Version**: 1.0  
**Last Updated**: 2025-06-05  
**Status**: All level files successfully converted and documented
