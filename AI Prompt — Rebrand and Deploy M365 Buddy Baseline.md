You are working inside the locally cloned repository that originated from:

`microsoft-foundry/foundry-agent-webapp`

There is also an architecture context file in the repository root:

`AI_CONTEXT_M365_BUDDY.md`

## Goal

Your task is to complete **Milestone 0 only**:

> Rebrand the existing working Microsoft Foundry Agent Web App into **M365 Buddy**, build it successfully, deploy it using the repository's existing deployment approach, and verify that the existing application still works end-to-end.

This is intentionally a very small first milestone.

Do **not** start implementing Microsoft Graph, OBO, Microsoft Agent Framework refactors, Mail tools, Calendar tools, persistence, approvals, or any other future architecture described in `AI_CONTEXT_M365_BUDDY.md`.

The purpose of this milestone is to establish a clean working baseline before architectural changes begin.

---

# 1. Read the repository first

Before changing any code:

1. Read `AI_CONTEXT_M365_BUDDY.md`.
2. Read `.github/copilot-instructions.md`.
3. Inspect the relevant files under `.github/skills/`.
4. Inspect the existing frontend structure.
5. Inspect the backend structure.
6. Inspect the existing authentication configuration.
7. Inspect the existing Foundry integration.
8. Inspect the existing `azd`, Bicep, deployment, and environment configuration.
9. Inspect the existing README for deployment instructions.

Do not assume paths or filenames.

Use the repository that actually exists on disk.

Do not create a new application.

Do not scaffold a new React project.

Do not scaffold a new ASP.NET project.

Work with the downloaded repository.

---

# 2. First give a short implementation plan

Before modifying files, briefly state:

- which files contain visible product branding;
- which files you intend to modify;
- how the application is currently deployed;
- what commands you intend to run to validate the result.

Keep the plan concise.

Then proceed with the implementation.

Do not wait for confirmation unless you encounter a real blocking issue involving credentials, Azure subscription access, or an irreversible action.

---

# 3. Rebrand the application to M365 Buddy

Change user-facing branding from the original sample/project name to:

**M365 Buddy**

Look for visible branding in places such as:

- browser/document title;
- page header;
- app shell;
- welcome screen;
- chat welcome message;
- landing text;
- accessible labels;
- application metadata;
- manifest metadata if relevant;
- favicon/logo metadata;
- README heading where appropriate.

Do not blindly replace every occurrence of the upstream repository name.

Internal technical names may remain unchanged if renaming them introduces unnecessary risk.

For this milestone, prioritize **visible user-facing branding**.

---

# 4. Welcome experience

The initial user experience should make sense for M365 Buddy.

If the application contains an existing welcome heading or introductory text, adapt it conservatively.

A suitable direction is:

**M365 Buddy**

> Your AI assistant for Microsoft 365.

And optionally a short supporting sentence such as:

> Ask Buddy questions, get help with your work, and later connect Outlook, Calendar, files, Teams, and other Microsoft 365 services.

Important:

Do not imply that Microsoft Graph features already work.

Do not claim Buddy can currently read email, calendar, Teams, SharePoint, or OneDrive.

The current application is still only the baseline Foundry chat experience.

If suggested prompts currently exist, keep them generic for now.

Examples:

- "What can you help me with?"
- "Help me summarize this information."
- "Create a short action plan."

Do not add fake Microsoft 365 functionality.

---

# 5. Visual design

Preserve the existing Fluent UI v9 / Fluent 2 implementation.

Do not replace the design system.

Do not introduce:

- Material UI;
- Bootstrap;
- Tailwind component libraries;
- Chakra;
- another UI framework.

Keep styling changes minimal.

The goal is not a complete redesign.

This milestone is:

```text
working upstream application
        ↓
light M365 Buddy branding
        ↓
successful deployment
```

Not:

```text
complete new Buddy UX
```

If an existing application logo can be replaced easily without disrupting deployment, you may create a simple text-based or existing Fluent-compatible placeholder.

Do not spend significant time creating brand assets.

---

# 6. Do not change architecture

