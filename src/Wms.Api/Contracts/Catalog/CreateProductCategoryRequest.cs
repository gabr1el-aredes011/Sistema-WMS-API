using System.ComponentModel.DataAnnotations;

namespace Wms.Api.Contracts.Catalog;

public sealed class CreateProductCategoryRequest
{
    [Required]
    [MaxLength(120)]
    public string Name { get; init; } = string.Empty;
}
