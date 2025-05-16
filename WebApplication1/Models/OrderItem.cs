namespace WebApplication1.Models;

using System.ComponentModel.DataAnnotations.Schema;
public class OrderItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? Price { get; set; }
    public int? Vat { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    
    public int Quantity { get; set; }
    
    public int OrderId { get; set; }
    
    [ForeignKey(nameof(OrderId))]
    public virtual Order? Order { get; set; }
    
    
    [NotMapped]
    public decimal UnitGrossDecimal 
        => (Price ?? 0) / 100m;

    [NotMapped]
    public decimal UnitNetDecimal
        => Vat.HasValue
            ? UnitGrossDecimal / (1 + Vat.Value / 100m)
            : UnitGrossDecimal;

    [NotMapped]
    public decimal UnitVatAmountDecimal
        => UnitGrossDecimal - UnitNetDecimal;
    
    [NotMapped]
    public decimal TotalGrossDecimal    // was LineGrossDecimal
        => UnitGrossDecimal * Quantity;

    [NotMapped]
    public decimal TotalNetDecimal      // was LineNetDecimal
        => UnitNetDecimal * Quantity;

    [NotMapped]
    public decimal TotalVatAmountDecimal  // was LineVatAmountDecimal
        => UnitVatAmountDecimal * Quantity;

    [NotMapped]
    public string UnitGrossCurrency
        => UnitGrossDecimal.ToString("C");

    [NotMapped]
    public string TotalGrossCurrency
        => TotalGrossDecimal.ToString("C");

    [NotMapped]
    public string TotalNetCurrency
        => TotalNetDecimal.ToString("C");

    [NotMapped]
    public string TotalVatAmountCurrency
        => TotalVatAmountDecimal.ToString("C");

    
}