namespace Biblioteca.Models
{
    public class Sesion
    {
        public short UsuarioId { get; set; }
        public string UsuarioLogin { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
