using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniECommerce.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Precision(10, 2)]
        public decimal UnitPriceAtPurchase { get; set; } // Price of each product at time of placing the order

        public int Quantity { get; set; }  // how much of same product got purchased
        
        [Precision(10, 2)]
        public decimal LineTotal { get; set; } // for each orderitem (Price of orderitem * Its Quantity)

        // Nav Properties
        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; } 
        public Order? Order { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

    }
}
