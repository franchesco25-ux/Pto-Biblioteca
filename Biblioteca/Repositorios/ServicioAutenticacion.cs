using Biblioteca.Excepciones;
using Biblioteca.Models;
using BCrypt.Net;
namespace Biblioteca.Repositorios
{
    public class ServicioAutenticacion : IServicioAutenticacion
    {

        public const string CLAVE_SESION = "SESION_USUARIO";
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ServicioAutenticacion(IRepositorioUsuario repositorioUsuario, IHttpContextAccessor htthttpContextAccessor)
        {
            _httpContextAccessor = htthttpContextAccessor;
            _repositorioUsuario = repositorioUsuario;
        }

        public async Task<Sesion> IniciarSesionAsync(string usuarioLogin, string password)
        {

            Usuario? usuario = await _repositorioUsuario.ObtenerPorUsuarioActivoAsync(usuarioLogin);
            if (usuario == null)
            {
                throw new UsuarioNoEncontradoException();
            }

            if (usuario.CuentaBloqueada)
            {
                throw new UsuarioBloqueadoException();
            }

            if (!usuario.PasswordActiva)
            {
                throw new PasswordInactivaException();
            }
            
            bool ok = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
            if (!ok)
            {
                throw new CredencialesInvalidasException();
            }

            Sesion sesion = new Sesion
            {
                UsuarioId = usuario.UsuarioId,
                UsuarioLogin = usuario.UsuarioLogin,
                NombreCompleto = usuario.Nombre + " " + usuario.Apellido,
                Rol = usuario.Rol
            };

            GuardarSesion(sesion);
            return sesion;
        }

        private void GuardarSesion(Sesion sesion)
        {
            HttpContext? http = _httpContextAccessor.HttpContext;
            if (http == null)
            {
                throw new System.InvalidOperationException("No hay HttpContext disponible.");
            }
            string json = System.Text.Json.JsonSerializer.Serialize(sesion);
            http.Session.SetString(CLAVE_SESION,json);
        }



        public void CerrarSesion()
        {
            HttpContext? http = _httpContextAccessor.HttpContext;
            if (http != null)
            {
                http.Session.Remove(CLAVE_SESION);
            }
        }

        

        public Sesion? ObtenerSesionActual()
        {
            HttpContext? http = _httpContextAccessor.HttpContext;
            if (http == null)
            {
                return null;
            }

            string? json = http.Session.GetString(CLAVE_SESION);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            Sesion? sesion = System.Text.Json.JsonSerializer.Deserialize<Sesion>(json);
            
            return sesion;
        }

        public bool UsuarioTieneRol(string rol)
        {
            Sesion sesion = ObtenerSesionActual();
            if (sesion == null)
            {
                return false;
            }
            if (rol == null || rol.Length == 0)
            {
                return true;
            }

            if (string.Equals(sesion.Rol, rol, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public bool UsuarioTieneAlgunRol(string[] roles)
        {
            Sesion sesion = ObtenerSesionActual();
            if (sesion == null || roles == null || roles.Length == 0)
            {
                return false;
            }

            return roles.Any(rol =>
                string.Equals(sesion.Rol, rol, StringComparison.OrdinalIgnoreCase)); 
        }
    }
}
