using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace P01_2022_SG_650_2022_PM_650.Models
{
    public class Reserva
    {
        [Key]
        public int id_reserva { get; set; }
        public int id_usuario { get; set; }
        public int id_espacio { get; set; }
        public Date? fecha { get; set; }
        public TimeSpan horaInicio { get; set; }
        public int cantidadHoras { get; set; }
        public bool Estado { get; set; } // true = Activa, false = Cancelada
    }
}
