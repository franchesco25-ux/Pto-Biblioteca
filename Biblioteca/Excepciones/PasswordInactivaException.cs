namespace Biblioteca.Excepciones
{
    public class PasswordInactivaException: System.Exception
    {
        public PasswordInactivaException() : base("password_inactiva") { }
    }
}
