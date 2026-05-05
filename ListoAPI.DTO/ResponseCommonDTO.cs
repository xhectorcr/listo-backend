
namespace ListoAPI.DTO
{
    public class ResponseCommonDTO
    {
        public bool success { get; set; }
        public string message { get; set; }

         public object? data { get; set; } // Aquí puedes enviar cualquier información adicional
    }
}
