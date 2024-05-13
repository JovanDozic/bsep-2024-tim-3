using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marketing_system.BL.Contracts.DTO;

namespace Marketing_system.BL.Contracts.IService
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetRoleById(int id);
        Task<bool> UpdateRole(RoleDto role);
        Task<IEnumerable<RoleDto>> GetAllRoles();
    }
}