During this task, do NOT implement or refactor:

- Microsoft Graph;
- Graph SDK;
- OBO for Graph;
- `Mail.Read`;
- `Calendars.Read`;
- Teams integration;
- SharePoint integration;
- OneDrive integration;
- People integration;
- Buddy tools;
- tool approvals;
- database persistence;
- Azure SQL;
- long-term memory;
- MCP;
- multi-agent orchestration;
- new backend project architecture;
- `IBuddyAgent`;
- new domain projects;
- Agent Framework migration;
- background jobs.

Also do not replace the current Foundry implementation.

The current agent/chat runtime must remain functional.

---

# 7. Preserve authentication

Do not weaken or bypass authentication.

Keep the existing Entra ID authentication architecture working.

Do not:

- disable authentication;
- use hard-coded users;
- remove token validation;
- add development authentication bypasses;
- commit secrets;
- expose secrets to the frontend.

If existing authentication is optional by design for local development, preserve the existing behavior rather than inventing a new one.

---

# 8. Preserve streaming chat

The existing chat functionality must continue to work.

Preserve:

- SSE/streaming behavior;
- conversation functionality;
- tool visualization if currently present;
- attachments if currently present;
- errors/retries;
- Foundry connection;
- telemetry.

Do not rewrite chat architecture just to change branding.

---

# 9. Update configuration carefully

If the visible app name comes from configuration rather than hard-coded frontend text, modify the correct configuration mechanism.

Do not duplicate configuration unnecessarily.

If Azure resources contain the original sample name internally, do not rename Azure resources unless required.

Renaming infrastructure resources during this milestone is unnecessary and may cause destructive redeployment.

Prefer:

```text
Azure internal resource names
→ leave alone

Visible application/product name
→ M365 Buddy
```

---

# 10. Local validation

After making the branding changes, validate the repository.

Use the commands appropriate for the actual project.

At minimum, verify:

## Backend

Run the backend build.

For example, if appropriate:

```bash
dotnet build
```

Run existing automated tests if present.

For example:

```bash
dotnet test
```

Do not invent commands if the repository uses another solution structure.

---

## Frontend

Install dependencies only if needed.

Use the package manager already used by the project.

Then run:

- type checking if available;
- linting if available;
- production build;
- existing frontend tests if available.

For example, depending on the actual repository:

```bash
npm install
npm run lint
npm run build
```

Use the actual package scripts from `package.json`.

Do not guess.

---

# 11. Run locally if possible

If local runtime configuration is available, start the application and verify:

1. application opens;
2. branding says **M365 Buddy**;
3. no obvious browser errors;
4. authentication still works;
5. chat UI loads;
6. a user can send a message;
7. the Foundry response streams normally.

Do not treat a successful compile as proof that the application works.

---

# 12. Deployment

After local validation succeeds, deploy the existing application using the deployment mechanism provided by the repository.

Prefer the existing:

```text
Azure Developer CLI (azd)
+
Bicep
```

workflow.

Do not create a parallel manual deployment architecture.

Inspect the repository first to determine the correct deployment command.

Likely commands may include things such as:

```bash
azd auth login
azd env select
azd up
```

or repository-specific scripts.

Use the actual documented workflow.

---

# 13. Important deployment safety rules

Before performing any deployment command that can create or modify Azure resources:

- inspect the current `azd` environment;
- inspect the selected Azure subscription;
- inspect the target resource group/environment;
- do not destroy an existing environment;
- do not run `azd down`;
- do not delete Azure resources;
- do not rotate secrets unless required.

If there is no configured Azure environment, prepare everything possible and clearly identify the exact user action needed.

If authentication requires an interactive browser login, perform the command if supported by the current environment.

Never invent successful deployment output.

---

# 14. Environment naming

Where a new `azd` environment name is required, prefer:

```text
m365buddy-dev
```

unless an existing environment is already configured.

Do not rename infrastructure resources purely for cosmetic reasons.

---

# 15. Deployment validation

After deployment, verify as much as possible:

