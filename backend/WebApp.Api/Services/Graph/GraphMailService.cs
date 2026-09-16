using System.Net.Http.Headers;
using System.Text.Json;
using WebApp.Api.Models;

namespace WebApp.Api.Services.Graph;

/// <inheritdoc cref="IGraphMailService"/>
public sealed class GraphMailService : IGraphMailService
{
    /// <summary>Hard ceiling on $top, independent of what the caller requests.</summary>
    internal const int MaxTop = 50;

    private readonly GraphOboCredentialFactory _credentialFactory;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GraphMailService> _logger;

    public GraphMailService(
        GraphOboCredentialFactory credentialFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<GraphMailService> logger)
    {
        _credentialFactory = credentialFactory;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<IReadOnlyList<MailSummaryDto>> GetRecentMailAsync(
        int top,
        CancellationToken cancellationToken = default)
    {
        var clampedTop = Math.Clamp(top, 1, MaxTop);
        var accessToken = await _credentialFactory.GetAccessTokenAsync(cancellationToken);

        var url = "https://graph.microsoft.com/v1.0/me/messages" +
            $"?$top={clampedTop}" +
            "&$select=id,subject,from,receivedDateTime,isRead,bodyPreview" +
            "&$orderby=receivedDateTime desc";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Graph /me/messages request failed: {StatusCode}", response.StatusCode);
            throw new GraphRequestException(
                "Failed to read your recent mail.",
                (int)response.StatusCode);
        }

        using var doc = JsonDocument.Parse(body);
        if (!doc.RootElement.TryGetProperty("value", out var items))
        {
            return [];
        }

        var results = new List<MailSummaryDto>(items.GetArrayLength());
        foreach (var item in items.EnumerateArray())
        {
            results.Add(MapMessage(item));
        }
        return results;
    }

    internal static MailSummaryDto MapMessage(JsonElement item)
    {
        string? senderName = null;
        string? senderAddress = null;
        if (item.TryGetProperty("from", out var from) &&
            from.ValueKind == JsonValueKind.Object &&
            from.TryGetProperty("emailAddress", out var emailAddress) &&
            emailAddress.ValueKind == JsonValueKind.Object)
        {
            senderName = GraphUserService.GetString(emailAddress, "name");
            senderAddress = GraphUserService.GetString(emailAddress, "address");
        }

        DateTimeOffset? receivedDateTime = null;
        if (item.TryGetProperty("receivedDateTime", out var received) &&
            received.ValueKind == JsonValueKind.String &&
            DateTimeOffset.TryParse(received.GetString(), out var parsed))
        {
            receivedDateTime = parsed;
        }

        var isRead = item.TryGetProperty("isRead", out var isReadEl) &&
            isReadEl.ValueKind == JsonValueKind.True;

        return new MailSummaryDto(
            Id: GraphUserService.GetString(item, "id") ?? string.Empty,
            Subject: GraphUserService.GetString(item, "subject"),
            SenderName: senderName,
            SenderAddress: senderAddress,
            ReceivedDateTime: receivedDateTime,
            IsRead: isRead,
            BodyPreview: GraphUserService.GetString(item, "bodyPreview"));
    }
}
