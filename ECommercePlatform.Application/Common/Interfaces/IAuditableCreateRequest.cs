namespace ECommercePlatform.Application.Common.Interfaces
{
    /// <summary>
    /// Marker interface for requests that require creation audit fields (CreatedBy, CreatedOn)
    /// to be automatically populated.
    /// </summary>
    public interface IAuditableCreateRequest
    {
        string? CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
    }
}
