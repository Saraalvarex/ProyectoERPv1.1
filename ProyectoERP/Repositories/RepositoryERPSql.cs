using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProyectoERP.Data;
using ProyectoERP.Helpers;
using ProyectoERP.Models;
using System.Collections.Generic;
using System.Data;
using System.Net;

#region VISTAS y PROCEDURES
//CREATE VIEW V_INTERESADOS_CURSOS
//AS
//SELECT I.IDINTERESADO, I.NOMBRE, I.TLF, I.EMAIL, I.COMENTARIOS, C.NOMBRE AS CURSO
//FROM INTERESADOS I
//INNER JOIN CURSOS C ON I.CODCURSO = C.CODCURSO
//GO

//SELECT * FROM V_INTERESADOS_CURSOS

//CREATE PROCEDURE SP_INSERT_CLIENTEP
//(@NOMBRE NVARCHAR(50), @TLF NVARCHAR(10), @EMAIL NVARCHAR(50),
//@COMENTARIOS NVARCHAR(100), @CODCURSO NVARCHAR(5))
//AS
//  INSERT INTO INTERESADOS
//	VALUES ((SELECT MAX(IDINTERESADO) +1 FROM INTERESADOS),
//	@NOMBRE, @TLF, @EMAIL, @COMENTARIOS, @CODCURSO)
//GO

//VISTA GRUPOS: PENDIENTE VER SI HAGO PAGINACION Y NO HAGO VISTA
//CREATE VIEW V_GRUPOS
//AS
//SELECT G.CODGRUPO, C.CODCURSO, C.NOMBRE AS CURSO,
//G.CODTURNO AS TURNO, G.DIAS, G.FECHAINICIO FROM GRUPOS G
//INNER JOIN CURSOS C ON C.CODCURSO = G.CODCURSO
//GO

//VISTA ALUMNOS PAGOS
//CREATE VIEW V_ALUMNOS_PAGOS
//AS
//    SELECT DISTINCT AG.CODGRUPO, A.FOTO, A.IDALUMNO, A.NOMBRE, C.MATRICULA, C.PRECIO,
//    AG.FINANCIACION, C.DURACION, AG.MONTOPAGADO AS PAGADO, G.FECHAINICIO,
//      (C.PRECIO + C.MATRICULA - AG.MONTOPAGADO) AS PENDIENTE
//    FROM ALUMNOS_GRUPOS AG
//    INNER JOIN ALUMNOS A ON AG.IDALUMNO = A.IDALUMNO
//    INNER JOIN GRUPOS G ON G.CODGRUPO = AG.CODGRUPO
//    INNER JOIN CURSOS C ON C.CODCURSO = G.CODCURSO;
//GO
//SELECT* FROM GRUPOS
//WHERE FECHAINICIO BETWEEN '2022-06-1' AND GETDATE()

//CREATE PROCEDURE SP_INSERT_GRUPO
//(@CODGRUPO NVARCHAR(10), @CODCURSO NVARCHAR(5),
//@CODTURNO NVARCHAR(5), @DIAS NVARCHAR(20), @FECHAINICIO DATE)
//AS
//  INSERT INTO GRUPOS VALUES (@CODGRUPO, @CODCURSO, @CODTURNO, @DIAS, @FECHAINICIO)
//GO
//SELECT* FROM ALUMNOS WHERE NOMBRE LIKE '%pepe%';

//FACTURAS
//CREATE PROCEDURE SP_INSERT_FACTURA
//(@IDALUMNO INT, @FACTURA NVARCHAR(100), @CODFACTURA INT OUT)
//AS
//BEGIN
//    DECLARE @MAX_CODFACTURA INT
//    SET @MAX_CODFACTURA = (SELECT COALESCE(MAX(CODFACTURA), 0) +1 FROM FACTURAS)
//    --Concatenar el código de factura al final de la ruta
//    SET @FACTURA = CONCAT(@FACTURA, '_', @MAX_CODFACTURA, '.pdf')
//    INSERT INTO FACTURAS
//    VALUES (@MAX_CODFACTURA, @IDALUMNO, @FACTURA)
//    SET @CODFACTURA = @MAX_CODFACTURA
//END

