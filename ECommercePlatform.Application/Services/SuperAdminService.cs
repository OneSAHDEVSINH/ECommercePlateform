using ECommercePlatform.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Services
{
    public class SuperAdminService : ISuperAdminService
    {
        private readonly string _superAdminEmail;
        private readonly IServiceProvider _serviceProvider;

        public SuperAdminService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _superAdminEmail = configuration["SuperAdmin:Email"] ?? "admin@admin.com";
            _serviceProvider = serviceProvider;
        }

        public bool IsSuperAdminEmail(string email)
        {
            return string.Equals(email, _superAdminEmail, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> IsSuperAdminAsync(Guid userId)
        {
            // Use scoped service provider to resolve UnitOfWork on demand
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var user = await unitOfWork.UserManager.FindByIdAsync(userId.ToString());
            return user != null && IsSuperAdminEmail(user.Email);
        }
    }
}
