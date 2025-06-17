namespace ECommercePlatform.Domain.Entities
{
    public class RolePermission : BaseEntity
    {
        public Guid RoleId { get; private set; }
        public Guid ModuleId { get; private set; }
        public bool CanView { get; private set; }
        public bool CanAdd { get; private set; }
        public bool CanEdit { get; private set; }
        public bool CanDelete { get; private set; }

        //Navigation Properties
        public virtual Role? Role { get; private set; }
        public virtual Module? Module { get; private set; }

        private RolePermission() { }

        public static RolePermission Create(
            Guid roleId,
            Guid moduleId,
            bool canView = false,
            bool canAdd = false,
            bool canEdit = false,
            bool canDelete = false)
        {
            return new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = roleId,
                ModuleId = moduleId,
                CanView = canView,
                CanAdd = canAdd,
                CanEdit = canEdit,
                CanDelete = canDelete
            };
        }

        public void UpdatePermissions(
            bool canView,
            bool canAdd,
            bool canEdit,
            bool canDelete)
        {
            CanView = canView;
            CanAdd = canAdd;
            CanEdit = canEdit;
            CanDelete = canDelete;
            ModifiedOn = DateTime.Now;
        }

        public void SetAllPermissions(bool value)
        {
            CanView = value;
            CanAdd = value;
            CanEdit = value;
            CanDelete = value;
            ModifiedOn = DateTime.Now;
        }
    }
}