using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServices.Domain.Entities
{
    public class Permiso
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PermisoId { get; set; }

        [Required]
        [MaxLength(100)]
        public string NombrePermiso { get; set; } = null!;

        public bool Estado { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public ICollection<PermRol> PermRoles { get; set; } = new List<PermRol>();
    }
}
