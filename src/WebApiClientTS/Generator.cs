using RazorEngine.Configuration;
using RazorEngine.Templating;
using System.Collections.Generic;
using System.IO;

namespace WebApiClientTS
{
    public sealed class Generator
    {
        private GeneratorConfig _configuration;

        public Generator(GeneratorConfig configuration)
        {
            this._configuration = configuration;
        }

        public void Run(IEnumerable<ApiControllerMetadata> apiControllers)
        {
            var templateDir = Path.GetDirectoryName(this._configuration.ControllerTemplate);
            var templateFile = Path.GetFileName(this._configuration.ControllerTemplate);

            var config = new TemplateServiceConfiguration();
            config.TemplateManager = new ResolvePathTemplateManager(new List<string>() {
                templateDir
            });
            var service = RazorEngineService.Create(config);
            service.Compile(templateFile, typeof(ApiControllerMetadata));

            foreach (var apiController in apiControllers)
            {
                var result = service.Run(templateFile, typeof(ApiControllerMetadata), apiController);
                WriteToFile(apiController.Name, result);
            }
        }

        private void WriteToFile(string name, string content)
        {
            if (!Directory.Exists(_configuration.OutputFolderPath))
            {
                Directory.CreateDirectory(_configuration.OutputFolderPath);
            }
            var outputFilePath = Path.Combine(_configuration.OutputFolderPath, string.Format("{0}{1}", name.ToLowerCaseFirstLetter(), _configuration.FileNameSuffix));

            File.WriteAllText(outputFilePath, content);
        }
    }
}
