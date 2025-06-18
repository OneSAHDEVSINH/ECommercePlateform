namespace ECommercePlatform.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        public virtual void SetCreatedBy(string createdBy)
        {
            CreatedBy = createdBy;
            CreatedOn = DateTime.Now;
        }

        public virtual void SetModifiedBy(string modifiedBy)
        {
            ModifiedBy = modifiedBy;
            ModifiedOn = DateTime.Now;
        }

        public virtual void MarkAsDeleted(string deletedBy)
        {
            IsDeleted = true;
            SetModifiedBy(deletedBy);
        }

        public virtual void SetActive(bool isActive, string modifiedBy)
        {
            IsActive = isActive;
            SetModifiedBy(modifiedBy);
        }
    }
}