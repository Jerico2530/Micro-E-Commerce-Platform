
namespace IdentityServices.Domain.Dto.PermRol;

public class PermRolDto
{

    public int PermRolId { get; set; }

    public int PermisoId { get; set; }

    public string  NombrePermiso { get; set; }

    public int RolId { get; set; }

    public string  NombreRol { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
