//using ECommercePlatform.Domain.Enums;

//namespace ECommercePlatform.Domain.Entities
//{
//    public class Permission : BaseEntity
//    {
//        //public string? Name { get; set; }
//        //public string? Description { get; set; }
//        //public PermissionType Type { get; set; }
//        public bool CanView { get; set; }
//        public bool CanAdd { get; set; }
//        public bool CanEdit { get; set; }
//        public bool CanDelete { get; set; }
//        public Guid ModuleId { get; set; }

//        //Navigation properties
//        public virtual Module? Module { get; set; }
//        public virtual ICollection<RolePermission>? RolePermissions { get; set; }

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