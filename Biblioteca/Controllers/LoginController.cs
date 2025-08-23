using Biblioteca.Excepciones;
using Biblioteca.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    public class LoginController : Controller

    {
        private readonly IServicioAutenticacion _servicioAutenticacion;

        public LoginController(IServicioAutenticacion servicioAutenticacion) { 
            _servicioAutenticacion = servicioAutenticacion;
        }

        public IActionResult Acceder(string? m)
        {
            ViewData["Mensaje"] = m;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Acceder (AccesoEntrada entrada)
        {
            if (!ModelState.IsValid)
            {
                return View(entrada);
            }
            try
            {
                await _servicioAutenticacion.IniciarSesionAsync(entrada.UsuarioLogin, entrada.Password);
                return RedirectToAction("Index", "Home");
            }
            catch (UsuarioNoEncontradoException)
            {
                ViewData["Mensaje"] = "Usuario no encontrado";
            }
            catch (UsuarioBloqueadoException)
            {
                ViewData["Mensaje"] = "usuario bloqueado";
            }
            catch (PasswordInactivaException)
            {
                ViewData["Mensaje"] = "password inactivo";
            }
            catch (CredencialesInvalidasException)
            {
                ViewData["Mensaje"] = "credenciales ivalidad";
            }

            return View(entrada);
        }

        public IActionResult Salir()
        {
            _servicioAutenticacion.CerrarSesion();
            return RedirectToAction("Acceder");
        }

        public IActionResult Index()
        {
            return View();
        }

        public class AccesoEntrada
        {
            public string UsuarioLogin { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

    }
}
