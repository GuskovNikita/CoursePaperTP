using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Data;
using RestaurantSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RestaurantSystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly RestaurantDbContext _context;
        private readonly Random _random = new Random();

        public BookingController(RestaurantDbContext context)
        {
            _context = context;
        }

        private static readonly TimeSpan BookingDuration = TimeSpan.FromMinutes(90);

        private static bool IsOverlap(TimeSpan startA, TimeSpan endA, TimeSpan startB, TimeSpan endB)
        {
            return startA < endB && startB < endA;
        }

        private async Task<HashSet<int>> GetUnavailableTableIdsAsync(DateTime date, TimeSpan requestedStart)
        {
            var requestedEnd = requestedStart.Add(BookingDuration);

            var sameDayBookings = await _context.Bookings
                .Where(b => b.Status == 1
                    && b.TableId != null
                    && b.BookingDate != null
                    && b.BookingTime != null
                    && b.BookingDate.Value.Date == date.Date)
                .Select(b => new { b.TableId, b.BookingTime })
                .ToListAsync();

            var blocked = new HashSet<int>();

            foreach (var b in sameDayBookings)
            {
                var start = b.BookingTime!.Value;
                var end = start.Add(BookingDuration);

                if (IsOverlap(start, end, requestedStart, requestedEnd))
                    blocked.Add(b.TableId!.Value);
            }

            return blocked;
        }

        // GET: /Booking/Create
        public async Task<IActionResult> Create(DateTime? date, TimeSpan? time)
        {
            var selectedDate = (date ?? DateTime.Now.AddDays(1)).Date;
            if (selectedDate <= DateTime.Today)
            {
                selectedDate = DateTime.Today.AddDays(1);
            }
            var selectedTime = time ?? new TimeSpan(19, 0, 0);

            var tables = await _context.TableTops
                .OrderBy(t => t.Code)
                .ToListAsync();

            var blockedIds = await GetUnavailableTableIdsAsync(selectedDate, selectedTime);

            ViewBag.SelectedDate = selectedDate;
            ViewBag.SelectedTime = selectedTime;
            ViewBag.BlockedIds = blockedIds;

            return View(tables);
        }

        // GET: /Booking/BookTable/5 
        public async Task<IActionResult> BookTable(int id, DateTime? date, TimeSpan? time)
        {
            var table = await _context.TableTops.FindAsync(id);

            if (table == null)
            {
                TempData["Error"] = "Столик не найден";
                return RedirectToAction("Create");
            }

            ViewBag.Table = table;

            var booking = new Booking
            {
                TableId = id,
                BookingDate = (date ?? DateTime.Now.AddDays(1)).Date,
                BookingTime = time ?? new TimeSpan(19, 0, 0),
                GuestsCount = 2,
                Status = 1 
            };

            return View(booking);
        }

        // POST: /Booking/BookTable
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookTable(Booking booking)
        {
            var table = await _context.TableTops.FindAsync(booking.TableId);
            if (table == null)
            {
                TempData["Error"] = "Столик не найден";
                return RedirectToAction("Create");
            }

            if (table.Capacity >= 6 && booking.GuestsCount < 4)
            {
                ModelState.AddModelError("GuestsCount",
                    $"Для столика на {table.Capacity} мест минимально 4 гостя");
            }
            else if (table.Capacity == 4 && booking.GuestsCount < 2)
            {
                ModelState.AddModelError("GuestsCount",
                    "Для столика на 4 места минимально 2 гостя");
            }

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    booking.Token = await GenerateUniqueTokenAsync();
                    booking.Status = 1; 
                    booking.UserId = null;

                    var blockedIds = await GetUnavailableTableIdsAsync(booking.BookingDate!.Value.Date, booking.BookingTime!.Value);
                    if (blockedIds.Contains(booking.TableId!.Value))
                    {
                        TempData["Error"] = "Этот столик уже забронирован на выбранное время. Выберите другое время или стол.";
                        return RedirectToAction("Create", new
                        {
                            date = booking.BookingDate.Value.ToString("yyyy-MM-dd"),
                            time = booking.BookingTime.Value.ToString(@"hh\:mm\:ss")
                        });
                    }

                    _context.Add(booking);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    TempData["BookingToken"] = booking.Token;
                    TempData["BookingId"] = booking.Id.ToString();
                    TempData["TableCode"] = table.Code;
                    TempData["BookingDate"] = booking.BookingDate?.ToString("dd.MM.yyyy");
                    TempData["BookingTime"] = booking.BookingTime?.ToString(@"hh\:mm");

                    return RedirectToAction("Success");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = $"Ошибка при сохранении брони: {ex.Message}";
                    ViewBag.Table = table;
                    return View(booking);
                }
            }

            ViewBag.Table = table;
            return View(booking);
        }

        private async Task<string> GenerateUniqueTokenAsync()
        {
            string token;
            bool isUnique;

            do
            {
                token = _random.Next(100000, 1000000).ToString();

                isUnique = !await _context.Bookings.AnyAsync(b => b.Token == token);
            }
            while (!isUnique); 

            return token;
        }

        // GET: /Booking/Success 
        public IActionResult Success()
        {
            if (TempData["BookingToken"] == null)
            {
                return RedirectToAction("Create");
            }

            ViewBag.Token = TempData["BookingToken"] as string;
            ViewBag.BookingId = TempData["BookingId"] as string;
            ViewBag.TableCode = TempData["TableCode"] as string;
            ViewBag.BookingDate = TempData["BookingDate"] as string;
            ViewBag.BookingTime = TempData["BookingTime"] as string;

            return View();
        }
    }
}