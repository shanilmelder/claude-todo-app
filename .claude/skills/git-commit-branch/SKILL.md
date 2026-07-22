---
name: git-commit-branch
description: Use this skill whenever the user asks Claude to commit changes, save/checkpoint their work, or push code — even if they just say "commit this" or "save my changes." Creates an appropriately named branch (feature/* or bug-fix/*) before committing, and pushes it to the remote. Always trigger this instead of running a plain `git commit` on the current branch.
---
 
# Git Commit + Branch Workflow
 
## Purpose
When the user asks Claude to commit changes, Claude must not commit directly to the current branch (especially not `main`/`master`). Instead, Claude creates a correctly-named branch first, commits there, and pushes it upstream.
 
## Steps
 
1. **Check current state**
   - `git status` to see staged/unstaged/untracked files.
   - `git branch --show-current` to see what branch we're on.
   - If there are no changes to commit, say so and stop.
2. **Classify the change: feature vs bug-fix**
   - If the user's request or the diff clearly indicates fixing broken behavior (error handling, crash, incorrect output, regression) → `bug-fix/`
   - If it clearly adds new capability → `feature/`
   - If ambiguous, ask the user with a single quick question rather than guessing silently.
3. **Derive the branch name**
   - Format: `feature/<kebab-case-summary>` or `bug-fix/<kebab-case-summary>`
   - Summary should be short (3-6 words), derived from the actual change (check `git diff` / `git diff --staged`), not a generic placeholder like "update-code".
   - Examples: `feature/csv-export-button`, `bug-fix/null-pointer-on-login`
   - If a related Jira ticket ID is known from context, prefix it: `feature/JIRA-123-csv-export-button`
4. **Create the branch**
   - If currently on `main`/`master` (or another shared base branch): `git checkout -b <branch-name>` from there.
   - If already on a feature branch and the user wants a fresh one: confirm the base branch to branch from before creating it.
   - Never reuse or rename an existing shared branch.
5. **Stage and commit**
   - Stage only the relevant changes (`git add <files>` — avoid a blind `git add -A` if unrelated files are dirty; ask if unsure).
   - Write a Conventional Commits-style message (`feat:`, `fix:`, etc.) matching the branch type.
   - Keep the summary line ≤50 chars, imperative mood, no period.
6. **Push**
   - `git push -u origin <branch-name>` to set upstream tracking on first push.
   - Report back the branch name and confirm the push succeeded (share the compare/PR URL git prints if available).
## Rules
- Never force-push.
- Never commit directly to `main`/`master`/`develop` — always branch first.
- If the working tree has unrelated changes mixed in, flag this to the user before committing rather than bundling everything silently.
- If a branch with the intended name already exists locally or remotely, ask before overwriting/reusing it.
## Output
After completing, tell the user: the branch name created, what was committed (short description, not the full diff), and that it's pushed and ready for a PR — this sets up the companion `git-pr-screenshot` skill.
