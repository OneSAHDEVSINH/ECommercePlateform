namespace ECommercePlatform.Domain.Entities
{
    public class UserRole : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }

        public User User { get; private set; } = null!;
        public Role Role { get; private set; } = null!;

        // Private constructor for EF Core
        private UserRole() { }

        public static UserRole Create(
            Guid userId,
            Guid roleId,
            string createdBy)
        {
            return new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                CreatedBy = createdBy,
                CreatedOn = DateTime.Now,
                ModifiedBy = createdBy,
                ModifiedOn = DateTime.Now,
                IsActive = true,
                IsDeleted = false
            };
        }
    }
}