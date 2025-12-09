using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Data;
using RestaurantSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: /Booking/Create
        public async Task<IActionResult> Create()
        {
            var tables = await _context.TableTops
                .OrderBy(t => t.Code)
                .ToListAsync();

            return View(tables);
        }

        // GET: /Booking/BookTable/5 
        public async Task<IActionResult> BookTable(int id)
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
                BookingDate = DateTime.Now.AddDays(1),
                BookingTime = new TimeSpan(19, 0, 0), 
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

            if (table.Status != 1)
            {
                TempData["Error"] = "Этот столик уже занят. Пожалуйста, выберите другой.";
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
                    booking.Token = GenerateToken();
                    booking.Status = 1; 
                    booking.UserId = null; 

                    _context.Add(booking);
                    await _context.SaveChangesAsync();

                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"UPDATE table_top SET status = 0 WHERE id = {table.Id}");

                    await transaction.CommitAsync();

                    TempData["BookingToken"] = booking.Token;
                    TempData["BookingId"] = booking.Id;
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

        private string GenerateToken()
        {
            return _random.Next(100000, 999999).ToString();
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