using System.IO;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApiClientTS;

namespace WebApiSample
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

#if DEBUG // Use it just on development

            // The endpoint will be http://localhost:16120/c/g/api/run

            WebApiClientTS.GeneratorConfig config = new GeneratorConfig()
            {
                ControllerTemplate = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/TsTemplates/template.cshtml"),
                OutputFolderPath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~"), "App_Data", "clientApi"),
                IgnoreThoseControllers = new string[]
                {
                    "Values"
                }
            };
            
            WebApiClientTS.Generator.ConfigureApiGenerator(GlobalConfiguration.Configuration.Routes, config);
#endif
        }
    }
}
