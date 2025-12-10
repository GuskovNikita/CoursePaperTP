namespace RestaurantSystem.Models
{
    public class OrderTrackingViewModel
    {
        public int BookingId { get; set; }
        public string? BookingToken { get; set; }
        public string CustomerInfo { get; set; } = string.Empty;
        public string TableInfo { get; set; } = string.Empty;
        public DateTime? OrderTime { get; set; }
        public bool IsClosed { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new();
        public bool CanBeClosed { get; set; }
    }

    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public float Quantity { get; set; }
        public float Price { get; set; }
        public int Status { get; set; }
        public string StatusText
        {
            get
            {
                return Status switch
                {
                    1 => "Передан",
                    2 => "Готов",
                    _ => "Неизвестно"
                };
            }
        }
        public string? Notes { get; set; }
        public bool RequiresCooking { get; set; }
    }
}