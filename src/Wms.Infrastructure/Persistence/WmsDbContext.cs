using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wms.Infrastructure.Authorization;
using Wms.Infrastructure.Authentication;
using Wms.Infrastructure.Identity;
using Wms.Domain.Catalog;
using Wms.Domain.Suppliers;

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

    public DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();

    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();

    public DbSet<ProductColor> ProductColors => Set<ProductColor>();

    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .HasSequence<long>("product_internal_code_sequence", "catalog")
            .StartsAt(1)
            .IncrementsBy(1);

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
