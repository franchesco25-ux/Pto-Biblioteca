using Biblioteca.Filtros;
using Biblioteca.Models;
using Biblioteca.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Biblioteca.Controllers
{
    [RequiereSesion]
    public class ReservasController : Controller
    {
        readonly IReservas _reservas;
        private readonly IServicioSesion _servicioSesion;
        public ReservasController(IReservas reservas, IServicioSesion servicioSesion)
        {
            _reservas = reservas;
            _servicioSesion = servicioSesion;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            int id = (_servicioSesion.Obtener().Rol == "ADMINISTRADOR" || _servicioSesion.Obtener().Rol == "BIBLIOTECARIO") ? 0 : _servicioSesion.Obtener().UsuarioId;
            
            return View(_reservas.listarReservas(id));
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            ViewBag.Recursos = new SelectList(_reservas.listaRecursos(), "id", "nombre");
            ViewBag.Clientes = new SelectList(_reservas.listaUsuarios(), "id", "nombre");
            return View();
        }
        [HttpPost]
        public IActionResult Registrar(Reserva reg)
        {

            if (ModelState.IsValid)
            {
                ResponseJSON resp = _reservas.registrarReserva(reg);
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
            ViewBag.Recursos = new SelectList(_reservas.listaRecursos(), "id", "nombre", reg.RecursoId);
            ViewBag.Clientes = new SelectList(_reservas.listaUsuarios(), "id", "nombre", reg.UsuarioId);
            return View(reg);
        }

    }
}
