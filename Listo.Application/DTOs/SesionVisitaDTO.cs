
namespace Listo.Application.DTOs
{
    public class SesionVisitaDTO
    {
        public int IdSesionVisita { get; set; }

        public int IdUsuario { get; set; }

        public string? NombreUsuario { get; set; }

        public DateTime? FechaEntrada { get; set; }

        public DateTime? FechaSalida { get; set; }

        public bool Activo { get; set; }
    }
}
