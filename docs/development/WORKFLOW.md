# Development Workflow

## Session start checklist

1. Read `README.md`.
2. Read `docs/STATUS.md`.
3. Read `docs/roadmap/TASKS.md`.
4. Pick one task only.
5. Confirm acceptance criteria before coding.

## During development

- Keep changes small.
- Prefer domain tests before UI work.
- Do not introduce a new project unless it has a real responsibility.
- Do not add advanced features before the MVP loop works.
- Do not use public clone language in UI, docs, or marketing.

## Before marking a task done

Run, where available:

```bash
dotnet restore
dotnet build
dotnet test
```

Then update:

- `docs/STATUS.md` if app capability changed.
- `docs/roadmap/TASKS.md` if a task moved.
- `CHANGELOG.md` if the change is user-facing or structural.

## Manual verification template

Use this template in commits or PR notes:

```text
Manual verification
- OS:
- .NET SDK:
- Command used:
- Scenario tested:
- Result:
- Known issues:
```

## Coding conventions

- Domain language: `SmartBox`, not `Fence`.
- Keep UI-specific classes in `TidyTop.App`.
- Keep domain logic in `TidyTop.Core`.
- Prefer interfaces for OS-specific behavior.
- Use async for file and desktop scan operations.
- Avoid silent catch blocks unless a best-effort scan should intentionally skip bad entries.

## Branch naming

Suggested branch format:

```text
feature/m1-render-desktop-items
fix/layout-save-load
chore/docs-cleanup
```
