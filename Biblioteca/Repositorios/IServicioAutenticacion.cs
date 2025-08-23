using Biblioteca.Models;

namespace Biblioteca.Repositorios
{
    public interface IServicioAutenticacion
    {
        Task<Sesion> IniciarSesionAsync(string usuarioLogin, string password);
        void CerrarSesion();
        Sesion? ObtenerSesionActual();
        bool UsuarioTieneRol(String rol);
        bool UsuarioTieneAlgunRol(string[] roles);

    }
}
