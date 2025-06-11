namespace ECommercePlatform.Domain.Entities
{
    public class RolePermission : BaseEntity
    {
        public Guid RoleId { get; private set; }
        public Guid PermissionId { get; private set; }

        public Role? Role { get; set; }
        public Permission? Permission { get; set; }

        private RolePermission() { }

        public static RolePermission Create(
            Guid roleId,
            Guid permissionId)
        {
            return new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            };
        }

        public void Update(Guid roleId,
            Guid permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
}