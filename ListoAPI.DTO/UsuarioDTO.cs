namespace ListoAPI.DTO
{
    public class UsuarioDTO
    {
        public int IDUsuario { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public int? IdRol { get; set; } 
        public string? Rol {get;set;}
        public string Nombre { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string Telefono { get; set; }
        public bool Estado { get; set; } = true;
    }
}