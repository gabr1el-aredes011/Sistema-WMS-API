using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Wms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SuppliersAndUnitsFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "suppliers");

            migrationBuilder.CreateTable(
                name: "suppliers",
                schema: "suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NormalizedLegalName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TradeName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TaxId = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "units_of_measure",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_units_of_measure", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "catalog",
                table: "units_of_measure",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), "UN", new DateTimeOffset(new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "Unidade" },
                    { new Guid("40000000-0000-0000-0000-000000000002"), "PAR", new DateTimeOffset(new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "Par" },
                    { new Guid("40000000-0000-0000-0000-000000000003"), "KIT", new DateTimeOffset(new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "Kit / conjunto" },
                    { new Guid("40000000-0000-0000-0000-000000000004"), "KG", new DateTimeOffset(new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "Quilograma" },
                    { new Guid("40000000-0000-0000-0000-000000000005"), "M", new DateTimeOffset(new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "Metro" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_suppliers_normalized_legal_name",
                schema: "suppliers",
                table: "suppliers",
                column: "NormalizedLegalName");

            migrationBuilder.CreateIndex(
                name: "ux_suppliers_tax_id",
                schema: "suppliers",
                table: "suppliers",
                column: "TaxId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_units_of_measure_code",
                schema: "catalog",
                table: "units_of_measure",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "suppliers",
                schema: "suppliers");

            migrationBuilder.DropTable(
                name: "units_of_measure",
                schema: "catalog");
        }
    }
}
