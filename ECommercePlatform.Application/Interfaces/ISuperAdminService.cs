using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Interfaces
{
    public interface ISuperAdminService
    {
        bool IsSuperAdminEmail(string email);
        Task<bool> IsSuperAdminAsync(Guid userId);
    }
}
