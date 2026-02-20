namespace BasketApi.Features.Basket.Models;

public class BasketResponse
{
    public string BuyerId { get; set; } = string.Empty;
    public List<BasketItemResponse> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
}
