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
        // 1. Inyección de Dependencias del Repositorio
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }
        
        [AllowAnonymous] // Permite entrar sin token
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


        [AllowAnonymous] // Permite entrar sin token
        [HttpPost("registrar")]
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
