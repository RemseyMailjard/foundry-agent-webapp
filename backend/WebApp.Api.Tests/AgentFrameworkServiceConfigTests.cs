using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApp.Api.Tests;

[TestClass]
public class AgentFrameworkServiceConfigTests
{
    // Note: Foundry calls always use the managed identity now, regardless of the signed-in
    // user's tenant — there is no Foundry OBO mode to test here anymore. See
    // AgentFrameworkService's constructor remarks and README's "Multi-tenant sign-in" section
    // for why (the AI Foundry resource's RBAC is scoped to our own subscription/MI, so per-user
    // OBO would just 403 for anyone outside our tenant). Microsoft Graph access remains
    // genuinely per-user/per-tenant OBO — see GraphOboCredentialFactoryTests.

    [TestMethod]
    public void AgentVersion_ParsedWhenSet()
    {
        string? configValue = "2";

        var agentVersion = string.IsNullOrWhiteSpace(configValue) ? null : configValue;

        Assert.AreEqual("2", agentVersion);
    }

    [TestMethod]
    public void AgentVersion_NullWhenMissing()
    {
        string? configValue = null;

        var agentVersion = string.IsNullOrWhiteSpace(configValue) ? null : configValue;

        Assert.IsNull(agentVersion);
    }

    [TestMethod]
    public void AgentVersion_NullWhenEmpty()
    {
        string? configValue = "";

        var agentVersion = string.IsNullOrWhiteSpace(configValue) ? null : configValue;

        Assert.IsNull(agentVersion);
    }

    [TestMethod]
    public void AgentVersion_NullWhenWhitespace()
    {
        string? configValue = "   ";

        var agentVersion = string.IsNullOrWhiteSpace(configValue) ? null : configValue;

        Assert.IsNull(agentVersion);
    }

    [TestMethod]
    public void PortalAgentId_SplitsNameAndVersion()
    {
        // Portal format: "dadjokes:2"
        var portalAgentId = "dadjokes:2";
        var parts = portalAgentId.Split(':', 2);

        var agentName = parts[0].Trim();
        var agentVersion = parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1])
            ? parts[1].Trim() : null;

        Assert.AreEqual("dadjokes", agentName);
        Assert.AreEqual("2", agentVersion);
    }

    [TestMethod]
    public void PortalAgentId_HandlesNoVersion()
    {
        // No version suffix
        var portalAgentId = "dadjokes";
        var parts = portalAgentId.Split(':', 2);

        var agentName = parts[0].Trim();
        var agentVersion = parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1])
            ? parts[1].Trim() : null;

        Assert.AreEqual("dadjokes", agentName);
        Assert.IsNull(agentVersion);
    }

    [TestMethod]
    public void PortalAgentId_HandlesWhitespaceVersion()
    {
        // Whitespace-only version suffix
        var portalAgentId = "dadjokes:   ";
        var parts = portalAgentId.Split(':', 2);

        var agentName = parts[0].Trim();
        var agentVersion = parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1])
            ? parts[1].Trim() : null;

        Assert.AreEqual("dadjokes", agentName);
        Assert.IsNull(agentVersion);
    }

    [TestMethod]
    public void PortalResourceId_ExtractsResourceName()
    {
        var armPath = "/subscriptions/abc/resourceGroups/rg/providers/Microsoft.CognitiveServices/accounts/my-foundry-resource";

        var resourceName = armPath.Split("/accounts/").Last().Split('/').First().Trim();

        Assert.AreEqual("my-foundry-resource", resourceName);
    }

    [TestMethod]
    public void PortalResourceId_HandlesProjectSuffix()
    {
        // ARM path with project suffix (AZURE_EXISTING_AIPROJECT_RESOURCE_ID format)
        var armPath = "/subscriptions/abc/resourceGroups/rg/providers/Microsoft.CognitiveServices/accounts/my-foundry/projects/my-project";

        var resourceName = armPath.Split("/accounts/").Last().Split('/').First().Trim();

        Assert.AreEqual("my-foundry", resourceName);
    }
}
