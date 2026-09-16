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
    private readonly IGraphCalendarService _graphCalendarService;
    private readonly IGraphFilesService _graphFilesService;

    public BuddyToolCatalog(
        IGraphUserService graphUserService,
        IGraphMailService graphMailService,
        IGraphCalendarService graphCalendarService,
        IGraphFilesService graphFilesService)
    {
        _graphUserService = graphUserService;
        _graphMailService = graphMailService;
        _graphCalendarService = graphCalendarService;
        _graphFilesService = graphFilesService;
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

        new BuddyToolDefinition(
            Name: "get_calendar",
            Description: "Get the signed-in user's upcoming calendar events (subject, start/end time, all-day flag, location, organizer).",
            DisplayName: "Reading your calendar",
            ParametersJsonSchema: """
                {
                  "type": "object",
                  "properties": {
                    "days": {
                      "type": ["integer", "null"],
                      "description": "How many days ahead to look (1-30). Pass null to use the default of 7.",
                      "minimum": 1,
                      "maximum": 30
                    }
                  },
                  "required": ["days"],
                  "additionalProperties": false
                }
                """,
            RiskLevel: ToolRiskLevel.Read,
            ExecuteAsync: async (argsJson, ct) =>
            {
                var days = ParseNullableInt(argsJson, "days", defaultValue: 7);
                var events = await _graphCalendarService.GetUpcomingEventsAsync(days, ct);
                return JsonSerializer.Serialize(events);
            }),

        new BuddyToolDefinition(
            Name: "search_files",
            Description: "Search the signed-in user's OneDrive files by name/content, or list their recently used files when no query is given (name, link, last modified, size). Does not read file content.",
            DisplayName: "Searching your files",
            ParametersJsonSchema: """
                {
                  "type": "object",
                  "properties": {
                    "query": {
                      "type": ["string", "null"],
                      "description": "Search text. Pass null to list recently used files instead of searching."
                    }
                  },
                  "required": ["query"],
                  "additionalProperties": false
                }
                """,
            RiskLevel: ToolRiskLevel.Read,
            ExecuteAsync: async (argsJson, ct) =>
            {
                var query = ParseNullableString(argsJson, "query");
                var files = await _graphFilesService.SearchFilesAsync(query, ct);
                return JsonSerializer.Serialize(files);
            }),
    ];

    internal static int ParseTop(string? argsJson) => ParseNullableInt(argsJson, "top", defaultValue: 10);

    internal static int ParseNullableInt(string? argsJson, string propertyName, int defaultValue)
    {
        if (string.IsNullOrWhiteSpace(argsJson))
            return defaultValue;

        try
        {
            using var doc = JsonDocument.Parse(argsJson);
            if (doc.RootElement.TryGetProperty(propertyName, out var el) &&
                el.ValueKind == JsonValueKind.Number &&
                el.TryGetInt32(out var value))
            {
                return value;
            }
        }
        catch (JsonException)
        {
            // Malformed arguments from the model — fall back to the default rather than fail the tool call.
        }

        return defaultValue;
    }

    internal static string? ParseNullableString(string? argsJson, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(argsJson))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(argsJson);
            if (doc.RootElement.TryGetProperty(propertyName, out var el) &&
                el.ValueKind == JsonValueKind.String)
            {
                return el.GetString();
            }
        }
        catch (JsonException)
        {
            // Malformed arguments from the model — fall back to null rather than fail the tool call.
        }

        return null;
    }
}
