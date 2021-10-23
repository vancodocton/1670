using System.Web.Mvc;

namespace WebApp.Areas.Staff
{
    public class StaffAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Staff";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Staff_default",
                "Staff/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "WebApp.Areas.Staff.Controllers" }
            );
            //context.MapRoute(
            //    "Staff_assign_index",
            //    "Staff/Assign/{action}/{courseId}",
            //    new { controller = "Assign", action = "Index", courseId = UrlParameter.Optional },
            //    namespaces: new[] { "WebApp.Areas.Staff.Controllers" }
            //);
            //context.MapRoute(
            //    "Staff_assign_add",
            //    "Staff/Assign/Add/{courseId}/{user}",
            //    new { controller = "Assign", action = "Add", courseId = UrlParameter.Optional, user = UrlParameter.Optional },
            //    namespaces: new[] { "WebApp.Areas.Staff.Controllers" }
            //);
        }
    }
}