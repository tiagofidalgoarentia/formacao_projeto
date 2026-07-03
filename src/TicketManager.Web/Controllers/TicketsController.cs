using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketManager.Web.Data;
using TicketManager.Web.Models;

namespace TicketManager.Web.Controllers;

/// <summary>
/// Handles the MVC pages used to list, create, inspect and update internal tickets.
/// </summary>
public class TicketsController : Controller
{
    private readonly AppDbContext dbContext;

    /// <summary>
    /// Creates a controller backed by the application database context.
    /// </summary>
    /// <param name="dbContext">Database context used to read and persist tickets.</param>
    public TicketsController(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Displays all tickets ordered from newest to oldest.
    /// </summary>
    /// <returns>The ticket list page.</returns>
    public async Task<IActionResult> Index()
    {
        var tickets = await dbContext.Tickets
            .OrderByDescending(ticket => ticket.CreatedAt)
            .ToListAsync();

        return View(tickets);
    }

    /// <summary>
    /// Displays the form used to create a new ticket.
    /// </summary>
    /// <returns>The ticket creation page.</returns>
    public IActionResult Create()
    {
        PopulatePriorityOptions();
        return View(new Ticket());
    }

    /// <summary>
    /// Validates and persists a new ticket submitted from the creation form.
    /// </summary>
    /// <param name="ticket">Ticket data submitted by the user.</param>
    /// <returns>The validation page when invalid, or the details page for the created ticket.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Ticket ticket)
    {
        if (!ModelState.IsValid)
        {
            PopulatePriorityOptions(ticket.Priority);
            return View(ticket);
        }

        ticket.Status = TicketStatus.Open;
        ticket.CreatedAt = DateTime.UtcNow;

        dbContext.Tickets.Add(ticket);
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = ticket.Id });
    }

    /// <summary>
    /// Displays the details page for a single ticket.
    /// </summary>
    /// <param name="id">Identifier of the ticket to display.</param>
    /// <returns>The ticket details page, or a not found response when the ticket does not exist.</returns>
    public async Task<IActionResult> Details(int id)
    {
        var ticket = await LoadTicketWithCommentsAsync(id);

        if (ticket is null)
        {
            return NotFound();
        }

        return View(new TicketDetailsViewModel { Ticket = ticket });
    }

    /// <summary>
    /// Validates and persists a new comment submitted from the ticket details page.
    /// </summary>
    /// <param name="id">Identifier of the ticket receiving the comment.</param>
    /// <param name="newComment">Comment data submitted by the user.</param>
    /// <returns>The validation page when invalid, or the details page for the updated ticket.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int id, TicketComment newComment)
    {
        var ticket = await LoadTicketWithCommentsAsync(id);
        if (ticket is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(nameof(Details), new TicketDetailsViewModel
            {
                Ticket = ticket,
                NewComment = newComment
            });
        }

        var comment = new TicketComment
        {
            TicketId = ticket.Id,
            AuthorName = newComment.AuthorName.Trim(),
            Content = newComment.Content.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.TicketComments.Add(comment);
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = ticket.Id });
    }

    /// <summary>
    /// Reopens a closed ticket from the details page.
    /// </summary>
    /// <param name="id">Identifier of the ticket to reopen.</param>
    /// <returns>The details page for the ticket, or a not found response when the ticket does not exist.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reopen(int id)
    {
        var ticket = await dbContext.Tickets.FindAsync(id);
        if (ticket is null)
        {
            return NotFound();
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            ticket.Status = TicketStatus.Open;
            await dbContext.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id = ticket.Id });
    }

    /// <summary>
    /// Closes an open ticket from the details page.
    /// </summary>
    /// <param name="id">Identifier of the ticket to close.</param>
    /// <returns>The details page for the ticket, or a not found response when the ticket does not exist.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(int id)
    {
        var ticket = await dbContext.Tickets.FindAsync(id);
        if (ticket is null)
        {
            return NotFound();
        }

        if (ticket.Status == TicketStatus.Open || ticket.Status == TicketStatus.InProgress)
        {
            ticket.Status = TicketStatus.Closed;
            await dbContext.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id = ticket.Id });
    }


    private void PopulatePriorityOptions(TicketPriority selected = TicketPriority.Medium)
    {
        ViewBag.PriorityOptions = Enum.GetValues<TicketPriority>()
            .Select(priority => new SelectListItem(priority.ToDisplayName(), priority.ToString(), priority == selected))
            .ToList();
    }

    private async Task<Ticket?> LoadTicketWithCommentsAsync(int id)
    {
        var ticket = await dbContext.Tickets
            .Include(current => current.Comments)
            .SingleOrDefaultAsync(current => current.Id == id);

        if (ticket is null)
        {
            return null;
        }

        ticket.Comments = ticket.Comments
            .OrderBy(comment => comment.CreatedAt)
            .ToList();

        return ticket;
    }
}
