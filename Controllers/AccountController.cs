using Microsoft.AspNetCore.Mvc;
using NotesSystemAdmin.Data;
using NotesSystemAdmin.Models;
using Microsoft.EntityFrameworkCore;

namespace NotesSystemAdmin.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            // If already logged in, redirect to dashboard
            if (HttpContext.Session.GetInt32("AdminId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = await _context.Admins
                    .FirstOrDefaultAsync(a => a.Email == model.Email && a.Password == model.Password && a.IsActive);

                if (admin != null)
                {
                    // Set session
                    HttpContext.Session.SetInt32("AdminId", admin.Id);
                    HttpContext.Session.SetString("AdminName", admin.FullName);
                    HttpContext.Session.SetString("AdminEmail", admin.Email);
                    HttpContext.Session.SetString("AdminRole", admin.Role);

                    TempData["Success"] = "Welcome back, " + admin.FullName + "!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }
            return View(model);
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }
    }
}
