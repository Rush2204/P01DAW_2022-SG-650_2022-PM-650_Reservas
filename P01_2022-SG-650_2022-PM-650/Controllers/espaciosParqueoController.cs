using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022_SG_650_2022_PM_650.Models;

namespace P01_2022_SG_650_2022_PM_650.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class espaciosParqueoController : ControllerBase
    {
        private readonly ReservasContext _ReservasContext;

        public espaciosParqueoController(ReservasContext BlogDBContext)
        {
            _ReservasContext = BlogDBContext;
        }

        // Para poder ver todos los registros:

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Index()
        {
            List<EspaciosParqueo> listadoespacios = (from e in _ReservasContext.espaciosParqueo select e).ToList();

            if (listadoespacios.Count == 0)
            {
                return NotFound();
            }
            return Ok(listadoespacios);
        }


        // Para agregar un nuevo registro:

        [HttpPost]
        [Route("Add")]

        public IActionResult GuardarEspacioParqueo([FromBody] EspaciosParqueo espaciosParqueo)
        {
            try
            {
                _ReservasContext.espaciosParqueo.Add(espaciosParqueo);
                _ReservasContext.SaveChanges();
                return Ok(espaciosParqueo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Para actualizar un registro

        [HttpPut]
        [Route("actualizar/{id}")]

        public IActionResult ActualizarEspacioParqueo(int id, [FromBody] EspaciosParqueo espacioModificar)
        {
            EspaciosParqueo? espacioActual = (from e in _ReservasContext.espaciosParqueo where e.id_espacio == id select e).FirstOrDefault();

            if (espacioActual == null)
            { return NotFound(); }

            espacioActual.id_sucursal = espacioModificar.id_sucursal;
            espacioActual.numero = espacioModificar.numero;
            espacioActual.ubicacion = espacioModificar.ubicacion;
            espacioActual.costoHora = espacioModificar.costoHora;
            espacioActual.estado = espacioModificar.estado;


            _ReservasContext.Entry(espacioActual).State = EntityState.Modified;
            _ReservasContext.SaveChanges();

            return Ok(espacioModificar);
        }

        // Para Eliminar un registro

        [HttpDelete]
        [Route("eliminar/{id}")]

        public IActionResult EliminarEspacioParqueo(int id)
        {
            EspaciosParqueo? espacios = (from e in _ReservasContext.espaciosParqueo where e.id_espacio == id select e).FirstOrDefault();

            if (espacios == null)
            { return NotFound(); }

            _ReservasContext.espaciosParqueo.Attach(espacios);
            _ReservasContext.espaciosParqueo.Remove(espacios);
            _ReservasContext.SaveChanges();

            return Ok(espacios);
        }

        // NOTA: Usar el siguiente formato para poner fechas: "2025-03-01T00:00:00" 0 "2025-03-01"

        // Mostrar una lista de todos los espacios de parqueo disponibles para reservar por día.

        [HttpGet]
        [Route("GetListaDeEspaciosDisponibleEnUnDía")]
        public IActionResult GetListaDeEspaciosDisponibleEnUnDía([FromQuery] DateTime fechaReserva)
        {
            var espaciosDisponibles = (from e in _ReservasContext.espaciosParqueo
                                       where !(from r in _ReservasContext.reserva
                                               where r.id_espacio == e.id_espacio && r.Estado == true && r.fecha.Date == fechaReserva.Date
                                               select r).Any()
                                       select new
                                       {
                                           e.id_espacio,
                                           e.id_sucursal,
                                           e.numero,
                                           e.ubicacion,
                                           e.costoHora,
                                           e.estado,
                                       }).ToList();

            if (espaciosDisponibles.Count == 0)
            {
                return NotFound($"No hay espacios de parqueo disponibles para la fecha {fechaReserva.ToShortDateString()}.");
            }

            return Ok(espaciosDisponibles);
        }

        // Mostrar una lista de los espacios reservados por día de todas las sucursales.

        [HttpGet]
        [Route("GetReservasDeEspaciosPorFecha")]
        public IActionResult GetReservasDeEspaciosPorFecha([FromQuery] DateTime fechaReserva)
        {
            var espaciosReservados = (from r in _ReservasContext.reserva
                                      join e in _ReservasContext.espaciosParqueo
                                      on r.id_espacio equals e.id_espacio
                                      where r.Estado == true && r.fecha.Date == fechaReserva.Date
                                      select new
                                      {
                                          r.id_reserva,
                                          r.id_usuario,
                                          r.id_espacio,
                                          r.fecha,
                                          r.horaInicio,
                                          r.cantidadHoras,
                                          e.numero,
                                          e.ubicacion,
                                          e.estado
                                      }).ToList();

            if (espaciosReservados.Count == 0)
            {
                return NotFound($"No hay espacios reservados para la fecha {fechaReserva.ToShortDateString()}.");
            }

            return Ok(espaciosReservados);
        }

        // Mostrar una lista de los espacios reservados entre dos fechas dadas de una sucursal especifica.

        [HttpGet]
        [Route("GetEspaciosReservadosEntreDosFechas/{id_sucursal}")]
        public IActionResult GetEspaciosReservadosEntreDosFechas(int id_sucursal, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var espaciosReservados = (from r in _ReservasContext.reserva
                                      join e in _ReservasContext.espaciosParqueo
                                      on r.id_espacio equals e.id_espacio
                                      where r.id_espacio == id_sucursal && r.Estado == true && r.fecha.Date >= startDate.Date && r.fecha.Date <= endDate.Date
                                      select new
                                      {
                                          r.id_reserva,
                                          r.id_usuario,
                                          r.id_espacio,
                                          r.fecha,
                                          r.horaInicio,
                                          r.cantidadHoras,
                                          e.numero,
                                          e.ubicacion,
                                          e.estado
                                      }).ToList();

            if (espaciosReservados.Count == 0)
            {
                return NotFound("No hay espacios reservados en este rango de fechas para la sucursal.");
            }

            return Ok(espaciosReservados);
        }

    }
}
