namespace Wms.Domain.Catalog;

public static class CatalogItemTypes
{
    public const string RawMaterial = "RawMaterial";
    public const string SemiFinished = "SemiFinished";
    public const string Component = "Component";
    public const string FinishedProduct = "FinishedProduct";
    public const string Kit = "Kit";
    public const string Packaging = "Packaging";

    public static IReadOnlyCollection<string> All { get; } =
    [
        RawMaterial,
        SemiFinished,
        Component,
        FinishedProduct,
        Kit,
        Packaging
    ];
}
