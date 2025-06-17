//using ECommercePlatform.Domain.Enums;

//namespace ECommercePlatform.Domain.Entities
//{
//    public class Permission : BaseEntity
//    {
//        //public string? Name { get; private set; }
//        //public string? Description { get; private set; }
//        //public PermissionType Type { get; private set; }
//        public bool CanView { get; private set; }
//        public bool CanAdd { get; private set; }
//        public bool CanEdit { get; private set; }
//        public bool CanDelete { get; private set; }
//        public Guid ModuleId { get; private set; }

//        //Navigation properties
//        public virtual Module? Module { get; private set; }
//        public virtual ICollection<RolePermission>? RolePermissions { get; private set; }

//        private Permission() { }

//        public static Permission Create(
//            bool canView,
//            bool canAdd,
//            bool canEdit,
//            bool canDelete,
//            Guid moduleId)
//        {
//            return new Permission
//            {
//                //Name = name,
//                //Description = description,
//                //Type = type,
//                CanView = canView,
//                CanAdd = canAdd,
//                CanEdit = canEdit,
//                CanDelete = canDelete,
//                ModuleId = moduleId
//            };
//        }

//        public void Update(bool canView,
//            bool canAdd,
//            bool canEdit,
//            bool canDelete,
//            Guid moduleId)
//        {
//            //Name = name;
//            //Description = description;
//            //Type = type;
//            ModuleId = moduleId;
//            CanView = canView;
//            CanAdd = canAdd;
//            CanEdit = canEdit;
//            CanDelete = canDelete;
//        }
//    }
//}