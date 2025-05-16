using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class Invoice
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    
    [ForeignKey(nameof(OrderId))]
    public virtual Order? Order { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; }
        = new List<InvoiceItem>();
    
    
}