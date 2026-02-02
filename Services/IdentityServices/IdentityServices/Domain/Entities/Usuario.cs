using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServices.Domain.Entities
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(100)]
        public string NombreCompleto { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string ApellidoCompleto { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        public string Contraseña { get; set; } = null!;
        [Required]
        public string ContraseñaVisible { get; set; } = null!;

        public bool Estado { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public ICollection<UserRol> UserRoles { get; set; } = new List<UserRol>();
    }
}

