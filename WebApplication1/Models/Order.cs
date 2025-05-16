// Order.cs
namespace WebApplication1.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Collections.Generic;

    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;

        public string Status { get; set; } = "Awaiting Payment";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // ← add this navigation
        public virtual Invoice? Invoice { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
        
        [NotMapped]
        public decimal TotalGrossPriceDecimal
            => OrderItems.Sum(oi => oi.TotalGrossDecimal);
        [NotMapped]
        public decimal TotalNetPriceDecimal
            => OrderItems.Sum(oi => oi.TotalNetDecimal);
        [NotMapped]
        public decimal TotalVatAmountDecimal
            => OrderItems.Sum(oi => oi.TotalVatAmountDecimal);

        [NotMapped]
        public string TotalGrossPriceCurrency 
            => TotalGrossPriceDecimal.ToString("C");
        [NotMapped]
        public string TotalNetPriceCurrency 
            => TotalNetPriceDecimal.ToString("C");
        [NotMapped]
        public string TotalVatAmountCurrency 
            => TotalVatAmountDecimal.ToString("C");

    }
}
