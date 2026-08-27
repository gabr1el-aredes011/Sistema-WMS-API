using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Application.Authorization;
using Wms.Application.Catalog;

namespace Wms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/product-colors")]
public sealed class ProductColorsController(
    IProductCatalogService productCatalogService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = SystemPermissions.Products.Read)]
    [ProducesResponseType<IReadOnlyCollection<ProductColorSummary>>(
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var colors = await productCatalogService.GetColorsAsync(cancellationToken);
        return Ok(colors);
    }
}
