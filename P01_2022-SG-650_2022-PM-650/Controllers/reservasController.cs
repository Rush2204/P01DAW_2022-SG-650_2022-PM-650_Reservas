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

        /*
            Nota: Seguir el siguiente formato para colocar las fechas y el tiempo de forma correcta:

            Formato para fechas: 2025-02-28 

            Borrar: }
                    "hour": 0,
                    "minute": 0
                    }

            en lo siguiente:

            {
              "id_reserva": 0,
              "id_usuario": 0,
              "id_espacio": 0,
              "fecha": "2025-02-28",
              "horaInicio": {
                "hour": 0,
                "minute": 0
              },
              "cantidadHoras": 0,
              "estado": true
            }

            Y comolar la hora con el formato HH:mm : ejemplo "4:30"

            como se muestra a continuación:
            {
              "id_reserva": 0,
              "id_usuario": 0,
              "id_espacio": 0,
              "fecha": "2025-02-28",
              "horaInicio": "15:45",
              "cantidadHoras": 0,
              "estado": true
            }
        */

        // Permitir a un usuario reservar un espacio disponible, indicando fecha, hora y cantidad de horas a reservar.

        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarReserva([FromBody] Reserva reserva)
        {
            try
            {
                var espacioDisponible = (from r in _ReservasContext.reserva
                                         where r.id_espacio == reserva.id_espacio
                                               && r.Estado == true
                                               && r.fecha.Date == reserva.fecha.Date
                                               && r.horaInicio < reserva.horaInicio.AddMinutes(reserva.cantidadHoras * 60)
                                               && r.horaInicio.AddMinutes(r.cantidadHoras * 60) > reserva.horaInicio
                                         select r).FirstOrDefault();

                if (espacioDisponible != null)
                {
                    return BadRequest("El espacio seleccionado ya está reservado en el horario indicado.");
                }

                reserva.Estado = true; 
                _ReservasContext.reserva.Add(reserva);
                _ReservasContext.SaveChanges();

                return Ok(reserva);
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
