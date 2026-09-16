using System.Net.Http.Headers;
using System.Text.Json;
using WebApp.Api.Models;

namespace WebApp.Api.Services.Graph;

/// <inheritdoc cref="IGraphCalendarService"/>
public sealed class GraphCalendarService : IGraphCalendarService
{
    /// <summary>Hard ceiling on the look-ahead window, independent of what the caller requests.</summary>
    internal const int MaxDays = 30;

    private readonly GraphOboCredentialFactory _credentialFactory;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GraphCalendarService> _logger;

    public GraphCalendarService(
        GraphOboCredentialFactory credentialFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<GraphCalendarService> logger)
    {
        _credentialFactory = credentialFactory;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<IReadOnlyList<CalendarEventSummaryDto>> GetUpcomingEventsAsync(
        int days,
        CancellationToken cancellationToken = default)
    {
        var clampedDays = Math.Clamp(days, 1, MaxDays);
        var accessToken = await _credentialFactory.GetAccessTokenAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var start = Uri.EscapeDataString(now.ToString("o"));
        var end = Uri.EscapeDataString(now.AddDays(clampedDays).ToString("o"));

        var url = "https://graph.microsoft.com/v1.0/me/calendarView" +
            $"?startDateTime={start}&endDateTime={end}" +
            "&$select=id,subject,start,end,isAllDay,location,organizer" +
            "&$orderby=start/dateTime" +
            "&$top=50";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        // calendarView requires a time zone header; UTC keeps start/end comparable to the request above.
        request.Headers.Add("Prefer", "outlook.timezone=\"UTC\"");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Graph /me/calendarView request failed: {StatusCode}", response.StatusCode);
            throw new GraphRequestException("Failed to read your calendar.", (int)response.StatusCode);
        }

        using var doc = JsonDocument.Parse(body);
        if (!doc.RootElement.TryGetProperty("value", out var items))
        {
            return [];
        }

        var results = new List<CalendarEventSummaryDto>(items.GetArrayLength());
        foreach (var item in items.EnumerateArray())
        {
            results.Add(MapEvent(item));
        }
        return results;
    }

    internal static CalendarEventSummaryDto MapEvent(JsonElement item)
    {
        var start = ParseGraphDateTime(item, "start");
        var end = ParseGraphDateTime(item, "end");

        var isAllDay = item.TryGetProperty("isAllDay", out var isAllDayEl) &&
            isAllDayEl.ValueKind == JsonValueKind.True;

        string? location = null;
        if (item.TryGetProperty("location", out var locationEl) &&
            locationEl.ValueKind == JsonValueKind.Object)
        {
            location = GraphUserService.GetString(locationEl, "displayName");
        }

        string? organizerName = null;
        if (item.TryGetProperty("organizer", out var organizerEl) &&
            organizerEl.ValueKind == JsonValueKind.Object &&
            organizerEl.TryGetProperty("emailAddress", out var emailAddressEl) &&
            emailAddressEl.ValueKind == JsonValueKind.Object)
        {
            organizerName = GraphUserService.GetString(emailAddressEl, "name");
        }

        return new CalendarEventSummaryDto(
            Id: GraphUserService.GetString(item, "id") ?? string.Empty,
            Subject: GraphUserService.GetString(item, "subject"),
            Start: start,
            End: end,
            IsAllDay: isAllDay,
            Location: location,
            OrganizerName: organizerName);
    }

    /// <summary>
    /// Graph's <c>dateTimeTimeZone</c> shape is <c>{ "dateTime": "...", "timeZone": "UTC" }</c> —
    /// distinct from the plain ISO strings mail/etc. use.
    /// </summary>
    private static DateTimeOffset? ParseGraphDateTime(JsonElement item, string propertyName)
    {
        if (item.TryGetProperty(propertyName, out var el) &&
            el.ValueKind == JsonValueKind.Object &&
            el.TryGetProperty("dateTime", out var dateTimeEl) &&
            dateTimeEl.ValueKind == JsonValueKind.String &&
            DateTime.TryParse(
                dateTimeEl.GetString(),
                null,
                System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
                out var parsed))
        {
            return new DateTimeOffset(parsed, TimeSpan.Zero);
        }

        return null;
    }
}
