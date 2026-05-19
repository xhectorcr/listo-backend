using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ListoAPI.Aplication.Core.Entities
{
    [Table("MetodoPago")]
    public class MetodoPago
    {

        [Key]
        [Column("id_metodopago")] 
        public int IdMetodoPago { get; set; }

        [Column("id_usuario")] 
        public int IdUsuario { get; set; }

        [Column("SaldoSimulado")] 
        public decimal Saldo { get; set; }

        [Column("TokenSimulado")] 
        public string TokenSimulado { get; set; }

        [Column("MarcaTarjeta")] 
        public string MarcaTarjeta { get; set; }

        [Column("ultimosDigitos")] 
        public string UltimosDigitos { get; set; }


        [Column("estado")]
        public bool Estado {get;set;}

        [ForeignKey("IdUsuario")]
public virtual Usuario Usuario { get; set; }

    }
}