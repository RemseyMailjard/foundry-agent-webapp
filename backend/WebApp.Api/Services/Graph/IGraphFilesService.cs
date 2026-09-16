using WebApp.Api.Models;

namespace WebApp.Api.Services.Graph;

/// <summary>
/// Read-only access to files the signed-in user can access in OneDrive, using the user's
/// delegated identity.
/// </summary>
public interface IGraphFilesService
{
    /// <param name="query">Search text. When null/empty, returns the user's recently used files instead.</param>
    Task<IReadOnlyList<FileSummaryDto>> SearchFilesAsync(
        string? query,
        CancellationToken cancellationToken = default);
}
