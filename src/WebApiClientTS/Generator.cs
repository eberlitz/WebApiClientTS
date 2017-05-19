using RazorEngine.Configuration;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace WebApiClientTS
{
    public sealed class Generator
    {
        private readonly GeneratorConfig _configuration;

        public Generator(GeneratorConfig configuration)
        {
            _configuration = configuration;
        }

        public void Run(IEnumerable<ApiControllerMetadata> apiControllers)
        {
            string templateDir = Path.GetDirectoryName(_configuration.ControllerTemplate);
            string templateFile = Path.GetFileName(_configuration.ControllerTemplate);

            var config = new TemplateServiceConfiguration();
            config.TemplateManager = new ResolvePathTemplateManager(new List<string>() {
                templateDir
            });

            var service = RazorEngineService.Create(config);
            service.Compile(templateFile, typeof(ApiControllerMetadata));

            foreach (var apiController in apiControllers)
            {
                string result = service.Run(templateFile, typeof(ApiControllerMetadata), apiController);
                WriteToFile(apiController.Name, result);
            }
        }

        private void WriteToFile(string name, string content)
        {
            if (!Directory.Exists(_configuration.OutputFolderPath))
            {
                Directory.CreateDirectory(_configuration.OutputFolderPath);
            }

            string outputFilePath = Path.Combine(_configuration.OutputFolderPath, string.Format("{0}{1}", name.ToLowerCaseFirstLetter(), _configuration.FileNameSuffix));

            using (StreamWriter streamWriter = new StreamWriter(outputFilePath))
            {
                streamWriter.Write(content);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        public static void ConfigureApiGenerator(System.Web.Http.HttpRouteCollection routes, GeneratorConfig configuration)
        {
            GeneratorConfig.CheckGeneratorConfig(configuration);

            routes.MapHttpRoute(
                   name: "CodeGenerator",
                   routeTemplate: "C/G/API/{action}",
                   defaults: null,
                   constraints: null,
                   handler: new GeneratorHandler(configuration)
               );
        }
    }

    internal sealed class GeneratorHandler : HttpMessageHandler
    {
        private readonly GeneratorConfig _configuration;

        public GeneratorHandler(GeneratorConfig configuration)
        {
            _configuration = configuration;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var controller = new CodeGeneratorController(_configuration)
            {
                // Do not change the order, I warned!
                RequestContext = request.GetRequestContext(),
                Request = request
            };

            // TODO: Think about changing this implementation to a more manual way in order to configure jsonFormatter and binding rules differently
            var httpControllerContext = new System.Web.Http.Controllers.HttpControllerContext
            {
                Controller = controller,
                Configuration = request.GetConfiguration(),
                Request = request,
                RouteData = request.GetRouteData(),
                RequestContext = request.GetRequestContext(),
                ControllerDescriptor = new System.Web.Http.Controllers.HttpControllerDescriptor(request.GetConfiguration(), "CodeGenerator", typeof(CodeGeneratorController))
            };

            controller.ControllerContext = httpControllerContext;

            var actionSelector = request.GetConfiguration().Services.GetActionSelector();
            var actionInvoker = request.GetConfiguration().Services.GetActionInvoker();
            var actionValueBinder = request.GetConfiguration().Services.GetActionValueBinder();
            var actionDescriptor = actionSelector.SelectAction(httpControllerContext);

            var httpActionContext = new System.Web.Http.Controllers.HttpActionContext()
            {
                ControllerContext = httpControllerContext,
                ActionDescriptor = actionDescriptor
            };

            await actionValueBinder.GetBinding(actionDescriptor).ExecuteBindingAsync(httpActionContext, cancellationToken);

            var result = await actionInvoker.InvokeActionAsync(httpActionContext, cancellationToken);

            var taskCompletionSource = new TaskCompletionSource<HttpResponseMessage>();
            taskCompletionSource.SetResult(result);

            return await taskCompletionSource.Task;
        }
    }
    
    //[Authorize]
    //[RoutePrefix("Api/CodeGenerator")]
    [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)] // Invisible in ApiExplorer, we don't want to generate this :D
    internal sealed class CodeGeneratorController : ApiController
    {
        private readonly GeneratorConfig _configuration;

        public CodeGeneratorController(GeneratorConfig configuration)
        {
            // I know, it was checked before, but nobody knows the future ;D
            GeneratorConfig.CheckGeneratorConfig(configuration);

            _configuration = configuration;
        }

        [HttpGet]
        [Route("Run")]
        public string Run()
        {
            Generator generator = new Generator(_configuration);

            List<ApiDescription> apis = Configuration.Services.GetApiExplorer().ApiDescriptions.OrderBy(o => o.RelativePath)
                .Where(w => !_configuration.IgnoreThoseControllers.Contains(w.ActionDescriptor.ControllerDescriptor.ControllerName))
                .ToList();

            // Sample of an stringify function
            Func<string, string> stringifyFunction = (parameterName) => $"JSON.stringify({parameterName})";
            //Func<string, string> stringifyFunction = (parameterName) => $"\"'\"+{parameterName}+\"'\"";

            var metadata = ApiDescriptorMetadata.From(apis, stringifyFunction).Where(ctrl => !_configuration.IgnoreThoseControllers.Contains(ctrl.Name));

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
