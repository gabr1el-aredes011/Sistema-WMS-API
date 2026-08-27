using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Domain.Catalog;

namespace Wms.Infrastructure.Persistence.Configurations.Catalog;

public sealed class UnitOfMeasureConfiguration
    : IEntityTypeConfiguration<UnitOfMeasure>
{
    private static readonly DateTimeOffset SeededAtUtc =
        new(2026, 8, 28, 0, 0, 0, TimeSpan.Zero);

    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.ToTable("units_of_measure", "catalog");
        builder.HasKey(unit => unit.Id);

        builder.Property(unit => unit.Code).HasMaxLength(12).IsRequired();
        builder.Property(unit => unit.Name).HasMaxLength(80).IsRequired();
        builder.Property(unit => unit.IsActive).IsRequired();
        builder.Property(unit => unit.CreatedAtUtc).IsRequired();

        builder.HasIndex(unit => unit.Code)
            .IsUnique()
            .HasDatabaseName("ux_units_of_measure_code");

        builder.HasData(
            Create("40000000-0000-0000-0000-000000000001", "UN", "Unidade"),
            Create("40000000-0000-0000-0000-000000000002", "PAR", "Par"),
            Create("40000000-0000-0000-0000-000000000003", "KIT", "Kit / conjunto"),
            Create("40000000-0000-0000-0000-000000000004", "KG", "Quilograma"),
            Create("40000000-0000-0000-0000-000000000005", "M", "Metro"));
    }

    private static UnitOfMeasure Create(string id, string code, string name) =>
        new()
        {
            Id = Guid.Parse(id),
            Code = code,
            Name = name,
            IsActive = true,
            CreatedAtUtc = SeededAtUtc
        };
}
