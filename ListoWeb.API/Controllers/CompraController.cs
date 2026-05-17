using Microsoft.AspNetCore.Mvc;
using ListoAPI.DTO;
using System;

namespace ListoWeb.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompraController : ControllerBase
    {
        [HttpPost("validar-qr")]
        public IActionResult ValidarQrDinamico([FromBody] QrValidationRequestDTO request)
        {

            long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (currentTimestamp > request.ExpirationTimestamp)
            {
                return BadRequest(new ResponseCommonDTO 
                { 
                    success = false, 
                    message = "El código QR ha expirado. Esto podría ser una captura de pantalla. Pide al cliente que muestre el código actual." 
                });
            }

            return Ok(new ResponseCommonDTO 
            { 
                success = true, 
                message = "QR válido. Compra procesada con éxito." 
            });
        }
    }
}