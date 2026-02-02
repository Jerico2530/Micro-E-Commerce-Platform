namespace IdentityServices.Api.Dto
{
    public class LoginResultDto
    {
        public string Token { get; set; }
        public UsuarioInfoDto Usuario { get; set; }
    }

    /// <summary>
    /// DTO que encapsula la información personal y permisos del usuario.
    /// </summary>
    public class UsuarioInfoDto
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; }
        public string ApellidoCompleto { get; set; }
        public string DNI { get; set; }
        public string Email { get; set; }
        public string Imagen { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Permisos { get; set; }

    }
}
    

