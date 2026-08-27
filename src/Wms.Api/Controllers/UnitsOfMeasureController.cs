using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Application.Authorization;
using Wms.Application.Catalog;

namespace Wms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/units-of-measure")]
public sealed class UnitsOfMeasureController(
    IProductCatalogService productCatalogService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = SystemPermissions.Products.Read)]
    [ProducesResponseType<IReadOnlyCollection<UnitOfMeasureSummary>>(
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var units = await productCatalogService.GetUnitsOfMeasureAsync(
            cancellationToken);
        return Ok(units);
    }
}
