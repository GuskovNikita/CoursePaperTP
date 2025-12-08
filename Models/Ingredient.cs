using System;
using System.Collections.Generic;

namespace RestaurantSystem.Models;

public partial class Ingredient
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? VendorId { get; set; }

    public string? Title { get; set; }

    public string? Summary { get; set; }

    public short? Type { get; set; }

    public string? Sku { get; set; }

    public float? Quantity { get; set; }

    public short? Unit { get; set; }

    public string? Content { get; set; }

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public virtual User? User { get; set; }

    public virtual User? Vendor { get; set; }
}
