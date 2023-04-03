using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoERP.Models
{
    [Table("ALUMNOS")]
    public class Alumno
    {
        [Key]
        [Column("IDALUMNO")]
        public int IdAlumno { get; set; }
        [Column("DOCIDENTIDAD")]
        public string DocId { get; set; }
        [Column("NOMBRE")]
        public string NombreAlumno { get; set; }
        [Column("TLF")]
        public string Tlf { get; set; }
        [Column("EMAIL")]
        public string Email { get; set; }
        [Column("DIRECCION")]
        public string Direccion { get; set; }
        [Column("FOTO")]
        public string Foto { get; set; }
    }
}
