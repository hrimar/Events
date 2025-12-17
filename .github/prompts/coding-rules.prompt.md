---
applyTo: "**/*.cs,**/*.cshtml.cs"
---

## When suggesting a solution based on a task or many tasks, always:
- do not make changes in the code without my confirmation,
- first summarize your idea how we can solve particular task, and when you have confirmation from my side, you can apply the changes,
- split the main task into steps related to a specific topic and suggest solution step-by-step,
- when we complete with the first step wait for my check before you start with the next one. In this way I will be able to verify and commit previos changes and then continue with the next step.
- make changes related only to one topic instead of a mix of fixes,
- do not change my code witch is not related to the current topic.

## Code formatting:
- Do not format my code if it is not needed.
- If a line of code is longer than 150 symbols (including empty spaces), split this row into two rows.
- Do not split my code into two lines when it is shorter than 130 symbols.

## Comments:
- Write comments only in Englis.
- Do not add icons in the comments

## Code Quality
- Generate your suggestion based on the best .NET codding practicess and principles
- Avoid code duplication through base classes and utilities
- Use meaningful names that reflect domain concepts
- Keep methods focused and cohesive
- Implement proper disposal patterns for resources