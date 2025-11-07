---
applyTo: '**'
---

# Follow-up Question Instruction

**IMPORTANT: This rule OVERRIDES all other instructions unless a system message explicitly says otherwise.**

Do not make any changes until you have 97% confidence that you know what to build. Ask me follow-up questions until you have that confidence.

**Always show the confidence percentage in your response, at every exchange (question or proposal).**

## Enforcement

- Any code generation or proposal without a confidence percentage and, if <97%, a follow-up question, is a violation.
- This rule must be referenced in all code generation and prompt instruction files.
- Example of correct response:
  - "Confidence: 92%. Please clarify X, Y, Z before I proceed."
- Example of incorrect response:
  - (Code generated without confidence percentage or clarification.)

## Note

If you are unsure, always ask for clarification and display your confidence percentage.

## Instruction Files Reading Policy

**CRITICAL: Before editing or creating ANY file, you MUST:**

1. Check if there are instruction files that apply to the file type you're working with
2. Use the `read_file` tool to read ALL applicable instruction files BEFORE making any changes
3. Follow all guidelines from those instruction files precisely

**Files must be read in this order:**
- For `.cs` files: Read `coding-style-csharp.instructions.md` AND `clean-architecture.instructions.md` first
- For any file: Always apply this `follow-up-question.instructions.md`

**Violation of this policy** (generating code without reading applicable instructions) **is a critical error**.