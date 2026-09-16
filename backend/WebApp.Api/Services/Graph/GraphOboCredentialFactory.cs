using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace WebApp.Api.Services.Graph;

/// <summary>
/// Builds a secretless <see cref="OnBehalfOfCredential"/> for calling Microsoft Graph with the
/// signed-in user's delegated identity.
/// </summary>
/// <remarks>
/// Mirrors the FIC-based OBO pattern already used by <see cref="AgentFrameworkService"/> for
/// Foundry (same <c>ENTRA_BACKEND_CLIENT_ID</c> / <c>ENTRA_TENANT_ID</c> / <c>MANAGED_IDENTITY_CLIENT_ID</c>
/// configuration and the same managed-identity federated credential), but is kept as its own
/// small class rather than folded into <see cref="AgentFrameworkService"/> so the working Foundry
/// path is not touched. Unlike Foundry, Graph access has no sensible non-user-delegated fallback —
/// when OBO is unavailable (e.g. local development), <see cref="GetOboCredential"/> throws.
/// </remarks>
public sealed class GraphOboCredentialFactory
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly ILogger<GraphOboCredentialFactory> _logger;
    private readonly string? _backendClientId;
    private readonly string? _tenantId;
    private readonly string? _managedIdentityClientId;

    // MI assertion cache (static - user-independent, safe to share across requests)
    private static ManagedIdentityClientAssertion? s_miAssertion;

    public GraphOboCredentialFactory(
        IConfiguration configuration,
        ILogger<GraphOboCredentialFactory> logger,
        IHttpContextAccessor? httpContextAccessor = null)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;

        _backendClientId = configuration["ENTRA_BACKEND_CLIENT_ID"];
        _tenantId = configuration["ENTRA_TENANT_ID"] ?? configuration["AzureAd:TenantId"];
        _managedIdentityClientId = configuration["MANAGED_IDENTITY_CLIENT_ID"]
            ?? configuration["OBO_MANAGED_IDENTITY_CLIENT_ID"]; // backward compat

        var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";

        IsAvailable = !string.IsNullOrEmpty(_backendClientId)
            && !string.IsNullOrEmpty(_tenantId)
            && environment != "Development";

        if (IsAvailable && string.IsNullOrEmpty(_managedIdentityClientId))
        {
            // Same requirement as Foundry OBO: the FIC assertion needs a user-assigned MI.
            IsAvailable = false;
            _logger.LogWarning(
                "Graph OBO configured (ENTRA_BACKEND_CLIENT_ID set) but MANAGED_IDENTITY_CLIENT_ID is missing. " +
                "Microsoft Graph endpoints will be unavailable.");
        }

        if (IsAvailable)
        {
            s_miAssertion ??= new ManagedIdentityClientAssertion(managedIdentityClientId: _managedIdentityClientId);
        }
    }

    /// <summary>
    /// Whether Graph OBO is configured for this environment. Local development always returns
    /// false today, matching the existing Foundry OBO behavior.
    /// </summary>
    public bool IsAvailable { get; }

    /// <summary>
    /// Build an OBO credential from the current request's bearer token.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when Graph OBO is not configured, or when no bearer token is present on the
    /// current request.
    /// </exception>
    public TokenCredential GetOboCredential()
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException(
                "Microsoft Graph integration requires OBO to be enabled (ENTRA_BACKEND_CLIENT_ID, " +
                "ENTRA_TENANT_ID, and MANAGED_IDENTITY_CLIENT_ID must be set, and the app must not be " +
                "running in Development). Deploy with ENABLE_OBO=true, or run against a deployed " +
                "environment, to use Microsoft Graph features.");
        }

        var userToken = ExtractBearerToken();
        if (string.IsNullOrEmpty(userToken))
        {
            throw new InvalidOperationException(
                "Microsoft Graph OBO requires a bearer token but none was found in the request.");
        }

        Func<CancellationToken, Task<string>> assertionCallback =
            async (ct) => await s_miAssertion!.GetSignedAssertionAsync(
                new AssertionRequestOptions { CancellationToken = ct });

        return new OnBehalfOfCredential(
            _tenantId!,
            _backendClientId!,
            assertionCallback,
            userToken,
            new OnBehalfOfCredentialOptions());
    }

    private static readonly string[] GraphScopes = ["https://graph.microsoft.com/.default"];

    /// <summary>
    /// Convenience wrapper: build the OBO credential and exchange it for a Graph access token.
    /// </summary>
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var credential = GetOboCredential();
        var token = await credential.GetTokenAsync(
            new TokenRequestContext(GraphScopes),
            cancellationToken);
        return token.Token;
    }

    private string? ExtractBearerToken()
    {
        var authHeader = _httpContextAccessor?.HttpContext?.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return null;

        return authHeader["Bearer ".Length..].Trim();
    }
}
