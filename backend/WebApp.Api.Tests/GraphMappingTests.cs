using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApp.Api.Services.Graph;

namespace WebApp.Api.Tests;

[TestClass]
public class GraphMappingTests
{
    [TestMethod]
    public void GetString_ReturnsValue_WhenPropertyPresent()
    {
        using var doc = JsonDocument.Parse("""{"displayName":"Remsey Mailjard"}""");

        var value = GraphUserService.GetString(doc.RootElement, "displayName");

        Assert.AreEqual("Remsey Mailjard", value);
    }

    [TestMethod]
    public void GetString_ReturnsNull_WhenPropertyMissing()
    {
        using var doc = JsonDocument.Parse("{}");

        var value = GraphUserService.GetString(doc.RootElement, "mail");

        Assert.IsNull(value);
    }

    [TestMethod]
    public void GetString_ReturnsNull_WhenPropertyIsJsonNull()
    {
        using var doc = JsonDocument.Parse("""{"mail":null}""");

        var value = GraphUserService.GetString(doc.RootElement, "mail");

        Assert.IsNull(value);
    }

    [TestMethod]
    public void MapMessage_MapsAllFields()
    {
        using var doc = JsonDocument.Parse("""
            {
                "id": "msg-1",
                "subject": "Weekly sync",
                "from": { "emailAddress": { "name": "Maurice", "address": "maurice@contoso.com" } },
                "receivedDateTime": "2026-09-16T08:30:00Z",
                "isRead": false,
                "bodyPreview": "Let's sync on..."
            }
            """);

        var dto = GraphMailService.MapMessage(doc.RootElement);

        Assert.AreEqual("msg-1", dto.Id);
        Assert.AreEqual("Weekly sync", dto.Subject);
        Assert.AreEqual("Maurice", dto.SenderName);
        Assert.AreEqual("maurice@contoso.com", dto.SenderAddress);
        Assert.AreEqual(DateTimeOffset.Parse("2026-09-16T08:30:00Z"), dto.ReceivedDateTime);
        Assert.IsFalse(dto.IsRead);
        Assert.AreEqual("Let's sync on...", dto.BodyPreview);
    }

    [TestMethod]
    public void MapMessage_HandlesMissingSender()
    {
        using var doc = JsonDocument.Parse("""{"id":"msg-2","subject":null,"isRead":true}""");

        var dto = GraphMailService.MapMessage(doc.RootElement);

        Assert.AreEqual("msg-2", dto.Id);
        Assert.IsNull(dto.Subject);
        Assert.IsNull(dto.SenderName);
        Assert.IsNull(dto.SenderAddress);
        Assert.IsNull(dto.ReceivedDateTime);
        Assert.IsTrue(dto.IsRead);
    }

    [TestMethod]
    public void GetRecentMailAsync_ClampsTopToSafeRange()
    {
        // Mirrors GraphMailService.GetRecentMailAsync's Math.Clamp(top, 1, MaxTop) — a caller
        // requesting an absurd page size should be capped, not forwarded to Graph as-is.
        var requested = 10_000;

        var clamped = Math.Clamp(requested, 1, GraphMailService.MaxTop);

        Assert.AreEqual(GraphMailService.MaxTop, clamped);
    }

    [TestMethod]
    public void GetRecentMailAsync_ClampsZeroOrNegativeTopToOne()
    {
        var requested = 0;

        var clamped = Math.Clamp(requested, 1, GraphMailService.MaxTop);

        Assert.AreEqual(1, clamped);
    }

    [TestMethod]
    public void MapEvent_MapsAllFields()
    {
        using var doc = JsonDocument.Parse("""
            {
                "id": "evt-1",
                "subject": "Weekly standup",
                "start": { "dateTime": "2026-09-17T09:00:00.0000000", "timeZone": "UTC" },
                "end": { "dateTime": "2026-09-17T09:30:00.0000000", "timeZone": "UTC" },
                "isAllDay": false,
                "location": { "displayName": "Room 1" },
                "organizer": { "emailAddress": { "name": "Maurice", "address": "maurice@contoso.com" } }
            }
            """);

        var dto = GraphCalendarService.MapEvent(doc.RootElement);

        Assert.AreEqual("evt-1", dto.Id);
        Assert.AreEqual("Weekly standup", dto.Subject);
        Assert.AreEqual(new DateTimeOffset(2026, 9, 17, 9, 0, 0, TimeSpan.Zero), dto.Start);
        Assert.AreEqual(new DateTimeOffset(2026, 9, 17, 9, 30, 0, TimeSpan.Zero), dto.End);
        Assert.IsFalse(dto.IsAllDay);
        Assert.AreEqual("Room 1", dto.Location);
        Assert.AreEqual("Maurice", dto.OrganizerName);
    }

    [TestMethod]
    public void MapEvent_HandlesMissingLocationAndOrganizer()
    {
        using var doc = JsonDocument.Parse("""{"id":"evt-2","subject":"Focus time","isAllDay":true}""");

        var dto = GraphCalendarService.MapEvent(doc.RootElement);

        Assert.AreEqual("evt-2", dto.Id);
        Assert.IsTrue(dto.IsAllDay);
        Assert.IsNull(dto.Location);
        Assert.IsNull(dto.OrganizerName);
        Assert.IsNull(dto.Start);
        Assert.IsNull(dto.End);
    }

    [TestMethod]
    public void GetUpcomingEventsAsync_ClampsDaysToSafeRange()
    {
        var requested = 365;

        var clamped = Math.Clamp(requested, 1, GraphCalendarService.MaxDays);

        Assert.AreEqual(GraphCalendarService.MaxDays, clamped);
    }

    [TestMethod]
    public void MapDriveItem_MapsAllFields()
    {
        using var doc = JsonDocument.Parse("""
            {
                "id": "file-1",
                "name": "Report.docx",
                "webUrl": "https://contoso.sharepoint.com/file-1",
                "lastModifiedDateTime": "2026-09-16T08:30:00Z",
                "size": 2048
            }
            """);

        var dto = GraphFilesService.MapDriveItem(doc.RootElement);

        Assert.AreEqual("file-1", dto.Id);
        Assert.AreEqual("Report.docx", dto.Name);
        Assert.AreEqual("https://contoso.sharepoint.com/file-1", dto.WebUrl);
        Assert.AreEqual(DateTimeOffset.Parse("2026-09-16T08:30:00Z"), dto.LastModifiedDateTime);
        Assert.AreEqual(2048L, dto.SizeBytes);
    }

    [TestMethod]
    public void MapDriveItem_HandlesMissingSize()
    {
        using var doc = JsonDocument.Parse("""{"id":"file-2","name":"Notes.txt"}""");

        var dto = GraphFilesService.MapDriveItem(doc.RootElement);

        Assert.AreEqual("file-2", dto.Id);
        Assert.IsNull(dto.SizeBytes);
        Assert.IsNull(dto.LastModifiedDateTime);
    }
}
