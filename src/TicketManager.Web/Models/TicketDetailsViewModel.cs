namespace TicketManager.Web.Models;

/// <summary>
/// Aggregates ticket details with the form model used to create a new comment.
/// </summary>
public class TicketDetailsViewModel
{
    /// <summary>
    /// Ticket currently being displayed.
    /// </summary>
    public required Ticket Ticket { get; init; }

    /// <summary>
    /// Comment form model bound to the details page.
    /// </summary>
    public TicketComment NewComment { get; init; } = new();
}