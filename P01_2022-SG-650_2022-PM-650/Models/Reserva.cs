using System.ComponentModel.DataAnnotations;


namespace P01_2022_SG_650_2022_PM_650.Models
{
    public class Reserva
    {
        [Key]
        public int id_reserva { get; set; }
        public int id_usuario { get; set; }
        public int id_espacio { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime fecha { get; set; }
        public TimeOnly horaInicio { get; set; }
        public int cantidadHoras { get; set; }
        public bool Estado { get; set; } // true = Activa, false = Cancelada
    }
}
