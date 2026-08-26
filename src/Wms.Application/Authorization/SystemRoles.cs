namespace Wms.Application.Authorization;

public static class SystemRoles
{
    public const string Administrator = "Administrator";
    public const string WarehouseManager = "WarehouseManager";
    public const string Buyer = "Buyer";
    public const string StockKeeper = "StockKeeper";
    public const string Picker = "Picker";
    public const string Auditor = "Auditor";

    public static IReadOnlyList<RoleDefinition> All { get; } =
    [
        new(
            Administrator,
            "Administrador do sistema com acesso a todas as permissões.",
            SystemPermissions.All.Select(permission => permission.Code).ToArray()),
        new(
            WarehouseManager,
            "Gestor responsável pela administração operacional do estoque.",
            [
                SystemPermissions.Users.Read,
                SystemPermissions.Products.Read,
                SystemPermissions.Products.Create,
                SystemPermissions.Products.Update,
                SystemPermissions.Products.Disable,
                SystemPermissions.Warehouses.Read,
                SystemPermissions.Warehouses.Manage,
                SystemPermissions.Inventory.Read,
                SystemPermissions.Inventory.Receive,
                SystemPermissions.Inventory.Issue,
                SystemPermissions.Inventory.Transfer,
                SystemPermissions.Inventory.Adjust,
                SystemPermissions.Inventory.Count,
                SystemPermissions.Suppliers.Read,
                SystemPermissions.Purchasing.Read,
                SystemPermissions.Reports.Read,
                SystemPermissions.Audit.Read
            ]),
        new(
            Buyer,
            "Responsável por fornecedores e processos de compra.",
            [
                SystemPermissions.Products.Read,
                SystemPermissions.Inventory.Read,
                SystemPermissions.Suppliers.Read,
                SystemPermissions.Suppliers.Manage,
                SystemPermissions.Purchasing.Read,
                SystemPermissions.Purchasing.Manage,
                SystemPermissions.Reports.Read
            ]),
        new(
            StockKeeper,
            "Responsável pelas movimentações físicas do estoque.",
            [
                SystemPermissions.Products.Read,
                SystemPermissions.Warehouses.Read,
                SystemPermissions.Inventory.Read,
                SystemPermissions.Inventory.Receive,
                SystemPermissions.Inventory.Issue,
                SystemPermissions.Inventory.Transfer,
                SystemPermissions.Inventory.Count
            ]),
        new(
            Picker,
            "Responsável pela separação e expedição de materiais.",
            [
                SystemPermissions.Products.Read,
                SystemPermissions.Warehouses.Read,
                SystemPermissions.Inventory.Read,
                SystemPermissions.Inventory.Issue
            ]),
        new(
            Auditor,
            "Responsável por inventários e auditoria operacional.",
            [
                SystemPermissions.Products.Read,
                SystemPermissions.Warehouses.Read,
                SystemPermissions.Inventory.Read,
                SystemPermissions.Inventory.Count,
                SystemPermissions.Reports.Read,
                SystemPermissions.Audit.Read
            ])
    ];
}
