using Wms.Application.Catalog;

namespace Wms.UnitTests;

public sealed class PagedProductsTests
{
    [Theory]
    [InlineData(0, 20, 0)]
    [InlineData(1, 20, 1)]
    [InlineData(20, 20, 1)]
    [InlineData(21, 20, 2)]
    public void TotalPages_IsCalculatedFromTheTotalCount(
        int totalCount,
        int pageSize,
        int expectedTotalPages)
    {
        var result = new PagedProducts([], Page: 1, pageSize, totalCount);

        Assert.Equal(expectedTotalPages, result.TotalPages);
    }
}
