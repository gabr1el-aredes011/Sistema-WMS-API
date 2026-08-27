using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Wms.Application.Suppliers;
using Wms.Domain.Suppliers;
using Wms.Infrastructure.Persistence;

namespace Wms.Infrastructure.Suppliers;

internal sealed class SupplierService(
    WmsDbContext dbContext,
    TimeProvider timeProvider) : ISupplierService
{
    public async Task<PagedSuppliers> GetSuppliersAsync(
        string? search,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = dbContext.Suppliers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = Normalize(search);
            var taxIdSearch = DigitsOnly(search);
            query = query.Where(supplier =>
                supplier.NormalizedLegalName.Contains(normalizedSearch) ||
                (supplier.TradeName != null &&
                    supplier.TradeName.ToUpper().Contains(normalizedSearch)) ||
                (taxIdSearch.Length > 0 && supplier.TaxId.Contains(taxIdSearch)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(supplier => supplier.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(supplier => supplier.LegalName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(supplier => Map(supplier))
            .ToArrayAsync(cancellationToken);

        return new PagedSuppliers(items, page, pageSize, totalCount);
    }

    public async Task<SupplierSummary?> GetSupplierAsync(
        Guid supplierId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Suppliers
            .AsNoTracking()
            .Where(supplier => supplier.Id == supplierId)
            .Select(supplier => Map(supplier))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<SupplierResult<SupplierSummary>> CreateSupplierAsync(
        CreateSupplierCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = Validate(command.LegalName, command.TaxId, command.Email);
        if (validationErrors.Length > 0)
        {
            return SupplierResult<SupplierSummary>.Fail(
                SupplierFailure.Validation,
                validationErrors);
        }

        var taxId = DigitsOnly(command.TaxId);
        if (await dbContext.Suppliers.AnyAsync(
            supplier => supplier.TaxId == taxId,
            cancellationToken))
        {
            return SupplierResult<SupplierSummary>.Fail(
                SupplierFailure.Conflict,
                "Já existe um fornecedor cadastrado com este CNPJ.");
        }

        var supplier = new Supplier
        {
            LegalName = command.LegalName.Trim(),
            NormalizedLegalName = Normalize(command.LegalName),
            TradeName = Optional(command.TradeName),
            TaxId = taxId,
            Email = Optional(command.Email)?.ToLowerInvariant(),
            Phone = Optional(command.Phone),
            CreatedAtUtc = timeProvider.GetUtcNow()
        };

        dbContext.Suppliers.Add(supplier);
        await dbContext.SaveChangesAsync(cancellationToken);
        return SupplierResult<SupplierSummary>.Success(Map(supplier));
    }

    public async Task<SupplierResult<SupplierSummary>> UpdateSupplierAsync(
        Guid supplierId,
        UpdateSupplierCommand command,
        CancellationToken cancellationToken = default)
    {
        var supplier = await dbContext.Suppliers.SingleOrDefaultAsync(
            item => item.Id == supplierId,
            cancellationToken);
        if (supplier is null)
        {
            return NotFound();
        }

        var validationErrors = Validate(command.LegalName, command.TaxId, command.Email);
        if (validationErrors.Length > 0)
        {
            return SupplierResult<SupplierSummary>.Fail(
                SupplierFailure.Validation,
                validationErrors);
        }

        var taxId = DigitsOnly(command.TaxId);
        if (await dbContext.Suppliers.AnyAsync(
            item => item.Id != supplierId && item.TaxId == taxId,
            cancellationToken))
        {
            return SupplierResult<SupplierSummary>.Fail(
                SupplierFailure.Conflict,
                "Já existe um fornecedor cadastrado com este CNPJ.");
        }

        supplier.LegalName = command.LegalName.Trim();
        supplier.NormalizedLegalName = Normalize(command.LegalName);
        supplier.TradeName = Optional(command.TradeName);
        supplier.TaxId = taxId;
        supplier.Email = Optional(command.Email)?.ToLowerInvariant();
        supplier.Phone = Optional(command.Phone);
        supplier.UpdatedAtUtc = timeProvider.GetUtcNow();

        await dbContext.SaveChangesAsync(cancellationToken);
        return SupplierResult<SupplierSummary>.Success(Map(supplier));
    }

    public async Task<SupplierResult<SupplierSummary>> SetSupplierStatusAsync(
        Guid supplierId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var supplier = await dbContext.Suppliers.SingleOrDefaultAsync(
            item => item.Id == supplierId,
            cancellationToken);
        if (supplier is null)
        {
            return NotFound();
        }

        supplier.IsActive = isActive;
        supplier.UpdatedAtUtc = timeProvider.GetUtcNow();
        await dbContext.SaveChangesAsync(cancellationToken);
        return SupplierResult<SupplierSummary>.Success(Map(supplier));
    }

    private static string[] Validate(string legalName, string taxId, string? email)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(legalName))
            errors.Add("Informe a razão social do fornecedor.");
        if (!IsValidCnpj(DigitsOnly(taxId)))
            errors.Add("Informe um CNPJ válido.");
        if (!string.IsNullOrWhiteSpace(email) && !MailAddress.TryCreate(email.Trim(), out _))
            errors.Add("Informe um e-mail válido.");
        return errors.ToArray();
    }

    private static bool IsValidCnpj(string value)
    {
        if (value.Length != 14 || value.All(character => character == value[0]))
            return false;

        int CalculateDigit(ReadOnlySpan<char> digits, ReadOnlySpan<int> weights)
        {
            var sum = 0;
            for (var index = 0; index < weights.Length; index++)
                sum += (digits[index] - '0') * weights[index];
            var remainder = sum % 11;
            return remainder < 2 ? 0 : 11 - remainder;
        }

        ReadOnlySpan<int> firstWeights = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        ReadOnlySpan<int> secondWeights = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        return CalculateDigit(value, firstWeights) == value[12] - '0' &&
            CalculateDigit(value, secondWeights) == value[13] - '0';
    }

    private static SupplierSummary Map(Supplier supplier) =>
        new(
            supplier.Id,
            supplier.LegalName,
            supplier.TradeName,
            supplier.TaxId,
            supplier.Email,
            supplier.Phone,
            supplier.IsActive,
            supplier.CreatedAtUtc,
            supplier.UpdatedAtUtc);

    private static string Normalize(string value) => value.Trim().ToUpperInvariant();
    private static string DigitsOnly(string value) => new(value.Where(char.IsDigit).ToArray());
    private static string? Optional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static SupplierResult<SupplierSummary> NotFound() =>
        SupplierResult<SupplierSummary>.Fail(
            SupplierFailure.NotFound,
            "Fornecedor não encontrado.");
}
