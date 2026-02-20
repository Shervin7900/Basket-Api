namespace BasketApi.Domain.Interfaces;

using BasketApi.Domain.Entities;

public interface IBasketRepository
{
    Task<Basket?> GetBasketAsync(string buyerId);
    Task<Basket?> UpdateBasketAsync(Basket basket);
    Task<bool> DeleteBasketAsync(string buyerId);
}
