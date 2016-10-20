#if DEBUG // Use it just on development
namespace WebApiSample.Controllers.Api
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;
    using WebApiClientTS;

    [RoutePrefix("Api/CodeGenerator")]
    [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)] // Invisible in ApiExplorer, we don't want to generate this :D
    public class CodeGeneratorController : ApiController
    {
        [HttpGet]
        [Route("Run")]
        public string Run()
        {
            GeneratorConfig config = new GeneratorConfig()
            {
                ControllerTemplate = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/TsTemplates/template.cshtml"),
                OutputFolderPath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~"), "App_Data", "clientApi"),
                IgnoreThoseControllers = new string[]
                {
                    "Values"
                }
            };

            Generator generator = new Generator(config);

            List<ApiDescription> apis = Configuration.Services.GetApiExplorer().ApiDescriptions.OrderBy(o => o.RelativePath)
                .Where(w => !config.IgnoreThoseControllers.Contains(w.ActionDescriptor.ControllerDescriptor.ControllerName))
                .ToList();

            var metadata = ApiDescriptorMetadata.From(apis).Where(ctrl => !config.IgnoreThoseControllers.Contains(ctrl.Name));

            // Add settings to the template of AngularJS request
            metadata.SelectMany(a => a.Methods).ToList().ForEach(m =>
            {
                m.Parameters.Add(new ParameterMetadata()
                {
                    Name = "config?",
                    Type = "ng.IRequestConfig"
                });
            });

            generator.Run(metadata);

            return "Ok";
        }
    }
}
#endif
