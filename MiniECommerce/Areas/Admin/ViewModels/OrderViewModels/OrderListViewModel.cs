using MiniECommerce.Results;

namespace MiniECommerce.Areas.Admin.ViewModels.OrderViewModels
{
    public class OrderListViewModel
    {
        public List<OrderRowViewModel> Orders { get; set; } = new();
    }

    public class OrderRowViewModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }

        // Shipping address — shown in expanded row
        public string? ShippingAddress { get; set; }

        // Order items — shown in expanded row
        public List<OrderItemRowViewModel> Items { get; set; } = new();
    }

    public class OrderItemRowViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}