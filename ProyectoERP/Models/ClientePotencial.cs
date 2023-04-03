using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoERP.Models
{
    [Table("V_INTERESADOS_CURSOS")]
    public class ClientePotencial
    {
        [Key]
        [Column("IDINTERESADO")]
        public int IdInteresado { get; set;}
        [Column("NOMBRE")]
        public string NombreCliente { get; set; }
        [Column("TLF")]
        public string Tlf { get; set; }
        [Column("EMAIL")]
        public string Email { get; set; }
        [Column("COMENTARIOS")]
        public string Comentarios { get; set; }
        [Column("CURSO")]
        public string Curso { get; set; }
    }
}
