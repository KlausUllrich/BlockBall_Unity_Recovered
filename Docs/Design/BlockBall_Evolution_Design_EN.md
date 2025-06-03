# BlockBall Evolution – Game Design Document (EN)

## Overview

- **Game Title:** BlockBall Evolution
- **Genre:** Physics-based 3D Puzzle Platformer
- **Platforms:** Originally PC, potential for mobile platforms
- **Core Gameplay:** Control a ball through a level, collect diamonds and hidden big diamonds, trigger gravity changes, and reach the goal as quickly as possible.

---

## Key Features

### Movement & Physics

- Player controls the ball via keyboard or gamepad
- The ball can jump (constant jump height; horizontal distance depends on speed)
- Gravity can be rotated in 90° or 180° steps using gravity switches. The camera aligns accordingly.
- Gravity affects platform orientation, controls, and camera alignment.

### Camera

- Fully rotatable (horizontal and vertical), includes zoom
- Smart visibility: obstructing blocks become transparent or fade out
- Camera aligns to gravity: "down" always points along the current gravity vector

### Level Design

- Modular block-based system using 8x8x8 "bixels" (block + pixel)
- Sloped blocks (e.g., "0 to 8 block", "2 to 4 block") allow for ramp creation
- Varied level types:
  - Speed-focused downhill levels
  - Puzzle-driven orientation levels

### Objects & Interactions

- **Regular Diamonds:** Score points, act as checkpoints
- **Big (Hidden) Diamonds:** High score value; collecting 5 unlocks a bonus level
- **Keys (4 colors):** Unlock matching colored gates
- **Question Marks:** Trigger help messages in bottom text bar

### Checkpoints & Lives

- Infinite lives
- On falling: respawn at last collected diamond
- Camera resets to orientation at collection time

### Time & Scoring System

- Main timer counts up (MM:SS)
- Additional right-side bonus time bar with 5 segments:
  - The quicker the goal is reached, the more score bonus is awarded
  - Segment reached = score tier bonus
  - Remaining seconds in the current segment = additional bonus
- Score sources:
  - Regular diamonds
  - Big diamonds
  - Segment bonus
  - Time bonus
  - Bonus for collecting all regular diamonds

### HUD

- Top left: Collected / colored keys
- Top right:
  - Time (MM:SS)
  - Diamonds (collected / total)
  - Bonus time bar + current remaining seconds (##.#)
- Bottom: Black bar for context messages (e.g., question marks)

---

## Menu System

### Main Menu

- Start Campaign / Continue Campaign (depending on profile status)
- Play Single Level
- Create Level
- Options
- Hall of Fame
- Credits
- Quit

### Options Menu

- **Camera:** Sensitivity, invert axes
- **Sound:** Volume sliders for effects and music
- **Video:** Resolution and quality settings
- **Profile:** Create, select, delete, reset
- **Online:** Website link

---

### Level & Campaign Editor

- Standalone tool to create levels and campaigns
- Object selection via object list (e.g., blocks, gravity switches, interactions)
- Control via keyboard + mouse
- Grid-based design with bixel blocks (8x8x8 base grid)

#### Menu Structure

- **File:** Load and save levels or campaigns
- **Settings:** Enter level name, select texture set, define target times for bonus segments, classify difficulty
- **Play:** Test run of the current level
- **Undo:** Undo the last edit

#### Editing Modes

- Modes affect AWSD (X/Z movement), Q/E (Y axis):
  - **Select:** Move the cursor (block frame)
  - **Move:** Move selected blocks
  - **Rotate:** Rotate selected blocks around their center

#### Additional Features

- **Color Selection:** Assign color to selected blocks
- **Edit Status Texts:** Input help or info messages (e.g., for question marks)
- **Coordinate Display:** Shows current X/Y/Z position of the cursor
- **Element Counter:** Displays number of placed objects in the level
