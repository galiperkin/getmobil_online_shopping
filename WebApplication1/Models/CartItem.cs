using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int Quantity { get; set; }
    
    
    [ForeignKey(nameof(CartId))]
    public virtual Cart? Cart { get; set; }
    
    [ForeignKey(nameof(ProductId))]
    public virtual Product? Product { get; set; }


    
    
}