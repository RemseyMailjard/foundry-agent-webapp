using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApp.Api.Models;
using WebApp.Api.Services.BuddyTools;
using WebApp.Api.Services.Graph;

namespace WebApp.Api.Tests;

[TestClass]
public class BuddyToolCatalogTests
{
    private sealed class FakeGraphUserService : IGraphUserService
    {
        public Task<UserProfileDto> GetCurrentUserAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new UserProfileDto("id-1", "Remsey Mailjard", "remsey@contoso.com", "remsey@contoso.com"));
    }

    private sealed class FakeGraphMailService : IGraphMailService
    {
        public int? LastRequestedTop;

        public Task<IReadOnlyList<MailSummaryDto>> GetRecentMailAsync(int top, CancellationToken cancellationToken = default)
        {
            LastRequestedTop = top;
            IReadOnlyList<MailSummaryDto> result = [new MailSummaryDto("m1", "Hi", "A", "a@contoso.com", null, false, "preview")];
            return Task.FromResult(result);
        }
    }

    [TestMethod]
    public void GetTools_ReturnsTwoReadOnlyTools()
    {
        var catalog = new BuddyToolCatalog(new FakeGraphUserService(), new FakeGraphMailService());

        var tools = catalog.GetTools();

        Assert.AreEqual(2, tools.Count);
        Assert.IsTrue(tools.All(t => t.RiskLevel == ToolRiskLevel.Read));
        CollectionAssert.AreEquivalent(
            new[] { "get_user_profile", "search_mail" },
            tools.Select(t => t.Name).ToArray());
    }

    [TestMethod]
    public void GetTools_ParametersAreValidJsonSchemaObjects()
    {
        var catalog = new BuddyToolCatalog(new FakeGraphUserService(), new FakeGraphMailService());

        foreach (var tool in catalog.GetTools())
        {
            using var doc = JsonDocument.Parse(tool.ParametersJsonSchema);
            Assert.AreEqual("object", doc.RootElement.GetProperty("type").GetString());
        }
    }

    [TestMethod]
    public async Task GetUserProfileTool_ReturnsSerializedProfile()
    {
        var catalog = new BuddyToolCatalog(new FakeGraphUserService(), new FakeGraphMailService());
        var tool = catalog.GetTools().Single(t => t.Name == "get_user_profile");

        var resultJson = await tool.ExecuteAsync("{}", CancellationToken.None);

        // JsonSerializer.Serialize uses default (PascalCase) naming here — this JSON is fed to
        // the model, not returned over HTTP, so it doesn't need to match the API's camelCase
        // convention (Minimal API's Results.Ok uses web defaults for the actual endpoint).
        using var doc = JsonDocument.Parse(resultJson);
        Assert.AreEqual("Remsey Mailjard", doc.RootElement.GetProperty("DisplayName").GetString());
    }

    [TestMethod]
    public async Task SearchMailTool_PassesRequestedTopThrough()
    {
        var mailService = new FakeGraphMailService();
        var catalog = new BuddyToolCatalog(new FakeGraphUserService(), mailService);
        var tool = catalog.GetTools().Single(t => t.Name == "search_mail");

        await tool.ExecuteAsync("""{"top": 25}""", CancellationToken.None);

        Assert.AreEqual(25, mailService.LastRequestedTop);
    }

    [TestMethod]
    public async Task SearchMailTool_DefaultsTopWhenArgumentsEmpty()
    {
        var mailService = new FakeGraphMailService();
        var catalog = new BuddyToolCatalog(new FakeGraphUserService(), mailService);
        var tool = catalog.GetTools().Single(t => t.Name == "search_mail");

        await tool.ExecuteAsync("{}", CancellationToken.None);

        Assert.AreEqual(10, mailService.LastRequestedTop);
    }

    [TestMethod]
    public void ParseTop_DefaultsWhenMalformedJson()
    {
        var top = BuddyToolCatalog.ParseTop("not json");

        Assert.AreEqual(10, top);
    }

    [TestMethod]
    public void ParseTop_DefaultsWhenNull()
    {
        var top = BuddyToolCatalog.ParseTop(null);

        Assert.AreEqual(10, top);
    }
}
