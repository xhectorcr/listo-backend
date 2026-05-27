using ListoAPI.Aplication.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ListoAPI.Aplication.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoAsync(string para, string asunto, string cuerpoHtml)
        {
            var emailEmisor = _config["EmailSettings:Email"];
            
            // Leemos el password de la configuración, y si no está, buscamos en la variable de entorno directamente (para Render)
            var password = _config["EmailSettings:Password"] 
                        ?? Environment.GetEnvironmentVariable("EmailSettings__Password")
                        ?? Environment.GetEnvironmentVariable("EmailSettings_Password")
                        ?? Environment.GetEnvironmentVariable("EMAILSETTINGS__PASSWORD");
                        
            var host = _config["EmailSettings:Host"];
            
            // Si el puerto no está en appsettings, se asume un default (ej. 587)
            if (!int.TryParse(_config["EmailSettings:Port"], out int port))
            {
                port = 587; 
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailEmisor, "Listo! GO"),
                Subject = asunto,
                Body = cuerpoHtml,
                IsBodyHtml = true
            };

            mailMessage.To.Add(para);

            using (var smtpClient = new SmtpClient(host, port))
            {
                smtpClient.Credentials = new NetworkCredential(emailEmisor, password);
                smtpClient.EnableSsl = true;
                
                // Enviar de forma asincrónica
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
