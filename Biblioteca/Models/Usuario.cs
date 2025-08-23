namespace Biblioteca.Models
{
    public class Usuario
    {

        public short UsuarioId { get; set; }
        public string Rol { get; set; } = string.Empty;                 // ADMINISTRADOR | PROFESOR | ESTUDIANTE | BIBLIOTECARIO
        public string Identidad { get; set; } = string.Empty;           // DNI | RUC | CE | RLA
        public string DocumentoIdent { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string UsuarioLogin { get; set; } = string.Empty;        // columna `usuario`
        public string PasswordHash { get; set; } = string.Empty;        // bcrypt
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Mail { get; set; }
        public System.DateTime FechaRegistro { get; set; }
        public bool PasswordActiva { get; set; }
        public bool CuentaBloqueada { get; set; }
        public bool Eliminado { get; set; }
    }



}
