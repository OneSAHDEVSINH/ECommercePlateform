namespace ECommercePlatform.Domain.Entities
{
    public class RolePermission : BaseEntity
    {
        public Guid RoleId { get; private set; }
        public Guid PermissionId { get; private set; }

        public Role Role { get; private set; }
        public Permission Permission { get; private set; }

        public static RolePermission Create(
            Guid roleId,
            Guid permissionId,
            string createdBy)
        {
            return new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = roleId,
                PermissionId = permissionId,
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