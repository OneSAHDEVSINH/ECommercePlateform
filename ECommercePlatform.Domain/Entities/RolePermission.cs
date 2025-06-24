namespace ECommercePlatform.Domain.Entities
{
    public class RolePermission : BaseEntity
    {
        public Guid RoleId { get; set; }
        public Guid ModuleId { get; set; }
        public bool CanView { get; set; }
        //public bool CanAdd { get; set; }
        //public bool CanEdit { get; set; }
        public bool CanAddEdit { get; set; }
        public bool CanDelete { get; set; }

        //Navigation Properties
        public virtual Role? Role { get; set; }
        public virtual Module? Module { get; set; }

        private RolePermission() { }

        public static RolePermission Create(
            Guid roleId,
            Guid moduleId,
            bool canView = false,
            //bool canAdd = false,
            //bool canEdit = false,
            bool canAddEdit = false,
            bool canDelete = false)
        {
            return new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = roleId,
                ModuleId = moduleId,
                CanView = canView,
                //CanAdd = canAdd,
                //CanEdit = canEdit,
                CanAddEdit = canAddEdit,
                CanDelete = canDelete
            };
        }

        public void UpdatePermissions(
            bool canView,
            //bool canAdd,
            //bool canEdit,
            bool canAddEdit,
            bool canDelete)
        {
            CanView = canView;
            //CanAdd = canAdd;
            //CanEdit = canEdit;
            CanAddEdit = canAddEdit;
            CanDelete = canDelete;
            ModifiedOn = DateTime.Now;
        }

        public void SetAllPermissions(bool value)
        {
            CanView = value;
            //CanAdd = value;
            //CanEdit = value;
            CanAddEdit = value;
            CanDelete = value;
            ModifiedOn = DateTime.Now;
        }
    }
}