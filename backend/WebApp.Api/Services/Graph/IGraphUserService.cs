using WebApp.Api.Models;

namespace WebApp.Api.Services.Graph;

/// <summary>
/// Reads the signed-in user's own Microsoft Graph profile, using the user's delegated identity.
/// </summary>
public interface IGraphUserService
{
    Task<UserProfileDto> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}
