using Microsoft.AspNetCore.Mvc;
using ProyectoERP.Filters;
using ProyectoERP.Repositories;

namespace ProyectoERP.Controllers
{
    [AuthorizeUsuarios]
    public class AdminController : Controller
    {
        private IRepo repo;

        public AdminController(IRepo repo)
        {
            this.repo = repo;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string nombreusuario, string email, string clave, string confirmarclave, string rol, IFormFile imagen)
        {
            if (clave == confirmarclave)
            {
                if (imagen != null)
                {
                    var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
                    var path = Path.Combine(baseUrl, "images", imagen.FileName);

                    await this.repo.RegisterUser(nombreusuario, email, clave, rol, path);
                }
                else
                {
                    await this.repo.RegisterUser(nombreusuario, email, clave, rol);
                }
                ViewBag.MENSAJE = "USUARIO REGISTRADO CORRECTAMENTE";
            }
            else
            {
                ViewBag.MENSAJE = "USUARIO NO REGISTRADO";
            }
            return RedirectToAction("Index");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
