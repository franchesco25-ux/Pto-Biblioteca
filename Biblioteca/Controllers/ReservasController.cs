using Biblioteca.Filtros;
using Biblioteca.Repositorios;
using Microsoft.AspNetCore.Mvc;

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
    }
}
