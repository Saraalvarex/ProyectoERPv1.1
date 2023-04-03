using Microsoft.AspNetCore.Mvc;
using MvcCoreUtilidades.Helpers;
using ProyectoERP.Helpers;
using static NuGet.Packaging.PackagingConstants;

namespace ProyectoERP.Controllers
{
    public class MailsController : Controller
    {
        private HelperMail helperMail;
        private HelperUploadFiles helperUploadFiles;
        public MailsController(HelperMail helperMail, HelperUploadFiles helperUploadFiles)
        {
            this.helperMail = helperMail;
            this.helperUploadFiles = helperUploadFiles;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string para, string asunto, string mensaje, List<IFormFile> files)
        {
            if (files.Count != 0)
            {
                if (files.Count > 1)
                {
                    List<string> paths = await this.helperUploadFiles.UploadFileAsync(files, Helpers.Folders.FotosAlumnos);
                    await this.helperMail.SendMailAsync(para, asunto, mensaje, paths);
                }
                else
                {
                    string path = await this.helperUploadFiles.UploadFileAsync(files[0], Helpers.Folders.FotosAlumnos);
                    await this.helperMail.SendMailAsync(para, asunto, mensaje, path);
                }
            }
            else
            {
                await this.helperMail.SendMailAsync(para, asunto, mensaje);
            }
            ViewBag.MENSAJE = "Email enviado correctamente";
            return View();
        }
    }
}
