namespace Listo.Application.DTOs
{
    public class ProductoDTO
    {

        public int IDProducto { get; set; }

        public  string Nombre { get; set; }

        public bool Activo {get;set;}

        public  string? Descripcion { get; set; }

        public string? Imagen { get; set; }

        public  string YoloLabel { get; set; }

        public int Stock { get; set; }

        public decimal Precio {get; set;}
    

        public int IDCategoria { get; set; }

        public string? Categoria {get; set;}

    }
}
