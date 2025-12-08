using System;
using System.Collections.Generic;

namespace RestaurantSystem.Models;

public partial class Recipe
{
    public int Id { get; set; }

    public int? ItemId { get; set; }

    public int? IngredientId { get; set; }

    public float? Quantity { get; set; }

    public short? Unit { get; set; }

    public string? Instructions { get; set; }

    public virtual Ingredient? Ingredient { get; set; }

    public virtual Item? Item { get; set; }
}
