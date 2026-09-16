using WebApp.Api.Models;

namespace WebApp.Api.Services.Graph;

/// <summary>
/// Read-only access to the signed-in user's calendar, using the user's delegated identity.
/// </summary>
public interface IGraphCalendarService
{
    Task<IReadOnlyList<CalendarEventSummaryDto>> GetUpcomingEventsAsync(
        int days,
        CancellationToken cancellationToken = default);
}
