namespace ECommercePlatform.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedOn { get; protected set; } = DateTime.Now;
        public string? CreatedBy { get; protected set; }
        public DateTime ModifiedOn { get; protected set; } = DateTime.Now;
        public string? ModifiedBy { get; protected set; }
        public bool IsActive { get; protected set; } = true;
        public bool IsDeleted { get; protected set; }

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