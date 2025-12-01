using Events.Services.Interfaces;
using Events.Web.Models.Admin;
using Events.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdminRole")]
public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;
    private readonly IEventService _eventService;

    public DashboardController(ILogger<DashboardController> logger, IEventService eventService)
    {
        _logger = logger;
        _eventService = eventService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var viewModel = new AdminDashboardViewModel();

            // Get total events count
            viewModel.TotalEvents = await _eventService.GetTotalEventsCountAsync();
            viewModel.PublishedEvents = await _eventService.GetTotalEventsCountAsync(Events.Models.Enums.EventStatus.Published);
            viewModel.DraftEvents = await _eventService.GetTotalEventsCountAsync(Events.Models.Enums.EventStatus.Draft);

            // Get featured events count (will need to add this method to service)
            var featuredEvents = await _eventService.GetFeaturedEventsAsync(100);
            viewModel.FeaturedEvents = featuredEvents.Count();

            // Get uncategorized events count (CategoryId = 11 is Undefined)
            var allEvents = await _eventService.GetAllEventsAsync();
            viewModel.UncategorizedEvents = allEvents.Count(e => e.CategoryId == 11);

            // Events added today
            var today = DateTime.Today;
            var todayEvents = allEvents.Where(e => e.CreatedAt.Date == today).Count();
            viewModel.EventsAddedToday = todayEvents;

            // Events added this week
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var weekEvents = allEvents.Where(e => e.CreatedAt >= weekStart).Count();
            viewModel.EventsAddedThisWeek = weekEvents;

            // Events added this month
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var monthEvents = allEvents.Where(e => e.CreatedAt >= monthStart).Count();
            viewModel.EventsAddedThisMonth = monthEvents;

            // Get category statistics
            viewModel.CategoryStatistics = allEvents
                .GroupBy(e => e.Category?.Name ?? "Uncategorized")
                .Select(g => new CategoryStatistic
                {
                    CategoryName = g.Key,
                    EventCount = g.Count(),
                    UncategorizedCount = g.Key == "Undefined" ? g.Count() : 0
                })
                .OrderByDescending(cs => cs.EventCount)
                .ToList();

            // Get recent events
            var recentEvents = allEvents
                .OrderByDescending(e => e.CreatedAt)
                .Take(10)
                .ToList();
            viewModel.RecentEvents = AdminEventViewModel.FromEntities(recentEvents);

            // Get pending categorization
            var pendingCategorization = allEvents
                .Where(e => e.CategoryId == 11)
                .OrderBy(e => e.CreatedAt)
                .Take(10)
                .ToList();
            viewModel.PendingCategorization = AdminEventViewModel.FromEntities(pendingCategorization);

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
            return View(new AdminDashboardViewModel());
        }
    }
}
