namespace ProductServices.Domain.Dto
{
    public class ProductoDto
    {
        public int ProductoId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    }
}
