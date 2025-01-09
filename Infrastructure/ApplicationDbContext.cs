using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using GKTodoManager.Domain.Entities;

namespace GKTodoManager.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Domain.Entities.Task> Tasks { get; set; }
    public DbSet<TaskGroup> TaskGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(b =>
        {
            // Indexes for "normalized" username and email, to allow efficient lookups
            b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
            b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");

            // Maps to the AspNetUsers table
            b.ToTable("AspNetUsers");

            // A concurrency token for use with the optimistic concurrency checking
            b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();
            b.Property(u => u.HasFirstLogin).HasDefaultValue(false);

            // Limit the size of columns to use efficient database types
            b.Property(u => u.UserName).HasMaxLength(256);
            b.Property(u => u.NormalizedUserName).HasMaxLength(256);
            b.Property(u => u.Email).HasMaxLength(256);
            b.Property(u => u.NormalizedEmail).HasMaxLength(256);

            // The relationships between User and other entity types
            // Note that these relationships are configured with no navigation properties

            // Each User can have many UserClaims
            b.HasMany<ApplicationUserClaim>().WithOne(u => u.User).HasForeignKey(uc => uc.UserId).IsRequired();

            // Each User can have many UserLogins
            b.HasMany<ApplicationUserLogin>().WithOne(u => u.User).HasForeignKey(ul => ul.UserId).IsRequired();

            // Each User can have many UserTokens
            b.HasMany<ApplicationUserToken>().WithOne(u => u.User).HasForeignKey(ut => ut.UserId).IsRequired();

            // Each User can have many entries in the UserRole join table
            b.HasMany<ApplicationUserRole>().WithOne(u => u.User).HasForeignKey(ur => ur.UserId).IsRequired();
        });

        modelBuilder.Entity<ApplicationRole>(b =>
        {
            // Indexes for "normalized" name to allow efficient lookups
            b.HasIndex(u => u.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();

            // Maps to the AspNetRoles table
            b.ToTable("AspNetRoles");

            // A concurrency token for use with the optimistic concurrency checking
            b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

            // Limit the size of columns to use efficient database types
            b.Property(u => u.Description).HasMaxLength(1000);

            // The relationships between User and other entity types
            // Note that these relationships are configured with no navigation properties

            b.HasMany(e => e.UserRoles)
             .WithOne(e => e.Role)
             .HasForeignKey(e => e.RoleId)
             .IsRequired();

            b.HasMany(e => e.RoleClaims)
             .WithOne(e => e.Role)
             .HasForeignKey(e => e.RoleId)
             .IsRequired();

            // Each User can have many UserTokens
            b.HasMany<ApplicationRoleClaim>().WithOne().HasForeignKey(ut => ut.RoleId).IsRequired();

            // Each User can have many entries in the UserRole join table
            b.HasMany<ApplicationUserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
        });

        modelBuilder.Entity<ApplicationUserClaim>(b =>
        {
            b.HasOne(u => u.User).WithMany(u => u.Claims).OnDelete(DeleteBehavior.Restrict);

            // Maps to the AspNetUserClaims table
            b.ToTable("AspNetUserClaims");
        });

        modelBuilder.Entity<ApplicationUserLogin>(b =>
        {
            // Composite primary key consisting of the LoginProvider and the key to use
            // with that provider
            b.HasOne(r => r.User).WithMany(e => e.Logins).OnDelete(DeleteBehavior.Restrict);

            // Limit the size of the composite key columns due to common DB restrictions
            b.Property(l => l.LoginProvider).HasMaxLength(128);
            b.Property(l => l.ProviderKey).HasMaxLength(128);

            // Maps to the AspNetUserLogins table
            b.ToTable("AspNetUserLogins");
        });

        modelBuilder.Entity<ApplicationUserToken>(b =>
        {
            // Composite primary key consisting of the UserId, LoginProvider and Name
            b.HasOne(u => u.User).WithMany(u => u.Tokens).HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.Restrict);

            // Limit the size of the composite key columns due to common DB restrictions
            b.Property(t => t.LoginProvider).HasMaxLength(2000);
            b.Property(t => t.Name).HasMaxLength(2000);

            // Maps to the AspNetUserTokens table
            b.ToTable("AspNetUserTokens");
        });

        modelBuilder.Entity<ApplicationUserRole>(b =>
        {
            b.HasOne(u => u.Role).WithMany(u => u.UserRoles).HasForeignKey(u => u.RoleId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(u => u.User).WithMany(u => u.UserRoles).HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApplicationRoleClaim>(b =>
        {
            // Primary key
            b.HasOne(u => u.Role).WithMany(u => u.RoleClaims).HasForeignKey(u => u.RoleId).OnDelete(DeleteBehavior.Restrict);
            // Maps to the AspNetRoleClaims table
            b.ToTable("AspNetRoleClaims");
        });

        modelBuilder.Entity<Domain.Entities.Task>(b =>
        {
            b.HasIndex(u => u.Name).HasDatabaseName("TaskNameIndex").IsUnique();

            b.ToTable("Tasks");

            b.Property(u => u.StartDate).HasDefaultValue(DateTime.UtcNow);
            b.Property(u => u.Name).HasMaxLength(100);
            b.Property(u => u.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<TaskGroup>(b =>
        {
            b.HasIndex(u => u.Name).HasDatabaseName("TaskGroupNameIndex").IsUnique();

            b.ToTable("TaskGroups");

            b.Property(u => u.Name).HasMaxLength(100);
            b.Property(u => u.Description).HasMaxLength(500);
        });
    }

    public async Task<int> SaveChangesLightAsync(CancellationToken ct = default)
    {
        return await base.SaveChangesAsync(ct);
    }

    public async override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        AuditInfo();
        return await base.SaveChangesAsync(ct);
    }

    private void AuditInfo()
    {
        foreach (var entry in ChangeTracker.Entries<dynamic>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.Modified = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Added)
            {
                entry.Entity.Created = DateTime.UtcNow;
            }
        }
    }
}