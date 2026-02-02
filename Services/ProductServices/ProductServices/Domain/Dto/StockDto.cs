namespace ProductServices.Domain.Dto
{
    public class StockRequestDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }

    public class StockResponseDto
    {
        public int ProductoId { get; set; }
        public int StockActual { get; set; }
        public bool Disponible { get; set; }
    }
}
