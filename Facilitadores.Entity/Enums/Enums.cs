using System.ComponentModel.DataAnnotations;

namespace Facilitadores.Entity.Enums
{
    public class Enums
    {
        public enum Tareas
        {
            [Display(Name = "Armar paquete para release")]
            ArmaPaqueteRelease = 1,
            [Display(Name = "Crear SPs de tabla")]
            CrearSps = 2
        }
    }
}
