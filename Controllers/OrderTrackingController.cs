using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Data;
using RestaurantSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantSystem.Controllers
{
    public class OrderTrackingController : Controller
    {
        private readonly RestaurantDbContext _context;

        public OrderTrackingController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: /OrderTracking
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var activeOrders = await GetActiveOrders();

            ViewBag.IsChef = HttpContext.Session.GetInt32("IsChef") == 1;
            ViewBag.IsAgent = HttpContext.Session.GetInt32("IsAgent") == 1;

            return View(activeOrders);
        }

        private async Task<List<OrderTrackingViewModel>> GetActiveOrders()
        {
            var orders = new List<OrderTrackingViewModel>();

            var bookingItems = await _context.BookingItems
                .Include(bi => bi.Booking)
                .ThenInclude(b => b.Table)
                .Include(bi => bi.Item)
                .Where(bi => bi.Status == 1 || bi.Status == 2) 
                .OrderByDescending(bi => bi.Id)
                .ToListAsync();

            var groupedOrders = bookingItems
                .GroupBy(bi => bi.BookingId)
                .ToList();

            foreach (var group in groupedOrders)
            {
                var bookingId = group.Key;
                var firstItem = group.First();
                var booking = firstItem.Booking;

                var order = new OrderTrackingViewModel
                {
                    BookingId = bookingId ?? 0,
                    BookingToken = booking?.Token,
                    CustomerInfo = booking != null
                        ? $"{booking.FirstName} {booking.LastName}"
                        : "Гость",
                    TableInfo = booking?.Table != null
                        ? $"Стол {booking.Table.Code}"
                        : string.Empty,
                    OrderTime = booking?.BookingDate,
                    Items = group.Select(bi => new OrderItemViewModel
                    {
                        Id = bi.Id,
                        ItemName = bi.Item?.Title ?? "Неизвестное блюдо",
                        Quantity = (float)bi.Quantity,
                        Price = (float)bi.Price,
                        Status = (int)bi.Status,
                        Notes = bi.Content,
                        RequiresCooking = bi.Item?.Cooking ?? false
                    }).ToList(),
                    CanBeClosed = group.All(bi => bi.Status == 2)
                };

                orders.Add(order);
            }

            return orders;
        }

        // POST: /OrderTracking/UpdateItemStatus
        [HttpPost]
        public async Task<IActionResult> UpdateItemStatus([FromBody] UpdateStatusRequest request)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var isChef = HttpContext.Session.GetInt32("IsChef") == 1;

                if (!isChef)
                {
                    return Json(new { success = false, error = "Доступ только для повара" });
                }

                var bookingItem = await _context.BookingItems
                    .Include(bi => bi.Booking)
                    .FirstOrDefaultAsync(bi => bi.Id == request.ItemId);

                if (bookingItem == null)
                {
                    return Json(new { success = false, error = "Позиция заказа не найдена" });
                }

                if (bookingItem.Status == 2)
                {
                    return Json(new { success = false, error = "Блюдо уже готово" });
                }

                bookingItem.Status = 2;
                await _context.SaveChangesAsync();

                var allItemsInOrder = await _context.BookingItems
                    .Where(bi => bi.BookingId == bookingItem.BookingId)
                    .ToListAsync();

                var allReady = allItemsInOrder.All(bi => bi.Status == 2);
                var readyCount = allItemsInOrder.Count(bi => bi.Status == 2);
                var totalCount = allItemsInOrder.Count;

                return Json(new
                {
                    success = true,
                    allReady,
                    bookingId = bookingItem.BookingId,
                    readyCount,
                    totalCount,
                    message = $"Статус обновлен: {readyCount}/{totalCount} готово"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public class UpdateStatusRequest
        {
            public int ItemId { get; set; }
            public int Status { get; set; }
        }

        // POST: /OrderTracking/CloseOrder
        [HttpPost]
        public async Task<IActionResult> CloseOrder([FromBody] CloseOrderRequest request)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var isAgent = HttpContext.Session.GetInt32("IsAgent") == 1;

                if (!isAgent)
                {
                    return Json(new { success = false, error = "Доступ только для официанта" });
                }

                var orderItems = await _context.BookingItems
                    .Where(bi => bi.BookingId == request.BookingId && (bi.Status == 1 || bi.Status == 2))
                    .ToListAsync();

                if (orderItems.Count == 0)
                {
                    return Json(new
                    {
                        success = false,
                        error = "Заказ не найден или уже закрыт"
                    });
                }

                if (orderItems.Any(bi => bi.Status != 2))
                {
                    var notReadyCount = orderItems.Count(bi => bi.Status != 2);
                    return Json(new
                    {
                        success = false,
                        error = $"Не все блюда готовы. Осталось: {notReadyCount}"
                    });
                }

                foreach (var item in orderItems)
                {
                    item.Status = 3; 
                }

                var booking = await _context.Bookings
                    .Include(b => b.Table)
                    .FirstOrDefaultAsync(b => b.Id == request.BookingId);

                string message;

                if (booking != null && booking.Status == 1) 
                {
                    booking.Status = 2;

                    if (booking.Table != null)
                    {
                        booking.Table.Status = 1; 
                    }

                    message = "Заказ закрыт, бронь завершена" +
                              (booking.Table != null ? " и столик освобожден" : "");
                }
                else
                {
                    message = "Заказ успешно закрыт";
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = message
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public class CloseOrderRequest
        {
            public int BookingId { get; set; }
        }
    }
}