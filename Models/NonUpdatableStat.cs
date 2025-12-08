using System;
using System.Collections.Generic;

namespace RestaurantSystem.Models;

public partial class NonUpdatableStat
{
    public int? Id { get; set; }

    public string? Title { get; set; }

    public long? ItemsCount { get; set; }

    public double? AvgPrice { get; set; }
}
