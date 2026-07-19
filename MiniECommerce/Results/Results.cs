namespace MiniECommerce.Results
{
    public enum StockStatus
    {
        InStock ,
        OutOfStock,
    }
    public enum OrderStatus 
    { 
        Placed = 0, 
        OutForDelivery = 1, 
        delivered = 2, 
        Cancelled = 3 
    }
    public enum AddToCartResult
    {
        Success,
        ProductNotFound,
        OutOfStock
    }
    public enum UpdateQuantityResult
    {
        Success,
        ProductNotFound,
        UnAvailableQuantity
    }
    public class CheckoutResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public int? OrderId { get; set; }
        public string? OrderNumber { get; set; }
    }

}
