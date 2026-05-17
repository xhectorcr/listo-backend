using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace ListoWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DescuentoController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DescuentoController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("enviar-cupon")]
        public IActionResult EnviarCupon([FromBody] string emailCliente)
        {
            try 
            {
                // Lee los datos que acabas de poner en appsettings.json
                var emailEmisor = _config["EmailSettings:Email"];
                var password = _config["EmailSettings:Password"];
                var host = _config["EmailSettings:Host"];
                var port = int.Parse(_config["EmailSettings:Port"]);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailEmisor, "Listo! GO "),
                    Subject = "🎁 Tu cupón de 15% de descuento está aquí",
                    Body = $@"
                        <h1>¡Gracias por registrarte en Listo!</h1>
                        <p>Como prometimos, aquí tienes tu código de descuento para tu primera compra:</p>
                        <h2 style='color: #2e7d32;'>BIENVENIDA15</h2>
                        <p>¡Esperamos verte pronto!</p>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(emailCliente);

                using (var smtpClient = new SmtpClient(host, port))
                {
                    smtpClient.Credentials = new NetworkCredential(emailEmisor, password);
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mailMessage);
                }

                return Ok(new { mensaje = "Cupón enviado con éxito a " + emailCliente });
            }
            catch (Exception ex)
            {
                // Si algo falla (ej. contraseña mal copiada), te avisará aquí
                return BadRequest(new { error = "Error al enviar: " + ex.Message });
            }
        }
    }
}