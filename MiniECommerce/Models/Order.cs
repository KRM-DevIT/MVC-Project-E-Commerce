using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MiniECommerce.Models.IdentityModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniECommerce.Models
{

    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [StringLength(20)]
        // to be unique in on modelCreating
        public string OrderNumber { get; set; } = null!;

        public OrderStatus Status { get; set; } = OrderStatus.Placed; // in EF-Core it will be stored as int we may edit this in modelcreating to store it as stirng using HasConversion<string>

        public DateTime OrderDate { get; set; }

        [Range(0,1000_000)]
        [DataType(DataType.Currency)]
        [Precision(10, 2)]
        public decimal TotalAmount { get; set; } // Sum of LineTotal of each OrderItem (LineTotal = UnitPriceAtPurshace * Quantitty)

        // Nav Properties
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        [ForeignKey(nameof(Address))]
        public int ShippingAddressId { get; set; }
        public Address? Address { get; set; }


        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser? ApplicationUser{ get; set; }
    
    }
}
