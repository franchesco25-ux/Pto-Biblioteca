using AspNetCoreGeneratedDocument;
using Biblioteca.Filtros;
using Biblioteca.Models;
using Biblioteca.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Controllers
{
    [RequiereRol("ADMINISTRADOR")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository<Usuario> _repo;
        private readonly IServicioAutenticacion _auth;

        public UsuarioController(IUsuarioRepository<Usuario> repo, IServicioAutenticacion auth)
        {
            _repo = repo;
            _auth = auth;
        }

        // -----------------------------
        // Helpers para combos en Create
        // -----------------------------
        private void CargarCombosCreate()
        {
            ViewBag.Roles = new List<string> { "ADMINISTRADOR", "PROFESOR", "ESTUDIANTE", "BIBLIOTECARIO" };
            ViewBag.Identidades = new List<string> { "DNI", "RUC", "CE", "RLA" };
        }

        // LISTAR
        public IActionResult IndexUsuario()
        {
            // (opcional) doble chequeo de rol vía servicio
            if (!_auth.UsuarioTieneRol("ADMINISTRADOR")) return Forbid();

            var lista = _repo.listarVigentes();
            return View(lista);
        }

        // DETAILS
        public IActionResult Details(short id)
        {
            if (!_auth.UsuarioTieneRol("ADMINISTRADOR")) return Forbid();
            if (id <= 0) return BadRequest();

            var usuario = _repo.buscarPorId(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // CREATE GET
        public IActionResult Create()
        {
            if (!_auth.UsuarioTieneRol("ADMINISTRADOR")) return Forbid();
            CargarCombosCreate();
            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario model)
        {
            if (!_auth.UsuarioTieneRol("ADMINISTRADOR")) return Forbid();
            CargarCombosCreate();

            if (!ModelState.IsValid) return View(model);

            // Asegurar campos mínimos
            if (string.IsNullOrWhiteSpace(model.UsuarioLogin))
                ModelState.AddModelError(nameof(model.UsuarioLogin), "El usuario es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Rol))
                ModelState.AddModelError(nameof(model.Rol), "El rol es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Identidad))
                ModelState.AddModelError(nameof(model.Identidad), "El tipo de identidad es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.DocumentoIdent))
                ModelState.AddModelError(nameof(model.DocumentoIdent), "El documento es obligatorio.");
            if (!ModelState.IsValid) return View(model);

            // TODO: si PasswordHash llega en claro desde la vista, aplicar aquí tu hash (bcrypt/lo que uses)
            // model.PasswordHash = MiHasher.Hash(model.PasswordHash);

            var id = _repo.crear(model);
            if (id <= 0)
            {
                ModelState.AddModelError("", "No se pudo crear el usuario (revise duplicados de usuario o documento).");
                return View(model);
            }

            TempData["ok"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(IndexUsuario));
        }

        // EDIT GET (sin password)
        public IActionResult Edit(short id)
        {
            if (!_auth.UsuarioTieneRol("ADMINISTRADOR")) return Forbid();
            if (id <= 0) return BadRequest();

            var usuario = _repo.buscarPorId(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // EDIT POST (sin password)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Usuario model)
        {
            if (!_auth.UsuarioTieneRol("ADMINISTRADOR")) return Forbid();

            // Solo se esperan cambios en Nombre, Apellido y UsuarioLogin según tu SP
            if (model.UsuarioId <= 0) return BadRequest("ID inválido.");
            if (string.IsNullOrWhiteSpace(model.UsuarioLogin))
                ModelState.AddModelError(nameof(model.UsuarioLogin), "El usuario es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Nombre))
                ModelState.AddModelError(nameof(model.Nombre), "El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Apellido))
                ModelState.AddModelError(nameof(model.Apellido), "El apellido es obligatorio.");
            if (!ModelState.IsValid) return View(model);

            var filas = _repo.actualizar(model);
            if (filas == 0)
            {
                ModelState.AddModelError("", "No se pudo actualizar el usuario.");
                return View(model);
            }

            TempData["ok"] = "Usuario actualizado.";
            return RedirectToAction(nameof(IndexUsuario));
        }

        // DELETE GET (eliminación lógica)
        public IActionResult Delete(short id)
        {
            if (!_auth.UsuarioTieneRol("ADMINISTRADOR")) return Forbid();
            if (id <= 0) return BadRequest();

            var usuario = _repo.buscarPorId(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // DELETE POST (eliminación lógica)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(short id)
        {
            if (!_auth.UsuarioTieneRol("ADMINISTRADOR")) return Forbid();

            var filas = _repo.eliminarLogico(id);
            if (filas == 0) return BadRequest("No se pudo eliminar el usuario.");

            TempData["ok"] = "Usuario eliminado.";
            return RedirectToAction(nameof(IndexUsuario));
        }

        // BLOQUEAR / DESBLOQUEAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarBloqueo(short id, bool bloquear)
        {
            if (!_auth.UsuarioTieneRol("ADMINISTRADOR")) return Forbid();

            var filas = _repo.bloquear(id, bloquear);
            if (filas == 0) return BadRequest("No se pudo actualizar el estado de bloqueo.");

            TempData["ok"] = bloquear ? "Usuario bloqueado." : "Usuario desbloqueado.";
            return RedirectToAction(nameof(IndexUsuario));
        }
    }
}
    
