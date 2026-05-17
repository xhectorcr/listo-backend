using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ListoAPI.Aplication.Core.Entities
{

    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("correo")]
        public string Correo { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("id_rol")]
        public int? IdRol { get; set; } 

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("fecha_registro")]
        public DateTime? FechaRegistro { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("estado")]
        public bool Estado { get; set; } = true;
    }
}