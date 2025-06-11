# Workflow Rules

- Always ensure when analysing a file you read the full file. Read files in chunks of 200 lines, but ensure you capture the full file.
- If reading an .md files you MUST read the full file. This is not optional.
- Always create a plan for integration before coding
- Always try to find and fix the root cause of a problem/issue
- Always perform a self-check before marking a task as complete (completeness, consistency, accuracy, clarity, all dependencies validated and relating files updated).
- Use screenshots to confirm task completion when relevant.
- Never assume, but always verify
- Respect existing patterns
- Stricly adhere to a single-source-of-truth policy
- No magic/hardcoded values
- Avoid overengineering or unnecessary complexity
- Focus on game requirements over technical elegance
- avoid creating files longer than 200 lines of code where possible

## User Interaction
- Assume users may have limited technical knowledge; explanations, prompts, and documentation should be clear, concise, and beginner-friendly.
- If unclear about architecture, ASK before assuming
- Assume you will do the acutal file changes or at least propose detailed changes to the human users.
- The human user has the creative authority.
- You are working on a Windows machine and use powershell.
- Whenever you want to offer the user multiple options, provide a numbered list, so he can easily choose by typing a number.

## Documentation
- Always update the /Status/Project_Overview.md file after completing a task.
- Always update the /Status/Issues_and_Required_Cleanup.md file after completing a task.
- Keep a thorough documentation of the architecture and the codebase under /Docs/Status/
- Add a YAML summary header to all key docs and code files

## On Session End:
- Propose obsolete files for removal at session end
- Ask user how to document changes

Read the complete /Status/Project_Overview.md file