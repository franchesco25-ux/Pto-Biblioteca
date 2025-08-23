using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Repositorios
{
    public interface IRecursos
    {
        IEnumerable<Recursos> ListResources();
        void deleteResources(int?  id);
    }
}
