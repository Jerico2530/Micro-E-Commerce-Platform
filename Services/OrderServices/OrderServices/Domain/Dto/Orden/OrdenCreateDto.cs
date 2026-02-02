using OrderServices.Domain.Dto.OrdenDetalle;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderServices.Domain.Dto.Orden
{
    public class OrdenCreateDto
    {
        public List<OrdenDetalleCreateDto> Items { get; set; }
    }
}
