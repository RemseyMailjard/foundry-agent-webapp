using System.Net.Http.Headers;
using System.Text.Json;
using WebApp.Api.Models;

namespace WebApp.Api.Services.Graph;

/// <inheritdoc cref="IGraphFilesService"/>
public sealed class GraphFilesService : IGraphFilesService
{
    internal const int MaxResults = 25;

    private readonly GraphOboCredentialFactory _credentialFactory;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GraphFilesService> _logger;

    public GraphFilesService(
        GraphOboCredentialFactory credentialFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<GraphFilesService> logger)
    {
        _credentialFactory = credentialFactory;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<IReadOnlyList<FileSummaryDto>> SearchFilesAsync(
        string? query,
        CancellationToken cancellationToken = default)
    {
        var accessToken = await _credentialFactory.GetAccessTokenAsync(cancellationToken);

        var select = "$select=id,name,webUrl,lastModifiedDateTime,size,folder";
        var url = string.IsNullOrWhiteSpace(query)
            ? $"https://graph.microsoft.com/v1.0/me/drive/recent?{select}&$top={MaxResults}"
            : $"https://graph.microsoft.com/v1.0/me/drive/root/search(q='{Uri.EscapeDataString(query)}')?{select}&$top={MaxResults}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Graph OneDrive request failed: {StatusCode}", response.StatusCode);
            throw new GraphRequestException("Failed to search your files.", (int)response.StatusCode);
        }

        using var doc = JsonDocument.Parse(body);
        if (!doc.RootElement.TryGetProperty("value", out var items))
        {
            return [];
        }

        var results = new List<FileSummaryDto>(items.GetArrayLength());
        foreach (var item in items.EnumerateArray())
        {
            // Skip folders — only files are useful to Buddy right now.
            if (item.TryGetProperty("folder", out _))
                continue;

            results.Add(MapDriveItem(item));
        }
        return results;
    }

    internal static FileSummaryDto MapDriveItem(JsonElement item)
    {
        long? sizeBytes = item.TryGetProperty("size", out var sizeEl) && sizeEl.ValueKind == JsonValueKind.Number
            ? sizeEl.GetInt64()
            : null;

        DateTimeOffset? lastModified = null;
        if (item.TryGetProperty("lastModifiedDateTime", out var lastModifiedEl) &&
            lastModifiedEl.ValueKind == JsonValueKind.String &&
            DateTimeOffset.TryParse(lastModifiedEl.GetString(), out var parsed))
        {
            lastModified = parsed;
        }

        return new FileSummaryDto(
            Id: GraphUserService.GetString(item, "id") ?? string.Empty,
            Name: GraphUserService.GetString(item, "name"),
            WebUrl: GraphUserService.GetString(item, "webUrl"),
            LastModifiedDateTime: lastModified,
            SizeBytes: sizeBytes);
    }
}
