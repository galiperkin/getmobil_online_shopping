namespace WebApplication1.Models;

public class Cart
{
    public int Id { get; set; }
    public string UserId { get; set; }
    
    public virtual ICollection<CartItem> CartItems { get; set; }
        = new List<CartItem>();
    
    public void AddCartItem(int productId, int quantity = 1)
    {
        var existing = CartItems.SingleOrDefault(ci => ci.ProductId == productId);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            CartItems.Add(new CartItem
            {
                ProductId = productId,
                Quantity  = quantity
            });
        }
    }
    
}