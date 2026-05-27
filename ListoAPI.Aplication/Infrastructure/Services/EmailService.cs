using ListoAPI.Aplication.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ListoAPI.Aplication.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private static readonly HttpClient _httpClient = new HttpClient(); // Se recomienda reutilizar HttpClient

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoAsync(string para, string asunto, string cuerpoHtml)
        {
            // Usaremos la variable donde tenías tu contraseña para guardar la URL del Script de Google
            var scriptUrl = _config["EmailSettings:Password"];
            if (string.IsNullOrWhiteSpace(scriptUrl) || !scriptUrl.StartsWith("https://script.google.com/"))
            {
                scriptUrl = Environment.GetEnvironmentVariable("EmailSettings__Password")
                            ?? Environment.GetEnvironmentVariable("EmailSettings_Password")
                            ?? Environment.GetEnvironmentVariable("EMAILSETTINGS__PASSWORD");
            }
            
            scriptUrl = scriptUrl?.Trim();

            if (string.IsNullOrWhiteSpace(scriptUrl))
            {
                throw new Exception("Falta configurar la URL del Google Apps Script.");
            }

            var payload = new
            {
                para = para,
                asunto = asunto,
                cuerpoHtml = cuerpoHtml
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Esto hace una petición HTTPS (puerto 443) hacia Google, que Render JAMÁS bloqueará.
            var response = await _httpClient.PostAsync(scriptUrl, content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                throw new Exception($"Fallo al enviar correo mediante Google Script: {errorText}");
            }
        }
    }
}
