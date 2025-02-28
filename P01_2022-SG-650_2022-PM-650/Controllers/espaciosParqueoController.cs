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
    }
}
