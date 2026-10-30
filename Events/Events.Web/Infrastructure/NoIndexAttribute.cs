using Microsoft.AspNetCore.Mvc.Filters;

namespace Events.Web.Infrastructure;

// Adds "X-Robots-Tag: noindex" to the response, for HTML fragments and JSON loaded by the pages' scripts
// (GS-REQ-008, 8.3). They are not blocked in robots.txt, because Google must fetch them to render the pages.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class NoIndexAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        context.HttpContext.Response.Headers["X-Robots-Tag"] = "noindex";
        base.OnResultExecuting(context);
    }
}
