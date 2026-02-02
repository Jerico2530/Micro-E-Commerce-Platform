using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServices.Domain.Dto.Usuario
{
    public class UsuarioCreateDto
    {

        public string NombreCompleto { get; set; } = null!;
        public string ApellidoCompleto { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Contraseña { get; set; } = null!;

        public string ContraseñaVisible { get; set; } = null!;

        public bool Estado { get; set; } = true;


    }
}
