using System.ComponentModel.DataAnnotations;
using TicketManager.Web.Models;
using Xunit;

namespace TicketManager.Web.Tests;

public sealed class TicketValidationTests
{
    [Fact]
    public void Ticket_WithValidRequiredFields_IsValid()
    {
        var ticket = CreateValidTicket();

        var results = Validate(ticket);

        Assert.Empty(results);
    }

    [Fact]
    public void Ticket_WithoutTitleAndDescription_IsInvalid()
    {
        var ticket = CreateValidTicket();
        ticket.Title = string.Empty;
        ticket.Description = string.Empty;

        var results = Validate(ticket);

        Assert.Contains(results, result => result.ErrorMessage == "O título é obrigatório.");
        Assert.Contains(results, result => result.ErrorMessage == "A descrição é obrigatória.");
    }

    [Theory]
    [InlineData(120, 2000)]
    [InlineData(1, 1)]
    public void Ticket_WithBoundaryLengths_IsValid(int titleLength, int descriptionLength)
    {
        var ticket = CreateValidTicket();
        ticket.Title = new string('T', titleLength);
        ticket.Description = new string('D', descriptionLength);

        var results = Validate(ticket);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(121, 10, "Title")]
    [InlineData(10, 2001, "Description")]
    public void Ticket_ExceedingMaximumLengths_IsInvalid(
        int titleLength,
        int descriptionLength,
        string expectedMemberName)
    {
        var ticket = CreateValidTicket();
        ticket.Title = new string('T', titleLength);
        ticket.Description = new string('D', descriptionLength);

        var results = Validate(ticket);

        Assert.Contains(results, result => result.MemberNames.Contains(expectedMemberName));
    }

    [Fact]
    public void TicketComment_WithValidRequiredFields_IsValid()
    {
        var comment = CreateValidComment();

        var results = Validate(comment);

        Assert.Empty(results);
    }

    [Fact]
    public void TicketComment_WithoutAuthorAndBody_IsInvalid()
    {
        var comment = CreateValidComment();
        comment.AuthorName = string.Empty;
        comment.Body = string.Empty;

        var results = Validate(comment);

        Assert.Contains(results, result => result.ErrorMessage == "O autor e obrigatorio.");
        Assert.Contains(results, result => result.ErrorMessage == "O comentario e obrigatorio.");
    }

    [Theory]
    [InlineData(80, 1000)]
    [InlineData(1, 1)]
    public void TicketComment_WithBoundaryLengths_IsValid(int authorLength, int bodyLength)
    {
        var comment = CreateValidComment();
        comment.AuthorName = new string('A', authorLength);
        comment.Body = new string('B', bodyLength);

        var results = Validate(comment);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(81, 10, "AuthorName")]
    [InlineData(10, 1001, "Body")]
    public void TicketComment_ExceedingMaximumLengths_IsInvalid(
        int authorLength,
        int bodyLength,
        string expectedMemberName)
    {
        var comment = CreateValidComment();
        comment.AuthorName = new string('A', authorLength);
        comment.Body = new string('B', bodyLength);

        var results = Validate(comment);

        Assert.Contains(results, result => result.MemberNames.Contains(expectedMemberName));
    }

    private static Ticket CreateValidTicket()
    {
        return new Ticket
        {
            Title = "Valid ticket",
            Description = "A valid ticket description.",
            Priority = TicketPriority.Medium,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static List<ValidationResult> Validate(Ticket ticket)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(ticket, new ValidationContext(ticket), results, validateAllProperties: true);
        return results;
    }

    private static TicketComment CreateValidComment()
    {
        return new TicketComment
        {
            TicketId = 1,
            AuthorName = "Support",
            Body = "A valid follow-up comment.",
            CreatedAt = DateTime.UtcNow
        };
    }

    private static List<ValidationResult> Validate(TicketComment comment)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(comment, new ValidationContext(comment), results, validateAllProperties: true);
        return results;
    }
}
