using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AI_Solutions.Portal.Web.Models.Entities;

namespace AI_Solutions.Portal.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Inquiry> Inquiries { get; set; }
    public DbSet<Solution> Solutions { get; set; }
    public DbSet<CaseStudy> CaseStudies { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventImage> EventImages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Customer>()
            .HasIndex(c => c.Email)
            .IsUnique();

        builder.Entity<Inquiry>()
            .Property(i => i.Status)
            .HasDefaultValue("New");
    }
}
