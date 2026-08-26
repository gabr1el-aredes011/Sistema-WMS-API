using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Wms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedAuthorizationCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "auth",
                table: "permissions",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "Description", "IsActive", "IsSystem", "Module", "Name", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "users.read", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite consultar usuários.", true, true, "Identity", "Visualizar usuários", null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "users.create", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite cadastrar novos usuários.", true, true, "Identity", "Criar usuários", null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "users.update", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite alterar dados de usuários.", true, true, "Identity", "Atualizar usuários", null },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "users.disable", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite bloquear o acesso de usuários.", true, true, "Identity", "Desativar usuários", null },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "users.roles.manage", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite atribuir e remover roles de usuários.", true, true, "Identity", "Gerenciar roles de usuários", null },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "roles.read", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite consultar roles e seus acessos.", true, true, "Identity", "Visualizar roles", null },
                    { new Guid("10000000-0000-0000-0000-000000000007"), "roles.manage", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite criar e alterar roles e suas permissões.", true, true, "Identity", "Gerenciar roles", null },
                    { new Guid("10000000-0000-0000-0000-000000000008"), "permissions.read", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite consultar o catálogo de permissões.", true, true, "Identity", "Visualizar permissões", null },
                    { new Guid("10000000-0000-0000-0000-000000000009"), "products.read", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite consultar produtos.", true, true, "Catalog", "Visualizar produtos", null },
                    { new Guid("10000000-0000-0000-0000-000000000010"), "products.create", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite cadastrar produtos.", true, true, "Catalog", "Criar produtos", null },
                    { new Guid("10000000-0000-0000-0000-000000000011"), "products.update", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite alterar produtos.", true, true, "Catalog", "Atualizar produtos", null },
                    { new Guid("10000000-0000-0000-0000-000000000012"), "products.disable", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite desativar produtos.", true, true, "Catalog", "Desativar produtos", null },
                    { new Guid("10000000-0000-0000-0000-000000000013"), "warehouses.read", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite consultar armazéns e endereços.", true, true, "Warehouse", "Visualizar armazéns", null },
                    { new Guid("10000000-0000-0000-0000-000000000014"), "warehouses.manage", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite configurar armazéns e endereços.", true, true, "Warehouse", "Gerenciar armazéns", null },
                    { new Guid("10000000-0000-0000-0000-000000000015"), "inventory.read", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite consultar saldos e movimentações.", true, true, "Inventory", "Visualizar estoque", null },
                    { new Guid("10000000-0000-0000-0000-000000000016"), "inventory.receive", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite registrar recebimentos de materiais.", true, true, "Inventory", "Receber estoque", null },
                    { new Guid("10000000-0000-0000-0000-000000000017"), "inventory.issue", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite registrar saídas de materiais.", true, true, "Inventory", "Realizar saídas", null },
                    { new Guid("10000000-0000-0000-0000-000000000018"), "inventory.transfer", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite transferir materiais entre endereços.", true, true, "Inventory", "Transferir estoque", null },
                    { new Guid("10000000-0000-0000-0000-000000000019"), "inventory.adjust", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite executar ajustes controlados de estoque.", true, true, "Inventory", "Ajustar estoque", null },
                    { new Guid("10000000-0000-0000-0000-000000000020"), "inventory.count", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite executar contagens de inventário.", true, true, "InventoryCount", "Realizar inventário", null },
                    { new Guid("10000000-0000-0000-0000-000000000021"), "suppliers.read", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite consultar fornecedores.", true, true, "Suppliers", "Visualizar fornecedores", null },
                    { new Guid("10000000-0000-0000-0000-000000000022"), "suppliers.manage", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite cadastrar e alterar fornecedores.", true, true, "Suppliers", "Gerenciar fornecedores", null },
                    { new Guid("10000000-0000-0000-0000-000000000023"), "purchasing.read", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite consultar processos de compra.", true, true, "Purchasing", "Visualizar compras", null },
                    { new Guid("10000000-0000-0000-0000-000000000024"), "purchasing.manage", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite administrar processos de compra.", true, true, "Purchasing", "Gerenciar compras", null },
                    { new Guid("10000000-0000-0000-0000-000000000025"), "reports.read", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite acessar relatórios e indicadores.", true, true, "Analytics", "Visualizar relatórios", null },
                    { new Guid("10000000-0000-0000-0000-000000000026"), "audit.read", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Permite consultar registros de auditoria.", true, true, "Audit", "Visualizar auditoria", null }
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "roles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAtUtc", "Description", "IsSystem", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "system-role-20000000000000000000000000000001", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Administrador do sistema com acesso a todas as permissões.", true, "Administrator", "ADMINISTRATOR" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "system-role-20000000000000000000000000000002", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Gestor responsável pela administração operacional do estoque.", true, "WarehouseManager", "WAREHOUSEMANAGER" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "system-role-20000000000000000000000000000003", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Responsável por fornecedores e processos de compra.", true, "Buyer", "BUYER" },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "system-role-20000000000000000000000000000004", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Responsável pelas movimentações físicas do estoque.", true, "StockKeeper", "STOCKKEEPER" },
                    { new Guid("20000000-0000-0000-0000-000000000005"), "system-role-20000000000000000000000000000005", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Responsável pela separação e expedição de materiais.", true, "Picker", "PICKER" },
                    { new Guid("20000000-0000-0000-0000-000000000006"), "system-role-20000000000000000000000000000006", new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Responsável por inventários e auditoria operacional.", true, "Auditor", "AUDITOR" }
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "role_permissions",
                columns: new[] { "PermissionId", "RoleId", "AssignedAtUtc", "AssignedByUserId" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000005"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000006"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000007"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000008"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000010"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000011"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000012"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000013"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000014"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000016"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000017"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000018"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000019"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000020"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000021"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000022"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000023"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000024"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000025"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000026"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000010"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000011"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000012"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000013"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000014"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000016"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000017"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000018"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000019"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000020"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000021"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000023"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000025"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000026"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000021"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000022"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000023"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000024"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000025"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000013"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000016"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000017"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000018"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000020"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("20000000-0000-0000-0000-000000000005"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000013"), new Guid("20000000-0000-0000-0000-000000000005"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000005"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000017"), new Guid("20000000-0000-0000-0000-000000000005"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("20000000-0000-0000-0000-000000000006"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000013"), new Guid("20000000-0000-0000-0000-000000000006"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000006"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000020"), new Guid("20000000-0000-0000-0000-000000000006"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000025"), new Guid("20000000-0000-0000-0000-000000000006"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null },
                    { new Guid("10000000-0000-0000-0000-000000000026"), new Guid("20000000-0000-0000-0000-000000000006"), new DateTimeOffset(new DateTime(2026, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000005"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000006"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000007"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000008"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000010"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000011"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000012"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000013"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000014"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000016"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000017"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000018"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000019"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000020"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000021"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000022"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000023"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000024"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000025"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000026"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000010"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000011"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000012"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000013"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000014"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000016"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000017"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000018"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000019"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000020"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000021"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000023"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000025"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000026"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("20000000-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000021"), new Guid("20000000-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000022"), new Guid("20000000-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000023"), new Guid("20000000-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000024"), new Guid("20000000-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000025"), new Guid("20000000-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("20000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000013"), new Guid("20000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000016"), new Guid("20000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000017"), new Guid("20000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000018"), new Guid("20000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000020"), new Guid("20000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("20000000-0000-0000-0000-000000000005") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000013"), new Guid("20000000-0000-0000-0000-000000000005") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000005") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000017"), new Guid("20000000-0000-0000-0000-000000000005") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("20000000-0000-0000-0000-000000000006") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000013"), new Guid("20000000-0000-0000-0000-000000000006") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000006") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000020"), new Guid("20000000-0000-0000-0000-000000000006") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000025"), new Guid("20000000-0000-0000-0000-000000000006") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000026"), new Guid("20000000-0000-0000-0000-000000000006") });

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000023"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000024"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000025"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000026"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000006"));
        }
    }
}
