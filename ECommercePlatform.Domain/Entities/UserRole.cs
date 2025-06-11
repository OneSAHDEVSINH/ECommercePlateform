namespace ECommercePlatform.Domain.Entities
{
    public class UserRole : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }

        public User? User { get; set; }
        public Role? Role { get; set; }

        // Private constructor for EF Core
        private UserRole() { }

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