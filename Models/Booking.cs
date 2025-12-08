using System;
using System.Collections.Generic;

namespace RestaurantSystem.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int? TableId { get; set; }

    public int? UserId { get; set; }

    public string? Token { get; set; }

    public short? Status { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public string? Mobile { get; set; }

    public string? Email { get; set; }

    public string? Content { get; set; }

    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

    public virtual TableTop? Table { get; set; }

    public virtual User? User { get; set; }
}
