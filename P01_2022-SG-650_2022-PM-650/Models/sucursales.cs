using System.ComponentModel.DataAnnotations;

namespace P01_2022_SG_650_2022_PM_650.Models
{
    public class sucursales
    {
        [Key]
        public int id_sucursal { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string administrador { get; set; }
        public string numeroEspacio { get; set; }
    }
}
