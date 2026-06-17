using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ListoAPI.Aplication.Core.Entities
{
    [Table("HistorialCompra")]
    public class HistorialCompra
    {
        [Key]
        [Column("id_historial")]
        public int IdHistorial { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("cantidad_items")]
        public int CantidadItems { get; set; }
    }
}
