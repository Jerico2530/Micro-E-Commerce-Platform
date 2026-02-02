using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServices.Domain.Entities
{
    public class Rol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RolId { get; set; }
        [Required]
        [MaxLength(100)]
        public string NombreRol { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public ICollection<UserRol> UserRoles { get; set; } = new List<UserRol>();
        public ICollection<PermRol> PermRoles { get; set; } = new List<PermRol>();

    }
}
