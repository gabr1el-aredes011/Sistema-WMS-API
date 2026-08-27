using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Api.Contracts.Suppliers;
using Wms.Application.Authorization;
using Wms.Application.Suppliers;

namespace Wms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/suppliers")]
public sealed class SuppliersController(ISupplierService supplierService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = SystemPermissions.Suppliers.Read)]
    [ProducesResponseType<PagedSuppliers>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await supplierService.GetSuppliersAsync(
            search,
            isActive,
            page,
            pageSize,
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{supplierId:guid}")]
    [Authorize(Policy = SystemPermissions.Suppliers.Read)]
    [ProducesResponseType<SupplierSummary>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid supplierId,
        CancellationToken cancellationToken)
    {
        var supplier = await supplierService.GetSupplierAsync(
            supplierId,
            cancellationToken);
        return supplier is null ? NotFound() : Ok(supplier);
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.Suppliers.Manage)]
    [ProducesResponseType<SupplierSummary>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        CreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var result = await supplierService.CreateSupplierAsync(
            new CreateSupplierCommand(
                request.LegalName,
                request.TradeName,
                request.TaxId,
                request.Email,
                request.Phone),
            cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(
                nameof(GetById),
                new { supplierId = result.Value!.Id },
                result.Value)
            : ToProblem(result);
    }

    [HttpPut("{supplierId:guid}")]
    [Authorize(Policy = SystemPermissions.Suppliers.Manage)]
    public async Task<IActionResult> Update(
        Guid supplierId,
        UpdateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var result = await supplierService.UpdateSupplierAsync(
            supplierId,
            new UpdateSupplierCommand(
                request.LegalName,
                request.TradeName,
                request.TaxId,
                request.Email,
                request.Phone),
            cancellationToken);
        return result.Succeeded ? Ok(result.Value) : ToProblem(result);
    }

    [HttpPut("{supplierId:guid}/status")]
    [Authorize(Policy = SystemPermissions.Suppliers.Manage)]
    public async Task<IActionResult> SetStatus(
        Guid supplierId,
        SetSupplierStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await supplierService.SetSupplierStatusAsync(
            supplierId,
            request.IsActive,
            cancellationToken);
        return result.Succeeded ? Ok(result.Value) : ToProblem(result);
    }

    private IActionResult ToProblem<T>(SupplierResult<T> result)
    {
        var statusCode = result.Failure switch
        {
            SupplierFailure.NotFound => StatusCodes.Status404NotFound,
            SupplierFailure.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };
        return Problem(
            statusCode: statusCode,
            title: "Não foi possível concluir a operação",
            detail: string.Join(" ", result.Errors));
    }
}
