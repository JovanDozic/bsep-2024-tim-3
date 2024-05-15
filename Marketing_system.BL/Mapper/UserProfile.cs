using AutoMapper;
using Marketing_system.BL.Contracts.DTO;
using Marketing_system.DA.Contracts.Model;

namespace Marketing_system.BL.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<RoleDto, Role>().ReverseMap();
            CreateMap<PermissionDto, Permission>().ReverseMap();
            CreateMap<RegistrationRequestDto, RegistrationRequest>().ReverseMap();
        }
    }
}
