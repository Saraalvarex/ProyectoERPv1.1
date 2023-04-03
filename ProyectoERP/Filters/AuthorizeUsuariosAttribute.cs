using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ProyectoERP.Filters
{
    public class AuthorizeUsuariosAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            string controller = context.RouteData.Values["controller"].ToString();
            string action = context.RouteData.Values["action"].ToString();
            ITempDataProvider provider = context.HttpContext.RequestServices.GetService<ITempDataProvider>();
            var TempData = provider.LoadTempData(context.HttpContext);
            TempData["controller"] = controller;
            TempData["action"] = action;
            provider.SaveTempData(context.HttpContext, TempData);
            if (user.Identity.IsAuthenticated == false)
            {
                context.Result = this.GetRoute("Managed", "LogIn");
            }
            //else
            //{
                //MAS CARACTERISTICAS: ROL
                //if (user.IsInRole("ADMIN") == false && user.IsInRole("RECEPCION") == false && user.IsInRole("ANALISTA") == false)
                //{
                //context.Result = this.GetRoute("Managed", "ErrorAcceso");
                //}
            //}
        }
        //Como haremos varias redirecciones, creamos un metodo para crear las rutas
        private RedirectToRouteResult GetRoute(string controller, string action)
        {
            RouteValueDictionary ruta = new RouteValueDictionary(new
            {
                controller = controller,
                action = action
            });
            RedirectToRouteResult result = new RedirectToRouteResult(ruta);
            return result;
        }
    }
}
