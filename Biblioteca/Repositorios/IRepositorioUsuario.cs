using Biblioteca.Models;

namespace Biblioteca.Repositorios
{
    public interface IRepositorioUsuario
    {
        Task<Usuario?> ObtenerPorUsuarioActivoAsync(String usuarioLogin);

    }
}
