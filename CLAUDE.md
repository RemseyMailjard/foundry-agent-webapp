# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Azure AI Foundry Agent Service sample app — Entra ID auth (MSAL.js PKCE), SSE streaming chat, deployed to Azure Container Apps as a single container. Full architecture and design decisions live in `.github/copilot-instructions.md` (always loaded for Copilot; read it) and `ARCHITECTURE-FLOW.md` (state machines, SSE event mapping, data flow diagrams).

## Commands

```powershell
# Local dev — starts backend (8080) + frontend (5173) together
.\deployment\scripts\start-local-dev.ps1
# or in VS Code: Ctrl+Shift+B → "Start Dev (VS Code Terminals)"

# Frontend (run from frontend/)
npm run dev            # Vite dev server with HMR
npm run build           # tsc -b && vite build
npm run lint             # eslint .
npm run test              # vitest (watch)
npm run test:run           # vitest run (single pass)
npm run test:coverage       # vitest run --coverage
npx vitest run path/to/File.test.ts    # single test file
npx vitest run -t "test name"          # single test by name

# Backend (run from backend/)
dotnet watch run --project WebApp.Api        # hot-reload API
dotnet test WebApp.Api.Tests                  # all backend tests
dotnet test WebApp.Api.Tests --filter "FullyQualifiedName~ClassName.MethodName"  # single test

# Deployment (azd)
azd up          # full deploy: infra + code (~10-12 min)
azd deploy       # code changes only (~3-5 min)
azd provision     # re-deploy infra / update RBAC
azd down --force --purge   # delete all Azure resources
```

Setup issues (`undefined` client ID, auth failures) almost always mean `azd up` hasn't been run — see README's "Setup Detection" table.

## Architecture (big picture)

| Layer | Tech | Port | Entry Point |
|-------|------|------|-------------|
| Frontend | React 19 + TS + Vite | 5173 | `frontend/src/App.tsx` |
| Backend | ASP.NET Core 9 Minimal APIs | 8080 | `backend/WebApp.Api/Program.cs` |
| Auth | MSAL.js → JWT Bearer | — | `frontend/src/config/authConfig.ts` |
| AI SDK | `Azure.AI.Projects` (GA) + `Azure.AI.Extensions.OpenAI` | — | `backend/WebApp.Api/.../AgentFrameworkService.cs` |
| Deploy | Bicep → Azure Container Apps | — | `infra/main.bicep` |

**Request flow**: React → MSAL token → `POST /api/chat/stream` → AI Foundry Agent Service → SSE chunks → UI.

Single container: the ASP.NET Core backend serves both `/api/*` and the built React SPA from `wwwroot`.

**Credential strategy** (mutually exclusive, chosen by `ENTRA_BACKEND_CLIENT_ID`):
- Production default: `ManagedIdentityCredential` (user-assigned MI)
- Production opt-in (`ENABLE_OBO=true`): `OnBehalfOfCredential` — secretless via Federated Identity Credential, gives per-user identity/audit trail to the Agent Service API only (NOT to agent tools, which use the agent's own Foundry identity)
- Local dev: `ChainedTokenCredential(AzureCliCredential, AzureDeveloperCliCredential)` regardless of OBO config

**`azd up` phases**: preprovision → provision (Bicep) → postprovision → predeploy → deploy. Some things are deliberately done via CLI in `postprovision.ps1`/`postdown.ps1` instead of Bicep (Entra redirect URI/identifierUri, Federated Identity Credential, cross-RG RBAC, Entra app deletion) — see `.github/copilot-instructions.md` for the "why" on each.

## Repo layout

```text
backend/WebApp.Api/          # ASP.NET Core API + serves frontend build
backend/WebApp.Api.Tests/    # MSTest backend unit tests
backend/WebApp.ServiceDefaults/  # shared OpenTelemetry/observability config
frontend/                    # React + TypeScript + Vite (Vitest tests colocated)
infra/                       # Bicep infrastructure templates
deployment/hooks/            # azd lifecycle automation (pre/postprovision, pre/postdeploy)
deployment/scripts/          # user-facing scripts (start-local-dev, list-agents)
deployment/docker/           # multi-stage Dockerfile
```

## AI assistant guidance already in this repo

This repo uses on-demand skills for GitHub Copilot under `.github/skills/` (e.g. `understanding-architecture`, `writing-csharp-code`, `writing-typescript-code`, `writing-bicep-templates`, `implementing-chat-streaming`, `troubleshooting-authentication`, `writing-unit-tests-csharp`, `writing-unit-tests-typescript`, `committing-code`, `validating-local-setup`). When working in a given area, check the matching skill file for repo-specific conventions before improvising.

`.github/hooks/` enforces: commits must go through the `committing-code` skill (direct `git commit` is blocked; commit via `-F COMMIT_MESSAGE.md`), and edits to architecture-sensitive files should be reflected in `ARCHITECTURE-FLOW.md`.

## Known limitations

- Office documents (DOCX, PPTX, XLSX) are not supported for chat upload — use PDF, images, or plain text.
- Uploaded image files accumulate in Foundry Files storage (no `expires_after` support in the GA SDKs yet); cleanup is manual via Settings → Uploaded files or the Foundry portal.
- React 19 has npm peer-dependency conflicts with some packages — new packages with peer deps must be added explicitly to `frontend/package.json`; verify with `npm ci` before committing.
