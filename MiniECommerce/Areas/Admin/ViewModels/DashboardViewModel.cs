using MiniECommerce.Results;

namespace MiniECommerce.Areas.Admin.ViewModels
{
    public class DashboardViewModel
    {
        // Stat Cards
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int PendingOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        // Tables
        public List<RecentOrderRow> RecentOrders { get; set; } = new();
        public List<LowStockRow> LowStockProducts { get; set; } = new();
    }

    public class RecentOrderRow
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class LowStockRow
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string? CategoryName { get; set; }
    }
}
