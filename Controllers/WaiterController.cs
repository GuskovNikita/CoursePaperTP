using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Data;
using RestaurantSystem.Models;
using System.Diagnostics;

namespace RestaurantSystem.Controllers
{
    public class WaiterController : Controller
    {
        private readonly RestaurantDbContext _context;

        public WaiterController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: /Waiter/CreateOrder?token=123456
        public async Task<IActionResult> CreateOrder(string token = null)
        {
            int? bookingId = null;

            if (!string.IsNullOrEmpty(token))
            {
                var booking = await _context.Bookings
                    .Include(b => b.Table)
                    .FirstOrDefaultAsync(b => b.Token == token && b.Status == 1);

                if (booking == null)
                {
                    TempData["Error"] = "Бронь с таким токеном не найдена или не активна";
                    return RedirectToAction("Dashboard", "Account");
                }

                bookingId = booking.Id;
                ViewBag.BookingInfo = $"Бронь #{booking.Id} | Стол {booking.Table?.Code} | {booking.FirstName} {booking.LastName}";
            }
            else
            {
                ViewBag.BookingInfo = "Обычный заказ (без брони)";

                var availableTables = await _context.TableTops
                    .Where(t => t.Status == 1) 
                    .OrderBy(t => t.Code)
                    .ToListAsync();

                ViewBag.AvailableTables = availableTables;
            }

            ViewBag.BookingId = bookingId;

            var items = await _context.Items
                .OrderBy(i => i.Title)
                .ToListAsync();

            return View(items);
        }

        // POST: /Waiter/CompleteOrderWithItems
        [HttpPost]
        public async Task<IActionResult> CompleteOrderWithItems([FromBody] CompleteOrderRequest request)
        {
            try
            {
                if (request?.Items == null || !request.Items.Any())
                {
                    return Json(new { success = false, error = "Нет блюд в заказе" });
                }

                int? bookingId = request.BookingId;
                int? tableId = request.TableId; 

                if (bookingId == null || bookingId == 0)
                {
                    TableTop? table = null;
                    if (tableId.HasValue && tableId > 0)
                    {
                        table = await _context.TableTops.FindAsync(tableId.Value);
                    }

                    var tempBooking = new Booking
                    {
                        TableId = tableId,
                        UserId = null,
                        Token = "NOBOOK" + new Random().Next(10000, 99999),
                        Status = 1,
                        FirstName = request.CustomerName ?? "Гость", 
                        LastName = "Ресторана",
                        Mobile = "",
                        Email = "",
                        BookingDate = DateTime.Today,
                        BookingTime = DateTime.Now.TimeOfDay,
                        GuestsCount = (short?)(request.GuestsCount ?? 1),
                        Content = "Заказ без бронирования"
                    };

                    _context.Bookings.Add(tempBooking);
                    await _context.SaveChangesAsync();

                    bookingId = tempBooking.Id;

                    if (table != null) table.Status = 0;
                    await _context.SaveChangesAsync();
                }

                var groupedItems = request.Items
                    .Where(i => i != null)
                    .GroupBy(i => i.ItemId)
                    .Select(g => new
                    {
                        ItemId = g.Key,
                        Quantity = g.Sum(x => x.Quantity),
                        Notes = string.Join(", ", g
                            .Where(x => !string.IsNullOrEmpty(x.Notes))
                            .Select(x => x.Notes)
                            .Distinct())
                    })
                    .ToList();

                foreach (var item in groupedItems)
                {
                    var menuItem = await _context.Items.FindAsync(item.ItemId);
                    if (menuItem != null)
                    {
                        double price = menuItem.Price ?? 0;
                        double totalPrice = price * item.Quantity;

                        var bookingItem = new BookingItem
                        {
                            BookingId = bookingId,
                            ItemId = item.ItemId,
                            Quantity = (float)item.Quantity,
                            Price = (float)totalPrice,
                            Discount = 0,
                            Status = 1,
                            Content = !string.IsNullOrEmpty(item.Notes) ? item.Notes : null
                        };

                        _context.BookingItems.Add(bookingItem);
                    }
                }

                await _context.SaveChangesAsync();

                if (request.ApplyDiscount && bookingId.HasValue && bookingId > 0)
                {
                    var items = await _context.BookingItems
                        .Where(bi => bi.BookingId == bookingId && bi.Status == 1)
                        .ToListAsync();

                    foreach (var item in items)
                    {
                        item.Discount = 10;
                        item.Price = item.Price * 0.9f;
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Заказ успешно оформлен!";
                return Json(new { success = true, redirectUrl = Url.Action("Dashboard", "Account") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public class CompleteOrderRequest
        {
            public int? BookingId { get; set; }
            public bool ApplyDiscount { get; set; }
            public List<OrderItemRequest> Items { get; set; } = new();
            public int? TableId { get; set; } 
            public string? CustomerName { get; set; } 
            public int? GuestsCount { get; set; } 
        }

        public class OrderItemRequest
        {
            public int ItemId { get; set; }
            public int Quantity { get; set; }
            public string Notes { get; set; } = string.Empty;
        }
    }
}