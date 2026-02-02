namespace ProductServices.Domain.Dto
{
    public class ProductoUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Estado { get; set; }
    }
}
