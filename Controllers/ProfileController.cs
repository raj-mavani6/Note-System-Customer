using Microsoft.AspNetCore.Mvc;
using NotesSystemAdmin.Data;
using NotesSystemAdmin.Models;
using Microsoft.EntityFrameworkCore;

namespace NotesSystemAdmin.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get current admin ID from session
        private int? GetAdminId()
        {
            return HttpContext.Session.GetInt32("AdminId");
        }

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            var adminId = GetAdminId();
            if (adminId == null)
            {
                TempData["Error"] = "Please login to view your profile.";
                return RedirectToAction("Login", "Account");
            }

            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Id == adminId);

            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var adminId = GetAdminId();
            if (adminId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var admin = await _context.Admins.FindAsync(adminId);
            if (admin == null)
            {
                return NotFound();
            }

            var viewModel = new EditProfileViewModel
            {
                FullName = admin.FullName,
                Email = admin.Email,
                ProfileImageUrl = admin.ProfileImageUrl
            };

            return View(viewModel);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            var adminId = GetAdminId();
            if (adminId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var admin = await _context.Admins.FindAsync(adminId);
                if (admin == null)
                {
                    return NotFound();
                }

                // Check if email is taken by another admin
                var existingAdmin = await _context.Admins
                    .FirstOrDefaultAsync(a => a.Email == model.Email && a.Id != adminId);
                if (existingAdmin != null)
                {
                    ModelState.AddModelError("Email", "This email is already in use.");
                    return View(model);
                }

                admin.FullName = model.FullName;
                admin.Email = model.Email;
                admin.ProfileImageUrl = model.ProfileImageUrl;

                await _context.SaveChangesAsync();

                // Update session
                HttpContext.Session.SetString("AdminName", admin.FullName);

                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Profile/ChangePassword
        public IActionResult ChangePassword()
        {
            var adminId = GetAdminId();
            if (adminId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var adminId = GetAdminId();
            if (adminId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var admin = await _context.Admins.FindAsync(adminId);
                if (admin == null)
                {
                    return NotFound();
                }

                // Verify current password
                if (admin.Password != model.CurrentPassword)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    return View(model);
                }

                // Update password
                admin.Password = model.NewPassword;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Password changed successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}
