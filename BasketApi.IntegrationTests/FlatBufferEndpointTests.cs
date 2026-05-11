extern alias BasketApiApp;
using FastEndpoints;
using FastEndpoints.Testing;
using Google.FlatBuffers;
using Moq;
using Xunit;

namespace BasketApi.IntegrationTests;

public class FlatBufferEndpointTests(BasketApiFixture f) : TestBase<BasketApiFixture>
{
    [Fact]
    public async Task GetBasketFB_ByUserId_ReturnsCorrectData()
    {
        // Arrange
        var userId = "user123";
        var basket = new BasketApiApp::BasketApi.Domain.Entities.Basket(userId);
        basket.AddItem("prod1", "Product 1", 10.5m, 2);
        
        f.BasketRepositoryMock
            .Setup(r => r.GetBasketAsync(userId))
            .ReturnsAsync(basket);

        // Act
        var response = await f.Client.GETAsync<BasketApiApp::BasketApi.Features.Basket.Get.GetBasketFlatBufferEndpoint, BasketApiApp::BasketApi.Features.Basket.Get.FlatBufferRequest>(new BasketApiApp::BasketApi.Features.Basket.Get.FlatBufferRequest
        {
            UserId = userId
        });

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/x-flatbuffers", response.Content.Headers.ContentType?.MediaType);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        var bb = new ByteBuffer(bytes);
        var basketFB = BasketApiApp::BasketApi.Infrastructure.FlatBuffers.BasketFB.GetRootAsBasketFB(bb);

        Assert.Equal(userId, basketFB.BuyerId);
        Assert.Equal(1, basketFB.ItemsLength);
        
        var item = basketFB.Items(0)!.Value;
        Assert.Equal("prod1", item.ProductId);
        Assert.Equal("Product 1", item.ProductName);
        Assert.Equal(10.5, item.Price);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(21.0, basketFB.TotalPrice);
    }

    [Fact]
    public async Task GetBasketFB_ByCookieId_ReturnsCorrectData()
    {
        // Arrange
        var cookieId = "cookie789";
        var basket = new BasketApiApp::BasketApi.Domain.Entities.Basket(cookieId);
        basket.AddItem("prod2", "Product 2", 50.0m, 1);
        
        f.BasketRepositoryMock
            .Setup(r => r.GetBasketAsync(cookieId))
            .ReturnsAsync(basket);

        // Act
        var response = await f.Client.GETAsync<BasketApiApp::BasketApi.Features.Basket.Get.GetBasketFlatBufferEndpoint, BasketApiApp::BasketApi.Features.Basket.Get.FlatBufferRequest>(new BasketApiApp::BasketApi.Features.Basket.Get.FlatBufferRequest
        {
            CookieId = cookieId
        });

        // Assert
        response.EnsureSuccessStatusCode();
        var bytes = await response.Content.ReadAsByteArrayAsync();
        var bb = new ByteBuffer(bytes);
        var basketFB = BasketApiApp::BasketApi.Infrastructure.FlatBuffers.BasketFB.GetRootAsBasketFB(bb);

        Assert.Equal(cookieId, basketFB.BuyerId);
        Assert.Equal(50.0, basketFB.TotalPrice);
    }

    [Fact]
    public async Task GetBasketFB_WithBothIds_MergesBaskets()
    {
        // Arrange
        var userId = "user-merged";
        var cookieId = "cookie-merged";
        
        var userBasket = new BasketApiApp::BasketApi.Domain.Entities.Basket(userId);
        userBasket.AddItem("p1", "Item 1", 10, 1);
        
        var cookieBasket = new BasketApiApp::BasketApi.Domain.Entities.Basket(cookieId);
        cookieBasket.AddItem("p2", "Item 2", 20, 2);
        
        f.BasketRepositoryMock
            .Setup(r => r.GetBasketAsync(userId))
            .ReturnsAsync(userBasket);
        
        f.BasketRepositoryMock
            .Setup(r => r.GetBasketAsync(cookieId))
            .ReturnsAsync(cookieBasket);

        // Act
        var response = await f.Client.GETAsync<BasketApiApp::BasketApi.Features.Basket.Get.GetBasketFlatBufferEndpoint, BasketApiApp::BasketApi.Features.Basket.Get.FlatBufferRequest>(new BasketApiApp::BasketApi.Features.Basket.Get.FlatBufferRequest
        {
            UserId = userId,
            CookieId = cookieId
        });

        // Assert
        response.EnsureSuccessStatusCode();
        
        // Verify merge was called
        f.BasketRepositoryMock.Verify(r => r.UpdateBasketAsync(It.Is<BasketApiApp::BasketApi.Domain.Entities.Basket>(b => b.BuyerId == userId && b.Items.Count == 2)), Times.Once);
        f.BasketRepositoryMock.Verify(r => r.DeleteBasketAsync(cookieId), Times.Once);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        var bb = new ByteBuffer(bytes);
        var basketFB = BasketApiApp::BasketApi.Infrastructure.FlatBuffers.BasketFB.GetRootAsBasketFB(bb);

        Assert.Equal(userId, basketFB.BuyerId);
        Assert.Equal(2, basketFB.ItemsLength);
        Assert.Equal(50.0, basketFB.TotalPrice);
    }

    [Fact]
    public async Task GetBasketFB_NoIdsProvided_ReturnsBadRequest()
    {
        // Act
        var response = await f.Client.GETAsync<BasketApiApp::BasketApi.Features.Basket.Get.GetBasketFlatBufferEndpoint, BasketApiApp::BasketApi.Features.Basket.Get.FlatBufferRequest>(new BasketApiApp::BasketApi.Features.Basket.Get.FlatBufferRequest());

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
