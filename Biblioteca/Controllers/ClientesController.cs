using Biblioteca.Filtros;
using Biblioteca.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    public class ClientesController : Controller
    {

        readonly IRepositorioUsuario _UsuarioDAO;

        public ClientesController(IRepositorioUsuario usuarioDAO)
        {
            _UsuarioDAO = usuarioDAO;
        }

        [RequiereRol("ADMINISTRADOR", "BIBLIOTECARIO")]
        public IActionResult Index()
        {
            return View(_UsuarioDAO.ListUsuarios());
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            return PartialView("_clienteDetail",_UsuarioDAO.ListUsuarios().FirstOrDefault(x => x.UsuarioId == id));
        }
    }
}



