using AutoMapper;
using IdentityServices.Domain.Dto.Permiso;
using IdentityServices.Domain.Dto.PermRol;
using IdentityServices.Domain.Dto.Rol;
using IdentityServices.Domain.Dto.UserRol;
using IdentityServices.Domain.Dto.Usuario;
using IdentityServices.Domain.Entities;

namespace IdentityServices.Api.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            //Usuario
            CreateMap<Usuario, UsuarioCreateDto>().ReverseMap();
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<Usuario, UsuarioUpdateDto>().ReverseMap();

            //ROL
            CreateMap<Rol, RolCreateDto>().ReverseMap();
            CreateMap<Rol, RolDto>().ReverseMap();
            CreateMap<Rol, RolUpdateDto>().ReverseMap();

            //USERROL
            CreateMap<UserRol, UserRolCreateDto>().ReverseMap();
            CreateMap<UserRol, UserRolDto>()
    .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Usuario.NombreCompleto))
    .ForMember(dest => dest.ApellidoCompleto, opt => opt.MapFrom(src => src.Usuario.ApellidoCompleto))
    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email))
    .ForMember(dest => dest.NombreRol, opt => opt.MapFrom(src => src.Rol.NombreRol));
            CreateMap<UserRol, UserRolUpdateDto>().ReverseMap();

            //PERMISO ROL
            CreateMap<PermRol, PermRolCreateDto>().ReverseMap();
            CreateMap<PermRol, PermRolDto>()
    .ForMember(dest => dest.NombrePermiso, opt => opt.MapFrom(src => src.Permiso.NombrePermiso))
    .ForMember(dest => dest.NombreRol, opt => opt.MapFrom(src => src.Rol.NombreRol));
            CreateMap<PermRol, PermRolUpdateDto>().ReverseMap();

            //PERMISO
            CreateMap<Permiso, PermisoCreateDto>().ReverseMap();
            CreateMap<Permiso, PermisoDto>().ReverseMap();
            CreateMap<Permiso, PermisoUpdateDto>().ReverseMap();



        }
    }
}

