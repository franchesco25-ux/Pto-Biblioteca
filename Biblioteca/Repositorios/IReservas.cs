using Biblioteca.Models;

namespace Biblioteca.Repositorios
{
    public interface IReservas
    {
        IEnumerable<Reserva> listarReservas(int id);
    }
}