//CREATE PROCEDURE SP_UPDATE_COMENTARIOS
//(@IDCLIENTE INT, @COMENTARIOS NVARCHAR(100))
//AS
//  UPDATE INTERESADOS
//	SET COMENTARIOS=@COMENTARIOS
//	WHERE IDINTERESADO=@IDCLIENTE
//GO

//CREATE PROCEDURE SP_INSERT_ALUMNO
//(@DNI NVARCHAR(10), @NOMBRE NVARCHAR(10),
//@TLF INT, @EMAIL NVARCHAR(20),
//@DIRECCION NVARCHAR(50), @FOTO NVARCHAR(20))
//AS
//    DECLARE @IDALUMNO INT;
//SELECT @IDALUMNO = MAX(IDALUMNO) + 1 FROM ALUMNOS;

//INSERT INTO ALUMNOS (idalumno, DOCIDENTIDAD, NOMBRE, TLF, EMAIL, DIRECCION, FOTO)
//	VALUES(@IDALUMNO, @DNI, @NOMBRE, @TLF, @EMAIL, @DIRECCION, @FOTO);
//GO
#endregion

namespace ProyectoERP.Repositories
{
    public class RepositoryERPSql : IRepo
    {
        private ErpContext context;

        public RepositoryERPSql(ErpContext context)
        {
            this.context = context;
        }
        private int GetMaxIdUsuario()
        {
            if (this.context.Usuarios.Count() == 0)
            {
                return 1;
            }
            else
            {
                return this.context.Usuarios.Max(x => x.IdUsuario) + 1;
            }
        }

