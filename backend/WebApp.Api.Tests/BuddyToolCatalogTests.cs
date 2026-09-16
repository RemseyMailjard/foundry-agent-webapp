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

    private sealed class FakeGraphCalendarService : IGraphCalendarService
    {
        public int? LastRequestedDays;

        public Task<IReadOnlyList<CalendarEventSummaryDto>> GetUpcomingEventsAsync(int days, CancellationToken cancellationToken = default)
        {
            LastRequestedDays = days;
            IReadOnlyList<CalendarEventSummaryDto> result = [new CalendarEventSummaryDto("e1", "Standup", null, null, false, "Room 1", "Maurice")];
            return Task.FromResult(result);
        }
    }

    private sealed class FakeGraphFilesService : IGraphFilesService
    {
        public string? LastRequestedQuery;
        public bool QueryWasRequested;

        public Task<IReadOnlyList<FileSummaryDto>> SearchFilesAsync(string? query, CancellationToken cancellationToken = default)
        {
            LastRequestedQuery = query;
            QueryWasRequested = true;
            IReadOnlyList<FileSummaryDto> result = [new FileSummaryDto("f1", "Report.docx", "https://contoso.sharepoint.com/f1", null, 1024)];
            return Task.FromResult(result);
        }
    }

    private static BuddyToolCatalog CreateCatalog(
        FakeGraphUserService? userService = null,
        FakeGraphMailService? mailService = null,
        FakeGraphCalendarService? calendarService = null,
        FakeGraphFilesService? filesService = null) =>
        new(
            userService ?? new FakeGraphUserService(),
            mailService ?? new FakeGraphMailService(),
            calendarService ?? new FakeGraphCalendarService(),
            filesService ?? new FakeGraphFilesService());

    [TestMethod]
    public void GetTools_ReturnsFourReadOnlyTools()
    {
        var catalog = CreateCatalog();

        var tools = catalog.GetTools();

        Assert.AreEqual(4, tools.Count);
        Assert.IsTrue(tools.All(t => t.RiskLevel == ToolRiskLevel.Read));
        CollectionAssert.AreEquivalent(
            new[] { "get_user_profile", "search_mail", "get_calendar", "search_files" },
            tools.Select(t => t.Name).ToArray());
    }

    [TestMethod]
    public void GetTools_ParametersAreValidJsonSchemaObjects()
    {
        var catalog = CreateCatalog();

        foreach (var tool in catalog.GetTools())
        {
            using var doc = JsonDocument.Parse(tool.ParametersJsonSchema);
            Assert.AreEqual("object", doc.RootElement.GetProperty("type").GetString());
        }
    }

    [TestMethod]
    public async Task GetUserProfileTool_ReturnsSerializedProfile()
    {
        var catalog = CreateCatalog();
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
        var catalog = CreateCatalog(mailService: mailService);
        var tool = catalog.GetTools().Single(t => t.Name == "search_mail");

        await tool.ExecuteAsync("""{"top": 25}""", CancellationToken.None);

        Assert.AreEqual(25, mailService.LastRequestedTop);
    }

    [TestMethod]
    public async Task SearchMailTool_DefaultsTopWhenArgumentsEmpty()
    {
        var mailService = new FakeGraphMailService();
        var catalog = CreateCatalog(mailService: mailService);
        var tool = catalog.GetTools().Single(t => t.Name == "search_mail");

        await tool.ExecuteAsync("{}", CancellationToken.None);

        Assert.AreEqual(10, mailService.LastRequestedTop);
    }

    [TestMethod]
    public async Task GetCalendarTool_PassesRequestedDaysThrough()
    {
        var calendarService = new FakeGraphCalendarService();
        var catalog = CreateCatalog(calendarService: calendarService);
        var tool = catalog.GetTools().Single(t => t.Name == "get_calendar");

        await tool.ExecuteAsync("""{"days": 14}""", CancellationToken.None);

        Assert.AreEqual(14, calendarService.LastRequestedDays);
    }

    [TestMethod]
    public async Task GetCalendarTool_DefaultsDaysWhenNull()
    {
        var calendarService = new FakeGraphCalendarService();
        var catalog = CreateCatalog(calendarService: calendarService);
        var tool = catalog.GetTools().Single(t => t.Name == "get_calendar");

        await tool.ExecuteAsync("""{"days": null}""", CancellationToken.None);

        Assert.AreEqual(7, calendarService.LastRequestedDays);
    }

    [TestMethod]
    public async Task SearchFilesTool_PassesQueryThrough()
    {
        var filesService = new FakeGraphFilesService();
        var catalog = CreateCatalog(filesService: filesService);
        var tool = catalog.GetTools().Single(t => t.Name == "search_files");

        await tool.ExecuteAsync("""{"query": "budget"}""", CancellationToken.None);

        Assert.AreEqual("budget", filesService.LastRequestedQuery);
    }

    [TestMethod]
    public async Task SearchFilesTool_NullQueryMeansRecentFiles()
    {
        var filesService = new FakeGraphFilesService();
        var catalog = CreateCatalog(filesService: filesService);
        var tool = catalog.GetTools().Single(t => t.Name == "search_files");

        await tool.ExecuteAsync("""{"query": null}""", CancellationToken.None);

        Assert.IsTrue(filesService.QueryWasRequested);
        Assert.IsNull(filesService.LastRequestedQuery);
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

    [TestMethod]
    public void ParseNullableString_ReturnsNullWhenPropertyIsJsonNull()
    {
        var value = BuddyToolCatalog.ParseNullableString("""{"query": null}""", "query");

        Assert.IsNull(value);
    }

    [TestMethod]
    public void ParseNullableString_ReturnsValueWhenPresent()
    {
        var value = BuddyToolCatalog.ParseNullableString("""{"query": "budget"}""", "query");

        Assert.AreEqual("budget", value);
    }
}
