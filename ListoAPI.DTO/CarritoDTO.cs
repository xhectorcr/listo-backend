namespace ListoAPI.DTO
{
    public class CarritoItemDTO
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => Precio * Cantidad;
    }

    public class CarritoRequestDTO
    {
        public int UsuarioId { get; set; }
        public string YoloLabel { get; set; } = string.Empty;
        public decimal Confidence { get; set; }
    }
}
