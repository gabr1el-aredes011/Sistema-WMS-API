using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OrdersProductionFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "operations");

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "operations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceKey = table.Column<string>(type: "character varying(44)", maxLength: 44, nullable: false),
                    InvoiceNumber = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    Channel = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Customer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    Responsible = table.Column<string>(type: "text", nullable: true),
                    DueAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "stock_balances",
                schema: "operations",
                columns: table => new
                {
                    VariantId = table.Column<Guid>(type: "uuid", nullable: false),
                    OnHand = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Reserved = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_balances", x => x.VariantId);
                    table.CheckConstraint("ck_stock_quantities", "\"OnHand\" >= 0 AND \"Reserved\" >= 0 AND \"Reserved\" <= \"OnHand\"");
                    table.ForeignKey(
                        name: "FK_stock_balances_product_variants_VariantId",
                        column: x => x.VariantId,
                        principalSchema: "catalog",
                        principalTable: "product_variants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_movements",
                schema: "operations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    VariantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Kind = table.Column<string>(type: "text", nullable: false),
                    Actor = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_movements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stock_movements_product_variants_VariantId",
                        column: x => x.VariantId,
                        principalSchema: "catalog",
                        principalTable: "product_variants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_documents",
                schema: "operations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SalesOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_documents_orders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalSchema: "operations",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_events",
                schema: "operations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SalesOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Actor = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_events_orders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalSchema: "operations",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_lines",
                schema: "operations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SalesOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    VariantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Reserved = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    IsCustom = table.Column<bool>(type: "boolean", nullable: false),
                    CustomCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_lines", x => x.Id);
                    table.CheckConstraint("ck_line_quantities", "\"Quantity\" > 0 AND \"Reserved\" >= 0 AND \"Reserved\" <= \"Quantity\"");
                    table.ForeignKey(
                        name: "FK_order_lines_orders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalSchema: "operations",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_lines_product_variants_VariantId",
                        column: x => x.VariantId,
                        principalSchema: "catalog",
                        principalTable: "product_variants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_documents_SalesOrderId_Kind",
                schema: "operations",
                table: "order_documents",
                columns: new[] { "SalesOrderId", "Kind" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_events_SalesOrderId",
                schema: "operations",
                table: "order_events",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_order_lines_SalesOrderId",
                schema: "operations",
                table: "order_lines",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_order_lines_VariantId",
                schema: "operations",
                table: "order_lines",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_InvoiceKey",
                schema: "operations",
                table: "orders",
                column: "InvoiceKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_movements_OperationId_VariantId",
                schema: "operations",
                table: "stock_movements",
                columns: new[] { "OperationId", "VariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_movements_VariantId",
                schema: "operations",
                table: "stock_movements",
                column: "VariantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_documents",
                schema: "operations");

            migrationBuilder.DropTable(
                name: "order_events",
                schema: "operations");

            migrationBuilder.DropTable(
                name: "order_lines",
                schema: "operations");

            migrationBuilder.DropTable(
                name: "stock_balances",
                schema: "operations");

            migrationBuilder.DropTable(
                name: "stock_movements",
                schema: "operations");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "operations");
        }
    }
}
