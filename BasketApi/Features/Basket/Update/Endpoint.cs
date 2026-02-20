namespace BasketApi.Features.Basket.Update;

using FastEndpoints;
using BasketApi.Domain.Entities;
using BasketApi.Domain.Interfaces;
using BasketApi.Features.Basket.Models;

public class Endpoint : Endpoint<Request, BasketResponse>
{
    private readonly IBasketRepository _repository;

    public Endpoint(IBasketRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/basket/update");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var basket = await _repository.GetBasketAsync(req.BuyerId);
        if (basket == null)
        {
            basket = new Basket(req.BuyerId);
        }

        switch (req.Action.ToLower())
        {
            case "add":
                if (string.IsNullOrEmpty(req.ProductName) || req.Price == null)
                {
                    await SendErrorsAsync(400, ct);
                    return;
                }
                basket.AddItem(req.ProductId, req.ProductName, req.Price.Value, req.Quantity);
                break;
            case "increase":
                basket.IncreaseQuantity(req.ProductId, req.Quantity);
                break;
            case "decrease":
                basket.DecreaseQuantity(req.ProductId, req.Quantity);
                break;
            default:
                await SendErrorsAsync(400, ct);
                return;
        }

        await _repository.UpdateBasketAsync(basket);

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

        await SendAsync(response, cancellation: ct);
    }
}
