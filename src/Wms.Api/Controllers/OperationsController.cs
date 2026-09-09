using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Application.Operations;

namespace Wms.Api.Controllers;

[ApiController, Route("api/v1/operations"), Authorize(Policy = "operations.read")]
public sealed class OperationsController(IOperationsService service) : ControllerBase
{
    private string Actor => $"{User.FindFirst("name")?.Value ?? "Operador"} ({User.FindFirst("sub")?.Value})";
    [HttpGet("orders")]
    public async Task<IActionResult> Orders(CancellationToken ct) => Ok(await service.Orders(ct));
    [HttpGet("stock")]
    public async Task<IActionResult> Stock(CancellationToken ct) => Ok(await service.Stock(ct));
    [HttpPost("orders"), Authorize(Policy = "operations.orders"), RequestSizeLimit(25_000_000)]
    public Task<IActionResult> Create(CreateOrderInput input, CancellationToken ct) => Execute(() => service.Create(input, Actor, ct));
    [HttpPost("production"), Authorize(Policy = "operations.production")]
    public Task<IActionResult> Production(ProductionInput input, CancellationToken ct) => Execute(() => service.Produce(input, Actor, ct));
    [HttpPost("orders/{id:guid}/actions")]
    public async Task<IActionResult> Action(Guid id, OrderActionInput input, [FromServices] IAuthorizationService authorization, CancellationToken ct)
    {
        var policy = input.Action == "Cancelled" ? "operations.orders" : input.Action == "CustomComplete" ? "operations.production" : "operations.dispatch";
        if (input.Action is "Comment" or "Assign" && (await authorization.AuthorizeAsync(User, null, "operations.orders")).Succeeded)
            return await Execute(() => service.Act(id, input, Actor, ct));
        if (!(await authorization.AuthorizeAsync(User, null, policy)).Succeeded) return Forbid();
        return await Execute(() => service.Act(id, input, Actor, ct));
    }
    [HttpGet("orders/{id:guid}/documents/{kind}")]
    public async Task<IActionResult> Document(Guid id, string kind, CancellationToken ct)
    {
        var document = (await service.Documents(id, ct)).SingleOrDefault(x => x.Kind == kind);
        if (document == null) return NotFound();
        var bytes = await service.Document(id, kind, ct);
        return bytes == null ? NotFound() : File(bytes, "application/octet-stream", document.Name);
    }
    [HttpGet("orders/{id:guid}/documents")]
    public async Task<IActionResult> Documents(Guid id, CancellationToken ct) => Ok(await service.Documents(id, ct));
    private async Task<IActionResult> Execute(Func<Task> operation)
    {
        try { await operation(); return NoContent(); }
        catch (ArgumentException e) { return Problem(statusCode: 400, detail: e.Message); }
    }
}
