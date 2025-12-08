using System;
using System.Collections.Generic;

namespace RestaurantSystem.Models;

public partial class UpdatableMenuView
{
    public int? Id { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }
}
