---
name: git-pr-screenshot
description: Use this skill whenever the user asks Claude to create, open, or raise a PR / pull request. Creates the PR via the GitHub MCP connector from the current feature/bug-fix branch into main, and always captures and attaches a screenshot of the feature or bug in the PR description — never skip the screenshot step even if the user doesn't mention it. Pairs with the git-commit-branch skill, which should already have created and pushed the branch.
---
 
# PR with Screenshot
 
## Purpose
Open a pull request from the current branch to `main` using the GitHub MCP connector, with a screenshot of the actual feature/bug embedded in the description — not just a text description.
 
## Prerequisites check
1. Confirm the current branch is a `feature/*` or `bug-fix/*` branch, not `main` — if not, stop and tell the user (they likely need the `git-commit-branch` skill first).
2. Confirm the branch is pushed to the remote (`git status` shows it's tracking origin and up to date). If not pushed yet, push it before opening the PR.
## Step 1: Capture the screenshot
 
Claude must capture the screenshot itself — do not ask the user to supply one.
 
1. Determine how to reproduce the feature/bug visually: run the app locally (e.g. `npm run dev`, `npm start`) if it's not already running, and find the relevant page/state described by the user or the diff.
2. Use whatever browser automation is available in the environment (e.g. a headless browser tool, Playwright/Puppeteer script, or the "Claude in Chrome" agent if connected) to navigate to the relevant screen and take a screenshot.
3. Save the screenshot as a PNG with a descriptive name, e.g. `docs/pr-assets/<branch-name>.png`.
4. If nothing can be launched/rendered (e.g. backend-only change with no UI), fall back to a terminal/output screenshot or explicitly tell the user no visual exists and ask whether to proceed without one — don't silently skip.
## Step 2: Get the screenshot into the PR description
 
GitHub PR descriptions render images via URL, not local file paths. The reliable approach without manual drag-and-drop upload:
 
1. Commit the screenshot file into the branch itself (small PNG, in a conventional folder like `docs/pr-assets/`) and push it.
2. Reference it in the PR body using the raw content URL for that branch:
   `https://raw.githubusercontent.com/<owner>/<repo>/<branch-name>/docs/pr-assets/<filename>.png`
3. Embed with standard markdown image syntax: `![screenshot](<raw-url>)`
Do not try to inline base64 image data in the PR body — GitHub doesn't render it.
 
## Step 3: Create the PR via GitHub MCP
 
Use the GitHub MCP connector's PR-creation tool with:
- `head`: the current feature/bug-fix branch
- `base`: `main`
- `title`: short, matches the branch's purpose (e.g. "Add CSV export button" / "Fix null pointer on login")
- `body`: structured as:
```
## Summary
<1-3 sentence description of what changed and why>
 
## Screenshot
![screenshot](<raw-url-from-step-2>)
 
## Type
- [ ] Feature
- [ ] Bug fix
(check the one that applies)
```
 
If the user is connected to GitHub MCP but the tool call fails (auth/permissions), surface the error clearly rather than silently falling back to `gh` CLI.
 
## Rules
- Never open a PR from `main` into `main`.
- Never skip the screenshot silently — either include it or explicitly flag why it's missing.
- Don't fabricate a screenshot description if capture failed; be explicit about what happened.
- Confirm the PR URL back to the user once created.
