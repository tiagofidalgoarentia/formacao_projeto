using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using TicketManager.Web.Data;
using TicketManager.Web.Models;
using Xunit;

namespace TicketManager.Web.Tests;

public sealed class TicketsIntegrationTests : IClassFixture<TicketManagerWebApplicationFactory>
{
    private static readonly Regex AntiforgeryTokenRegex = new(
        "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"(?<token>[^\"]+)\"",
        RegexOptions.Compiled);

    private readonly TicketManagerWebApplicationFactory factory;
    private readonly HttpClient client;

    public TicketsIntegrationTests(TicketManagerWebApplicationFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
    }

    [Fact]
    public async Task Index_ReturnsSuccessAndShowsSeedTickets()
    {
        var response = await client.GetAsync("/Tickets");

        await EnsureSuccessAsync(response);
        var body = await ReadDecodedBodyAsync(response);
        Assert.Contains("Criar ticket", body);
    }

    [Fact]
    public async Task Index_OrdersTicketsByNewestFirst()
    {
        await using var dbContext = CreateDbContext();
        var oldestTicket = await dbContext.Tickets.OrderBy(ticket => ticket.CreatedAt).FirstAsync();
        var newestTicket = await CreateTicketAsync(
            "Newest ticket should be first",
            "A recently created ticket should appear before older tickets.",
            TicketPriority.Medium,
            TicketStatus.Open,
            DateTime.UtcNow.AddMinutes(5));

        var response = await client.GetAsync("/Tickets");

        await EnsureSuccessAsync(response);
        var body = await ReadDecodedBodyAsync(response);
        Assert.True(
            body.IndexOf(newestTicket.Title, StringComparison.Ordinal) <
            body.IndexOf(oldestTicket.Title, StringComparison.Ordinal));
    }

    [Fact]
    public async Task Root_ReturnsTicketsIndex()
    {
        var response = await client.GetAsync("/");

        await EnsureSuccessAsync(response);
        var body = await ReadDecodedBodyAsync(response);
        Assert.Contains("Criar ticket", body);
    }

    [Fact]
    public async Task Create_Get_ReturnsSuccess()
    {
        var response = await client.GetAsync("/Tickets/Create");

        await EnsureSuccessAsync(response);
        var body = await ReadDecodedBodyAsync(response);
        Assert.Contains("Registar um novo pedido interno", body);
    }

    [Fact]
    public async Task Create_Post_WithValidData_CreatesTicketAndRedirectsToDetails()
    {
        var token = await GetAntiforgeryTokenAsync("/Tickets/Create");

        var response = await client.PostAsync("/Tickets/Create", FormContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Title"] = "Test ticket from integration test",
            ["Description"] = "This ticket validates the create flow.",
            ["Priority"] = TicketPriority.High.ToString()
        }));

        if (response.StatusCode != HttpStatusCode.Redirect)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Expected redirect, got {response.StatusCode}. Body: {body}");
        }
        Assert.StartsWith("/Tickets/Details/", response.Headers.Location?.OriginalString);

