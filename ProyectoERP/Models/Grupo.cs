using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoERP.Models
{
    [Table("V_GRUPOS")]
    public class Grupo
    {
        [Key]
        [Column("CODGRUPO")]
        public string CodGrupo { get; set; }
        [Column("CURSO")]
        public string Curso { get; set; }
        [Column("TURNO")]
        public string Turno { get; set; }
        [Column("DIAS")]
        public string Dias { get; set; }
        [Column("FECHAINICIO")]
        public DateTime FechaInicio { get; set; }
        [Column("CODCURSO")]
        public string CodCurso { get; set; }
    }
}
