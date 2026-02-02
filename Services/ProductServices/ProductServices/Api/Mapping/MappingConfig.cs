using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductServices.Domain.Dto;
using ProductServices.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProductServices.Api.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            //PRODUCTO
            CreateMap<Producto, ProductoCreateDto>().ReverseMap();
            CreateMap<Producto, ProductoDto>().ReverseMap();
            CreateMap<Producto, ProductoUpdateDto>().ReverseMap();

  
        }
    }
}