        await using var dbContext = CreateDbContext();
        var ticket = await dbContext.Tickets.SingleAsync(t => t.Title == "Test ticket from integration test");
        Assert.Equal(TicketStatus.Open, ticket.Status);
        Assert.Equal(TicketPriority.High, ticket.Priority);
    }

    [Fact]
    public async Task Create_Post_WithInvalidData_ReturnsValidationErrorsAndDoesNotCreateTicket()
    {
        var token = await GetAntiforgeryTokenAsync("/Tickets/Create");
        await using var dbContextBefore = CreateDbContext();
        var beforeCount = await dbContextBefore.Tickets.CountAsync();

        var response = await client.PostAsync("/Tickets/Create", FormContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Title"] = "",
            ["Description"] = "",
            ["Priority"] = TicketPriority.Medium.ToString()
        }));

        await EnsureSuccessAsync(response);

        await using var dbContextAfter = CreateDbContext();
        Assert.Equal(beforeCount, await dbContextAfter.Tickets.CountAsync());
    }

    [Fact]
    public async Task Create_Post_WithoutAntiforgeryToken_ReturnsBadRequestAndDoesNotCreateTicket()
    {
        await using var dbContextBefore = CreateDbContext();
        var beforeCount = await dbContextBefore.Tickets.CountAsync();

        var response = await client.PostAsync("/Tickets/Create", FormContent(new Dictionary<string, string>
        {
            ["Title"] = "No token ticket",
            ["Description"] = "This request should be rejected before model persistence.",
            ["Priority"] = TicketPriority.High.ToString()
        }));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await using var dbContextAfter = CreateDbContext();
        Assert.Equal(beforeCount, await dbContextAfter.Tickets.CountAsync());
    }

    [Fact]
    public async Task Details_WithExistingTicket_ReturnsSuccess()
    {
        var ticket = await CreateTicketAsync(
            "Ticket detail placeholder",
            "This ticket should show the base detail page.",
            TicketPriority.Medium,
            TicketStatus.Open,
            DateTime.UtcNow);

        var response = await client.GetAsync($"/Tickets/Details/{ticket.Id}");

        await EnsureSuccessAsync(response);
        var body = await ReadDecodedBodyAsync(response);
        Assert.Contains(ticket.Title, body);
        Assert.Contains("Adicionar comentario", body);
        Assert.Contains("Fechar ticket", body);
        Assert.DoesNotContain("Editar estado", body);
    }

    [Fact]
    public async Task Details_WithMissingTicket_ReturnsNotFound()
    {
        var response = await client.GetAsync("/Tickets/Details/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Edit_Get_WithExistingTicket_ReturnsNotFound()
    {
        var ticket = await GetSeedTicketAsync();

        var response = await client.GetAsync($"/Tickets/Edit/{ticket.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Details_WhenClosed_ShowsReopenButton()
    {
        var ticket = await CreateTicketAsync(
            "Closed ticket for reopen",
            "Closed ticket should expose reopen action.",
            TicketPriority.Medium,
            TicketStatus.Closed,
            DateTime.UtcNow);

        var response = await client.GetAsync($"/Tickets/Details/{ticket.Id}");

        await EnsureSuccessAsync(response);
        var body = await ReadDecodedBodyAsync(response);
        Assert.Contains("Reabrir ticket", body);
        Assert.DoesNotContain("Fechar ticket", body);
    }

    [Fact]
    public async Task Details_WhenInProgress_ShowsCloseButton()
    {
        var ticket = await CreateTicketAsync(
            "In progress ticket to close",
            "In progress ticket should expose close action.",
            TicketPriority.Medium,
            TicketStatus.InProgress,
            DateTime.UtcNow);

        var response = await client.GetAsync($"/Tickets/Details/{ticket.Id}");

        await EnsureSuccessAsync(response);
        var body = await ReadDecodedBodyAsync(response);
        Assert.Contains("Fechar ticket", body);
        Assert.DoesNotContain("Reabrir ticket", body);
    }

    [Fact]
    public async Task Close_Post_WhenTicketIsOpen_ChangesStatusToClosed()
    {
        var ticket = await CreateTicketAsync(
            "Open ticket to close",
            "Should be closed by explicit action.",
            TicketPriority.Medium,
            TicketStatus.Open,
            DateTime.UtcNow);

        var token = await GetAntiforgeryTokenAsync($"/Tickets/Details/{ticket.Id}");

        var response = await client.PostAsync($"/Tickets/Close/{ticket.Id}", FormContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        }));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal($"/Tickets/Details/{ticket.Id}", response.Headers.Location?.OriginalString);

        await using var dbContext = CreateDbContext();
        var persisted = await dbContext.Tickets.FindAsync(ticket.Id);
        Assert.NotNull(persisted);
        Assert.Equal(TicketStatus.Closed, persisted.Status);
    }

    [Theory]
    [InlineData(TicketStatus.Closed)]
    public async Task Close_Post_WhenTicketNotOpen_DoesNotChangeStatus(TicketStatus initialStatus)
    {
        var ticket = await CreateTicketAsync(
            "Ticket no-op close",
            "Close should be no-op when ticket is not open.",
            TicketPriority.Low,
            initialStatus,
            DateTime.UtcNow);

        var token = await GetAntiforgeryTokenAsync($"/Tickets/Details/{ticket.Id}");

        var response = await client.PostAsync($"/Tickets/Close/{ticket.Id}", FormContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        }));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        await using var dbContext = CreateDbContext();
        var persisted = await dbContext.Tickets.FindAsync(ticket.Id);
        Assert.NotNull(persisted);
        Assert.Equal(initialStatus, persisted.Status);
    }

    [Fact]
    public async Task Close_Post_WhenTicketIsInProgress_ChangesStatusToClosed()
    {
        var ticket = await CreateTicketAsync(
            "In progress ticket to close",
            "Should be closed when in progress by explicit action.",
            TicketPriority.Low,
            TicketStatus.InProgress,
            DateTime.UtcNow);

        var token = await GetAntiforgeryTokenAsync($"/Tickets/Details/{ticket.Id}");

        var response = await client.PostAsync($"/Tickets/Close/{ticket.Id}", FormContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        }));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal($"/Tickets/Details/{ticket.Id}", response.Headers.Location?.OriginalString);

        await using var dbContext = CreateDbContext();
        var persisted = await dbContext.Tickets.FindAsync(ticket.Id);
        Assert.NotNull(persisted);
        Assert.Equal(TicketStatus.Closed, persisted.Status);
    }

    [Fact]
    public async Task Close_Post_WithMissingTicket_ReturnsNotFound()
    {
        var ticket = await GetSeedTicketAsync();
        var token = await GetAntiforgeryTokenAsync($"/Tickets/Details/{ticket.Id}");

        var response = await client.PostAsync("/Tickets/Close/999999", FormContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        }));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddComment_Post_WithValidData_CreatesCommentAndRedirectsToDetails()
    {
        var ticket = await GetSeedTicketAsync();
        var token = await GetAntiforgeryTokenAsync($"/Tickets/Details/{ticket.Id}");

        var response = await client.PostAsync($"/Tickets/AddComment/{ticket.Id}", FormContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["newComment.AuthorName"] = "QA Tester",
            ["newComment.Content"] = "Comment created from integration test."
        }));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal($"/Tickets/Details/{ticket.Id}", response.Headers.Location?.OriginalString);

        await using var dbContext = CreateDbContext();
        var comment = await dbContext.TicketComments.SingleAsync(current => current.TicketId == ticket.Id);
        Assert.Equal("QA Tester", comment.AuthorName);
        Assert.Equal("Comment created from integration test.", comment.Content);
    }

    [Fact]
    public async Task AddComment_Post_WithWhitespaceOnlyContent_ReturnsValidationErrorAndDoesNotPersist()
    {
        var ticket = await GetSeedTicketAsync();
        var token = await GetAntiforgeryTokenAsync($"/Tickets/Details/{ticket.Id}");
        await using var dbContextBefore = CreateDbContext();
        var beforeCount = await dbContextBefore.TicketComments.CountAsync();

        var response = await client.PostAsync($"/Tickets/AddComment/{ticket.Id}", FormContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["newComment.AuthorName"] = "QA Tester",
            ["newComment.Content"] = "    "
        }));

        await EnsureSuccessAsync(response);

        await using var dbContextAfter = CreateDbContext();
        Assert.Equal(beforeCount, await dbContextAfter.TicketComments.CountAsync());
    }

    [Fact]
    public async Task AddComment_Post_WithoutAntiforgeryToken_ReturnsBadRequestAndDoesNotCreateComment()
    {
        var ticket = await GetSeedTicketAsync();
        await using var dbContextBefore = CreateDbContext();
        var beforeCount = await dbContextBefore.TicketComments.CountAsync();

        var response = await client.PostAsync($"/Tickets/AddComment/{ticket.Id}", FormContent(new Dictionary<string, string>
        {
            ["newComment.AuthorName"] = "No Token",
            ["newComment.Content"] = "This request should fail."
        }));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await using var dbContextAfter = CreateDbContext();
        Assert.Equal(beforeCount, await dbContextAfter.TicketComments.CountAsync());
    }

    [Fact]
    public async Task AddComment_Post_WithMissingTicket_ReturnsNotFound()
    {
        var ticket = await GetSeedTicketAsync();
        var token = await GetAntiforgeryTokenAsync($"/Tickets/Details/{ticket.Id}");

        var response = await client.PostAsync("/Tickets/AddComment/999999", FormContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["newComment.AuthorName"] = "QA Tester",
            ["newComment.Content"] = "Missing ticket case"
        }));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddComment_Post_OnClosedTicket_LeavesStatusClosed()
    {
        var ticket = await CreateTicketAsync(
            "Closed ticket comment",
            "Ticket should remain closed after comment.",
            TicketPriority.High,
            TicketStatus.Closed,
            DateTime.UtcNow);

        var token = await GetAntiforgeryTokenAsync($"/Tickets/Details/{ticket.Id}");

        var response = await client.PostAsync($"/Tickets/AddComment/{ticket.Id}", FormContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["newComment.AuthorName"] = "QA Tester",
            ["newComment.Content"] = "Comment on closed ticket"
        }));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        await using var dbContext = CreateDbContext();
        var persisted = await dbContext.Tickets.FindAsync(ticket.Id);
        Assert.NotNull(persisted);
        Assert.Equal(TicketStatus.Closed, persisted.Status);
    }

    [Fact]
    public async Task Reopen_Post_WhenTicketIsClosed_ChangesStatusToOpen()
    {
        var ticket = await CreateTicketAsync(
            "Closed ticket to reopen",
            "Should be reopened by explicit action.",
            TicketPriority.Medium,
            TicketStatus.Closed,
            DateTime.UtcNow);

        var token = await GetAntiforgeryTokenAsync($"/Tickets/Details/{ticket.Id}");

        var response = await client.PostAsync($"/Tickets/Reopen/{ticket.Id}", FormContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        }));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal($"/Tickets/Details/{ticket.Id}", response.Headers.Location?.OriginalString);

        await using var dbContext = CreateDbContext();
        var persisted = await dbContext.Tickets.FindAsync(ticket.Id);
        Assert.NotNull(persisted);
        Assert.Equal(TicketStatus.Open, persisted.Status);
    }

    [Theory]
    [InlineData(TicketStatus.Open)]
    [InlineData(TicketStatus.InProgress)]
    public async Task Reopen_Post_WhenTicketNotClosed_DoesNotChangeStatus(TicketStatus initialStatus)
    {
        var ticket = await CreateTicketAsync(
            "Ticket no-op reopen",
            "Reopen should be no-op when ticket is not closed.",
            TicketPriority.Low,
            initialStatus,
            DateTime.UtcNow);

        var token = await GetAntiforgeryTokenAsync($"/Tickets/Details/{ticket.Id}");

        var response = await client.PostAsync($"/Tickets/Reopen/{ticket.Id}", FormContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        }));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        await using var dbContext = CreateDbContext();
        var persisted = await dbContext.Tickets.FindAsync(ticket.Id);
        Assert.NotNull(persisted);
        Assert.Equal(initialStatus, persisted.Status);
    }

    [Fact]
    public async Task Reopen_Post_WithMissingTicket_ReturnsNotFound()
    {
        var ticket = await GetSeedTicketAsync();
        var token = await GetAntiforgeryTokenAsync($"/Tickets/Details/{ticket.Id}");

        var response = await client.PostAsync("/Tickets/Reopen/999999", FormContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        }));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<Ticket> GetSeedTicketAsync()
    {
        await using var dbContext = CreateDbContext();
        return await dbContext.Tickets.OrderBy(t => t.Id).FirstAsync();
    }

    private async Task<Ticket> CreateTicketAsync(
        string title,
        string description,
        TicketPriority priority,
        TicketStatus status,
        DateTime createdAt)
    {
        await using var dbContext = CreateDbContext();
        var ticket = new Ticket
        {
            Title = title,
            Description = description,
            Priority = priority,
            Status = status,
            CreatedAt = createdAt
        };

        dbContext.Tickets.Add(ticket);
        await dbContext.SaveChangesAsync();
        return ticket;
    }

    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(factory.ConnectionString)
            .Options;

        return new AppDbContext(options);
    }

    private async Task<string> GetAntiforgeryTokenAsync(string url)
    {
        var response = await client.GetAsync(url);
        await EnsureSuccessAsync(response);

        var body = await ReadDecodedBodyAsync(response);
        var match = AntiforgeryTokenRegex.Match(body);

        Assert.True(match.Success, "Expected an antiforgery token in the form.");
        return WebUtility.HtmlDecode(match.Groups["token"].Value);
    }

    private static FormUrlEncodedContent FormContent(Dictionary<string, string> values)
    {
        var content = new FormUrlEncodedContent(values);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        return content;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode}). Body: {body}");
    }

    private static async Task<string> ReadDecodedBodyAsync(HttpResponseMessage response)
    {
        return WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
    }
}
