using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022_SG_650_2022_PM_650.Models;

namespace P01_2022_SG_650_2022_PM_650.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class usuariosController : ControllerBase
    {
        private readonly ReservasContext _ReservasContext;

        public usuariosController(ReservasContext ReservasContext)
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
                List<Usuario> listadoUsuario = (from e in _ReservasContext.usuario select e).ToList();

                if (listadoUsuario.Count == 0)
                {
                    return NotFound();
                }
                return Ok(listadoUsuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error: {ex.Message}");
            }
        }

        // Para agregar un nuevo registro:

        [HttpPost]
        [Route("Add")]

        public IActionResult GuardarUsuario([FromBody] Usuario usuario)
        {
            try
            {
                _ReservasContext.usuario.Add(usuario);
                _ReservasContext.SaveChanges();
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Para actualizar un registro

        [HttpPut]
        [Route("actualizar/{id}")]

        public IActionResult ActualizarUsuario(int id, [FromBody] Usuario usuarioModificar)
        {
            Usuario? usuarioActual = (from e in _ReservasContext.usuario where e.id_usuario == id select e).FirstOrDefault();

            if (usuarioActual == null)
            { return NotFound(); }

            usuarioActual.nombre = usuarioModificar.nombre;
            usuarioActual.correo = usuarioModificar.correo;
            usuarioActual.telefono = usuarioModificar.telefono;
            usuarioActual.contrasena = usuarioModificar.contrasena;
            usuarioActual.rol = usuarioModificar.rol;

            _ReservasContext.Entry(usuarioActual).State = EntityState.Modified;
            _ReservasContext.SaveChanges();

            return Ok(usuarioModificar);
        }

        // Para Eliminar un registro

        [HttpDelete]
        [Route("eliminar/{id}")]

        public IActionResult EliminarUsuario(int id)
        {
            Usuario? usuario = (from e in _ReservasContext.usuario where e.id_usuario == id select e).FirstOrDefault();

            if (usuario == null)
            { return NotFound(); }

            _ReservasContext.usuario.Attach(usuario);
            _ReservasContext.usuario.Remove(usuario);
            _ReservasContext.SaveChanges();

            return Ok(usuario);
        }

        //endpoint para validar credenciales (usuario/contraseña) y retornar si son válidas o invalidas las credenciales.
        // en base a correo/contraseña

        [HttpPost]
        [Route("login")]

        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                return BadRequest("Correo o contraseña no pueden estar vacíos.");
            }
            var usuario = await _ReservasContext.usuario
                                        .Where(u => u.correo == correo)
                                        .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return Unauthorized("Correo no encontrado.");
            }

            if (usuario.contrasena != contrasena)
            {
                return Unauthorized("Contraseña incorrecta.");
            }

            return Ok(new { message = "Credenciales válidas", usuario = usuario.nombre });
        }


    }
}

