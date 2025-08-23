using Biblioteca.Models;
using Biblioteca.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Biblioteca.Filtros
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class RequiereSesionAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IServicioAutenticacion auth = (IServicioAutenticacion)context.HttpContext.RequestServices.GetService(typeof(IServicioAutenticacion))!;
            Sesion? sesion = auth.ObtenerSesionActual();
            if (sesion == null)
            {
                context.Result = new RedirectToActionResult("Acceder", "Login", new { m = "unauthorized" });
                return;
            }
            await next();
        }
    }
}
