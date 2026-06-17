namespace Listo.Application.DTOs
{
    public class UsuarioDTO
    {
        public int IDUsuario { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public int? IdRol { get; set; } 
        public string Rol {get;set;}
        public string Nombre { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string Telefono { get; set; }
        public bool Estado { get; set; } = true;
        public string EstadoSesion { get; set; }
    }

    public class ValidarAccesoRequestDTO
    {
        public string PinTemporal { get; set; }
    }

    public class AsignarTrackRequestDTO
    {
        public int IdUsuario { get; set; }
        public string TrackId { get; set; }
    }
}