```text
Public application URL
        ↓
M365 Buddy UI
        ↓
Entra authentication
        ↓
ASP.NET backend
        ↓
Foundry
        ↓
streaming answer
```

Specifically check:

- site returns successfully;
- M365 Buddy branding is visible;
- user can sign in;
- chat loads;
- sending a message succeeds;
- response streams;
- there are no obvious authentication failures;
- backend logs do not show obvious critical exceptions.

If Application Insights is already configured, verify telemetry still exists if feasible.

---

# 16. Do not fabricate validation

If you cannot perform part of the validation because of:

- interactive login;
- unavailable Azure credentials;
- unavailable Foundry credentials;
- missing environment variables;
- unavailable subscription;

state exactly which step could not be completed.

Do not claim deployment or end-to-end verification succeeded unless it actually succeeded.

---

# 17. Documentation

Make a small documentation update if appropriate.

Document:

- that this repository is now being developed as **M365 Buddy**;
- that the current milestone is only a rebranded Foundry baseline;
- that Microsoft 365 Graph integrations will be introduced in a later milestone.

Do not rewrite the entire upstream README unnecessarily.

Preserve useful upstream setup documentation.

---

# 18. Git hygiene

Do not commit:

- `.env` secrets;
- Entra client secrets;
- Foundry keys;
- access tokens;
- Azure credentials;
- generated local secret files.

Respect the existing `.gitignore`.

Do not create a Git commit unless explicitly requested.

You may provide a suggested commit message at the end.

Suggested message:

```text
feat: establish M365 Buddy baseline branding
```

---

# 19. Definition of Done

Milestone 0 is complete when all applicable checks succeed.

## Branding

- [ ] Application is visibly named `M365 Buddy`.
- [ ] Browser title uses `M365 Buddy`.
- [ ] Welcome experience references M365 Buddy.
- [ ] No misleading claims about Microsoft 365 integrations exist.

## Architecture

- [ ] Existing Foundry architecture remains intact.
- [ ] Existing ASP.NET architecture remains intact.
- [ ] Existing authentication remains intact.
- [ ] Existing SSE/streaming remains intact.
- [ ] Microsoft Graph has NOT been added.

## Build

- [ ] Backend builds successfully.
- [ ] Existing backend tests pass.
- [ ] Frontend production build succeeds.
- [ ] Existing frontend lint/tests pass where available.

## Runtime

- [ ] App starts.
- [ ] Authentication works where configured.
- [ ] Chat loads.
- [ ] Message can be submitted.
- [ ] Foundry answer streams correctly.

## Deployment

- [ ] Existing Azure deployment process was used.
- [ ] Deployment completed successfully, OR any external blocker is explicitly documented.
- [ ] Deployed app shows M365 Buddy branding.
- [ ] Deployed chat remains functional.

---

# 20. Final report

When finished, provide a concise implementation report with these sections:

## Changed

List every changed file and one sentence about what changed.

## Preserved

Confirm that you did not alter:

- Graph/OBO;
- agent architecture;
- authentication architecture;
- streaming architecture;
- infrastructure architecture.

## Validation

Report the actual results of:

```text
backend build:
backend tests:
frontend lint:
frontend tests:
frontend build:
local runtime:
authentication:
chat:
streaming:
deployment:
production verification:
```

Use:

```text
PASS
FAIL
NOT RUN
BLOCKED
```

Do not hide failures.

## Deployment URL

Provide the deployed URL if deployment succeeded.

## Blockers

List remaining blockers, if any.

## Next milestone

State only:

> Next milestone: Microsoft Graph delegated OBO with `/api/m365/profile`.

Do not start implementing that milestone.

---

# Core instruction

**Keep this task deliberately boring and reliable.**

We are establishing a known-good M365 Buddy baseline.

Do not improve architecture simply because you see an opportunity.

Do not add Microsoft 365 functionality yet.

Do not refactor unrelated code.

Do not turn this into a full redesign.

Change the product-facing identity to **M365 Buddy**, prove that everything still builds and runs, deploy it through the existing Microsoft-provided workflow, and stop.