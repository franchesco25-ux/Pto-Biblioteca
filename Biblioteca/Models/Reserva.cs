namespace Biblioteca.Models
{
    public class Reserva
    {
        public int id { get; set; }
        public int RecursoId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime fechaReserva { get; set; }
        public DateOnly fechaExpiracion { get; set; }
        public string estado { get; set; }
        public string ? sustento { get; set; }
        public short prioridad { get; set; }
        public short notificado { get; set; }
        public DateTime ? fechaNotificacion { get; set; }
        public short eliminado { get; set; } = 0;

        public string ? recurso_titulo { get; set; }
        public string ? usuario_nombre { get; set; }
    }
}
