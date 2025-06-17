using Microsoft.AspNetCore.Identity;

namespace ECommercePlatform.Domain.Entities
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        //Navigation properties
        public virtual User? User { get; private set; }
        public virtual Role? Role { get; private set; }

        // Private constructor for EF Core
        public UserRole() { }

        public static UserRole Create(
            Guid userId,
            Guid roleId)
        {
            return new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };
        }

        public void Update(Guid userId,
            Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}