       //USUARIOS
        public async Task<Usuario> ExisteUsuario(string nombreusuario, string clave)
        {
            var consulta = this.context.Usuarios.Where(x => x.NombreUsuario == nombreusuario && x.Clave == clave);
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task RegisterUser(string nombreusuario, string email, string clave, string rol, string foto)
        {
                Usuario user = new Usuario();
                user.IdUsuario = this.GetMaxIdUsuario();
                user.NombreUsuario = nombreusuario;
                user.Email = email;
                user.Rol = rol;
                user.Foto = foto;
                //Cada usuario tendrá un salt diferente
                user.Salt = HelperAuth.GenerarSalt();
                //Clave sin cifrar
                user.Clave = clave;
                //Ciframos clave con su salt
                user.ClaveEncrip = HelperAuth.EncriptarClave(clave, user.Salt);
                this.context.Usuarios.Add(user);
                await this.context.SaveChangesAsync();
        }
        //SIN FOTO
        public async Task RegisterUser(string nombreusuario, string email, string clave, string rol)
        {
            Usuario user = new Usuario();
            user.IdUsuario = this.GetMaxIdUsuario();
            user.NombreUsuario = nombreusuario;
            user.Email = email;
            user.Rol = rol;
            user.Foto = null;
            //Cada usuario tendrá un salt diferente
            user.Salt = HelperAuth.GenerarSalt();
            //Clave sin cifrar
            user.Clave = clave;
            //Ciframos clave con su salt
            user.ClaveEncrip = HelperAuth.EncriptarClave(clave, user.Salt);
            this.context.Usuarios.Add(user);
            await this.context.SaveChangesAsync();
        }
        public Usuario LoginUser(string nombreusuario, string clave)
        {
            Usuario user = this.context.Usuarios.FirstOrDefault(x => x.NombreUsuario == nombreusuario);
            if (user == null)
            {
                return null;
            }
            else
            {
                //Recuperar password cifrado de la bbd
                byte[] claveuser = user.ClaveEncrip;
                //Debemos cifrar de nuevo el password de usuario
                //junto a su salt utilizando la misma tecnica
                string salt = user.Salt;
                byte[] temp = HelperAuth.EncriptarClave(clave, salt);
                bool respuesta = HelperAuth.CompararClaves(claveuser, temp);
                if (respuesta == true)
                {
                    //GUARDAR SESION AQUÍ
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }
        //ALUMNOSPAGOS
        public async Task InsertAlumno(string dni, string nombrealumno, int tlf, string email, string direccion, string? foto)
        {
            string sql = "SP_INSERT_ALUMNO @DNI, @NOMBRE, @TLF, @EMAIL, @DIRECCION, @FOTO";
            SqlParameter pamdni = new SqlParameter("@DNI", dni);
            SqlParameter pamnombre = new SqlParameter("@NOMBRE", nombrealumno);
            SqlParameter pamtlf = new SqlParameter("@TLF", tlf);
            SqlParameter pamemail = new SqlParameter("@EMAIL", email);
            SqlParameter pamdireccion = new SqlParameter("@DIRECCION", direccion);
            SqlParameter pamfoto = new SqlParameter("@FOTO", "woman1.jpg");
            await this.context.Database.ExecuteSqlRawAsync(sql, pamdni, pamnombre, pamtlf, pamemail, pamdireccion, pamfoto);
        }
        public async Task<int> InsertFactAsync(int idalumno, string rutafact)
        {
            string sql = "SP_INSERT_FACTURA @IDALUMNO, @FACTURA, @CODFACTURA OUT";
            SqlParameter pamidalumno = new SqlParameter("@IDALUMNO", idalumno);
            SqlParameter pamrutafact = new SqlParameter("@FACTURA", rutafact);
            SqlParameter pamcodfactura = new SqlParameter("@CODFACTURA", -1);
            pamcodfactura.Direction = ParameterDirection.Output;
            await this.context.Database.ExecuteSqlRawAsync(sql, pamidalumno, pamrutafact, pamcodfactura);
            int codfactura = (int)pamcodfactura.Value;
            return codfactura;
        }
        public async Task<List<AlumnoPagos>> GetAlumnosGrupoAsync(string codgrupo)
        {
            var grupos = from datos in this.context.AlumnosPagos
                         where datos.CodGrupo==codgrupo
                         select datos;
            return await grupos.ToListAsync();
        }
        public async Task<List<AlumnoPagos>> GetAlumnosPagos()
        {
            var grupos = from datos in this.context.AlumnosPagos
                         select datos;
            return await grupos.ToListAsync();
        }
        public async Task<Alumno> GetAlumno(int idalumno)
        {
            var alumno = from datos in this.context.Alumnos
                         where datos.IdAlumno==idalumno
                         select datos;
            return await alumno.FirstOrDefaultAsync();
        }
        public async Task<List<AlumnoPagos>> FiltroNombreAlumnoAsync(string nombrealumno)
        {
            var alumnos = await this.context.AlumnosPagos
                                 .Where(a => a.NombreAlumno.Contains(nombrealumno))
                                 .ToListAsync();
            return alumnos;
        }
        public Task<List<AlumnoPagos>> FiltroAlumnosPagosFecha(DateTime fechainicio)
        {
            //AQUI SI DEBERIA PONER PERIODO DE TIEMPO
            var consulta = from datos in this.context.AlumnosPagos
                           where datos.FechaInicio >= fechainicio
                           select datos;
            return consulta.ToListAsync();
        }
        public Task<List<AlumnoPagos>> FiltroAlumnosPagosGrupoAsync(string codgrupo)
        {
            var consulta = from datos in this.context.AlumnosPagos
                           where datos.CodGrupo == codgrupo
                           select datos;
            return consulta.ToListAsync();
        }

        //CLIENTESPOTENCIALES
        public List<ClientePotencial> FindClientesP(string curso)
        {
            var consulta = from datos in this.context.ClientesPotenciales
                           where datos.Curso == curso
                           select datos;
            return consulta.ToList();
        }
        public async Task<List<ClientePotencial>> FindClientesPNombre(string nombrecliente)
        {
            var consulta = await this.context.ClientesPotenciales
                                 .Where(a => a.NombreCliente.Contains(nombrecliente))
                                 .ToListAsync();
            return consulta;
        }

        public ClientePotencial GetCliente(int idinteresado)
        {
            var consulta = from datos in this.context.ClientesPotenciales
                           where datos.IdInteresado==idinteresado
                           select datos;
            return consulta.FirstOrDefault();
        }

        public List<ClientePotencial> GetClientesP()
        {
            var consulta = from datos in this.context.ClientesPotenciales
                           select datos;
            return consulta.ToList();
        }
        public async Task<List<Alumno>> GetAlumnos()
        {
            return await this.context.Alumnos.ToListAsync();
        }
        public async Task UpdateComentariosCliente(int idinteresado, string comentarios)
        {
            string sql = "SP_UPDATE_COMENTARIOS @IDALUMNO, @COMENTARIOS";
            SqlParameter pamidcliente = new SqlParameter("@IDALUMNO", idinteresado);
            SqlParameter pamcomentarios = new SqlParameter("@COMENTARIOS", comentarios);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamidcliente, pamcomentarios);
        }

        //CURSOS
        public List<Curso> GetCursos()
        {
            var cursos = from datos in this.context.Cursos
                         select datos;
            return cursos.ToList();
        }

        //GRUPOS
        public async Task<List<Grupo>> GetGrupos()
        {
            var grupos = from datos in this.context.Grupos
                         select datos;
            return grupos.ToList();
        }
        private string GetNextCodGrupo()
        {
            //Obtengo el último código de grupo en la base de datos
            var lastCodGrupo = context.Grupos
                .OrderByDescending(g => g.CodGrupo)
                .FirstOrDefault()?.CodGrupo;

            //Si no hay códigos de grupo, empezar en G001
            if (string.IsNullOrEmpty(lastCodGrupo))
            {
                return "G001";
            }
            //Obtener el número del último código de grupo
            var lastNum = int.Parse(lastCodGrupo.Substring(1));
            //Incrementar el número en uno y formatear el nuevo código de grupo
            return $"G{lastNum + 1:D3}";
        }

        public async Task InsertGrupo(string codcurso, string turno, string dias, string fechainicio)
        {
            string sql = "SP_INSERT_GRUPO @CODGRUPO, @CODCURSO, @CODTURNO, @DIAS, @FECHAINICIO";
            string codgrupo = this.GetNextCodGrupo();
            SqlParameter pamcodgrupo = new SqlParameter("@CODGRUPO", codgrupo);
            SqlParameter pamcodcurso = new SqlParameter("@CODCURSO", codcurso);
            SqlParameter pamcodturno = new SqlParameter("@CODTURNO", turno);
            SqlParameter pamdias = new SqlParameter("@DIAS", dias);
            SqlParameter pamfechainicio = new SqlParameter("@FECHAINICIO", fechainicio);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamcodgrupo, pamcodcurso, pamcodturno, pamdias, pamfechainicio);
        }
        public async Task InsertClienteP(string nombrecliente, string tlf, string email, string? comentarios, string codcurso)
        {
            string sql = "SP_INSERT_CLIENTEP @NOMBRE, @TLF, @EMAIL, @COMENTARIOS, @CODCURSO";
            SqlParameter pamnombre = new SqlParameter("@NOMBRE", nombrecliente);
            SqlParameter pamtlf = new SqlParameter("@TLF", tlf);
            SqlParameter pamemail = new SqlParameter("@EMAIL", email);
            SqlParameter pamcomentarios = new SqlParameter("@COMENTARIOS", comentarios);
            SqlParameter pamcodcurso = new SqlParameter("@CODCURSO", codcurso);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamnombre, pamtlf, pamemail, pamcomentarios, pamcodcurso);
        }

        public Task<List<Grupo>> FiltroGruposCurso(string codcurso)
        {
            var consulta = from datos in this.context.Grupos
                           where datos.CodCurso==codcurso
                           select datos;
            return consulta.ToListAsync();
        }

        public Task<Grupo> FiltroGruposCod(string codgrupo)
        {
            var consulta = from datos in this.context.Grupos
                           where datos.CodGrupo == codgrupo
                           select datos;
            return consulta.FirstOrDefaultAsync();
        }

        public Task<List<Grupo>> FiltroGruposFecha(DateTime fechainicio)
        {
            //WHERE FECHAINICIO BETWEEN '2022-06-1' AND GETDATE()
            var consulta = from datos in this.context.Grupos
                           where datos.FechaInicio>=fechainicio
                           select datos;
            return consulta.ToListAsync();
        }
    }
}
