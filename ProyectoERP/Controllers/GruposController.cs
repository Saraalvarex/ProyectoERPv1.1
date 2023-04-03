using Microsoft.AspNetCore.Mvc;
using ProyectoERP.Filters;
using ProyectoERP.Models;
using ProyectoERP.Repositories;

namespace ProyectoERP.Controllers
{
    [AuthorizeUsuarios]
    public class GruposController : Controller
    {
        List<string> diasdelasemana = new List<string>() { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Fin de semana" };
        Dictionary<string, string> turnos = new Dictionary<string, string>(){{"M", "Mañana"},
                                                                            {"T", "Tarde"},
                                                                            {"C", "Continuo"}};
        List<string> codturnos = new List<string>() { "M", "T", "C" };
        private IRepo repo;
        //private HelperPathProvider helperPath;
        public GruposController(IRepo repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.DIAS = this.diasdelasemana;
            ViewBag.TURNOS = this.turnos;
            ViewBag.CODTURNOS = this.codturnos;
            ViewBag.CURSOS = this.repo.GetCursos();
            List<Grupo> grupos = await this.repo.GetGrupos();
            return View(grupos);
        }

        public async Task<IActionResult> _Grupo(string codgrupo)
        {
            Grupo grupo = await this.repo.FiltroGruposCod(codgrupo);
            ViewBag.CURSOS = this.repo.GetCursos();
            ViewBag.GRUPO = grupo;
            return PartialView("_Grupo", grupo);
        }

        public async Task<IActionResult> _Grupos(string? curso, DateTime? fechainicio)
        {
            List<Grupo> grupos = new List<Grupo>();
            grupos = await this.repo.GetGrupos();
            if (curso != null)
            {
                grupos = await this.repo.FiltroGruposCurso(curso);
                ViewBag.Cursos = this.repo.GetCursos();
            }else if(fechainicio != null)
            {
                grupos = await this.repo.FiltroGruposFecha(fechainicio.Value);
                ViewBag.Cursos = this.repo.GetCursos();
            }
            return PartialView("_Grupos", grupos);
        }
        public IActionResult InsertarGrupo()
        {
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> InsertarGrupo(string codcurso, string turno, string dias, string fechainicio)
        {
            await this.repo.InsertGrupo(codcurso, turno, dias, fechainicio);
            return RedirectToAction("Index");
        }
    }
}
