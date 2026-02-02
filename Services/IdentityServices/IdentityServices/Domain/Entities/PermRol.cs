using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServices.Domain.Entities
{
    public class PermRol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PermRolId { get; set; }

        [Required]
        public int PermisoId { get; set; }

        [ForeignKey(nameof(PermisoId))]
        public Permiso Permiso { get; set; } = null!;

        [Required]
        public int RolId { get; set; }

        [ForeignKey(nameof(RolId))]
        public Rol Rol { get; set; } = null!;

        public bool Estado { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
