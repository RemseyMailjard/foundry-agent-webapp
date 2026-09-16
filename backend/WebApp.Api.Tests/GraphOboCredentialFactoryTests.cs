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
        string? tenantId,
        string? managedIdentityClientId,
        string environment)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ENTRA_BACKEND_CLIENT_ID"] = backendClientId,
                ["ENTRA_TENANT_ID"] = tenantId,
                ["MANAGED_IDENTITY_CLIENT_ID"] = managedIdentityClientId,
                ["ASPNETCORE_ENVIRONMENT"] = environment,
            })
            .Build();

        return new GraphOboCredentialFactory(
            config,
            NullLogger<GraphOboCredentialFactory>.Instance);
    }

    [TestMethod]
    public void IsAvailable_TrueWhenFullyConfiguredInProduction()
    {
        var factory = CreateFactory("backend-id", "tenant-id", "mi-id", "Production");

        Assert.IsTrue(factory.IsAvailable);
    }

    [TestMethod]
    public void IsAvailable_FalseInDevelopment()
    {
        var factory = CreateFactory("backend-id", "tenant-id", "mi-id", "Development");

        Assert.IsFalse(factory.IsAvailable);
    }

    [TestMethod]
    public void IsAvailable_FalseWhenBackendClientIdMissing()
    {
        var factory = CreateFactory(null, "tenant-id", "mi-id", "Production");

        Assert.IsFalse(factory.IsAvailable);
    }

    [TestMethod]
    public void IsAvailable_FalseWhenManagedIdentityClientIdMissing()
    {
        // Same requirement as Foundry OBO: the FIC assertion needs a user-assigned MI.
        var factory = CreateFactory("backend-id", "tenant-id", null, "Production");

        Assert.IsFalse(factory.IsAvailable);
    }

    [TestMethod]
    public void GetOboCredential_ThrowsWhenNotAvailable()
    {
        var factory = CreateFactory(null, null, null, "Development");

        Assert.ThrowsExactly<InvalidOperationException>(() => factory.GetOboCredential());
    }

    [TestMethod]
    public void GetOboCredential_ThrowsWhenNoBearerTokenOnRequest()
    {
        // IHttpContextAccessor is not supplied (no ambient request), so no bearer token
        // can be extracted even though OBO is otherwise configured.
        var factory = CreateFactory("backend-id", "tenant-id", "mi-id", "Production");

        Assert.ThrowsExactly<InvalidOperationException>(() => factory.GetOboCredential());
    }
}
