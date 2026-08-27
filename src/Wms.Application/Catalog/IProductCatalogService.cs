namespace Wms.Application.Catalog;

public interface IProductCatalogService
{
    Task<IReadOnlyCollection<ProductCategorySummary>> GetCategoriesAsync(
        CancellationToken cancellationToken = default);

    Task<CatalogResult<ProductCategorySummary>> CreateCategoryAsync(
        CreateCategoryCommand command,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProductColorSummary>> GetColorsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UnitOfMeasureSummary>> GetUnitsOfMeasureAsync(
        CancellationToken cancellationToken = default);

    Task<PagedProducts> GetProductsAsync(
        string? search,
        Guid? categoryId,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ProductDetails?> GetProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<CatalogResult<ProductDetails>> CreateProductAsync(
        CreateProductCommand command,
        CancellationToken cancellationToken = default);

    Task<CatalogResult<ProductDetails>> UpdateProductAsync(
        Guid productId,
        UpdateProductCommand command,
        CancellationToken cancellationToken = default);

    Task<CatalogResult<ProductDetails>> SetProductStatusAsync(
        Guid productId,
        bool isActive,
        CancellationToken cancellationToken = default);

    Task<CatalogResult<bool>> DeleteProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}
