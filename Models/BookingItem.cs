using System;
using System.Collections.Generic;

namespace RestaurantSystem.Models;

public partial class BookingItem
{
    public int Id { get; set; }

    public int? BookingId { get; set; }

    public int? ItemId { get; set; }

    public string? Sku { get; set; }

    public float? Price { get; set; }

    public float? Discount { get; set; }

    public float? Quantity { get; set; }

    public short? Unit { get; set; }

    public short? Status { get; set; }

    public string? Content { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Item? Item { get; set; }
}
