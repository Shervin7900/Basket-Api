using BasketApi.Domain.Interfaces;
using FastEndpoints;
using Google.FlatBuffers;
using BasketApi.Infrastructure.FlatBuffers;
using Microsoft.AspNetCore.Http;

namespace BasketApi.Features.Basket.Get;

public class FlatBufferRequest
{
    public string? UserId { get; set; }
    public string? CookieId { get; set; }
}

public class GetBasketFlatBufferEndpoint : Endpoint<FlatBufferRequest>
{
    private readonly IBasketRepository _repository;

    public GetBasketFlatBufferEndpoint(IBasketRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/basket/fb");
        AllowAnonymous();
    }

    public override async Task HandleAsync(FlatBufferRequest req, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(req.UserId) && string.IsNullOrEmpty(req.CookieId))
        {
            await this.HttpContext.Response.SendAsync(new { error = "Ids missing" }, 400, null, ct);
            return;
        }

        var basket = await GetOrMergeBasket(req.UserId, req.CookieId);

        var builder = new FlatBufferBuilder(1024);

        // Serialize BuyerId
        var buyerIdOffset = builder.CreateString(basket.BuyerId ?? "");

        // Serialize Items
        var itemOffsets = new Offset<BasketItemFB>[basket.Items.Count];
        for (int i = 0; i < basket.Items.Count; i++)
        {
            var item = basket.Items[i];
            var pId = builder.CreateString(item.ProductId ?? "");
            var pName = builder.CreateString(item.ProductName ?? "");
            
            BasketItemFB.StartBasketItemFB(builder);
            BasketItemFB.AddProductId(builder, pId);
            BasketItemFB.AddProductName(builder, pName);
            BasketItemFB.AddPrice(builder, (double)item.Price);
            BasketItemFB.AddQuantity(builder, item.Quantity);
            itemOffsets[i] = BasketItemFB.EndBasketItemFB(builder);
        }

        var itemsVector = BasketFB.CreateItemsVector(builder, itemOffsets);

        // Create Basket
        BasketFB.StartBasketFB(builder);
        BasketFB.AddBuyerId(builder, buyerIdOffset);
        BasketFB.AddItems(builder, itemsVector);
        BasketFB.AddTotalPrice(builder, (double)basket.TotalPrice);
        var basketOffset = BasketFB.EndBasketFB(builder);

        builder.Finish(basketOffset.Value);

        var finalBytes = builder.SizedByteArray();
        
        this.HttpContext.Response.ContentType = "application/x-flatbuffers";
        await this.HttpContext.Response.Body.WriteAsync(finalBytes, 0, finalBytes.Length, ct);
    }

    private async Task<Domain.Entities.Basket> GetOrMergeBasket(string? userId, string? cookieId)
    {
        if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(cookieId) && userId != cookieId)
        {
            var userBasket = await _repository.GetBasketAsync(userId) ?? new Domain.Entities.Basket(userId);
            var cookieBasket = await _repository.GetBasketAsync(cookieId);

            if (cookieBasket != null)
            {
                userBasket.Merge(cookieBasket);
                await _repository.UpdateBasketAsync(userBasket);
                await _repository.DeleteBasketAsync(cookieId);
            }
            return userBasket;
        }

        var id = userId ?? cookieId!;
        return await _repository.GetBasketAsync(id) ?? new Domain.Entities.Basket(id);
    }
}
