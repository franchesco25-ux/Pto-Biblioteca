using Biblioteca.Models;
using Biblioteca.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Biblioteca.Filtros
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiereRolAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string[] _rol;
        public RequiereRolAttribute(params string[] rolPermitido)
        {
            _rol = rolPermitido;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IServicioAutenticacion auth = (IServicioAutenticacion)context.HttpContext.RequestServices.GetService(typeof(IServicioAutenticacion))!;
            Sesion? sesion = auth.ObtenerSesionActual();
            if (sesion == null)
            {
                context.Result = new RedirectToActionResult("Acceder", "Login", new { m = "unauthorized" });
                return;
            }
            
            bool tieneAcceso = _rol.Any(rol => auth.UsuarioTieneRol(rol));

            if (!tieneAcceso)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }
            await next();
        }
    }
}
