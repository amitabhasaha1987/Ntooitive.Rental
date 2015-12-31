using Configuration;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UserInterface
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AutoMapperConfiguration.Configure();

        }
        //protected void Application_Error(Object sender, EventArgs e)
        //{
        //    Exception ex = Server.GetLastError();
        //    Console.WriteLine(ex.Message);
        //    // Logger
        //    Response.Redirect("/index/index");
        //}

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

            // Get the exception object.
            Exception exc = Server.GetLastError();

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
                Response.Redirect("/index/index");
            }

            // For other kinds of errors give the user some information
            // but stay on the default page
            //Response.Write("<h2>Global Page Error</h2>\n");
            //Response.Write(
            //    "<p>" + exc.Message + "</p>\n");
            //Response.Write("Return to the <a href='Default.aspx'>" +
            //    "Default Page</a>\n");

            // Log the exception and notify system operators
            

            // Clear the error from the server
            Server.ClearError();
        }
    }
}
