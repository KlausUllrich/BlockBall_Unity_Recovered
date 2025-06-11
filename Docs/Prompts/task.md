analyse all documents in /docs/implementation/Phase1_Core_Architecture/ and provide suggestions on how to tailor this to an LLM instead of a human. Consider context length, but also a zero-error policy, where the LLM needs to fully understand what it is doing. Use the file naming convention used in /Phase0_Migration_Strategy/ (LLM_01X to LLM_04X). 




include the file overview as shown in /implememtation/Phase0_Migration_Strategy/01_overview.md









Validate Phase 3:
- read /Docs/Implementation/1_BlockBall_Physics_Spec.md
- read /Docs/Implementation/3_Physics_Implementation_Tasks.md
- analyse all documents in /docs/implementation/Phase3_Collision_System/ 

Ensure the specifications in Phase 3 do match the game requirements. There should be no overengineering or missing features.

Common pitfalls to check for:
- ensure gravity transition while airborne is possible
- ensure the ball can transition to a different gravity direction happens instantaneously while within the gravity switch trigger range
- ensure the ball always feels like rolling on slopes
- ensure all gameplay relevant physics settings can be edited by the user
- try to make editing of physics values comfortable to users not familiar with the physics engine or the formulae behind it
- the specifications deviate from the game requirements
- inventing new physics features is not allowed



- modify the files so they are tailored to an LLM instead of a human. Consider context length, but also a zero-error policy, where the LLM needs to fully understand what it is doing.
- all documents that exeed 200 lines must be split to ensure the LLM can understand them. Do not create files larger than 200 lines.
- all documents adhere to the single source of truth principle










The idea is to replace the current physics implementation with a custom version, as specified in the 1_BlockBall_Physics_Spec.md. We are in the process of creating the detailed programming specifications. However, I fear they are inconsistent and do not match the requirements. Hence this checking task. 

Create a plan on how to solve all identified issues.

Regarding your points:
1) Do not compare against the current code base, but instead against the plan. This would be so far all documents in:
/Implementation/Phase0_Migration_Strategy/
/Implementation/Phase1_Core_Architecture/
/Implementation/Phase2_Ball_Physics/
/Implementation/Phase3_Collision_System/
/Implementation/Phase4_Gravits_System/
/Implementation/Phase5_Speed_Control/

This will be likely several ten-thousends of lines of text and you will not be able to analyse these at once. We need a strategy how to do this and what precisely to check for.

2) The current collision system does not work well within the game, hence this major task. The ball sometimes unexpectetly jumps when transitioning from one block to another. This is a blocker for the game. Further, the intention is to have a more tailored physics experience to ensure great player control and predictability.

3) Again, this needs to be validated against the plan, not the current code base.

4) I am still unsure on the effects. The whole specifications have been created by an LLM and might have been compromised by dilusions. It is important, that the ball always feels like rolling. 

5) This is very important. However, you have shown again and again in previous sessions, that you actually create a massive increase in code lines and do not generate files smaller than 200 lines. This is a blocker and needs to be handled carefully to achive the desired results.

6) You might want to read the /Docs/Design/original_full_game_design.md as a basis to deduct requirements. This is also a very large file.

As you  can see this task is impossible to do in one run. We need a smart plan that strongly considers LLM context length and still achives the desired result over several sessions.









verify /docs/implementation/phase3_collision_system/ (all documents) against /docs/implementation/1_blockball_physics_specs.md and 4_requirements_matrix.md. You MUST read the full file size of all documents. 

Propose a plan on how to fix the documents under phase 3.





take a look at the /docs/implementation/Phase2_ball_physics/01_overview.md as reference. Please do the same to the /Phase3_collision_system/01_overview.md and ensure an LLM has a precice plan how to handle phase 3. Notify me if tasks seem to be mentioned multiple times in this phase.