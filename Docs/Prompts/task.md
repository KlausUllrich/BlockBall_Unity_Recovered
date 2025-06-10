analyse all documents in /docs/implementation/Phase1_Core_Architecture/ and provide suggestions on how to tailor this to an LLM instead of a human. Consider context length, but also a zero-error policy, where the LLM needs to fully understand what it is doing. Use the file naming convention used in /Phase0_Migration_Strategy/ (LLM_01X to LLM_04X). 




include the file overview as shown in /implememtation/Phase0_Migration_Strategy/01_overview.md









Validate Phase 2:
read /Docs/Implementation/1_BlockBall_Physics_Spec.md
read /Docs/Implementation/3_Physics_Implementation_Tasks.md
analyse all documents in /docs/implementation/Phase2_Ball_Physics/ 

- modify the files so they are tailored to an LLM instead of a human. Consider context length, but also a zero-error policy, where the LLM needs to fully understand what it is doing.
- all documents that exeed 200 lines must be split to ensure the LLM can understand them. Do not create files larger than 200 lines.
- all documents adhere to the single source of truth principle

Common pitfalls to check for:
- ensure gravity transition while airborne is possible
- ensure the ball can transition to a different gravity direction happens instantaneously while within the gravity switch trigger range
- ensure the ball always feels like rolling on slopes
- ensure all gameplay relevant physics settings can be edited by the user
- try to make editing of physics values comfortable to users not familiar with the physics engine or the formulae behind it