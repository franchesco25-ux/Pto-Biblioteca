using Biblioteca.Filtros;
using Biblioteca.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    [RequiereSesion]
    public class RecursosController : Controller
    {

        readonly IRecursos _recursos;
        private readonly IServicioSesion _servicioSesion;

        public RecursosController(IRecursos recursos, IServicioSesion servicioSesion)
        {
            _recursos = recursos;
            _servicioSesion = servicioSesion;

        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.recursos = _recursos.ListResources();
            return View(_servicioSesion.Obtener());
        }

        [HttpGet]
        public IActionResult ListDetails(int? id)
        {
            IRecursos rec = (IRecursos)_recursos.ListResources().FirstOrDefault(x => x.id == id);
            return View(rec);
        }


        public IActionResult Delete(int? id)
        {
            _recursos.deleteResources(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Denegado(string? m)
        {
            ViewData["Mensaje"] = m ?? "Acceso denegado";
            return View();
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            return PartialView("_recursoModal", _recursos.ListResources().FirstOrDefault(x => x.id == id));
        }
    }
}
