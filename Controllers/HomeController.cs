using Microsoft.AspNetCore.Mvc;
using NotesSystemAdmin.Data;
using NotesSystemAdmin.Models;
using Microsoft.EntityFrameworkCore;

namespace NotesSystemAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Check if admin is logged in
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.AdminName = HttpContext.Session.GetString("AdminName");

            // Get statistics from database
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalNotes = await _context.Notes.CountAsync();
            ViewBag.ActiveUsers = await _context.Users.CountAsync(u => u.IsActive);
            ViewBag.TodayNotes = await _context.Notes.CountAsync(n => n.CreatedAt.Date == DateTime.Today);

            // Get recent users and notes for activity
            ViewBag.RecentUsers = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentNotes = await _context.Notes
                .Include(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

