namespace BasketApi.Infrastructure.Persistence;

using BasketApi.Domain.Entities;
using BasketApi.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _cache;

    public BasketRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<Basket?> GetBasketAsync(string buyerId)
    {
        var data = await _cache.GetStringAsync(buyerId);
        if (string.IsNullOrEmpty(data))
        {
            return null;
        }

        return JsonSerializer.Deserialize<Basket>(data);
    }

    public async Task<Basket?> UpdateBasketAsync(Basket basket)
    {
        await _cache.SetStringAsync(basket.BuyerId, JsonSerializer.Serialize(basket));
        return await GetBasketAsync(basket.BuyerId);
    }

    public async Task<bool> DeleteBasketAsync(string buyerId)
    {
        await _cache.RemoveAsync(buyerId);
        return true;
    }
}
