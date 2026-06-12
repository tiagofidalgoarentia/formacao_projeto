using Microsoft.EntityFrameworkCore;
using TicketManager.Web.Models;

namespace TicketManager.Web.Data;

/// <summary>
/// Entity Framework database context for the ticket management application.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Creates a database context using the options configured by dependency injection.
    /// </summary>
    /// <param name="options">Database provider and connection options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Tickets persisted by the application.
    /// </summary>
    public DbSet<Ticket> Tickets => Set<Ticket>();

    /// <summary>
    /// Comments recorded on tickets.
    /// </summary>
    public DbSet<TicketComment> TicketComments => Set<TicketComment>();

    /// <summary>
    /// Configures the database mapping and constraints for the domain model.
    /// </summary>
    /// <param name="modelBuilder">Builder used to configure EF Core metadata.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(ticket => ticket.Title).HasMaxLength(120).IsRequired();
            entity.Property(ticket => ticket.Description).HasMaxLength(2000).IsRequired();
            entity.Property(ticket => ticket.Priority).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(ticket => ticket.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(ticket => ticket.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<TicketComment>(entity =>
        {
            entity.Property(comment => comment.AuthorName).HasMaxLength(80).IsRequired();
            entity.Property(comment => comment.Body).HasMaxLength(1000).IsRequired();
            entity.Property(comment => comment.CreatedAt).IsRequired();

            entity.HasOne(comment => comment.Ticket)
                .WithMany(ticket => ticket.Comments)
                .HasForeignKey(comment => comment.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
