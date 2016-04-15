﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Excess.Web
{
    public class MvcApplication : HttpApplication
    {
        protected virtual void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
        }

        protected virtual void Session_Start(object sender, EventArgs e)
        {
        }

        protected virtual void Session_End(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     This method is called by ASP.NET system when a request starts.
        /// </summary>
        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     This method is called by ASP.NET system when a request ends.
        /// </summary>
        protected virtual void Application_EndRequest(object sender, EventArgs e)
        {
        }

        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

            // Get the exception object.
            var exc = Server.GetLastError();

            // Handle HTTP errors
            if (exc.GetType() == typeof(HttpException))
            {
                // The Complete Error Handling Example generates
                // some errors using URLs with "NoCatch" in them;
                // ignore these here to simulate what would happen
                // if a global.asax handler were not implemented.
                if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
                    return;

                //Redirect HTTP errors to HttpError page
                Server.Transfer("HttpErrorPage.aspx");
            }

            // For other kinds of errors give the user some information
            // but stay on the default page
            Response.Write("<h2>Global Page Error</h2>\n");
            Response.Write(
                "<p>" + exc.Message + "</p>\n");

            // Clear the error from the server
            Server.ClearError();
        }

        //    //            if (ctxFactory == null)
        //    //            var ctxFactory = HttpContext.Current.Session["dsl"] as IDSLFactory;
        //    //        .UsingFactoryMethod(() => {
        //    //    Component.For<IDSLFactory>()
        //    //IocManager.Instance.IocContainer.Register(
        //    //IocManager.Instance.IocContainer.AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));
        //{
        //public override void Init()
        //    //                return new SimpleFactory();

        //    //            return ctxFactory;
        //    //        }).LifeStyle.Singleton);

        //    //base.Application_Start(sender, e);
        //}
    }
}