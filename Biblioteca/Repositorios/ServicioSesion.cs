using Biblioteca.Models;

namespace Biblioteca.Repositorios
{
    public class ServicioSesion : IServicioSesion
    {
        private readonly IServicioAutenticacion _servicioAutenticacion;

        public ServicioSesion(IServicioAutenticacion servicioAutenticacion)
        {
            _servicioAutenticacion = servicioAutenticacion;
        }

        public Sesion? Obtener()
        {
            return _servicioAutenticacion.ObtenerSesionActual();
        }
    }
}
