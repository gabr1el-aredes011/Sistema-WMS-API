using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wms.Infrastructure.Authorization;
using Wms.Infrastructure.Identity;

namespace Wms.Infrastructure.Persistence;

public sealed class WmsDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public WmsDbContext(
        DbContextOptions<WmsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureIdentityTables(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(WmsDbContext).Assembly);
    }

    private static void ConfigureIdentityTables(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserRole<Guid>>()
            .ToTable("user_roles", "auth");

        modelBuilder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("user_claims", "auth");

        modelBuilder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("user_logins", "auth");

        modelBuilder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("role_claims", "auth");

        modelBuilder.Entity<IdentityUserToken<Guid>>()
            .ToTable("user_tokens", "auth");
    }
}
