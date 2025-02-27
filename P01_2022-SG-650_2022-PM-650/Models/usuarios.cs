using System.ComponentModel.DataAnnotations;

namespace P01_2022_SG_650_2022_PM_650.Models
{
    public class usuarios
    {
        [Key]
        public int id_usuario { get; set; }
        public string nombre { get; set; }
        public string correo { get; set; }
        public string telefono { get; set; }
        public string contrasena { get; set; }
        public string rol { get; set; }
    }
}
