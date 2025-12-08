using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Models;

namespace RestaurantSystem.Data;

public partial class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }
    public virtual DbSet<BookingItem> BookingItems { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<NonUpdatableStat> NonUpdatableStats { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<TableTop> TableTops { get; set; }

    public virtual DbSet<UpdatableMenuView> UpdatableMenuViews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("booking_pkey");

            entity.ToTable("booking");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("firstName");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("lastName");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middleName");
            entity.Property(e => e.Mobile)
                .HasMaxLength(15)
                .HasColumnName("mobile");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TableId).HasColumnName("tableId");
            entity.Property(e => e.Token)
                .HasMaxLength(100)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Table).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TableId)
                .HasConstraintName("tableId");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("userId");
        });

        modelBuilder.Entity<BookingItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bookingItem_pkey");

            entity.ToTable("booking_item");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.ItemId).HasColumnName("itemId");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("sku");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Unit).HasColumnName("unit");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingItems)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("bookingId");

            entity.HasOne(d => d.Item).WithMany(p => p.BookingItems)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("itemId");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ingredient_pkey");

            entity.ToTable("ingredient");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("sku");
            entity.Property(e => e.Summary).HasColumnName("summary");
            entity.Property(e => e.Title)
                .HasMaxLength(75)
                .HasColumnName("title");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Unit).HasColumnName("unit");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.VendorId).HasColumnName("vendorId");

            entity.HasOne(d => d.User).WithMany(p => p.IngredientUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("userId");

            entity.HasOne(d => d.Vendor).WithMany(p => p.IngredientVendors)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("vendorId");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_pkey");

            entity.ToTable("item");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Cooking).HasColumnName("cooking");
            entity.Property(e => e.Instructions).HasColumnName("instructions");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Recipe).HasColumnName("recipe");
            entity.Property(e => e.Sku)
                .HasColumnType("character varying")
                .HasColumnName("sku");
            entity.Property(e => e.Summary).HasColumnName("summary");
            entity.Property(e => e.Title)
                .HasMaxLength(75)
                .HasColumnName("title");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Unit).HasColumnName("unit");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.VendorId).HasColumnName("vendorId");

            entity.HasOne(d => d.User).WithMany(p => p.ItemUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("userId");

            entity.HasOne(d => d.Vendor).WithMany(p => p.ItemVendors)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("vendorId");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("menu_pkey");

            entity.ToTable("menu");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Summary).HasColumnName("summary");
            entity.Property(e => e.Title)
                .HasMaxLength(75)
                .HasColumnName("title");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Menus)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("userId");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("menuItem_pkey");

            entity.ToTable("menu_item");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.ItemId).HasColumnName("itemId");
            entity.Property(e => e.MenuId).HasColumnName("menuId");

            entity.HasOne(d => d.Item).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("itemId");

            entity.HasOne(d => d.Menu).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.MenuId)
                .HasConstraintName("menuId");
        });

        modelBuilder.Entity<NonUpdatableStat>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("non_updatable_stats");

            entity.Property(e => e.AvgPrice).HasColumnName("avg_price");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ItemsCount).HasColumnName("items_count");
            entity.Property(e => e.Title)
                .HasMaxLength(75)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("recipe_pkey");

            entity.ToTable("recipe");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredientId");
            entity.Property(e => e.Instructions).HasColumnName("instructions");
            entity.Property(e => e.ItemId).HasColumnName("itemId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Unit).HasColumnName("unit");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.IngredientId)
                .HasConstraintName("ingredientId");

            entity.HasOne(d => d.Item).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("itemId");
        });

        modelBuilder.Entity<TableTop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tableTop_pkey");

            entity.ToTable("table_top");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .HasColumnName("code");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<UpdatableMenuView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("updatable_menu_view");

            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title)
                .HasMaxLength(75)
                .HasColumnName("title");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("user");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Admin)
                .HasDefaultValue(false)
                .HasColumnName("admin");
            entity.Property(e => e.Agent)
                .HasDefaultValue(false)
                .HasColumnName("agent");
            entity.Property(e => e.Chef)
                .HasDefaultValue(false)
                .HasColumnName("chef");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("firstName");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("lastName");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middleName");
            entity.Property(e => e.Mobile)
                .HasMaxLength(15)
                .HasColumnName("mobile");
            entity.Property(e => e.Password)
                .HasMaxLength(32)
                .HasColumnName("password");
            entity.Property(e => e.Vendor)
                .HasDefaultValue(false)
                .HasColumnName("vendor");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
