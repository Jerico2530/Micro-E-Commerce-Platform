namespace ProductServices.Domain.Dto
{
    public class ProductoCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool Estado { get; set; }

    }
}
