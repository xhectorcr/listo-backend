
namespace Listo.Application.DTOs
{
    public class ResponseCommonDTO
    {
        public bool success { get; set; }
        public string message { get; set; } = string.Empty;

         public object? data { get; set; } // Aquí puedes enviar cualquier información adicional
    }
}
