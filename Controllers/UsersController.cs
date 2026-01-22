using Microsoft.AspNetCore.Mvc;
using NotesSystemAdmin.Data;
using NotesSystemAdmin.Models;
using Microsoft.EntityFrameworkCore;

namespace NotesSystemAdmin.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Check if admin is logged in
        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetInt32("AdminId") != null;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var users = await _context.Users
                .Include(u => u.Notes)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Notes)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(user);
                }

                user.CreatedAt = DateTime.Now;
                user.IsActive = true;
                _context.Add(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "User created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if email is taken by another user
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email && u.Id != id);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email is already in use by another user.");
                    return View(user);
                }

                _context.Update(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "User updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Notes)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "User deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Users/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();
                TempData["Success"] = user.IsActive ? "User activated!" : "User deactivated!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
