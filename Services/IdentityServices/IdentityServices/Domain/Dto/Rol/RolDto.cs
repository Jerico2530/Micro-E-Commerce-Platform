namespace IdentityServices.Domain.Dto.Rol
{
    public class RolDto
    {
        public int RolId { get; set; }
        public string NombreRol { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
