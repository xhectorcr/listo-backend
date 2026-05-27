using ListoAPI.Aplication.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

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
            // Usaremos tu cuenta de Gmail original
            var emailEmisor = _config["EmailSettings:Email"] ?? "angelinatipacti@gmail.com";
            
            var password = _config["EmailSettings:Password"];
            if (string.IsNullOrWhiteSpace(password))
            {
                password = Environment.GetEnvironmentVariable("EmailSettings__Password")
                            ?? Environment.GetEnvironmentVariable("EmailSettings_Password")
                            ?? Environment.GetEnvironmentVariable("EMAILSETTINGS__PASSWORD");
            }
            password = password?.Trim();

            var host = _config["EmailSettings:Host"] ?? "smtp.gmail.com";
            if (!int.TryParse(_config["EmailSettings:Port"], out int port))
            {
                port = 587; 
            }

            // Construimos el mensaje con MimeKit
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Listo! GO", emailEmisor));
            message.To.Add(new MailboxAddress("", para)); // Se envía a cualquier correo
            message.Subject = asunto;

            var bodyBuilder = new BodyBuilder { HtmlBody = cuerpoHtml };
            message.Body = bodyBuilder.ToMessageBody();

            // Enviamos usando MailKit (que resuelve automáticamente el error de "Network unreachable" de IPv6)
            using (var client = new SmtpClient())
            {
                // El socket interrumpe si tarda más de 10 segundos
                client.Timeout = 10000;
                
                // Conectamos de forma segura
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                
                // Autenticación con tu clave generada de Google
                await client.AuthenticateAsync(emailEmisor, password);
                
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
