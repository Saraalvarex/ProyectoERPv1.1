using Microsoft.AspNetCore.Mvc;
using ProyectoERP.Filters;
using ProyectoERP.Helpers;
using ProyectoERP.Models;
using ProyectoERP.Repositories;

namespace ProyectoERP.Controllers
{
    [AuthorizeUsuarios]
    public class ClientesPotencialesController : Controller
    {
        private IRepo repo;
        private HelperMail helperMail;
        //private HelperPathProvider helperPath;
        public ClientesPotencialesController(IRepo repo, HelperMail helperMail)
        {
            this.repo = repo;
            this.helperMail = helperMail;
            //this.helperPath = helperPath;
        }
        public IActionResult Index(int? idinteresado, string? correos)
        {
            List<Curso> cursos = this.repo.GetCursos();
            ViewBag.CURSOS = cursos;
            List<ClientePotencial> clientes = this.repo.GetClientesP();
            if (idinteresado != null)
            {
                ClientePotencial cliente = this.repo.GetCliente(idinteresado.Value);
                ViewBag.CLIENTE = cliente;
            }
            if (correos != null)
            {
                string[] correosArray = correos.Split(',');
                ViewBag.CORREOS = new List<string>(correosArray);
            }
            return View(clientes);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string? curso, string? nombrecliente)
        {
            List<Curso> cursos = this.repo.GetCursos();
            ViewBag.CURSOS = cursos;
            List<ClientePotencial> clientes = new List<ClientePotencial>();
            if (curso!=null)
            {
                clientes = this.repo.FindClientesP(curso);
            }
            else if(nombrecliente!=null)
            {
                clientes = await this.repo.FindClientesPNombre(nombrecliente);
            }
            return View(clientes);
        }
        public IActionResult InsertarCliente()
        {
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> InsertarCliente(string nombrecliente, string tlf, string email, string? comentarios, string codcurso)
        {
            await this.repo.InsertClienteP(nombrecliente, tlf, email, comentarios, codcurso);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateComentarios(int idcliente, string comentarios)
        {
            await this.repo.UpdateComentariosCliente(idcliente, comentarios);
            return RedirectToAction("Index");
        }

        //Enviar correo info interesados
        public IActionResult SendMail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMail(string para, string asunto, string mensaje)
        {
            await this.helperMail.SendMailAsync(para, asunto, mensaje);
            ViewBag.MENSAJE = "Email enviado correctamente";
            return RedirectToAction("Index");
        }

        //MailsMasivos
        [HttpPost]
        public async Task<IActionResult> SendMails(string para, string asunto, string mensaje)
        {
            string[] correosarray = para.Split(',');
            List<string> correos = new List<string>(correosarray);
            await this.helperMail.SendMailAsync(correos, asunto, mensaje);
            ViewBag.MENSAJE = "Emails enviado correctamente";
            return RedirectToAction("Index");
        }
        public IActionResult _CorreosMasivos(string correos)
        {
            List<Curso> cursos = this.repo.GetCursos();
            ViewBag.CURSOS = cursos;
            ViewBag.CORREOS = correos;
            return PartialView("_CorreosMasivos");
        }
    }
}
