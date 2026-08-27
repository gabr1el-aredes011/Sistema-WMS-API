namespace Wms.Application.Suppliers;

public interface ISupplierService
{
    Task<PagedSuppliers> GetSuppliersAsync(
        string? search,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<SupplierSummary?> GetSupplierAsync(
        Guid supplierId,
        CancellationToken cancellationToken = default);

    Task<SupplierResult<SupplierSummary>> CreateSupplierAsync(
        CreateSupplierCommand command,
        CancellationToken cancellationToken = default);

    Task<SupplierResult<SupplierSummary>> UpdateSupplierAsync(
        Guid supplierId,
        UpdateSupplierCommand command,
        CancellationToken cancellationToken = default);

    Task<SupplierResult<SupplierSummary>> SetSupplierStatusAsync(
        Guid supplierId,
        bool isActive,
        CancellationToken cancellationToken = default);
}
