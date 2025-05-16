namespace WebApplication1.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Price cannot be negative")]
        public int? Price { get; set; }

        public int? Vat { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }

        
        [Range(0, 100, ErrorMessage = "Discount percent must be between 0 and 100")]
        public int DiscountPercent { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; } = 0;
        
        [NotMapped]
        public decimal GrossPriceDecimal => (Price ?? 0) / 100m;
        
        [NotMapped]
        public decimal NetPriceDecimal
            => Vat.HasValue
                ? GrossPriceDecimal / (1 + (Vat.Value / 100m))
                : GrossPriceDecimal;
        [NotMapped]
        public decimal VatAmountDecimal => GrossPriceDecimal - NetPriceDecimal;
        
        [NotMapped]
        public decimal DiscountAmountDecimal
            => Math.Round(GrossPriceDecimal * DiscountPercent / 100m, 2);
        
        [NotMapped]
        public decimal DiscountedGrossPriceDecimal
            => Math.Max(0, GrossPriceDecimal - DiscountAmountDecimal);
        
        [NotMapped]
        public decimal DiscountedNetPriceDecimal
            => Vat.HasValue
                ? DiscountedGrossPriceDecimal / (1 + (Vat.Value / 100m))
                : DiscountedGrossPriceDecimal;
        [NotMapped]
        public decimal DiscountedVatAmountDecimal
            => DiscountedGrossPriceDecimal - DiscountedNetPriceDecimal;
        
        [NotMapped]
        public string GrossPriceCurrency => GrossPriceDecimal.ToString("C");
        [NotMapped]
        public string NetPriceCurrency => NetPriceDecimal.ToString("C");
        [NotMapped]
        public string VatAmountCurrency => VatAmountDecimal.ToString("C");
        [NotMapped]
        public string DiscountAmountCurrency => DiscountAmountDecimal.ToString("C");
        [NotMapped]
        public string DiscountedGrossPriceCurrency => DiscountedGrossPriceDecimal.ToString("C");
        [NotMapped]
        public string DiscountedNetPriceCurrency => DiscountedNetPriceDecimal.ToString("C");
        [NotMapped]
        public string DiscountedVatAmountCurrency => DiscountedVatAmountDecimal.ToString("C");

        [NotMapped]
        public bool InStock => Stock > 0;
    }
}
