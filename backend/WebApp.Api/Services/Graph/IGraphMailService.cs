using WebApp.Api.Models;

namespace WebApp.Api.Services.Graph;

/// <summary>
/// Read-only access to the signed-in user's mailbox, using the user's delegated identity.
/// </summary>
public interface IGraphMailService
{
    Task<IReadOnlyList<MailSummaryDto>> GetRecentMailAsync(
        int top,
        CancellationToken cancellationToken = default);
}
