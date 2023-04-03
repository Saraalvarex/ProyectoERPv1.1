using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoERP.Models
{
    [Table("CURSOS")]
    public class Curso
    {
        [Key]
        [Column("CODCURSO")]
        public string CodCurso {get; set;}
        [Column("NOMBRE")]
        public string NombreCurso { get; set; }
        [Column("MATRICULA")]
        public decimal Matricula { get; set; }
        [Column("PRECIO")]
        public decimal Precio { get; set; }
        [Column("DURACION")]
        public int Duracion { get; set; }
    }
}
