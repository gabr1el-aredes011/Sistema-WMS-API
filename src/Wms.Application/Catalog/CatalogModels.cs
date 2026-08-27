namespace Wms.Application.Catalog;

public sealed record ProductCategorySummary(
    Guid Id,
    string Name,
    bool IsActive,
    int ProductCount);

public sealed record ProductColorSummary(
    Guid Id,
    string Name,
    string HexCode);

public sealed record UnitOfMeasureSummary(
    Guid Id,
    string Code,
    string Name);

public sealed record ProductVariantDetails(
    Guid Id,
    string InternalCode,
    string Color,
    string? ExternalReference,
    string? ExternalBarcode,
    string UnitOfMeasure,
    bool IsActive);

public sealed record ProductSummary(
    Guid Id,
    string Name,
    Guid CategoryId,
    string CategoryName,
    string Type,
    string ItemType,
    string? Model,
    int? HeightMillimeters,
    int? DepthMillimeters,
    int? LengthMillimeters,
    bool IsActive,
    int VariantCount,
    IReadOnlyCollection<string> Colors);

public sealed record ProductDetails(
    Guid Id,
    string Name,
    Guid CategoryId,
    string CategoryName,
    string Type,
    string ItemType,
    string? Model,
    int? HeightMillimeters,
    int? DepthMillimeters,
    int? LengthMillimeters,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc,
    IReadOnlyCollection<ProductVariantDetails> Variants);

public sealed record PagedProducts(
    IReadOnlyCollection<ProductSummary> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => TotalCount == 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public sealed record CreateCategoryCommand(string Name);

public sealed record CreateProductVariantCommand(
    string Color,
    string? ExternalReference,
    string? ExternalBarcode,
    string UnitOfMeasure);

public sealed record CreateProductCommand(
    Guid CategoryId,
    string Name,
    string Type,
    string ItemType,
    string? Model,
    int? HeightMillimeters,
    int? DepthMillimeters,
    int? LengthMillimeters,
    IReadOnlyCollection<CreateProductVariantCommand> Variants);

public sealed record UpdateProductCommand(
    Guid CategoryId,
    string Name,
    string Type,
    string ItemType,
    string? Model,
    int? HeightMillimeters,
    int? DepthMillimeters,
    int? LengthMillimeters);

public enum CatalogFailure
{
    None,
    NotFound,
    Validation,
    Conflict
}

public sealed record CatalogResult<T>(
    CatalogFailure Failure,
    T? Value,
    IReadOnlyCollection<string> Errors)
{
    public bool Succeeded => Failure == CatalogFailure.None;

    public static CatalogResult<T> Success(T value) =>
        new(CatalogFailure.None, value, []);

    public static CatalogResult<T> Fail(
        CatalogFailure failure,
        params string[] errors) => new(failure, default, errors);
}
