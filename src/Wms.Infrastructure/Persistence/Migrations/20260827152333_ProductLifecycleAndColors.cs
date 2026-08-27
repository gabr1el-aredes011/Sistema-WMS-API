using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Wms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProductLifecycleAndColors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAtUtc",
                schema: "catalog",
                table: "products",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "product_colors",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    HexCode = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_colors", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "permissions",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "Description", "IsActive", "IsSystem", "Module", "Name", "UpdatedAtUtc" },
                values: new object[] { new Guid("10000000-0000-0000-0000-000000000027"), "products.delete", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite arquivar produtos de forma auditável.", true, true, "Catalog", "Excluir produtos", null });

            migrationBuilder.InsertData(
                schema: "catalog",
                table: "product_colors",
                columns: new[] { "Id", "CreatedAtUtc", "HexCode", "IsActive", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "#808080", true, "Cinza", "CINZA" },
                    { new Guid("30000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "#F97316", true, "Laranja", "LARANJA" },
                    { new Guid("30000000-0000-0000-0000-000000000003"), new DateTimeOffset(new DateTime(2026, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "#111827", true, "Preto", "PRETO" },
                    { new Guid("30000000-0000-0000-0000-000000000004"), new DateTimeOffset(new DateTime(2026, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "#2563EB", true, "Azul", "AZUL" }
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "role_permissions",
                columns: new[] { "PermissionId", "RoleId", "AssignedAtUtc", "AssignedByUserId" },
                values: new object[] { new Guid("10000000-0000-0000-0000-000000000027"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null });

            migrationBuilder.CreateIndex(
                name: "ux_product_colors_normalized_name",
                schema: "catalog",
                table: "product_colors",
                column: "NormalizedName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_colors",
                schema: "catalog");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000027"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000027"));

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "catalog",
                table: "products");
        }
    }
}
