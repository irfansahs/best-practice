# Cursor Workflow

## First Step

Open **`App.code-workspace`** — never open only `backend/` or `frontend/` alone. Root folder is required for `.cursor/rules/` and `AGENTS.md`.

## Rules

| File | Scope |
| --- | --- |
| `00-architecture.mdc` | Always apply |
| `10-csharp-style.mdc` | `backend/**/*.cs` |
| `20-cqrs-slice.mdc` | Application + Api endpoints |
| `30-frontend.mdc` | `frontend/**/*.{ts,tsx}` |
| `40-localization.mdc` | Backend + frontend strings |
| `50-caching.mdc` | Cache usage |
| `60-logging.mdc` | Logging rules |

## Prompting Tips

- "Add CreateCategory command following vertical slice rules"
- "Add TypedResults endpoint with Produces<T> for GetCategoryById"
- "Add RTK Query injectEndpoints for categories feature"

## Chat History

Cursor stores chats outside the repo under your user profile. They are not committed to git.

## Plan File

Architecture plan lives in `.cursor/plans/` — do not edit during implementation unless requirements change.
