namespace IdentityServices.Domain.Dto.Usuario
{
    public class AuthResponseDto
    {
        public int UsuarioId { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Email { get; set; }

        public List<string> Roles { get; set; } = new();
        public List<string> Permisos { get; set; } = new();
        public string Token { get; set; } = string.Empty;
    }
}
