namespace BasketApi.Domain.Entities;

public class Basket
{
    public string BuyerId { get; set; }
    public List<BasketItem> Items { get; private set; } = new();

    public Basket(string buyerId)
    {
        BuyerId = buyerId;
    }

    public void AddItem(string productId, string productName, decimal price, int quantity = 1)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            Items.Add(new BasketItem
            {
                ProductId = productId,
                ProductName = productName,
                Price = price,
                Quantity = quantity
            });
        }
    }

    public void IncreaseQuantity(string productId, int quantity = 1)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            item.Quantity += quantity;
        }
    }

    public void DecreaseQuantity(string productId, int quantity = 1)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            item.Quantity -= quantity;
            if (item.Quantity <= 0)
            {
                Items.Remove(item);
            }
        }
    }

    public void RemoveItem(string productId)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            Items.Remove(item);
        }
    }

    public void Merge(Basket other)
    {
        if (other == null || other.BuyerId == BuyerId) return;

        foreach (var item in other.Items)
        {
            AddItem(item.ProductId, item.ProductName, item.Price, item.Quantity);
        }
    }

    public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
}
