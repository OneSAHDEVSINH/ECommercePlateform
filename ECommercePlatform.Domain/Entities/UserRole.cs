using Microsoft.AspNetCore.Identity;

namespace ECommercePlatform.Domain.Entities
{
    public class UserRole : IdentityUserRole<string>
    {
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public User? User { get; set; }
        public Role? Role { get; set; }

        // Private constructor for EF Core
        public UserRole() { }

        public static UserRole Create(
            string userId,
            string roleId)
        {
            return new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };
        }

        public void Update(string userId,
            string roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}