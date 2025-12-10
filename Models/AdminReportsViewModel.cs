namespace RestaurantSystem.Models
{
    public class AdminReportsViewModel
    {
        public List<OrderReport> Orders { get; set; } = new();
        public List<BookingReport> Bookings { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalBookings { get; set; }
        public int ActiveBookings { get; set; }
        public DateTime ReportDate { get; set; } = DateTime.Now;
    }

    public class OrderReport
    {
        public int BookingId { get; set; }
        public string? BookingToken { get; set; }
        public string CustomerInfo { get; set; } = string.Empty;
        public string TableInfo { get; set; } = string.Empty;
        public DateTime? OrderDate { get; set; }
        public TimeSpan? OrderTime { get; set; }
        public bool IsClosed { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }
        public List<OrderItemReport> Items { get; set; } = new();
    }

    public class OrderItemReport
    {
        public string ItemName { get; set; } = string.Empty;
        public float Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public int Status { get; set; }
        public string StatusText => Status switch
        {
            1 => "Передан",
            2 => "Готов",
            3 => "Закрыт",
            _ => "Неизвестно"
        };
    }

    public class BookingReport
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string CustomerInfo { get; set; } = string.Empty;
        public string TableInfo { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public TimeSpan BookingTime { get; set; }
        public int GuestsCount { get; set; }
        public int Status { get; set; }
        public string StatusText => Status switch
        {
            1 => "Активна",
            2 => "Завершена",
            _ => "Неизвестно"
        };
        public DateTime CreatedAt { get; set; }
        public decimal? OrderTotal { get; set; }
    }
}