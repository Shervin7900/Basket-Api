namespace BasketApi.Features.Basket.Update;

public class Request
{
    public required string BuyerId { get; set; }
    public required string ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal? Price { get; set; }
    public int Quantity { get; set; } = 1;
    public string Action { get; set; } = "Add"; // Add, Increase, Decrease
}
