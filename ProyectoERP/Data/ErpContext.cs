using Microsoft.EntityFrameworkCore;
using ProyectoERP.Models;
//using System.Security.Cryptography.X509Certificates;

namespace ProyectoERP.Data
{
    public class ErpContext: DbContext
    {
        public ErpContext(DbContextOptions<ErpContext> options) : base(options) { }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ClientePotencial> ClientesPotenciales { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<AlumnoPagos> AlumnosPagos { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<Factura> Facturas { get; set; }
    }
    
}
