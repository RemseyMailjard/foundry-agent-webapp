namespace WebApp.Api.Models;

/// <summary>
/// Buddy-owned projection of the signed-in user's Microsoft Graph profile (<c>/me</c>).
/// Never expose the raw Graph response to the frontend.
/// </summary>
public sealed record UserProfileDto(
    string? Id,
    string? DisplayName,
    string? UserPrincipalName,
    string? Mail);

/// <summary>
/// Buddy-owned projection of a single Microsoft Graph mail message.
/// Intentionally excludes attachments and full body content.
/// </summary>
public sealed record MailSummaryDto(
    string Id,
    string? Subject,
    string? SenderName,
    string? SenderAddress,
    DateTimeOffset? ReceivedDateTime,
    bool IsRead,
    string? BodyPreview);

/// <summary>
/// Buddy-owned projection of a single Microsoft Graph calendar event.
/// </summary>
public sealed record CalendarEventSummaryDto(
    string Id,
    string? Subject,
    DateTimeOffset? Start,
    DateTimeOffset? End,
    bool IsAllDay,
    string? Location,
    string? OrganizerName);

/// <summary>
/// Buddy-owned projection of a single Microsoft Graph drive item (file or folder).
/// </summary>
public sealed record FileSummaryDto(
    string Id,
    string? Name,
    string? WebUrl,
    DateTimeOffset? LastModifiedDateTime,
    long? SizeBytes);
