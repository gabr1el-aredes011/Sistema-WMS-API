namespace Wms.Application.Authorization;

public static class SystemPermissions
{
    public static class Users
    {
        public const string Read = "users.read";
        public const string Create = "users.create";
        public const string Update = "users.update";
        public const string Disable = "users.disable";
        public const string ManageRoles = "users.roles.manage";
    }

    public static class Roles
    {
        public const string Read = "roles.read";
        public const string Manage = "roles.manage";
    }

    public static class Permissions
    {
        public const string Read = "permissions.read";
    }

    public static class Products
    {
        public const string Read = "products.read";
        public const string Create = "products.create";
        public const string Update = "products.update";
        public const string Disable = "products.disable";
        public const string Delete = "products.delete";
    }

    public static class Warehouses
    {
        public const string Read = "warehouses.read";
        public const string Manage = "warehouses.manage";
    }

    public static class Inventory
    {
        public const string Read = "inventory.read";
        public const string Receive = "inventory.receive";
        public const string Issue = "inventory.issue";
        public const string Transfer = "inventory.transfer";
        public const string Adjust = "inventory.adjust";
        public const string Count = "inventory.count";
    }

    public static class Suppliers
    {
        public const string Read = "suppliers.read";
        public const string Manage = "suppliers.manage";
    }

    public static class Purchasing
    {
        public const string Read = "purchasing.read";
        public const string Manage = "purchasing.manage";
    }

    public static class Carriers
    {
        public const string Read = "carriers.read";
        public const string Manage = "carriers.manage";
    }

    public static class Dispatch
    {
        public const string Read = "dispatch.read";
        public const string Manage = "dispatch.manage";
        public const string UpdateReadiness = "dispatch.readiness.update";
    }

    public static class Reports
    {
        public const string Read = "reports.read";
    }

    public static class Audit
    {
        public const string Read = "audit.read";
    }

    public static IReadOnlyList<PermissionDefinition> All { get; } =
    [
        new(Users.Read, "Visualizar usuários", "Identity", "Permite consultar usuários."),
        new(Users.Create, "Criar usuários", "Identity", "Permite cadastrar novos usuários."),
        new(Users.Update, "Atualizar usuários", "Identity", "Permite alterar dados de usuários."),
        new(Users.Disable, "Desativar usuários", "Identity", "Permite bloquear o acesso de usuários."),
        new(Users.ManageRoles, "Gerenciar roles de usuários", "Identity", "Permite atribuir e remover roles de usuários."),
        new(Roles.Read, "Visualizar roles", "Identity", "Permite consultar roles e seus acessos."),
        new(Roles.Manage, "Gerenciar roles", "Identity", "Permite criar e alterar roles e suas permissões."),
        new(Permissions.Read, "Visualizar permissões", "Identity", "Permite consultar o catálogo de permissões."),
        new(Products.Read, "Visualizar produtos", "Catalog", "Permite consultar produtos."),
        new(Products.Create, "Criar produtos", "Catalog", "Permite cadastrar produtos."),
        new(Products.Update, "Atualizar produtos", "Catalog", "Permite alterar produtos."),
        new(Products.Disable, "Desativar produtos", "Catalog", "Permite desativar produtos."),
        new(Warehouses.Read, "Visualizar armazéns", "Warehouse", "Permite consultar armazéns e endereços."),
        new(Warehouses.Manage, "Gerenciar armazéns", "Warehouse", "Permite configurar armazéns e endereços."),
        new(Inventory.Read, "Visualizar estoque", "Inventory", "Permite consultar saldos e movimentações."),
        new(Inventory.Receive, "Receber estoque", "Inventory", "Permite registrar recebimentos de materiais."),
        new(Inventory.Issue, "Realizar saídas", "Inventory", "Permite registrar saídas de materiais."),
        new(Inventory.Transfer, "Transferir estoque", "Inventory", "Permite transferir materiais entre endereços."),
        new(Inventory.Adjust, "Ajustar estoque", "Inventory", "Permite executar ajustes controlados de estoque."),
        new(Inventory.Count, "Realizar inventário", "InventoryCount", "Permite executar contagens de inventário."),
        new(Suppliers.Read, "Visualizar fornecedores", "Suppliers", "Permite consultar fornecedores."),
        new(Suppliers.Manage, "Gerenciar fornecedores", "Suppliers", "Permite cadastrar e alterar fornecedores."),
        new(Purchasing.Read, "Visualizar compras", "Purchasing", "Permite consultar processos de compra."),
        new(Purchasing.Manage, "Gerenciar compras", "Purchasing", "Permite administrar processos de compra."),
        new(Reports.Read, "Visualizar relatórios", "Analytics", "Permite acessar relatórios e indicadores."),
        new(Audit.Read, "Visualizar auditoria", "Audit", "Permite consultar registros de auditoria."),
        new(Products.Delete, "Excluir produtos", "Catalog", "Permite arquivar produtos de forma auditável."),
        new(Carriers.Read, "Visualizar transportadoras", "Shipping", "Permite consultar transportadoras."),
        new(Carriers.Manage, "Gerenciar transportadoras", "Shipping", "Permite cadastrar e alterar transportadoras."),
        new(Dispatch.Read, "Visualizar expedições", "Shipping", "Permite consultar coletas e expedições."),
        new(Dispatch.Manage, "Gerenciar expedições", "Shipping", "Permite criar e administrar solicitações de coleta."),
        new(Dispatch.UpdateReadiness, "Atualizar prontidão", "Shipping", "Permite informar preparação, prontidão e coleta.")
    ];
}
