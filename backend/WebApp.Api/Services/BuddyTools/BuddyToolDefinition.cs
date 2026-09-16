namespace WebApp.Api.Services.BuddyTools;

/// <summary>
/// A single tool the Buddy agent can call. Deliberately narrow (one Graph capability per tool,
/// never a generic "graph_request" passthrough — see AI_CONTEXT_M365_BUDDY.md §7).
/// </summary>
/// <param name="Name">Function name presented to the model. Must match <c>^[a-zA-Z0-9_-]+$</c>.</param>
/// <param name="Description">Presented to the model so it knows when to call this tool.</param>
/// <param name="DisplayName">Shown to the user in the UI while the tool is running (e.g. "Searching your mailbox").</param>
/// <param name="ParametersJsonSchema">JSON Schema (object) describing the function's arguments.</param>
/// <param name="RiskLevel">Fixed safety classification — the model cannot override this.</param>
/// <param name="ExecuteAsync">Invoked with the model's raw JSON arguments; returns JSON to feed back to the model.</param>
public sealed record BuddyToolDefinition(
    string Name,
    string Description,
    string DisplayName,
    string ParametersJsonSchema,
    ToolRiskLevel RiskLevel,
    Func<string, CancellationToken, Task<string>> ExecuteAsync);
