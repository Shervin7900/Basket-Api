namespace BasketApi.Domain.Entities;

public class BasketItem
{
    public required string ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public decimal TotalPrice => Price * Quantity;
}
