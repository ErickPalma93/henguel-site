using HenguelSistemas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HenguelSistemas.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<WaitlistEntry> WaitlistEntries => Set<WaitlistEntry>();
    public DbSet<PageVisit> PageVisits => Set<PageVisit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WaitlistEntry>(e => {
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(254);
            e.Property(x => x.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<PageVisit>(e => {
            e.HasIndex(x => x.VisitedAt);
            e.Property(x => x.Page).HasMaxLength(200);
            e.Property(x => x.UserAgent).HasMaxLength(500);
        });
    }
}