using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ListoAPI.Aplication.Core.Entities
{
    [Table("Categoria")]
    public class Categoria
    {

        [Key]
        [Column("id_categoria")] 
        public int IdCategoria { get; set; }

        [Column("nombre")] 
        public string Nombre { get; set; }

        [Column("estado")]
        public bool Activo {get;set;}

    }
}