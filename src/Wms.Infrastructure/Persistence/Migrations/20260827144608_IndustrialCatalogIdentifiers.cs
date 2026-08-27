using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IndustrialCatalogIdentifiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_product_variants_barcode",
                schema: "catalog",
                table: "product_variants");

            migrationBuilder.DropIndex(
                name: "ux_product_variants_normalized_sku",
                schema: "catalog",
                table: "product_variants");

            migrationBuilder.DropColumn(
                name: "NormalizedSku",
                schema: "catalog",
                table: "product_variants");

            migrationBuilder.RenameColumn(
                name: "Barcode",
                schema: "catalog",
                table: "product_variants",
                newName: "ExternalBarcode");

            migrationBuilder.RenameColumn(
                name: "Sku",
                schema: "catalog",
                table: "product_variants",
                newName: "ExternalReference");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalReference",
                schema: "catalog",
                table: "product_variants",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.CreateSequence(
                name: "product_internal_code_sequence",
                schema: "catalog");

            migrationBuilder.AddColumn<string>(
                name: "ItemType",
                schema: "catalog",
                table: "products",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "Component");

            migrationBuilder.AddColumn<string>(
                name: "InternalCode",
                schema: "catalog",
                table: "product_variants",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValueSql: "'PV-' || LPAD(nextval('catalog.product_internal_code_sequence')::text, 8, '0')");

            migrationBuilder.CreateIndex(
                name: "ix_product_variants_external_reference",
                schema: "catalog",
                table: "product_variants",
                column: "ExternalReference");

            migrationBuilder.CreateIndex(
                name: "ux_product_variants_external_barcode",
                schema: "catalog",
                table: "product_variants",
                column: "ExternalBarcode",
                unique: true,
                filter: "\"ExternalBarcode\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ux_product_variants_internal_code",
                schema: "catalog",
                table: "product_variants",
                column: "InternalCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_product_variants_external_reference",
                schema: "catalog",
                table: "product_variants");

            migrationBuilder.DropIndex(
                name: "ux_product_variants_external_barcode",
                schema: "catalog",
                table: "product_variants");

            migrationBuilder.DropIndex(
                name: "ux_product_variants_internal_code",
                schema: "catalog",
                table: "product_variants");

            migrationBuilder.Sql(
                "UPDATE catalog.product_variants " +
                "SET \"ExternalReference\" = \"InternalCode\" " +
                "WHERE \"ExternalReference\" IS NULL;");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalReference",
                schema: "catalog",
                table: "product_variants",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "ItemType",
                schema: "catalog",
                table: "products");

            migrationBuilder.DropColumn(
                name: "InternalCode",
                schema: "catalog",
                table: "product_variants");

            migrationBuilder.DropSequence(
                name: "product_internal_code_sequence",
                schema: "catalog");

            migrationBuilder.RenameColumn(
                name: "ExternalBarcode",
                schema: "catalog",
                table: "product_variants",
                newName: "Barcode");

            migrationBuilder.RenameColumn(
                name: "ExternalReference",
                schema: "catalog",
                table: "product_variants",
                newName: "Sku");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedSku",
                schema: "catalog",
                table: "product_variants",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ux_product_variants_barcode",
                schema: "catalog",
                table: "product_variants",
                column: "Barcode",
                unique: true,
                filter: "\"Barcode\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ux_product_variants_normalized_sku",
                schema: "catalog",
                table: "product_variants",
                column: "NormalizedSku",
                unique: true);
        }
    }
}
