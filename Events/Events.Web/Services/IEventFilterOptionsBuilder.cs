using Events.Models.Entities;
using Events.Models.Queries;
using Events.Web.Models.Admin;

namespace Events.Web.Services;

public interface IEventFilterOptionsBuilder
{
    Task<EventFilterOptions> BuildAdminOptionsAsync(EventListCriteria filters, IReadOnlyList<Category> categories);
}
