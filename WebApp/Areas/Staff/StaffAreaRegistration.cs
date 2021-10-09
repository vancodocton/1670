﻿using System.Web.Mvc;

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
        }
    }
}