using System.Diagnostics;
using System.Threading.Tasks;
using Events.Data.Context;
using Events.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Events.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EventsDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, EventsDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            //// Get users with their roles for testing
            //var users = await _context.Users.ToListAsync();
            //var userEmails = new List<string>();

            //foreach (var user in users)
            //{
            //    var roles = await _userManager.GetRolesAsync(user);
            //    var rolesString = roles.Any() ? $" ({string.Join(", ", roles)})" : "";
            //    userEmails.Add($"{user.Email}{rolesString}");
            //}

            //ViewData["Emails"] = userEmails;
            //return View();

            var events = await _context.Events
                 //.Where(e => e.Status == Data.Enums.EventStatus.Active && e.Date >= DateTime.UtcNow)
                .Where(e => e.Date >= DateTime.UtcNow)
                .OrderBy(e => e.Date)
                .Take(12)
                .ToListAsync();
            return View(events);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
