using AutoMapper;
using IAMService.Application.Models.Role;
using IAMService.Application.Models.User;

namespace IAMService.Application.Mapping
{
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// </summary>
        public MappingProfile()
        {
            // User to UserDTO mapping
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.DateOfBirth,
                    opt => opt.MapFrom(src => src.DateOfBirth.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom(src => src.Role));

            // Role to RoleDTO mapping
            CreateMap<Role, RoleDTO>();
        }
    }
}
