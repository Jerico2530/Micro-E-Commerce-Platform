namespace IdentityServices.Domain.Dto.Usuario
{
    public class UsuarioDto
    {
        public int UsuarioId { get; set; }

        public string NombreCompleto { get; set; } = null!;
        public string ApellidoCompleto { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Contraseña { get; set; } = null!;

        public string ContraseñaVisible { get; set; } = null!;

        public bool Estado { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}

