# Codex Verification Log

Use this file to record lightweight verification notes for project tasks. Add one entry per meaningful check.

## Template

### YYYY-MM-DD HH:mm - Task Name

**Expected Observation**

- What should be true after the command, edit, render, test, or check?

**Actual Result**

- What actually happened?

**Deviation / Surprise**

- What differed from expectation?
- Was the difference acceptable, unexplained, or a blocker?

**Verification Command**

```powershell
# command or check used
```

**Residual Risk**

- What remains unverified?
- What should the next agent or teammate watch?

## Entries

### 2026-06-24 - Enable Local Codex Workflow

**Expected Observation**

- `AGENTS.md` exists with a project-local Codex Workflow section.
- `work/codex-blackboard.md`, `work/codex-verification-log.md`, and `work/codex-evaluation-harness.md` exist.
- No global config, hooks, or memory files are changed.

**Actual Result**

- Pending final file existence and git status verification.

**Deviation / Surprise**

- None at creation time.

**Verification Command**

```powershell
Get-Item -LiteralPath 'AGENTS.md','work/codex-blackboard.md','work/codex-verification-log.md','work/codex-evaluation-harness.md'
git status --short
```

**Residual Risk**

- Future tasks must keep this workflow lightweight and avoid treating it as a substitute for actual testing.
