namespace BasketApi.Features.Basket.Delete;

using FastEndpoints;
using BasketApi.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

public class Request
{
    public required string BuyerId { get; set; }
    public string? ProductId { get; set; }
}

public class DeleteBasketEndpoint : FastEndpoints.Endpoint<Request>
{
    private readonly IBasketRepository _repository;

    public DeleteBasketEndpoint(IBasketRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Delete("/api/basket/{BuyerId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(req.ProductId))
        {
            await _repository.DeleteBasketAsync(req.BuyerId);
        }
        else
        {
            var basket = await _repository.GetBasketAsync(req.BuyerId);
            if (basket != null)
            {
                basket.RemoveItem(req.ProductId);
                await _repository.UpdateBasketAsync(basket);
            }
        }

        await this.HttpContext.Response.SendNoContentAsync(ct);
    }
}
