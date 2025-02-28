using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022_SG_650_2022_PM_650.Models;

namespace P01_2022_SG_650_2022_PM_650.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class sucursalesController : ControllerBase
    {
        private readonly ReservasContext _ReservasContext;

        public sucursalesController(ReservasContext ReservasContext)
        {
            _ReservasContext = ReservasContext;
        }

        // Para poder ver todos los registros:

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Index()
        {
            try
            {
                List<Sucursal> listadoSucursal = (from e in _ReservasContext.sucursal select e).ToList();

                if (listadoSucursal.Count == 0)
                {
                    return NotFound();
                }
                return Ok(listadoSucursal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error: {ex.Message}");
            }
        }

        // Para agregar un nuevo registro:

        [HttpPost]
        [Route("Add")]

        public IActionResult GuardarSucursal([FromBody] Sucursal sucursal)
        {
            try
            {
                _ReservasContext.sucursal.Add(sucursal);
                _ReservasContext.SaveChanges();
                return Ok(sucursal);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Para actualizar un registro

        [HttpPut]
        [Route("actualizar/{id}")]

        public IActionResult ActualizarSucursal(int id, [FromBody] Sucursal sucursalModificar)
        {
            Sucursal? sucursalActual = (from e in _ReservasContext.sucursal where e.id_sucursal == id select e).FirstOrDefault();

            if (sucursalActual == null)
            { return NotFound(); }

            sucursalActual.nombre = sucursalActual.nombre;
            sucursalActual.direccion = sucursalActual.direccion;
            sucursalActual.telefono = sucursalActual.telefono;
            sucursalActual.administrador = sucursalActual.administrador;
            sucursalActual.numeroEspacio = sucursalActual.numeroEspacio;

            _ReservasContext.Entry(sucursalActual).State = EntityState.Modified;
            _ReservasContext.SaveChanges();

            return Ok(sucursalModificar);
        }

        // Para Eliminar un registro

        [HttpDelete]
        [Route("eliminar/{id}")]

        public IActionResult EliminarSucursal(int id)
        {
            Sucursal? sucursal = (from e in _ReservasContext.sucursal where e.id_sucursal == id select e).FirstOrDefault();

            if (sucursal == null)
            { return NotFound(); }

            _ReservasContext.sucursal.Attach(sucursal);
            _ReservasContext.sucursal.Remove(sucursal);
            _ReservasContext.SaveChanges();

            return Ok(sucursal);
        }


    }
}

