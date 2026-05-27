using ListoAPI.Aplication.Core.Interfaces;
using ListoAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Text.Json;

namespace ListoAPI.API.Controllers
{
    [Route("api/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;

        public UsuarioController(IUsuarioRepository usuarioRepository, IMemoryCache cache, IEmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _cache = cache;
            _emailService = emailService;
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
                try 
                {
                    // 1. Correo de Confirmación de Cuenta
                    var asuntoBienvenida = "¡Bienvenido a Listo GO! Tu cuenta ha sido creada";
                    var cuerpoBienvenida = $@"
                        <div style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; border: 1px solid #e0e0e0; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.05); text-align: center;"">
                            <div style=""background-color: #FF6B00; padding: 25px 0;"">
                                <h1 style=""color: #ffffff; margin: 0; font-size: 36px; letter-spacing: 1px; font-weight: 900;"">Listo GO!</h1>
                            </div>
                            <div style=""padding: 40px 30px;"">
                                <h2 style=""color: #333333; margin-top: 0; font-size: 24px;"">¡Hola {pItem.Nombre}! 👋</h2>
                                <p style=""color: #555555; font-size: 16px; line-height: 1.6; margin: 20px 0;"">Tu cuenta ha sido creada exitosamente.</p>
                                <p style=""color: #555555; font-size: 16px; line-height: 1.6; margin: 20px 0;"">Estamos muy emocionados de tenerte con nosotros. Ya puedes empezar a disfrutar de todos nuestros servicios y beneficios exclusivos.</p>
                                <a href=""#"" style=""display: inline-block; background-color: #FF6B00; color: #ffffff; text-decoration: none; padding: 14px 30px; border-radius: 25px; font-weight: bold; font-size: 16px; margin-top: 20px;"">Comenzar ahora</a>
                            </div>
                            <div style=""background-color: #f5f5f5; padding: 15px; color: #888888; font-size: 12px;"">
                                <p style=""margin: 0;"">© 2026 Listo GO!. Todos los derechos reservados.</p>
                            </div>
                        </div>";
                    
                    // 2. Correo de Cupón de Descuento
                    var asuntoCupon = "🎁 Tu cupón de 15% de descuento está aquí";
                    var cuerpoCupon = $@"
                        <div style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; border: 1px solid #e0e0e0; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.05); text-align: center;"">
                            <div style=""background-color: #FF6B00; padding: 25px 0;"">
                                <h1 style=""color: #ffffff; margin: 0; font-size: 36px; letter-spacing: 1px; font-weight: 900;"">Listo GO!</h1>
                            </div>
                            <div style=""padding: 40px 30px;"">
                                <h2 style=""color: #333333; margin-top: 0; font-size: 24px;"">¡Gracias por unirte! 🎁</h2>
                                <p style=""color: #555555; font-size: 16px; line-height: 1.6; margin: 20px 0;"">Como lo prometimos, aquí tienes tu código de descuento especial para tu primera compra:</p>
                                
                                <div style=""margin: 30px auto; padding: 20px; border: 2px dashed #FF6B00; border-radius: 8px; background-color: #fff3e0; display: inline-block;"">
                                    <h2 style=""color: #FF6B00; margin: 0; font-size: 32px; letter-spacing: 4px; font-family: monospace; font-weight: bold;"">BIENVENIDA15</h2>
                                </div>
                                
                                <p style=""color: #555555; font-size: 16px; line-height: 1.6; margin: 20px 0;"">¡Aplica este código en el carrito de compras y disfruta de un <strong style=""color: #FF6B00;"">15% de descuento</strong>!</p>
                            </div>
                            <div style=""background-color: #f5f5f5; padding: 15px; color: #888888; font-size: 12px;"">
                                <p style=""margin: 0;"">© 2026 Listo GO!. Todos los derechos reservados.</p>
                            </div>
                        </div>";

                    await _emailService.EnviarCorreoAsync(pItem.Correo, asuntoBienvenida, cuerpoBienvenida);
                    await _emailService.EnviarCorreoAsync(pItem.Correo, asuntoCupon, cuerpoCupon);
                }
                catch (Exception ex)
                {
                    // Registramos el error pero no impedimos que se retorne Ok
                    var innerMsg = ex.InnerException != null ? ex.InnerException.Message : "No hay detalles adicionales";
                    Console.WriteLine($"Error al enviar correos de bienvenida: {ex.Message}. Detalle interno: {innerMsg}");
                }

                return Ok(resultado);
            }

            return BadRequest(resultado);
        }

        [AllowAnonymous]
        [HttpGet("dni/{dni}")]
        public async Task<IActionResult> ConsultarDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni) || dni.Length != 8)
            {
                return BadRequest(new ResponseCommonDTO { success = false, message = "El DNI debe tener exactamente 8 dígitos." });
            }

            // 1. Obtener identificadores del dispositivo o IP
            string deviceId = Request.Headers["X-Device-ID"].ToString()?.Trim();
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString()?.Trim();

            // Usamos Device ID como clave principal, o IP como respaldo
            string cacheKey = !string.IsNullOrEmpty(deviceId) 
                ? $"DniLimit_Device_{deviceId}" 
                : $"DniLimit_IP_{ipAddress}";

            // 2. Verificar y aplicar límite de 3 consultas
            if (_cache.TryGetValue(cacheKey, out int requestCount))
            {
                if (requestCount >= 10)
                {
                    return StatusCode(429, new ResponseCommonDTO 
                    { 
                        success = false, 
                        message = "Límite superado. Solo se permiten hasta 3 consultas de DNI por dispositivo/red cada 24 horas." 
                    });
                }
            }
            else
            {
                requestCount = 0;
            }

            try
            {
                // 3. Consultar el microservicio externo
                using (var httpClient = new HttpClient())
                {
                    var url = $"https://microservicio-reniec.onrender.com/dni/{dni}";
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        // 4. Incrementar contador en cache y establecer expiración de 24 horas
                        requestCount++;
                        _cache.Set(cacheKey, requestCount, TimeSpan.FromHours(24));

                        return Content(content, "application/json");
                    }

                    return NotFound(new ResponseCommonDTO 
                    { 
                        success = false, 
                        message = "No se encontraron datos para el DNI ingresado en la RENIEC." 
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseCommonDTO 
                { 
                    success = false, 
                    message = $"Error de comunicación con el servicio de RENIEC: {ex.Message}" 
                });
            }
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