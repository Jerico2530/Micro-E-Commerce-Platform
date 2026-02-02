using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServices.Domain.Dto.Permiso
{
    public class PermisoDto
    {

        public int PermisoId { get; set; }

        public string NombrePermiso { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
