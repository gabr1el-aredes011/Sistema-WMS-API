using Microsoft.EntityFrameworkCore;
using Wms.Application.Catalog;
using Wms.Domain.Catalog;
using Wms.Infrastructure.Persistence;

namespace Wms.Infrastructure.Catalog;

internal sealed class ProductCatalogService(
    WmsDbContext dbContext,
    TimeProvider timeProvider) : IProductCatalogService
{
    public async Task<IReadOnlyCollection<ProductCategorySummary>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ProductCategories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new ProductCategorySummary(
                category.Id,
                category.Name,
                category.IsActive,
                category.Products.Count))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<CatalogResult<ProductCategorySummary>> CreateCategoryAsync(
        CreateCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var name = command.Name.Trim();
        var normalizedName = Normalize(name);

        if (string.IsNullOrWhiteSpace(name))
        {
            return CatalogResult<ProductCategorySummary>.Fail(
                CatalogFailure.Validation,
                "Informe o nome da categoria.");
        }

        if (await dbContext.ProductCategories.AnyAsync(
            category => category.NormalizedName == normalizedName,
            cancellationToken))
        {
            return CatalogResult<ProductCategorySummary>.Fail(
                CatalogFailure.Conflict,
                "Já existe uma categoria com este nome.");
        }

        var category = new ProductCategory
        {
            Name = name,
            NormalizedName = normalizedName,
            CreatedAtUtc = timeProvider.GetUtcNow()
        };

        dbContext.ProductCategories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CatalogResult<ProductCategorySummary>.Success(
            new ProductCategorySummary(
                category.Id,
                category.Name,
                category.IsActive,
                ProductCount: 0));
    }

    public async Task<IReadOnlyCollection<ProductColorSummary>> GetColorsAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ProductColors
            .AsNoTracking()
            .Where(color => color.IsActive)
            .OrderBy(color => color.Name)
            .Select(color => new ProductColorSummary(
                color.Id,
                color.Name,
                color.HexCode))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<UnitOfMeasureSummary>> GetUnitsOfMeasureAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.UnitsOfMeasure
            .AsNoTracking()
            .Where(unit => unit.IsActive)
            .OrderBy(unit => unit.Code)
            .Select(unit => new UnitOfMeasureSummary(
                unit.Id,
                unit.Code,
                unit.Name))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<PagedProducts> GetProductsAsync(
        string? search,
        Guid? categoryId,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = dbContext.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .Include(product => product.Variants)
            .AsSplitQuery()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = Normalize(search);
            query = query.Where(product =>
                product.NormalizedName.Contains(normalizedSearch) ||
                product.Variants.Any(variant =>
                    variant.InternalCode.Contains(normalizedSearch) ||
                    variant.ExternalReference == search.Trim() ||
                    variant.ExternalBarcode == search.Trim()));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(product => product.CategoryId == categoryId);
        }

        if (isActive.HasValue)
        {
            query = query.Where(product => product.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var products = await query
            .OrderBy(product => product.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return new PagedProducts(
            products.Select(MapSummary).ToArray(),
            page,
            pageSize,
            totalCount);
    }

    public async Task<ProductDetails?> GetProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Include(item => item.Category)
            .Include(item => item.Variants)
            .SingleOrDefaultAsync(item => item.Id == productId, cancellationToken);

        return product is null ? null : MapDetails(product);
    }

    public async Task<CatalogResult<ProductDetails>> CreateProductAsync(
        CreateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = ValidateProduct(command);

        if (validationErrors.Length > 0)
        {
            return CatalogResult<ProductDetails>.Fail(
                CatalogFailure.Validation,
                validationErrors);
        }

        var category = await dbContext.ProductCategories
            .SingleOrDefaultAsync(
                item => item.Id == command.CategoryId && item.IsActive,
                cancellationToken);

        if (category is null)
        {
            return CatalogResult<ProductDetails>.Fail(
                CatalogFailure.NotFound,
                "Categoria não encontrada ou inativa.");
        }

        var normalizedName = Normalize(command.Name);

        if (await dbContext.Products.IgnoreQueryFilters().AnyAsync(
            product => product.CategoryId == command.CategoryId &&
                product.NormalizedName == normalizedName,
            cancellationToken))
        {
            return CatalogResult<ProductDetails>.Fail(
                CatalogFailure.Conflict,
                "Já existe um produto com este nome na categoria selecionada.");
        }

        var colorsByNormalizedName = await dbContext.ProductColors
            .AsNoTracking()
            .Where(color => color.IsActive)
            .ToDictionaryAsync(
                color => color.NormalizedName,
                color => color.Name,
                cancellationToken);

        if (command.Variants.Any(variant =>
            !colorsByNormalizedName.ContainsKey(Normalize(variant.Color))))
        {
            return CatalogResult<ProductDetails>.Fail(
                CatalogFailure.Validation,
                "Selecione somente cores ativas no catálogo da empresa.");
        }

        var activeUnitCodes = await dbContext.UnitsOfMeasure
            .AsNoTracking()
            .Where(unit => unit.IsActive)
            .Select(unit => unit.Code)
            .ToArrayAsync(cancellationToken);

        if (command.Variants.Any(variant =>
            !activeUnitCodes.Contains(
                variant.UnitOfMeasure.Trim().ToUpperInvariant(),
                StringComparer.Ordinal)))
        {
            return CatalogResult<ProductDetails>.Fail(
                CatalogFailure.Validation,
                "Selecione somente unidades de medida ativas no catálogo.");
        }

        var normalizedColors = command.Variants
            .Select(variant => Normalize(variant.Color))
            .ToArray();

        if (normalizedColors.Distinct(StringComparer.Ordinal).Count() !=
            normalizedColors.Length)
        {
            return CatalogResult<ProductDetails>.Fail(
                CatalogFailure.Validation,
                "Não repita a mesma cor nas variantes do produto.");
        }

        var barcodes = command.Variants
            .Select(variant => variant.ExternalBarcode?.Trim())
            .Where(barcode => !string.IsNullOrWhiteSpace(barcode))
            .Cast<string>()
            .ToArray();

        if (barcodes.Distinct(StringComparer.Ordinal).Count() != barcodes.Length)
        {
            return CatalogResult<ProductDetails>.Fail(
                CatalogFailure.Validation,
                "Os códigos de barras das variantes precisam ser únicos.");
        }

        if (barcodes.Length > 0 && await dbContext.ProductVariants.IgnoreQueryFilters().AnyAsync(
            variant => variant.ExternalBarcode != null &&
                barcodes.Contains(variant.ExternalBarcode),
            cancellationToken))
        {
            return CatalogResult<ProductDetails>.Fail(
                CatalogFailure.Conflict,
                "Um ou mais códigos de barras já estão cadastrados.");
        }

        var now = timeProvider.GetUtcNow();
        var product = new Product
        {
            CategoryId = category.Id,
            Category = category,
            Name = command.Name.Trim(),
            NormalizedName = normalizedName,
            Type = command.Type.Trim(),
            ItemType = NormalizeItemType(command.ItemType),
            Model = NormalizeOptional(command.Model),
            HeightMillimeters = command.HeightMillimeters,
            DepthMillimeters = command.DepthMillimeters,
            LengthMillimeters = command.LengthMillimeters,
            CreatedAtUtc = now,
            Variants = command.Variants.Select(variant => new ProductVariant
            {
                Color = colorsByNormalizedName[Normalize(variant.Color)],
                ExternalReference = NormalizeOptional(variant.ExternalReference),
                ExternalBarcode = NormalizeOptional(variant.ExternalBarcode),
                UnitOfMeasure = variant.UnitOfMeasure.Trim().ToUpperInvariant(),
                CreatedAtUtc = now
            }).ToArray()
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CatalogResult<ProductDetails>.Success(MapDetails(product));
    }

    public async Task<CatalogResult<ProductDetails>> UpdateProductAsync(
        Guid productId,
        UpdateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products
            .Include(item => item.Category)
            .Include(item => item.Variants)
            .SingleOrDefaultAsync(item => item.Id == productId, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        if (!HasValidDimensions(
            command.HeightMillimeters,
            command.DepthMillimeters,
            command.LengthMillimeters))
        {
            return CatalogResult<ProductDetails>.Fail(
                CatalogFailure.Validation,
                "As dimensões precisam ser maiores que zero.");
        }

        if (string.IsNullOrWhiteSpace(command.Name) ||
            string.IsNullOrWhiteSpace(command.Type))
        {
            return CatalogResult<ProductDetails>.Fail(
                CatalogFailure.Validation,
                "Nome e tipo do produto são obrigatórios.");
        }

        if (!CatalogItemTypes.All.Contains(
            command.ItemType,
            StringComparer.OrdinalIgnoreCase))
        {
            return CatalogResult<ProductDetails>.Fail(
                CatalogFailure.Validation,
                "Informe uma classificação de item válida.");
        }

        var category = await dbContext.ProductCategories
            .SingleOrDefaultAsync(
                item => item.Id == command.CategoryId && item.IsActive,
                cancellationToken);

        if (category is null)
        {
            return CatalogResult<ProductDetails>.Fail(
                CatalogFailure.NotFound,
                "Categoria não encontrada ou inativa.");
        }

        var normalizedName = Normalize(command.Name);
        var hasDuplicate = await dbContext.Products.AnyAsync(
            item => item.Id != productId &&
                item.CategoryId == command.CategoryId &&
                item.NormalizedName == normalizedName,
            cancellationToken);

        if (hasDuplicate)
        {
            return CatalogResult<ProductDetails>.Fail(
                CatalogFailure.Conflict,
                "Já existe um produto com este nome na categoria selecionada.");
        }

        product.CategoryId = category.Id;
        product.Category = category;
        product.Name = command.Name.Trim();
        product.NormalizedName = normalizedName;
        product.Type = command.Type.Trim();
        product.ItemType = NormalizeItemType(command.ItemType);
        product.Model = NormalizeOptional(command.Model);
        product.HeightMillimeters = command.HeightMillimeters;
        product.DepthMillimeters = command.DepthMillimeters;
        product.LengthMillimeters = command.LengthMillimeters;
        product.UpdatedAtUtc = timeProvider.GetUtcNow();

        await dbContext.SaveChangesAsync(cancellationToken);
        return CatalogResult<ProductDetails>.Success(MapDetails(product));
    }

    public async Task<CatalogResult<ProductDetails>> SetProductStatusAsync(
        Guid productId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products
            .Include(item => item.Category)
            .Include(item => item.Variants)
            .SingleOrDefaultAsync(item => item.Id == productId, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        product.IsActive = isActive;
        product.UpdatedAtUtc = timeProvider.GetUtcNow();
        await dbContext.SaveChangesAsync(cancellationToken);

        return CatalogResult<ProductDetails>.Success(MapDetails(product));
    }

    public async Task<CatalogResult<bool>> DeleteProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products
            .Include(item => item.Variants)
            .SingleOrDefaultAsync(item => item.Id == productId, cancellationToken);

        if (product is null)
        {
            return CatalogResult<bool>.Fail(
                CatalogFailure.NotFound,
                "Produto não encontrado.");
        }

        if (product.IsActive)
        {
            return CatalogResult<bool>.Fail(
                CatalogFailure.Conflict,
                "Inative o produto antes de excluí-lo.");
        }

        var now = timeProvider.GetUtcNow();
        product.DeletedAtUtc = now;
        product.UpdatedAtUtc = now;

        foreach (var variant in product.Variants)
        {
            variant.IsActive = false;
            variant.UpdatedAtUtc = now;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return CatalogResult<bool>.Success(true);
    }

    private static string[] ValidateProduct(CreateProductCommand command)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(command.Name))
            errors.Add("Informe o nome do produto.");
        if (string.IsNullOrWhiteSpace(command.Type))
            errors.Add("Informe o tipo do produto.");
        if (!CatalogItemTypes.All.Contains(
            command.ItemType,
            StringComparer.OrdinalIgnoreCase))
            errors.Add("Informe uma classificação de item válida.");
        if (command.Variants.Count == 0)
            errors.Add("Cadastre ao menos uma variante.");
        if (command.Variants.Any(variant =>
            string.IsNullOrWhiteSpace(variant.Color) ||
            string.IsNullOrWhiteSpace(variant.UnitOfMeasure)))
            errors.Add("Cor e unidade são obrigatórias em todas as variantes.");
        if (!HasValidDimensions(
            command.HeightMillimeters,
            command.DepthMillimeters,
            command.LengthMillimeters))
            errors.Add("As dimensões precisam ser maiores que zero.");

        return errors.ToArray();
    }

    private static bool HasValidDimensions(
        int? height,
        int? depth,
        int? length)
    {
        return (!height.HasValue || height.Value > 0) &&
            (!depth.HasValue || depth.Value > 0) &&
            (!length.HasValue || length.Value > 0);
    }

    private static ProductSummary MapSummary(Product product)
    {
        return new ProductSummary(
            product.Id,
            product.Name,
            product.CategoryId,
            product.Category.Name,
            product.Type,
            product.ItemType,
            product.Model,
            product.HeightMillimeters,
            product.DepthMillimeters,
            product.LengthMillimeters,
            product.IsActive,
            product.Variants.Count,
            product.Variants
                .Select(variant => variant.Color)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Order(StringComparer.OrdinalIgnoreCase)
                .ToArray());
    }

    private static ProductDetails MapDetails(Product product)
    {
        return new ProductDetails(
            product.Id,
            product.Name,
            product.CategoryId,
            product.Category.Name,
            product.Type,
            product.ItemType,
            product.Model,
            product.HeightMillimeters,
            product.DepthMillimeters,
            product.LengthMillimeters,
            product.IsActive,
            product.CreatedAtUtc,
            product.UpdatedAtUtc,
            product.Variants
                .OrderBy(variant => variant.Color)
                .Select(variant => new ProductVariantDetails(
                    variant.Id,
                    variant.InternalCode,
                    variant.Color,
                    variant.ExternalReference,
                    variant.ExternalBarcode,
                    variant.UnitOfMeasure,
                    variant.IsActive))
                .ToArray());
    }

    private static string Normalize(string value) =>
        value.Trim().ToUpperInvariant();

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string NormalizeItemType(string value) =>
        CatalogItemTypes.All.Single(itemType =>
            itemType.Equals(value, StringComparison.OrdinalIgnoreCase));

    private static CatalogResult<ProductDetails> NotFound() =>
        CatalogResult<ProductDetails>.Fail(
            CatalogFailure.NotFound,
            "Produto não encontrado.");
}
