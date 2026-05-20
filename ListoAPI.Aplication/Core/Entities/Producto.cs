using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ListoAPI.Aplication.Core.Entities
{
    [Table("Producto")]
    public class Producto
    {

        [Key]
        [Column("id_producto")] 
        public int IdProducto { get; set; }

        [Column("nombre")] 
        public required string Nombre { get; set; }

        [Column("activo")]
        public bool Activo {get;set;}

        [Column("descripcion")] 
        public  string? Descripcion { get; set; }

        [Column("imagen")] 
        public string? Imagen { get; set; }

        [Column("yolo_label")] 
        public required string YoloLabel { get; set; }

        [Column("stock")] 
        public int Stock { get; set; }

        [Column("precio")] 
         public decimal Precio {get; set;}

        [Column("id_categoria")] 
        public int IdCategoria { get; set; }

        // --- CONSTRAINT AGREGADO ---
        [ForeignKey("IdCategoria")]
        public virtual Categoria Categoria { get; set; }

        

    }
}