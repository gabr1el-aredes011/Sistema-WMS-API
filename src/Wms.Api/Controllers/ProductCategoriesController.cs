using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Api.Contracts.Catalog;
using Wms.Application.Authorization;
using Wms.Application.Catalog;

namespace Wms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/product-categories")]
public sealed class ProductCategoriesController(
    IProductCatalogService productCatalogService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = SystemPermissions.Products.Read)]
    [ProducesResponseType<IReadOnlyCollection<ProductCategorySummary>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var categories = await productCatalogService.GetCategoriesAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.Products.Create)]
    [ProducesResponseType<ProductCategorySummary>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        CreateProductCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await productCatalogService.CreateCategoryAsync(
            new CreateCategoryCommand(request.Name),
            cancellationToken);

        return result.Succeeded
            ? Created($"/api/v1/product-categories/{result.Value!.Id}", result.Value)
            : ToProblem(result);
    }

    private IActionResult ToProblem<T>(CatalogResult<T> result)
    {
        var statusCode = result.Failure == CatalogFailure.Conflict
            ? StatusCodes.Status409Conflict
            : StatusCodes.Status400BadRequest;

        return Problem(
            statusCode: statusCode,
            title: "Não foi possível concluir a operação",
            detail: string.Join(" ", result.Errors));
    }
}
