using Microsoft.AspNetCore.Mvc;
using NotesSystemAdmin.Data;
using NotesSystemAdmin.Models;
using Microsoft.EntityFrameworkCore;

namespace NotesSystemAdmin.Controllers
{
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Check if admin is logged in
        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetInt32("AdminId") != null;
        }

        // GET: Notes
        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var notes = await _context.Notes
                .Include(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notes);
        }

        // GET: Notes/Details/5
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

            var note = await _context.Notes
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // GET: Notes/Delete/5
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

            var note = await _context.Notes
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var note = await _context.Notes.FindAsync(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Note deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
