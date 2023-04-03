using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using ProyectoERP.Filters;
using ProyectoERP.Helpers;
using ProyectoERP.Models;
using ProyectoERP.Repositories;
using System.Diagnostics;

namespace ProyectoERP.Controllers
{
    [AuthorizeUsuarios]
    public class AlumnosController : Controller
    {
        private IRepo repo;
        private HelperPathProvider helperPath;
        private HelperExcelToPdf helperFact;
        public AlumnosController(IRepo repo, HelperPathProvider helperPath, HelperExcelToPdf helperFact)
        {
            this.repo = repo;
            this.helperPath = helperPath;
            this.helperFact = helperFact;
        }
        public async Task<IActionResult> Factura(int idalumno)
        {
            List<Curso> cursos = this.repo.GetCursos();
            ViewBag.CURSOS = cursos;
            Alumno alumno = await this.repo.GetAlumno(idalumno);
            return View(alumno);
        }
        //public async Task<IActionResult> GenerarFactura(string dni, int idalumno, string nombrealumno, string direccion, int pago, string concepto, DateTime? fecha, string? curso)
        //{
        //    string rutaArchivoFinal = await helperFact.GenerarFactura(dni, nombrealumno, direccion, pago, concepto, fecha, curso);
        //    string nombreSinEspacios = nombrealumno.Replace(" ", "");
        //    int codfactura = await this.repo.InsertFactAsync(idalumno, "\\" + nombreSinEspacios);

        //    //Aquí puedes usar alguna librería para convertir el archivo Excel a PDF, por ejemplo:
        //    //https://github.com/OfficeDev/Open-XML-SDK/tree/main/src/DocumentFormat.OpenXml.Office2010.Wordprocessing#using-the-presentationmldocument-and-presentationdocument-classes

        //    ProcessStartInfo startInfo = new ProcessStartInfo
        //    {
        //        FileName = rutaArchivoFinal,
        //        UseShellExecute = true
        //    };
        //    Process.Start(startInfo);
        //    return RedirectToAction("Index");
        //}

        public async Task<IActionResult> GenerarFactura(string dni, int idalumno, string nombrealumno,
            string direccion, int pago, string concepto, DateTime? fecha, string? curso)
        {
            if (fecha == null)
            {
                fecha = DateTime.Now;
            }
            //Me pide licencia comercial
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            //Ruta de excell en el que escribir
            string rutaArchivoInicial = helperPath.MapPath("FACTURA.xlsx", Folders.Facturas);
            var file = new FileInfo(rutaArchivoInicial);
            var package = new ExcelPackage(file);
            //Aqui cojo la hoja de trabajo del excell donde quiero escribir
            var sheet = package.Workbook.Worksheets["Hoja1"];
            //Agregar los datos de la factura
            sheet.Cells["B18"].Value = nombrealumno;
            sheet.Cells["B19"].Value = direccion;
            sheet.Cells["C22"].Value = concepto;
            sheet.Cells["D22"].Value = curso;
            sheet.Cells["G22"].Value = pago;
            sheet.Cells["G17"].Value = fecha.Value;
            sheet.Cells["G18"].Value = dni;
            //Calcula todas las fórmulas y actualiza los valores de las celdas
            sheet.Calculate();

            //Guardamos el archivo de Excel en la ruta
            string nombreSinEspacios = nombrealumno.Replace(" ", ""); //quitamos todos los espacios en blanco
            int codfactura = await this.repo.InsertFactAsync(idalumno, "\\" + nombreSinEspacios);
            sheet.Cells["G16"].Value = codfactura;
            string rutaArchivoExcel = helperPath.MapPath(nombreSinEspacios + "_" + codfactura + ".xlsx", Folders.Facturas);
            
            var fileModificado = new FileInfo(rutaArchivoExcel);
            package.SaveAs(fileModificado);

            // Convertimos el archivo de Excel a PDF y guardamos en una ruta específica
            string rutaArchivoPdf = helperPath.MapPath(nombreSinEspacios + "_" + codfactura + ".pdf", Folders.Facturas);
            string pdfFilePath = helperFact.ConvertToPdf(rutaArchivoExcel, rutaArchivoPdf);

            // Abre el archivo PDF en nuestro navegador predeterminado
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = pdfFilePath,
                UseShellExecute = true
            };
            Process.Start(startInfo);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index(string? codgrupo)
        {
            List<Curso> cursos = this.repo.GetCursos();
            ViewBag.CURSOS = cursos;
            List<AlumnoPagos> alumnos = new List<AlumnoPagos>();
            if (codgrupo != null)
            {
                alumnos = await this.repo.GetAlumnosGrupoAsync(codgrupo);
                ViewBag.GRUPO = codgrupo;
            }else
            {
                alumnos = await this.repo.GetAlumnosPagos();
            }
            return View(alumnos);
        }
        [HttpPost]
        public async Task<IActionResult> Index(string? nombrealumno, DateTime? fecha)
        {
            List<AlumnoPagos> alumnos = new List<AlumnoPagos>();
            List<Curso> cursos = this.repo.GetCursos();
            ViewBag.CURSOS = cursos;
            //alumnos = await this.repo.GetAlumnosPagos();
            if (nombrealumno != null)
            {
                alumnos = await this.repo.FiltroNombreAlumnoAsync(nombrealumno);
            }
            else if (fecha != null)
            {
                alumnos = await this.repo.FiltroAlumnosPagosFecha(fecha.Value);
            }
            return View("Index", alumnos);
        }
        [HttpPost]
        public async Task<IActionResult> InsertarAlumno(string dni, string nombrealumno, int tlf, string email, string direccion, string? foto)
        {
            await this.repo.InsertAlumno(dni, nombrealumno, tlf, email, direccion, foto);
            return RedirectToAction("Index");
        }
    }
}
