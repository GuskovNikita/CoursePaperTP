using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    [Column("booking_date")]
    public DateTime? BookingDate { get; set; }

    [Column("booking_time")]
    public TimeSpan? BookingTime { get; set; }

    [Column("guests_count")]
    public short? GuestsCount { get; set; }

    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

    public virtual TableTop? Table { get; set; }

    public virtual User? User { get; set; }
}
