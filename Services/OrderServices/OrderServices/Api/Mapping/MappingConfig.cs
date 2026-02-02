using AutoMapper;
using OrderServices.Domain.Dto.Orden;
using OrderServices.Domain.Dto.OrdenDetalle;
using OrderServices.Domain.Entities;

namespace OrderServices.Api.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            //ORDEN
            CreateMap<Orden, OrdenCreateDto>().ReverseMap();
            CreateMap<Orden, OrdenDto>()
    .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
    .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId));
            CreateMap<Orden, OrdenUpdateDto>().ReverseMap();

            //ORDENDETALLE
            CreateMap<OrdenDetalle, OrdenDetalleCreateDto>().ReverseMap();
            CreateMap<OrdenDetalle, OrdenDetalleDto>().ReverseMap();
            CreateMap<OrdenDetalle, OrdenDetalleUpdateDto>().ReverseMap();




        }
    }
}
