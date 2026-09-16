namespace WebApp.Api.Services.Graph;

/// <summary>
/// Thrown when a Microsoft Graph request fails. Carries a user-safe message and the HTTP
/// status code Graph returned (or a fallback), so endpoints can map it without leaking
/// upstream error details or tokens.
/// </summary>
public sealed class GraphRequestException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
