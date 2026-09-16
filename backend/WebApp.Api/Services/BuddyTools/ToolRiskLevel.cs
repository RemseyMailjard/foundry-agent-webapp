namespace WebApp.Api.Services.BuddyTools;

/// <summary>
/// Safety classification for a Buddy tool. The agent (LLM) never decides this — it is fixed
/// per tool by application code. See AI_CONTEXT_M365_BUDDY.md §8.
/// </summary>
public enum ToolRiskLevel
{
    /// <summary>May execute automatically without user approval.</summary>
    Read,

    /// <summary>Proposes an action; requires explicit user approval before executing.</summary>
    Write,

    /// <summary>Always requires explicit approval, ideally with extra confirmation.</summary>
    HighImpact
}
