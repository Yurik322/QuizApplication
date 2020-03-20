﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QuizApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Test",       // http://localhost:53029/Test/{guid}

                url: "Test/{guid}", //  http://localhost:53029/Quiz/GetInfoAndStartTest /Test/{testGuid}
                defaults: new {controller = "Quiz", action = "Quiz"});

            routes.MapRoute(
                name: "CSV",
                url: "CSV/{testGuid}",
                defaults: new {controller = "Admin", action = "GetResultsForTestCsv" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Account", action = "Login" }
            );
        }
    }
}
