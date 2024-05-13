using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;
using Marketing_system.BL.Converters;
using Marketing_system.DA.Contracts;
using Marketing_system.DA.Contracts.Model;
using Marketing_system.DA.Contracts.Shared;

namespace Marketing_system.BL.Service
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<RoleDto>> GetRoleById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateRole(RoleDto role)
        {
            var roleToUpdate = await _unitOfWork.GetRoleRepository().GetByIdAsync(role.Id);
            if (roleToUpdate == null)
            {
                return false;
            }
            List<Permission> permissions = new(); 
            foreach(PermissionDto p in role.Permissions)
            {
                permissions.Add(PermissionConverter.ToDomain(p));
            }
            roleToUpdate.Name = role.Name;
            roleToUpdate.Permissions = permissions;

            _unitOfWork.GetRoleRepository().Update(roleToUpdate);
            await _unitOfWork.Save();

            return true;
        }
        public Task<IEnumerable<RoleDto>> GetAllRoles()
        {
            throw new NotImplementedException();
        }
    }
}
