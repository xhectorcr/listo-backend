using ListoAPI.Aplication.Core.Interfaces;
using ListoAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListoAPI.API.Controllers
{
    [Route("api/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {

            if (string.IsNullOrWhiteSpace(loginRequest.Correo) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest(new ResponseCommonDTO { success = false, message = "El correo y la contraseña son obligatorios." });
            }
            var resultado = await _usuarioRepository.ValidarLogin(loginRequest.Correo, loginRequest.Password);

            if (resultado.success)
            {
                return Ok(resultado);
            }

            return Unauthorized(resultado);
        }


        [AllowAnonymous]
        [HttpPost("registrarCliente")]
        public async Task<IActionResult> RegistrarCliente([FromBody] RegistroClienteDTO pItem)
        {

            if (pItem == null)
            {
                return BadRequest(new ResponseCommonDTO { success = false, message = "Los datos del cliente son nulos." });
            }

            var resultado = await _usuarioRepository.RegisterClientAsync(pItem);

            if (resultado.success)
            {
                return Ok(resultado);
            }

            return BadRequest(resultado);
        }

        [Route("lista/activos")]
        [HttpGet]
        public async Task<IActionResult> GetUsuariosLista(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string pSearch = "",
        [FromQuery] int idRol = 0)
        {
            try
            {
            
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var (usuarios, totalCount) = await _usuarioRepository.GetUsuarioList(
                    pageNumber,
                    pageSize,
                    pSearch,
                    idRol
                );

                return Ok(new
                {
                    data = usuarios,
                    pageNumber,
                    pageSize,
                    totalCount,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                // 4. Manejo de errores
                return BadRequest(new ResponseCommonDTO
                {
                    success = false,
                    message = $"Error al obtener la lista de usuarios: {ex.Message}"
                });
            }
        }


        [AllowAnonymous]
        [HttpPost("crearUsuario")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] UsuarioDTO pItem)
        {

            if (pItem == null)
            {
                return BadRequest(new ResponseCommonDTO { success = false, message = "Los datos del usuario son nulos." });
            }
            var resultado = await _usuarioRepository.saveItem(pItem);

            if (resultado.success)
            {
                return Ok(resultado);
            }

            return BadRequest(resultado);
        }


    }



}
