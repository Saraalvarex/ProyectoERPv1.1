using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoERP.Models
{
    [Table("USUARIOS")]
    public class Usuario
    {
        [Key]
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }
        [Column("NOMBRE")]
        public string NombreUsuario { get; set; }
        [Column("EMAIL")]
        public string Email { get; set; }
        [Column("CLAVE")]
        //Los tipos varbinary, clob, blob
        //son convertidos automaticamente a byte[]
        //por entity framework
        public string Clave { get; set; }
        [Column("CLAVEENCRIP")]
        //Los tipos varbinary, clob, blob
        //son convertidos automaticamente a byte[]
        //por entity framework
        public byte[] ClaveEncrip { get; set; }
        [Column("SALT")]
        public string Salt { get; set; }
        [Column("ROL")]
        public string Rol { get; set; }
        [Column("FOTO")]
        public string? Foto { get; set; }
    }
}
