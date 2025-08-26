using Biblioteca.Filtros;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    public class ClientesController : Controller
    {

        [RequiereRol("ADMINISTRADOR", "BIBLIOTECARIO")]
        public IActionResult Index()
        {
            return View();
        }
    }
}



