using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TenderingSystem.Domain.Entities;
using TenderingSystem.Infrastructure.Identity;

namespace TenderingSystem.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Tender> Tenders => Set<Tender>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Bid> Bids => Set<Bid>();
    public DbSet<AiSearchLog> AiSearchLogs => Set<AiSearchLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relations and constraints
        
        modelBuilder.Entity<Bid>()
            .HasOne(b => b.Tender)
            .WithMany(t => t.Bids)
            .HasForeignKey(b => b.TenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Bid>()
            .HasOne(b => b.Supplier)
            .WithMany(s => s.Bids)
            .HasForeignKey(b => b.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AiSearchLog>()
            .HasOne(a => a.Tender)
            .WithMany(t => t.AiSearchLogs)
            .HasForeignKey(a => a.TenderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Bid>()
            .Property(b => b.SubmittedPrice)
            .HasPrecision(18, 2);
    }
}
