using System;
using System.Collections.Generic;

namespace RestaurantSystem.Models;

public partial class Menu
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Title { get; set; }

    public string? Summary { get; set; }

    public short? Type { get; set; }

    public string? Content { get; set; }

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    public virtual User? User { get; set; }
}
