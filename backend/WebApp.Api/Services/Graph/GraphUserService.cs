using System.Net.Http.Headers;
using System.Text.Json;
using WebApp.Api.Models;

namespace WebApp.Api.Services.Graph;

/// <inheritdoc cref="IGraphUserService"/>
public sealed class GraphUserService : IGraphUserService
{
    private readonly GraphOboCredentialFactory _credentialFactory;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GraphUserService> _logger;

    public GraphUserService(
        GraphOboCredentialFactory credentialFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<GraphUserService> logger)
    {
        _credentialFactory = credentialFactory;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<UserProfileDto> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var accessToken = await _credentialFactory.GetAccessTokenAsync(cancellationToken);

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://graph.microsoft.com/v1.0/me?$select=id,displayName,userPrincipalName,mail");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Graph /me request failed: {StatusCode}", response.StatusCode);
            throw new GraphRequestException(
                "Failed to read your Microsoft 365 profile.",
                (int)response.StatusCode);
        }

        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        return new UserProfileDto(
            Id: GetString(root, "id"),
            DisplayName: GetString(root, "displayName"),
            UserPrincipalName: GetString(root, "userPrincipalName"),
            Mail: GetString(root, "mail"));
    }

    internal static string? GetString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var value) && value.ValueKind != JsonValueKind.Null
            ? value.GetString()
            : null;
}
