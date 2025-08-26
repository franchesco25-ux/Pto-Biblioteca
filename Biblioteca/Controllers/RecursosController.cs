using Biblioteca.Filtros;
using Biblioteca.Models;
using Biblioteca.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    [RequiereSesion]
    public class RecursosController : Controller
    {

        IRecursos _recursos;
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

        [RequiereSesion]                 
        public IActionResult Create()
        {
            return View(new Recursos()); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequiereSesion]
        // [RequiereRol("ADMINISTRADOR")]  
        public IActionResult Create(Recursos reg, string? autorNombre, string? autorApellido, string tipoAutor = "principal")
        {
            if (!ModelState.IsValid) return View(reg);

            int id = _recursos.insertResource(reg);
            if (id > 0)
            {
                if (!string.IsNullOrWhiteSpace(autorNombre))
                {
                    int rel = _recursos.asociarAutorARecurso(id, autorNombre, autorApellido, tipoAutor);
                    ViewBag.mensaje = rel > 0
                        ? $"Recurso creado (ID {id}) y autor asociado."
                        : $"Recurso creado (ID {id}), pero no se pudo asociar el autor (verifique que exista).";
                }
                else
                {
                    ViewBag.mensaje = $"Recurso creado (ID {id}).";
                }
                return View(reg); 
            }

            ViewBag.mensaje = "No se pudo crear el recurso (verifica Tipo/Editorial/Género por NOMBRE).";
            return View(reg);
        }

        public IActionResult Details(int? id)
        {
            var recurso = _recursos.ListResources().FirstOrDefault(x => x.id == id);
            if (recurso == null)
            {
                return NotFound();
            }
            return PartialView("_recursoModal",recurso);
        }

    }
}
