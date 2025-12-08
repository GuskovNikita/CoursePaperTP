using System;
using System.Collections.Generic;

namespace RestaurantSystem.Models;

public partial class MenuItem
{
    public int Id { get; set; }

    public int? MenuId { get; set; }

    public int? ItemId { get; set; }

    public bool? Active { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Menu? Menu { get; set; }
}
