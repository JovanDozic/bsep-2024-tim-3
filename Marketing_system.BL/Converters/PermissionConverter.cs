using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marketing_system.BL.Contracts.DTO;
using Marketing_system.DA.Contracts.Model;

namespace Marketing_system.BL.Converters
{
    public static class PermissionConverter
    {
        public static PermissionDto ToDto(this Permission p)
        {
            if (p == null)
            {
                return null;
            }
            return new PermissionDto
            {
                Name = p.Name,
            };
        }
        public static Permission ToDomain(this PermissionDto permissionDto)
        {
            return permissionDto == null ? null :
                new Permission(permissionDto.Name);
        }
    }
}
