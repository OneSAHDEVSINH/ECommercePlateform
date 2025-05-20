namespace ECommercePlatform.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get;  set; }
        public DateTime CreatedOn { get;  set; } = DateTime.Now;
        public string? CreatedBy { get;  set; }
        public DateTime ModifiedOn { get;  set; } = DateTime.Now;
        public string? ModifiedBy { get;  set; }
        public bool IsActive { get;  set; } = true;
        public bool IsDeleted { get;  set; }

        // BaseEntity() { }

        public void SetCreationTracking(string? createdBy)
        {
            CreatedBy = createdBy;
            CreatedOn = DateTime.Now;
        }

        public void SetModificationTracking(string? modifiedBy)
        {
            ModifiedBy = modifiedBy;
            ModifiedOn = DateTime.Now;
        }

    }
}