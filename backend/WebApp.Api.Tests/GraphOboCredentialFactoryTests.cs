using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApp.Api.Services.Graph;

namespace WebApp.Api.Tests;

[TestClass]
public class GraphOboCredentialFactoryTests
{
    private static GraphOboCredentialFactory CreateFactory(
        string? backendClientId,
        string? managedIdentityClientId,
        string environment,
        IHttpContextAccessor? httpContextAccessor = null)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ENTRA_BACKEND_CLIENT_ID"] = backendClientId,
                ["MANAGED_IDENTITY_CLIENT_ID"] = managedIdentityClientId,
                ["ASPNETCORE_ENVIRONMENT"] = environment,
            })
            .Build();

        return new GraphOboCredentialFactory(
            config,
            NullLogger<GraphOboCredentialFactory>.Instance,
            httpContextAccessor);
    }

    private static IHttpContextAccessor CreateAccessor(string? bearerToken, string? tid)
    {
        var httpContext = new DefaultHttpContext();
        if (bearerToken != null)
        {
            httpContext.Request.Headers.Authorization = $"Bearer {bearerToken}";
        }
        if (tid != null)
        {
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("tid", tid)]));
        }
        return new HttpContextAccessor { HttpContext = httpContext };
    }

    [TestMethod]
    public void IsAvailable_TrueWhenFullyConfiguredInProduction()
    {
        // Multi-tenant: IsAvailable no longer depends on a fixed ENTRA_TENANT_ID — the tenant
        // for each OBO exchange comes from the caller's own token (see GetOboCredential tests).
        var factory = CreateFactory("backend-id", "mi-id", "Production");

        Assert.IsTrue(factory.IsAvailable);
    }

    [TestMethod]
    public void IsAvailable_FalseInDevelopment()
    {
        var factory = CreateFactory("backend-id", "mi-id", "Development");

        Assert.IsFalse(factory.IsAvailable);
    }

    [TestMethod]
    public void IsAvailable_FalseWhenBackendClientIdMissing()
    {
        var factory = CreateFactory(null, "mi-id", "Production");

        Assert.IsFalse(factory.IsAvailable);
    }

    [TestMethod]
    public void IsAvailable_FalseWhenManagedIdentityClientIdMissing()
    {
        // Same requirement as Foundry MI mode: the FIC assertion needs a user-assigned MI.
        var factory = CreateFactory("backend-id", null, "Production");

        Assert.IsFalse(factory.IsAvailable);
    }

    [TestMethod]
    public void GetOboCredential_ThrowsWhenNotAvailable()
    {
        var factory = CreateFactory(null, null, "Development");

        Assert.ThrowsExactly<InvalidOperationException>(() => factory.GetOboCredential());
    }

    [TestMethod]
    public void GetOboCredential_ThrowsWhenNoBearerTokenOnRequest()
    {
        var accessor = CreateAccessor(bearerToken: null, tid: "caller-tenant");
        var factory = CreateFactory("backend-id", "mi-id", "Production", accessor);

        Assert.ThrowsExactly<InvalidOperationException>(() => factory.GetOboCredential());
    }

    [TestMethod]
    public void GetOboCredential_ThrowsWhenNoTidClaimOnRequest()
    {
        // Multi-tenant: the exchange must target the caller's own tenant. A token validated
        // without a "tid" claim (shouldn't normally happen post-JWT-validation, but defensively)
        // must not silently fall back to some default tenant.
        var accessor = CreateAccessor(bearerToken: "user-token", tid: null);
        var factory = CreateFactory("backend-id", "mi-id", "Production", accessor);

        Assert.ThrowsExactly<InvalidOperationException>(() => factory.GetOboCredential());
    }

    [TestMethod]
    public void GetOboCredential_SucceedsWithBearerTokenAndTidClaim()
    {
        var accessor = CreateAccessor(bearerToken: "user-token", tid: "caller-tenant");
        var factory = CreateFactory("backend-id", "mi-id", "Production", accessor);

        var credential = factory.GetOboCredential();

        Assert.IsNotNull(credential);
    }
}
