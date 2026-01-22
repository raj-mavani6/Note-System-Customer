using Microsoft.AspNetCore.Mvc;
using NotesSystemAdmin.Data;
using Microsoft.EntityFrameworkCore;

namespace NotesSystemAdmin.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Check if admin is logged in
        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetInt32("AdminId") != null;
        }

        // GET: Analytics (Main Dashboard)
        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // Quick stats for main analytics dashboard
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalNotes = await _context.Notes.CountAsync();
            ViewBag.ActiveUsers = await _context.Users.CountAsync(u => u.IsActive);
            ViewBag.ImportantNotes = await _context.Notes.CountAsync(n => n.IsImportant);
            ViewBag.NewUsersToday = await _context.Users.CountAsync(u => u.CreatedAt.Date == DateTime.Today);
            ViewBag.NotesToday = await _context.Notes.CountAsync(n => n.CreatedAt.Date == DateTime.Today);

            return View("~/Views/Analytics/Home/Index.cshtml");
        }

        // GET: Analytics/Users - User Analytics
        public async Task<IActionResult> Users(string period = "7days")
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var today = DateTime.Today;
            var weekAgo = today.AddDays(-7);
            var monthAgo = today.AddDays(-30);

            // New users this week/month
            ViewBag.NewUsersThisWeek = await _context.Users.CountAsync(u => u.CreatedAt >= weekAgo);
            ViewBag.NewUsersThisMonth = await _context.Users.CountAsync(u => u.CreatedAt >= monthAgo);

            // Active vs Inactive ratio
            ViewBag.ActiveUsers = await _context.Users.CountAsync(u => u.IsActive);
            ViewBag.InactiveUsers = await _context.Users.CountAsync(u => !u.IsActive);
            ViewBag.TotalUsers = await _context.Users.CountAsync();

            // Determine date range based on period
            DateTime startDate;
            string periodLabel;
            string dateFormat;

            switch (period.ToLower())
            {
                case "30days":
                    startDate = today.AddDays(-30);
                    periodLabel = "Last 30 Days";
                    dateFormat = "MMM dd";
                    break;
                case "thismonth":
                    startDate = new DateTime(today.Year, today.Month, 1);
                    periodLabel = "This Month";
                    dateFormat = "MMM dd";
                    break;
                case "thisyear":
                    startDate = new DateTime(today.Year, 1, 1);
                    periodLabel = "This Year";
                    dateFormat = "MMM";
                    break;
                case "alltime":
                    startDate = DateTime.MinValue;
                    periodLabel = "All Time";
                    dateFormat = "MMM yyyy";
                    break;
                default: // 7days
                    startDate = weekAgo;
                    periodLabel = "Last 7 Days";
                    dateFormat = "ddd";
                    break;
            }

            ViewBag.CurrentPeriod = period;
            ViewBag.PeriodLabel = periodLabel;
            ViewBag.DateFormat = dateFormat;

            // User registration trend based on selected period
            if (period.ToLower() == "thisyear")
            {
                // Group by month for year period
                var monthlyTrend = await _context.Users
                    .Where(u => u.CreatedAt >= startDate)
                    .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                    .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToListAsync();
                ViewBag.RegistrationTrend = monthlyTrend.Select(x => new {
                    Date = new DateTime(x.Year, x.Month, 1),
                    Count = x.Count
                }).ToList();
            }
            else
            {
                // Group by day for all other periods including All Time
                var dailyTrend = await _context.Users
                    .Where(u => startDate == DateTime.MinValue || u.CreatedAt >= startDate)
                    .GroupBy(u => u.CreatedAt.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Date)
                    .ToListAsync();
                ViewBag.RegistrationTrend = dailyTrend;
            }

            // Top users by notes count
            var topUsers = await _context.Users
                .Include(u => u.Notes)
                .OrderByDescending(u => u.Notes.Count)
                .Take(5)
                .Select(u => new { u.Id, u.FullName, u.Email, u.ProfileImageUrl, NotesCount = u.Notes.Count })
                .ToListAsync();
            ViewBag.TopUsers = topUsers;

            // Monthly registration data for chart (last 6 months)
            var sixMonthsAgo = today.AddMonths(-6);
            var monthlyRegistrations = await _context.Users
                .Where(u => u.CreatedAt >= sixMonthsAgo)
                .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();
            ViewBag.MonthlyRegistrations = monthlyRegistrations;

            return View("~/Views/Analytics/User/Index.cshtml");
        }

        // GET: Analytics/Notes - Notes Analytics
        public async Task<IActionResult> Notes(string period = "7days")
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var today = DateTime.Today;
            var weekAgo = today.AddDays(-7);
            var monthAgo = today.AddDays(-30);

            // Notes created this week/month
            ViewBag.NotesThisWeek = await _context.Notes.CountAsync(n => n.CreatedAt >= weekAgo);
            ViewBag.NotesThisMonth = await _context.Notes.CountAsync(n => n.CreatedAt >= monthAgo);
            ViewBag.TotalNotes = await _context.Notes.CountAsync();

            // Important vs Normal notes
            ViewBag.ImportantNotes = await _context.Notes.CountAsync(n => n.IsImportant);
            ViewBag.NormalNotes = await _context.Notes.CountAsync(n => !n.IsImportant);

            // Notes with images vs without
            ViewBag.NotesWithImages = await _context.Notes.CountAsync(n => !string.IsNullOrEmpty(n.ImageUrl));
            ViewBag.NotesWithoutImages = await _context.Notes.CountAsync(n => string.IsNullOrEmpty(n.ImageUrl));

            // Determine date range based on period
            DateTime startDate;
            string periodLabel;
            string dateFormat;

            switch (period.ToLower())
            {
                case "30days":
                    startDate = today.AddDays(-30);
                    periodLabel = "Last 30 Days";
                    dateFormat = "MMM dd";
                    break;
                case "thismonth":
                    startDate = new DateTime(today.Year, today.Month, 1);
                    periodLabel = "This Month";
                    dateFormat = "MMM dd";
                    break;
                case "thisyear":
                    startDate = new DateTime(today.Year, 1, 1);
                    periodLabel = "This Year";
                    dateFormat = "MMM";
                    break;
                case "alltime":
                    startDate = DateTime.MinValue;
                    periodLabel = "All Time";
                    dateFormat = "MMM dd, yyyy";
                    break;
                default: // 7days
                    startDate = weekAgo;
                    periodLabel = "Last 7 Days";
                    dateFormat = "ddd";
                    break;
            }

            ViewBag.CurrentPeriod = period;
            ViewBag.PeriodLabel = periodLabel;
            ViewBag.DateFormat = dateFormat;

            // Notes trend based on selected period
            if (period.ToLower() == "thisyear")
            {
                // Group by month for year period
                var monthlyTrend = await _context.Notes
                    .Where(n => n.CreatedAt >= startDate)
                    .GroupBy(n => new { n.CreatedAt.Year, n.CreatedAt.Month })
                    .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToListAsync();
                ViewBag.DailyNotesTrend = monthlyTrend.Select(x => new {
                    Date = new DateTime(x.Year, x.Month, 1),
                    Count = x.Count
                }).ToList();
            }
            else
            {
                // Group by day for all other periods including All Time
                var dailyTrend = await _context.Notes
                    .Where(n => startDate == DateTime.MinValue || n.CreatedAt >= startDate)
                    .GroupBy(n => n.CreatedAt.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Date)
                    .ToListAsync();
                ViewBag.DailyNotesTrend = dailyTrend;
            }

            // Monthly notes data for chart (last 6 months)
            var sixMonthsAgo = today.AddMonths(-6);
            var monthlyNotes = await _context.Notes
                .Where(n => n.CreatedAt >= sixMonthsAgo)
                .GroupBy(n => new { n.CreatedAt.Year, n.CreatedAt.Month })
                .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();
            ViewBag.MonthlyNotes = monthlyNotes;

            return View("~/Views/Analytics/Note/Index.cshtml");
        }

        // GET: Analytics/Activity - Activity Analytics
        public async Task<IActionResult> Activity()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // Peak activity hours (when most notes are created)
            var hourlyActivity = await _context.Notes
                .GroupBy(n => n.CreatedAt.Hour)
                .Select(g => new { Hour = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();
            ViewBag.HourlyActivity = hourlyActivity;
            ViewBag.PeakHour = hourlyActivity.FirstOrDefault()?.Hour ?? 0;

            // Most active days of the week
            var weekdayActivity = await _context.Notes
                .GroupBy(n => n.CreatedAt.DayOfWeek)
                .Select(g => new { DayOfWeek = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();
            ViewBag.WeekdayActivity = weekdayActivity;
            ViewBag.MostActiveDay = weekdayActivity.FirstOrDefault()?.DayOfWeek.ToString() ?? "N/A";

            // Recent activity timeline (last 20 activities)
            var recentNotes = await _context.Notes
                .Include(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .Take(20)
                .Select(n => new { n.Id, n.Title, n.CreatedAt, UserName = n.User != null ? n.User.FullName : "Unknown" })
                .ToListAsync();
            ViewBag.RecentNotes = recentNotes;

            var recentUsers = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(20)
                .Select(u => new { u.Id, u.FullName, u.CreatedAt })
                .ToListAsync();
            ViewBag.RecentUsers = recentUsers;

            return View("~/Views/Analytics/Activity/Index.cshtml");
        }

        // GET: Analytics/Storage - Storage/Content Analytics
        public async Task<IActionResult> Storage()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // Total content size (approximate based on content length)
            var allNotes = await _context.Notes.Select(n => n.Content).ToListAsync();
            var totalContentLength = allNotes.Sum(c => c?.Length ?? 0);
            ViewBag.TotalContentSize = totalContentLength;
            ViewBag.TotalContentSizeKB = Math.Round(totalContentLength / 1024.0, 2);

            // Average note length
            ViewBag.TotalNotes = await _context.Notes.CountAsync();
            ViewBag.AverageNoteLength = ViewBag.TotalNotes > 0 ? Math.Round((double)totalContentLength / ViewBag.TotalNotes, 0) : 0;

            // Total images uploaded
            ViewBag.TotalImages = await _context.Notes.CountAsync(n => !string.IsNullOrEmpty(n.ImageUrl));

            // Content length distribution
            var shortNotes = await _context.Notes.CountAsync(n => n.Content.Length < 100);
            var mediumNotes = await _context.Notes.CountAsync(n => n.Content.Length >= 100 && n.Content.Length < 500);
            var longNotes = await _context.Notes.CountAsync(n => n.Content.Length >= 500);
            ViewBag.ShortNotes = shortNotes;
            ViewBag.MediumNotes = mediumNotes;
            ViewBag.LongNotes = longNotes;

            // Top longest notes
            var longestNotes = await _context.Notes
                .Include(n => n.User)
                .OrderByDescending(n => n.Content.Length)
                .Take(5)
                .Select(n => new { n.Id, n.Title, Length = n.Content.Length, UserName = n.User != null ? n.User.FullName : "Unknown" })
                .ToListAsync();
            ViewBag.LongestNotes = longestNotes;

            return View("~/Views/Analytics/Storage/Index.cshtml");
        }
    }
}
