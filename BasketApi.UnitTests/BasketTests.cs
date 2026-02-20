using BasketApi.Domain.Entities;
using Xunit;

namespace BasketApi.UnitTests;

public class BasketTests
{
    [Fact]
    public void Merge_ShouldCombineItems_WhenBasketsHaveDifferentItems()
    {
        // Arrange
        var basket1 = new Basket("user1");
        basket1.AddItem("prod1", "Product 1", 100, 1);

        var basket2 = new Basket("user2");
        basket2.AddItem("prod2", "Product 2", 200, 2);

        // Act
        basket1.Merge(basket2);

        // Assert
        Assert.Equal(2, basket1.Items.Count);
        Assert.Equal(500, basket1.TotalPrice);
    }

    [Fact]
    public void Merge_ShouldUpdateQuantities_WhenBasketsHaveSameItems()
    {
        // Arrange
        var basket1 = new Basket("user1");
        basket1.AddItem("prod1", "Product 1", 100, 1);

        var basket2 = new Basket("user2");
        basket2.AddItem("prod1", "Product 1", 100, 2);

        // Act
        basket1.Merge(basket2);

        // Assert
        Assert.Single(basket1.Items);
        Assert.Equal(3, basket1.Items[0].Quantity);
        Assert.Equal(300, basket1.TotalPrice);
    }
}
