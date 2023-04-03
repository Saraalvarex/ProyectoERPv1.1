using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoERP.Models
{
    [Table("FACTURAS")]
    public class Factura
    {
        [Key]
        [Column("IDALUMNO")]
        public int IdAlumno { get; set; }
        [Column("FACTURA")]
        public string Fact { get; set; }
        [Column("CODFACTURA")]
        public int CodFact { get; set; }
    }
}
