using Biblioteca.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    public class RecursosController : Controller
    {

        IRecursos _recursos;

        public RecursosController(IRecursos recursos)
        {
            _recursos = recursos;
        }

        public IActionResult Index()
        {

            return View(_recursos.ListResources());
        }

        [HttpGet]
        public IActionResult ListDetails(int ? id)
        {
            IRecursos rec = (IRecursos)_recursos.ListResources().FirstOrDefault(x => x.id == id);
            return View(rec);
        }

        
        public IActionResult Delete(int ? id)
        {
            _recursos.deleteResources(id);
            return RedirectToAction("Index");
        }
    }
}
