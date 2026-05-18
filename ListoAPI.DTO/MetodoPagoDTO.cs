namespace ListoAPI.DTO
{
    public class MetodoPagoDTO
    {
        public int IdMetodoPago { get; set; }

        public int IdUsuario { get; set; }
        public string Usuario { get; set; }

        public decimal Saldo { get; set; }
        
         public string MarcaTarjeta { get; set; }

        public string TokenSimulado { get; set; }

        public string UltimosDigitos { get; set; }

        public bool Estado {get;set;}

    }
}