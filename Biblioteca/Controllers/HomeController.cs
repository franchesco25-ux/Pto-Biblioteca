using System.Diagnostics;
using Biblioteca.Filtros;
using Biblioteca.Models;
using Biblioteca.Repositorios;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
namespace Biblioteca.Controllers
{
    [RequiereSesion]
    public class HomeController : Controller
    {

        private readonly IServicioSesion _servicioSesion;

        public HomeController(IServicioSesion servicioSesion)
        {
            _servicioSesion = servicioSesion;
        }

        [HttpGet]
        public IActionResult Index()
        {

            return View(_servicioSesion.Obtener());
        }

        [HttpGet]
        public IActionResult Denegado(string? m)
        {
            ViewData["Mensaje"] = m ?? "Acceso denegado";
            return View();
        }

    }
}
