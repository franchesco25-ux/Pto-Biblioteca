using Biblioteca.Models;

namespace Biblioteca.Repositorios
{
    public interface IReservas
    {
        IEnumerable<Reserva> listarReservas(int id);
        IEnumerable<SelectView> listaRecursos();
        IEnumerable<SelectView> listaUsuarios();
        ResponseJSON registrarReserva(Reserva reg);
    }
}
