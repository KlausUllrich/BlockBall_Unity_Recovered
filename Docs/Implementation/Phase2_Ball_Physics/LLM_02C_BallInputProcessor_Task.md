---
title: "Phase 2 Ball Physics - BallInputProcessor Implementation Task"
phase: "Phase 2 - Ball Physics"
dependencies:
  - "LLM_01A_Phase2_Mission_and_Objectives.md"
  - "LLM_01B_Phase2_Technical_Specifications.md"
  - "LLM_01E_Phase2_Integration_Strategy.md"
  - "LLM_02B_BallPhysics_Component_Task.md"
validation_steps:
  - "Verify that input processing is camera-relative for intuitive control."
  - "Confirm diagonal normalization ensures consistent speed in all directions."
  - "Ensure jump buffering (0.1s) and coyote time (0.15s) are implemented for responsive jumping."
integration_points:
  - "Integrates with BallPhysics.cs for applying processed input."
  - "Connects to camera system for relative movement direction."
---

# Phase 2: Ball Physics - BallInputProcessor Implementation Task

## Objective
Implement `BallInputProcessor.cs` to handle player input for ball movement and jumping, ensuring camera-relative movement, diagonal normalization for consistent speed, jump buffering, and coyote time for responsive control.

## Component Overview
- **File**: `BallInputProcessor.cs`
- **Purpose**: Processes raw player input into actionable physics commands for `BallPhysics`, adjusting movement direction based on camera orientation and handling jump timing forgiveness.
- **Key Features**:
  - Converts input to camera-relative direction for intuitive movement.
  - Normalizes diagonal input to prevent faster movement in diagonal directions.
  - Implements jump buffering to allow jump input slightly before or after valid conditions.
  - Applies coyote time to permit jumps shortly after leaving a platform.

## Implementation Steps
1. **Input Collection**: Gather raw input from Unity’s input system (e.g., WASD, arrow keys, or controller) for movement and jump actions.
2. **Camera-Relative Transformation**: Transform input vectors based on camera orientation, projecting onto a gravity-perpendicular plane for correct direction under varying gravity.
3. **Diagonal Normalization**: Normalize input vector magnitude to ensure consistent speed (e.g., moving diagonally doesn’t exceed straight movement speed).
4. **Jump Buffering**: Track jump input timestamps and allow a buffer window before/after valid jump conditions to trigger a jump.
5. **Coyote Time**: Maintain a grace period after leaving the ground during which a jump can still be initiated.
6. **Input Relay**: Pass processed movement vectors to `BallPhysics.ApplyInput()` and jump requests to `BallPhysics.RequestJump()`.

## Code Template
Below is a partial code template for `BallInputProcessor.cs`. Implement this in the Unity project under the appropriate namespace (e.g., `BlockBall.Physics`).

```csharp
using UnityEngine;

namespace BlockBall.Physics
{
    public class BallInputProcessor : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera; // Reference to main camera for relative input
        [Header("Input Settings")]
        // Reference PhysicsSettings instead of hardcoded values
        private float jumpBufferTime => PhysicsSettings.Instance.jumpBufferTime;
        private float coyoteTime => PhysicsSettings.Instance.coyoteTime;

        private BallPhysics ballPhysics;
        private BallStateMachine stateMachine; // For state checking
        private float jumpBufferTimer = 0f;
        private float coyoteTimer = 0f;
        private bool wasGroundedLastFrame = false;
        private Vector2 rawInput = Vector2.zero;

        void Awake()
        {
            ballPhysics = GetComponent<BallPhysics>();
            stateMachine = ballPhysics.GetComponent<BallStateMachine>(); // Assuming access via BallPhysics
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        void Update()
        {
            // Collect raw input
            rawInput.x = Input.GetAxisRaw("Horizontal");
            rawInput.y = Input.GetAxisRaw("Vertical");

            // Handle jump input with buffering
            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferTimer = jumpBufferTime;
            }
            if (jumpBufferTimer > 0)
            {
                jumpBufferTimer -= Time.deltaTime;
                if (CanJump())
                {
                    ballPhysics.RequestJump();
                    jumpBufferTimer = 0; // Reset after successful jump
                }
            }

            // Update coyote time
            bool isGrounded = stateMachine.CurrentState == BallState.Grounded;
            if (!isGrounded && wasGroundedLastFrame)
            {
                coyoteTimer = coyoteTime; // Start coyote time on leaving ground
            }
            if (coyoteTimer > 0)
            {
                coyoteTimer -= Time.deltaTime;
            }
            wasGroundedLastFrame = isGrounded;
        }

        void FixedUpdate()
        {
            // Process movement input as camera-relative
            Vector3 inputDirection = ProcessCameraRelativeInput(rawInput);
            ballPhysics.ApplyInput(inputDirection);
        }

        private Vector3 ProcessCameraRelativeInput(Vector2 input)
        {
            if (mainCamera == null || input.magnitude == 0)
                return Vector3.zero;

            // Get camera forward and right vectors, ignoring vertical tilt
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;
            camForward.y = 0; // Project onto horizontal plane (adjust if gravity varies)
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            // Calculate input direction based on camera
            Vector3 inputDir = (camRight * input.x) + (camForward * input.y);

            // Normalize diagonal input to prevent speed boost
            if (inputDir.magnitude > 1f)
            {
                inputDir.Normalize();
            }

            return inputDir;
        }

        private bool CanJump()
        {
            // Jump allowed if grounded or within coyote time
            return stateMachine.CurrentState == BallState.Grounded || coyoteTimer > 0;
        }
    }
}
```

## Context & Dependencies
**Requires Phase 2 Components**: This component builds on:
- `BallPhysics.cs` for relaying processed input and jump requests.
- `BallStateMachine` for checking current state (Grounded or not).
- Unity’s camera system for relative input calculation.

## Validation Instructions
1. **Camera-Relative Input**: Ensure movement direction adjusts based on camera orientation, tested with various camera angles.
2. **Diagonal Normalization**: Verify that diagonal movement (e.g., pressing both horizontal and vertical inputs) doesn’t result in faster speed than straight movement.
3. **Jump Buffer**: Confirm jump triggers if input is within buffer window before or after valid conditions.
4. **Coyote Time**: Test that jumps are allowed up to grace period after leaving a platform, enhancing player control feel.
5. **Input Relay**: Check that movement vectors and jump requests correctly pass to `BallPhysics` methods.

## Next Steps
After implementing `BallInputProcessor.cs`, proceed to `LLM_02D_BallController_Task.md` for the high-level ball controller component. Log progress in `/Status/Project_Overview.md` and any issues in `/Status/Issues_and_Required_Cleanup.md`.
