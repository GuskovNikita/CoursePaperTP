using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Data;
using RestaurantSystem.Models;

namespace RestaurantSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly RestaurantDbContext _context;

        public AdminController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Reports
        public async Task<IActionResult> Reports()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetInt32("IsAdmin") == 1;

            if (userId == null || !isAdmin)
            {
                TempData["Error"] = "Доступ только для администратора";
                return RedirectToAction("Index", "Home");
            }

            var model = await GetReportsViewModel();
            return View(model);
        }

        private async Task<AdminReportsViewModel> GetReportsViewModel()
        {
            var model = new AdminReportsViewModel();

            var bookingItems = await _context.BookingItems
                .Include(bi => bi.Item)
                .Where(bi => bi.BookingId != null)
                .ToListAsync();

            var bookingIds = bookingItems.Select(bi => bi.BookingId).Distinct().ToList();
            var bookings = await _context.Bookings
                .Include(b => b.Table)
                .Where(b => bookingIds.Contains(b.Id))
                .ToDictionaryAsync(b => b.Id, b => b);

            var ordersByBooking = bookingItems
                .GroupBy(bi => bi.BookingId)
                .ToList();

            foreach (var group in ordersByBooking)
            {
                var bookingId = group.Key.Value;

                if (!bookings.TryGetValue(bookingId, out var booking))
                    continue;

                bool isRealBooking = booking?.Token != null && !booking.Token.StartsWith("NOBOOK");

                var orderReport = new OrderReport
                {
                    BookingId = bookingId,
                    BookingToken = booking?.Token,
                    CustomerInfo = booking != null
                        ? $"{booking.FirstName} {booking.LastName}"
                        : "Гость",
                    TableInfo = booking?.Table != null
                        ? $"Стол {booking.Table.Code}"
                        : string.Empty,
                    OrderDate = booking?.BookingDate,
                    OrderTime = booking?.BookingTime,
                    IsClosed = group.All(bi => bi.Status == 3),
                    ItemsCount = group.Count(),
                    Items = group.Select(bi => new OrderItemReport
                    {
                        ItemName = bi.Item?.Title ?? "Неизвестное блюдо",
                        Quantity = (float)bi.Quantity,
                        Price = (decimal)bi.Price,
                        Discount = (decimal?)bi.Discount,
                        Status = (int)bi.Status
                    }).ToList()
                };

                orderReport.TotalAmount = group.Sum(bi => (decimal)bi.Price);
                model.Orders.Add(orderReport);
                model.TotalOrders++;
                model.TotalRevenue += orderReport.TotalAmount;
            }

            var allBookings = await _context.Bookings
                .Include(b => b.Table)
                .Where(b => !b.Token.StartsWith("NOBOOK"))
                .ToListAsync();

            var allBookingIds = allBookings.Select(b => b.Id).ToList();
            var allItemsDict = await _context.BookingItems
                .Where(bi => allBookingIds.Contains((int)bi.BookingId))
                .GroupBy(bi => bi.BookingId)
                .ToDictionaryAsync(g => g.Key, g => g.ToList());

            foreach (var booking in allBookings)
            {
                var bookingItemsForThisBooking = allItemsDict.TryGetValue(booking.Id, out var items)
                    ? items
                    : new List<BookingItem>();

                var bookingReport = new BookingReport
                {
                    Id = booking.Id,
                    Token = booking.Token,
                    CustomerInfo = $"{booking.FirstName} {booking.LastName}",
                    TableInfo = booking.Table != null ? $"Стол {booking.Table.Code}" : "Без столика",
                    ContactInfo = $"{booking.Email} | {booking.Mobile}",
                    BookingDate = booking.BookingDate ?? DateTime.MinValue,
                    BookingTime = booking.BookingTime ?? TimeSpan.Zero,
                    GuestsCount = booking.GuestsCount ?? 1,
                    Status = (int)booking.Status,
                    CreatedAt = booking.BookingDate ?? DateTime.MinValue,
                    OrderTotal = bookingItemsForThisBooking.Any()
                        ? (decimal?)bookingItemsForThisBooking.Sum(bi => bi.Price)
                        : null
                };

                model.Bookings.Add(bookingReport);
                model.TotalBookings++;

                if (booking.Status == 1)
                {
                    model.ActiveBookings++;
                }
            }

            return model;
        }
    }
}