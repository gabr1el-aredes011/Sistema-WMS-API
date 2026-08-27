namespace Wms.Application.Suppliers;

public sealed record SupplierSummary(
    Guid Id,
    string LegalName,
    string? TradeName,
    string TaxId,
    string? Email,
    string? Phone,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);

public sealed record PagedSuppliers(
    IReadOnlyCollection<SupplierSummary> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => TotalCount == 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public sealed record CreateSupplierCommand(
    string LegalName,
    string? TradeName,
    string TaxId,
    string? Email,
    string? Phone);

public sealed record UpdateSupplierCommand(
    string LegalName,
    string? TradeName,
    string TaxId,
    string? Email,
    string? Phone);

public enum SupplierFailure
{
    None,
    NotFound,
    Validation,
    Conflict
}

public sealed record SupplierResult<T>(
    SupplierFailure Failure,
    T? Value,
    IReadOnlyCollection<string> Errors)
{
    public bool Succeeded => Failure == SupplierFailure.None;

    public static SupplierResult<T> Success(T value) =>
        new(SupplierFailure.None, value, []);

    public static SupplierResult<T> Fail(
        SupplierFailure failure,
        params string[] errors) => new(failure, default, errors);
}
