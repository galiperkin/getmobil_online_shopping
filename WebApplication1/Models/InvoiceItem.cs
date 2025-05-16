using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class InvoiceItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? Price { get; set; }
    public int? Vat { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int Quantity { get; set; }
    
    public int InvoiceId { get; set; }
    
    [ForeignKey(nameof(InvoiceId))]
    public virtual Invoice? Invoice { get; set; }
    
    
    
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