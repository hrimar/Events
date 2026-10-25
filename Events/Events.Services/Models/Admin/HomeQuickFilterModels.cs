using Events.Models.Enums;

namespace Events.Services.Models.Admin;

// Input for creating or editing a homepage quick filter from the admin screen.
public sealed record HomeQuickFilterInput(
    string LabelBg,
    string LabelEn,
    HomeQuickFilterType Type,
    IReadOnlyList<string> Values,
    bool IsActive);

// Outcome of a quick filter change; Error is an admin-facing (English) message when it failed.
public sealed record HomeQuickFilterResult(bool Succeeded, string? Error)
{
    public static HomeQuickFilterResult Success() => new(true, null);
    public static HomeQuickFilterResult Failure(string error) => new(false, error);
}
