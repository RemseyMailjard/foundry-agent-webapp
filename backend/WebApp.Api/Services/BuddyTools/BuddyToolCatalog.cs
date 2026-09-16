using System.Text.Json;
using WebApp.Api.Services.Graph;

namespace WebApp.Api.Services.BuddyTools;

/// <summary>
/// Read-only Buddy tools backed by Microsoft Graph (M365 Buddy vertical slice, STEP 3).
/// Only <see cref="ToolRiskLevel.Read"/> tools exist so far — write tools require the approval
/// design from AI_CONTEXT_M365_BUDDY.md §9, not yet implemented.
/// </summary>
public sealed class BuddyToolCatalog
{
    private readonly IGraphUserService _graphUserService;
    private readonly IGraphMailService _graphMailService;

    public BuddyToolCatalog(IGraphUserService graphUserService, IGraphMailService graphMailService)
    {
        _graphUserService = graphUserService;
        _graphMailService = graphMailService;
    }

    public IReadOnlyList<BuddyToolDefinition> GetTools() =>
    [
        new BuddyToolDefinition(
            Name: "get_user_profile",
            Description: "Get the signed-in user's own Microsoft 365 profile (name, email, user principal name).",
            DisplayName: "Reading your profile",
            ParametersJsonSchema: """{"type":"object","properties":{},"required":[],"additionalProperties":false}""",
            RiskLevel: ToolRiskLevel.Read,
            ExecuteAsync: async (_, ct) =>
            {
                var profile = await _graphUserService.GetCurrentUserAsync(ct);
                return JsonSerializer.Serialize(profile);
            }),

        new BuddyToolDefinition(
            Name: "search_mail",
            Description: "Get the signed-in user's most recent Outlook messages (read-only; subject, sender, received time, read state, and a short preview — no full body).",
            DisplayName: "Searching your mailbox",
            // "required": ["top"] with a nullable type is OpenAI strict-mode's way of expressing
            // an optional parameter — strict mode requires every property to be listed in
            // "required" (see README's "Important" note in Microsoft Graph Integration).
            ParametersJsonSchema: """
                {
                  "type": "object",
                  "properties": {
                    "top": {
                      "type": ["integer", "null"],
                      "description": "Maximum number of messages to return (1-50). Pass null to use the default of 10.",
                      "minimum": 1,
                      "maximum": 50
                    }
                  },
                  "required": ["top"],
                  "additionalProperties": false
                }
                """,
            RiskLevel: ToolRiskLevel.Read,
            ExecuteAsync: async (argsJson, ct) =>
            {
                var top = ParseTop(argsJson);
                var mail = await _graphMailService.GetRecentMailAsync(top, ct);
                return JsonSerializer.Serialize(mail);
            }),
    ];

    internal static int ParseTop(string? argsJson)
    {
        if (string.IsNullOrWhiteSpace(argsJson))
            return 10;

        try
        {
            using var doc = JsonDocument.Parse(argsJson);
            if (doc.RootElement.TryGetProperty("top", out var topEl) &&
                topEl.ValueKind == JsonValueKind.Number &&
                topEl.TryGetInt32(out var top))
            {
                return top;
            }
        }
        catch (JsonException)
        {
            // Malformed arguments from the model — fall back to the default rather than fail the tool call.
        }

        return 10;
    }
}
