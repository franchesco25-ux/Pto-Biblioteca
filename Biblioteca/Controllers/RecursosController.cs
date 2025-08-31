using Biblioteca.Filtros;
using Biblioteca.Models;
using Biblioteca.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        [HttpPost]
        public IActionResult generarMulta()
        {
            _recursos.generarMulta();
            TempData["mensaje"] = "Multa generada exitosamente";
            return RedirectToAction("index");
        }

        [HttpGet]
        public IActionResult registrar()
        {
            ViewBag.tiporecursos = _recursos.listTipoRecursos();
            ViewBag.editoriales = _recursos.listEditoriales();
            ViewBag.generos = _recursos.listGeneros();
            return View();
        }

        [HttpPost]
        public IActionResult registrar(Recursos reg)
        {
            if (ModelState.IsValid)
            {
                ResponseJSON resp = _recursos.crearRecurso(reg);
                if (resp.StatusCode == 200)
                {
                    TempData["mensaje"] = resp.Mensaje;
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, resp.Mensaje);
                }
            }
            ViewBag.tiporecursos = _recursos.listTipoRecursos();
            ViewBag.editoriales = _recursos.listEditoriales();
            ViewBag.generos = _recursos.listGeneros();
            return View(reg);
        }

        [HttpGet]
        public IActionResult Editar(int? id)
        {
            
            //ViewBag.Generos = new SelectList(ObtenerGeneros(), "Value", "Text", recurso.GeneroId);
            Recursos reg = _recursos.ListResources().FirstOrDefault(x => x.id == id) ?? new Recursos();
            //new SelectList(_recursos.listGeneros(), "id", "nombre",reg.id);
            ViewBag.tiporecursos = new SelectList(_recursos.listTipoRecursos(), "id", "nombre",reg.id);
            ViewBag.editoriales = new SelectList(_recursos.listEditoriales(), "id", "nombre", reg.id);
            ViewBag.generos = new SelectList(_recursos.listGeneros(), "id", "nombre", reg.id);
            return View("EditarRecurso",reg);
        }

        [HttpPost]
        public IActionResult Editar(Recursos reg)
        {
            if (ModelState.IsValid)
            {
                ResponseJSON resp = _recursos.editarRecurso(reg);
                if (resp.StatusCode == 200)
                {
                    TempData["mensaje"] = resp.Mensaje;
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, resp.Mensaje);
                }
            }
            ViewBag.tiporecursos = new SelectList(_recursos.listTipoRecursos(), "id", "nombre", reg.id);
            ViewBag.editoriales = new SelectList(_recursos.listEditoriales(), "id", "nombre", reg.id);
            ViewBag.generos = new SelectList(_recursos.listGeneros(), "id", "nombre", reg.id);
            return View("EditarRecurso", reg);
        }
    }
}
