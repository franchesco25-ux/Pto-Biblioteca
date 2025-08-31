namespace Biblioteca.Repositorios
{
    public interface IUsuarioRepository<T> where T : class
    {
        // CREATE: retorna el nuevo ID (SCOPE_IDENTITY) o 0 si falla
        short crear(T entity);
        // UPDATE: retorna filas afectadas
        int actualizar(T entity);
        // DELETE lógico: eliminado = 1 por ID
        int eliminarLogico(short id);
        // READ por ID (solo vigentes)
        T? buscarPorId(short id);
        // LIST: solo vigentes (eliminado = 0)
        IEnumerable<T> listarVigentes();
        // BLOQUEAR / DESBLOQUEAR: cuenta_bloqueada = 1/0
        int bloquear(short id, bool bloquear);

    }
}
