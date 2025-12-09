using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Data;
using RestaurantSystem.Models;

namespace RestaurantSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly RestaurantDbContext _context;

        public AccountController(RestaurantDbContext context)
        {
            _context = context;
        }

        // POST: /Account/Login 
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Введите email и пароль";
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                TempData["Error"] = "Неверный email или пароль";
                return RedirectToAction("Index", "Home");
            }

            if (!user.Agent && !user.Chef && !user.Admin)
            {
                TempData["Error"] = "Доступ только для персонала ресторана";
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");

            string role = "Официант";
            if (user.Chef) role = "Повар";
            if (user.Admin) role = "Администратор";

            HttpContext.Session.SetString("UserRole", role);
            HttpContext.Session.SetInt32("IsAdmin", user.Admin ? 1 : 0);
            HttpContext.Session.SetInt32("IsChef", user.Chef ? 1 : 0);
            HttpContext.Session.SetInt32("IsAgent", user.Agent ? 1 : 0);

            // Временная
            return RedirectToAction("Dashboard", "Account");
        }

        // GET: /Account/Dashboard
        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            ViewBag.IsChef = HttpContext.Session.GetInt32("IsChef") == 1;
            ViewBag.IsAgent = HttpContext.Session.GetInt32("IsAgent") == 1;
            ViewBag.IsAdmin = HttpContext.Session.GetInt32("IsAdmin") == 1;

            return View();
        }

        // GET: /Account/Logout 
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}