using System;
using System.Collections.Generic;

namespace RestaurantSystem.Models;

public partial class TableTop
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public short? Status { get; set; }

    public short? Capacity { get; set; }

    public string? Content { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
