using Microsoft.AspNetCore.Mvc;
using ProyectoERP.Models;
using ProyectoERP.Repositories;

namespace ProyectoERP.Controllers
{
    public class LoginController : Controller
    {
        private IRepo repo;

        public LoginController(IRepo repo)
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
            }else
            {
                ViewBag.MENSAJE = "USUARIO NO REGISTRADO";
            }
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string nombreusuario, string clave)
        {
            Usuario user = this.repo.LoginUser(nombreusuario, clave);
            if (user == null)
            {
                ViewBag.MENSAJE = "Credenciales incorrectas";
                return View();
            }
            else
            {
                //return View(user);
                ViewBag.MENSAJE = "Credenciales correctas";
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}
