namespace BasketApi.Features.Basket.Get;

using FastEndpoints;
using BasketApi.Domain.Entities;
using BasketApi.Domain.Interfaces;
using BasketApi.Features.Basket.Models;
using Microsoft.AspNetCore.Http;

public class Request
{
    public string BuyerId { get; set; } = string.Empty;
    public string? AnonymousId { get; set; }
}

public class GetBasketEndpoint : FastEndpoints.Endpoint<Request, BasketResponse>
{
    private readonly IBasketRepository _repository;

    public GetBasketEndpoint(IBasketRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/basket/{BuyerId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var basket = await _repository.GetBasketAsync(req.BuyerId);

        if (!string.IsNullOrEmpty(req.AnonymousId) && req.BuyerId != req.AnonymousId)
        {
            var anonymousBasket = await _repository.GetBasketAsync(req.AnonymousId);
            if (anonymousBasket != null)
            {
                basket ??= new Basket(req.BuyerId);
                basket.Merge(anonymousBasket);
                await _repository.UpdateBasketAsync(basket);
                await _repository.DeleteBasketAsync(req.AnonymousId);
            }
        }

        if (basket == null)
        {
            await this.HttpContext.Response.SendAsync(new BasketResponse { BuyerId = req.BuyerId }, 200, null, ct);
            return;
        }

        var response = new BasketResponse
        {
            BuyerId = basket.BuyerId,
            Items = basket.Items.Select(i => new BasketItemResponse
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity,
                TotalPrice = i.TotalPrice
            }).ToList(),
            TotalPrice = basket.TotalPrice
        };

        await this.HttpContext.Response.SendAsync(response, 200, null, ct);
    }
}
