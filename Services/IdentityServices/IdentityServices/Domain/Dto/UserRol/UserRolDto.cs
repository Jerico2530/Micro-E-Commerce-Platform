namespace IdentityServices.Domain.Dto.UserRol
{
    public class UserRolDto
    {
        public int UserRolId { get; set; }
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; }
        public string? ApellidoCompleto { get; set; }
        public string? DNI { get; set; }
        public string? Email { get; set; }
        public int RolId { get; set; }
        public string NombreRol { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
