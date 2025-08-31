using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Repositorios
{
    public interface IRecursos
    {
        IEnumerable<Recursos> ListResources();
        void deleteResources(int?  id);

        int insertResource(Recursos reg);
        int asociarAutorARecurso(int recursoId, string autorNombre, string? autorApellido, string tipoAutor = "principal");
        void generarMulta();

        ResponseJSON crearRecurso(Recursos recursos);

        IEnumerable<SelectView> listTipoRecursos();
        IEnumerable<SelectView> listEditoriales();
        IEnumerable<SelectView> listGeneros();

        ResponseJSON editarRecurso(Recursos recursos);
    }
}
