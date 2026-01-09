namespace Events.Web.Models.Admin;

public class BulkEventOperationViewModel
{
    public List<int> SelectedEventIds { get; set; } = new();
    
    // Category operation
    public int? BulkCategoryId { get; set; }
    
    // SubCategory operation
    public int? BulkSubCategoryId { get; set; }
    
    // Tags operation (up to 3)
    public List<int> BulkTagIds { get; set; } = new();
    
    // Start Time operation
    public TimeSpan? BulkStartTime { get; set; }
    
    // Which operations to apply
    public List<string> OperationsToApply { get; set; } = new();
}
