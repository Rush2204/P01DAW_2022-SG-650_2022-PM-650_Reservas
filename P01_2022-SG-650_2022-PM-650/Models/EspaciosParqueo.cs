using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace P01_2022_SG_650_2022_PM_650.Models
{
    public class EspaciosParqueo
    {
        [Key]
        public int id_espacio { get; set; }

        public int id_sucursal { get; set; }

        public int numero { get; set; }
        public string? ubicacion { get; set; }

        public decimal costoHora { get; set; }
        public bool estado { get; set; } // true = Ocupado, false = Disponible
    }
}
