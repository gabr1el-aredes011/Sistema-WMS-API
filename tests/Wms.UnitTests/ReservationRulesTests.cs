using Wms.Domain.Operations;

namespace Wms.UnitTests;

public sealed class ReservationRulesTests
{
    [Theory]
    [InlineData(6, 10, 6)]
    [InlineData(10, 4, 4)]
    [InlineData(0, 4, 0)]
    [InlineData(-2, 4, 0)]
    public void ReservationNeverExceedsAvailableOrMissing(decimal available, decimal missing, decimal expected) =>
        Assert.Equal(expected, ReservationRules.Allocate(available, missing));

    [Fact]
    public void CustomAndCatalogItemsMustBothBeComplete()
    {
        var lines = new[] { new OrderLine { Quantity = 10, Reserved = 6 }, new OrderLine { IsCustom = true, Quantity = 1 } };
        Assert.False(ReservationRules.IsComplete(lines));
        lines[0].Reserved = 10;
        Assert.False(ReservationRules.IsComplete(lines));
        lines[1].CustomCompleted = true;
        Assert.True(ReservationRules.IsComplete(lines));
    }

    [Theory]
    [InlineData("Preparing", "Dispatched")]
    [InlineData("Separating", "Dispatched")]
    [InlineData("Cancelled", "Ready")]
    [InlineData("Dispatched", "Preparing")]
    public void CannotSkipConferenceOrReopenClosedOrder(string from, string to) => Assert.False(ReservationRules.CanTransition(from, to));
}
