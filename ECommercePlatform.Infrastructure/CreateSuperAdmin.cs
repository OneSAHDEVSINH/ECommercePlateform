//public class SuperAdminCreator
//{
//    public static async Task CreateSuperAdmin(IServiceProvider serviceProvider)
//    {
//        using var scope = serviceProvider.CreateScope();
//        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
//        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
//        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//        // Create SuperAdmin role if it doesn't exist
//        if (!await roleManager.RoleExistsAsync("SuperAdmin"))
//        {
//            var superAdminRole = new Role("SuperAdmin")
//            {
//                Description = "Super Administrator with all permissions",
//                IsActive = true
//            };
//            await roleManager.CreateAsync(superAdminRole);
//        }

//        // Create SuperAdmin user if it doesn't exist
//        var adminEmail = "admin@admin.com";
//        var adminUser = await userManager.FindByEmailAsync(adminEmail);

//        if (adminUser == null)
//        {
//            adminUser = new User
//            {
//                UserName = adminEmail,
//                Email = adminEmail,
//                EmailConfirmed = true,
//                FirstName = "Super",
//                LastName = "Admin",
//                PhoneNumber = "1234567890",
//                PhoneNumberConfirmed = true,
//                IsActive = true,
//                Gender = Gender.Other,
//                DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30))
//            };

//            var result = await userManager.CreateAsync(adminUser, "Admin@123");
//            if (result.Succeeded)
//            {
//                await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
//            }
//        }

//        // Grant all permissions to SuperAdmin role
//        var superAdminRoleEntity = await roleManager.FindByNameAsync("SuperAdmin");
//        if (superAdminRoleEntity == null)
//        {
//            throw new InvalidOperationException("SuperAdmin role could not be found.");
//        }

//        var modules = await unitOfWork.Modules.GetAllAsync();

//        foreach (var module in modules)
//        {
//            var existingPermission = await unitOfWork.RolePermissions
//                .GetByRoleAndModuleAsync(superAdminRoleEntity.Id, module.Id);

//            if (existingPermission == null)
//            {
//                var rolePermission = RolePermission.Create(
//                    superAdminRoleEntity.Id,
//                    module.Id,
//                    canView: true,
//                    canAdd: true,
//                    canEdit: true,
//                    canDelete: true
//                );

//                await unitOfWork.RolePermissions.AddAsync(rolePermission);
//            }
//        }

//        await unitOfWork.SaveChangesAsync();
//    }
//}
