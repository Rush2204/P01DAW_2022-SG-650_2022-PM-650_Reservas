using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P01_2022_SG_650_2022_PM_650.Models;
using Microsoft.EntityFrameworkCore;

namespace P01_2022_SG_650_2022_PM_650.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class reservasController : ControllerBase
    {
        private readonly ReservasContext _ReservasContext;

        public reservasController(ReservasContext BlogDBContext)
        {
            _ReservasContext = BlogDBContext;
        }

        // Para poder ver todos los registros:

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<Reserva> listadoReserva = (from e in _ReservasContext.reserva select e).ToList();

            if (listadoReserva.Count == 0)
            {
                return NotFound();
            }
            return Ok(listadoReserva);
        }

        // Para agregar un nuevo registro:

        [HttpPost]
        [Route("Add")]

        public IActionResult GuardarReserva([FromBody] Reserva reservas)
        {
            try
            {
                _ReservasContext.reserva.Add(reservas);
                _ReservasContext.SaveChanges();
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Para actualizar un registro

        [HttpPut]
        [Route("actualizar/{id}")]

        public IActionResult ActualizarReserva(int id, [FromBody] Reserva reservaModificar)
        {
            Reserva? reservaActual = (from e in _ReservasContext.reserva where e.id_reserva == id select e).FirstOrDefault();

            if (reservaActual == null)
            { return NotFound(); }

            reservaActual.id_usuario = reservaModificar.id_usuario;
            reservaActual.id_espacio = reservaModificar.id_espacio;
            reservaActual.fecha = reservaModificar.fecha;
            reservaActual.horaInicio = reservaModificar.horaInicio;
            reservaActual.cantidadHoras = reservaModificar.cantidadHoras;
            reservaActual.Estado = reservaModificar.Estado;

            _ReservasContext.Entry(reservaActual).State = EntityState.Modified;
            _ReservasContext.SaveChanges();

            return Ok(reservaModificar);
        }

        // Para Eliminar un registro

        [HttpDelete]
        [Route("eliminar/{id}")]

        public IActionResult EliminarReserva(int id)
        {
            Reserva? reserva = (from e in _ReservasContext.reserva where e.id_reserva == id select e).FirstOrDefault();

            if (reserva == null)
            { return NotFound(); }

            _ReservasContext.reserva.Attach(reserva);
            _ReservasContext.reserva.Remove(reserva);
            _ReservasContext.SaveChanges();

            return Ok(reserva);
        }

        //Mostrar una lista de reservas activas del usuario.

        [HttpPost]
        [Route("reservas/activas/{id_usuario}")]
        public IActionResult ReservasActivas(int id_usuario) 
        {
            try
            {
                var reservasActivas = from reserva in _ReservasContext.reserva
                                      join usuario in _ReservasContext.usuario on reserva.id_usuario equals usuario.id_usuario
                                      where reserva.id_usuario == id_usuario && reserva.Estado == true
                                      select new
                                      {
                                          reserva.id_reserva,
                                          reserva.id_usuario,
                                          nombre_usuario = usuario.nombre,  
                                          reserva.id_espacio,
                                          reserva.fecha,
                                          reserva.horaInicio,
                                          reserva.cantidadHoras,
                                          reserva.Estado
                                      };

                var listaReservas = reservasActivas.ToList();

                if (listaReservas.Count == 0)
                {
                    return NotFound(new { message = "No se encontraron reservas activas para este usuario." });
                }

                return Ok(listaReservas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al obtener las reservas activas: {ex.Message}");
            }
        }

        //Cancelar una reserva antes de su uso

        [HttpPut]
        [Route("reservas/cancelar/{id_reserva}")]
        public IActionResult CancelarReserva(int id_reserva)
        {
            try
            {
                var reserva = _ReservasContext.reserva.FirstOrDefault(r => r.id_reserva == id_reserva);

                if (reserva == null)
                {
                    return NotFound(new { message = "Reserva no encontrada." });
                }

                // Se cambia el estado :)
                reserva.Estado = false;
                _ReservasContext.SaveChanges();

                return Ok(new { message = "Reserva cancelada con éxito." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al cancelar la reserva: {ex.Message}");
            }
        }



    }
}
