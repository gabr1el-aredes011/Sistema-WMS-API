using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Api.Contracts.Catalog;
using Wms.Application.Authorization;
using Wms.Application.Catalog;

namespace Wms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/products")]
public sealed class ProductsController(
    IProductCatalogService productCatalogService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = SystemPermissions.Products.Read)]
    [ProducesResponseType<PagedProducts>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] Guid? categoryId,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var products = await productCatalogService.GetProductsAsync(
            search,
            categoryId,
            isActive,
            page,
            pageSize,
            cancellationToken);

        return Ok(products);
    }

    [HttpGet("{productId:guid}")]
    [Authorize(Policy = SystemPermissions.Products.Read)]
    [ProducesResponseType<ProductDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var product = await productCatalogService.GetProductAsync(productId, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.Products.Create)]
    [ProducesResponseType<ProductDetails>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await productCatalogService.CreateProductAsync(
            new CreateProductCommand(
                request.CategoryId,
                request.Name,
                request.Type,
                request.ItemType,
                request.Model,
                request.HeightMillimeters,
                request.DepthMillimeters,
                request.LengthMillimeters,
                request.Variants.Select(MapVariant).ToArray()),
            cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { productId = result.Value!.Id }, result.Value)
            : ToProblem(result);
    }

    [HttpPut("{productId:guid}")]
    [Authorize(Policy = SystemPermissions.Products.Update)]
    [ProducesResponseType<ProductDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid productId,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await productCatalogService.UpdateProductAsync(
            productId,
            new UpdateProductCommand(
                request.CategoryId,
                request.Name,
                request.Type,
                request.ItemType,
                request.Model,
                request.HeightMillimeters,
                request.DepthMillimeters,
                request.LengthMillimeters),
            cancellationToken);

        return result.Succeeded ? Ok(result.Value) : ToProblem(result);
    }

    [HttpPut("{productId:guid}/status")]
    [Authorize(Policy = SystemPermissions.Products.Disable)]
    [ProducesResponseType<ProductDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetStatus(
        Guid productId,
        SetProductStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await productCatalogService.SetProductStatusAsync(
            productId,
            request.IsActive,
            cancellationToken);

        return result.Succeeded ? Ok(result.Value) : ToProblem(result);
    }

    private static CreateProductVariantCommand MapVariant(CreateProductVariantRequest request) =>
        new(
            request.Color,
            request.ExternalReference,
            request.ExternalBarcode,
            request.UnitOfMeasure);

    private IActionResult ToProblem<T>(CatalogResult<T> result)
    {
        var statusCode = result.Failure switch
        {
            CatalogFailure.NotFound => StatusCodes.Status404NotFound,
            CatalogFailure.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        return Problem(
            statusCode: statusCode,
            title: "Não foi possível concluir a operação",
            detail: string.Join(" ", result.Errors));
    }
}
