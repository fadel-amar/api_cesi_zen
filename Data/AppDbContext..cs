using System.Diagnostics;
using CesiZen_API.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> User { get; set; }
    public DbSet<Activite> Activite { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<SaveActivity> SaveActivity { get; set; }
    public DbSet<Menu> Menu { get; set; }
    public DbSet<Page> Page { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().ToTable("user");
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<Category>();
        modelBuilder.Entity<Menu>();
        modelBuilder.Entity<Page>();
        modelBuilder.Entity<Activite>();
        modelBuilder.Entity<SaveActivity>().HasKey(sa => new { sa.UserId, sa.ActiviteId });
    }
}   