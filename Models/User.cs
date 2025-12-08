using System;
using System.Collections.Generic;

namespace RestaurantSystem.Models;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public string? Mobile { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool Admin { get; set; }

    public bool Vendor { get; set; }

    public bool Chef { get; set; }

    public bool Agent { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Ingredient> IngredientUsers { get; set; } = new List<Ingredient>();

    public virtual ICollection<Ingredient> IngredientVendors { get; set; } = new List<Ingredient>();

    public virtual ICollection<Item> ItemUsers { get; set; } = new List<Item>();

    public virtual ICollection<Item> ItemVendors { get; set; } = new List<Item>();

    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
}
