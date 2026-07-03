using System.ComponentModel.DataAnnotations;
using TicketManager.Web.Models;
using Xunit;

namespace TicketManager.Web.Tests;

public sealed class TicketCommentValidationTests
{
    [Fact]
    public void TicketComment_WithValidRequiredFields_IsValid()
    {
        var comment = CreateValidComment();

        var results = Validate(comment);

        Assert.Empty(results);
    }

    [Fact]
    public void TicketComment_WithoutAuthorAndContent_IsInvalid()
    {
        var comment = CreateValidComment();
        comment.AuthorName = string.Empty;
        comment.Content = string.Empty;

        var results = Validate(comment);

        Assert.Contains(results, result => result.ErrorMessage == "O autor é obrigatório.");
        Assert.Contains(results, result => result.ErrorMessage == "O comentário é obrigatório.");
    }

    [Fact]
    public void TicketComment_WithWhitespaceOnlyContent_IsInvalid()
    {
        var comment = CreateValidComment();
        comment.Content = "   ";

        var results = Validate(comment);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(TicketComment.Content)));
    }

    [Theory]
    [InlineData(2, 1)]
    [InlineData(80, 1000)]
    public void TicketComment_WithBoundaryLengths_IsValid(int authorLength, int contentLength)
    {
        var comment = CreateValidComment();
        comment.AuthorName = new string('A', authorLength);
        comment.Content = new string('C', contentLength);

        var results = Validate(comment);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(1, 10, "AuthorName")]
    [InlineData(81, 10, "AuthorName")]
    [InlineData(10, 1001, "Content")]
    public void TicketComment_ExceedingLimits_IsInvalid(int authorLength, int contentLength, string expectedMemberName)
    {
        var comment = CreateValidComment();
        comment.AuthorName = new string('A', authorLength);
        comment.Content = new string('C', contentLength);

        var results = Validate(comment);

        Assert.Contains(results, result => result.MemberNames.Contains(expectedMemberName));
    }

    private static TicketComment CreateValidComment()
    {
        return new TicketComment
        {
            TicketId = 1,
            AuthorName = "Autor Valido",
            Content = "Comentário válido.",
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