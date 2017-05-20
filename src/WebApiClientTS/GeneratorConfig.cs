using System;
using System.IO;

namespace WebApiClientTS
{
    /// <summary>
    /// Represents the configuration that you can pass to generator.
    /// </summary>
    public sealed class GeneratorConfig
    {
        /// <summary>
        /// The template file wich you want to use to generate.
        /// </summary>
        public string ControllerTemplate { get; set; }

        /// <summary>
        /// The file name suffix wich will be wrote at the generated files:
        /// Ex..:
        /// .api.service.ts
        /// .service.ts
        /// .api.ts
        /// </summary>
        public object FileNameSuffix { get; set; } = ".api.service.ts";

        /// <summary>
        /// You can pass the especific controller names to don't generate the ts client api for those.
        /// </summary>
        public string[] IgnoreThoseControllers { get; set; } = new string[] { };

        /// <summary>
        /// The folder where you want to generate the files.
        /// </summary>
        public string OutputFolderPath { get; set; }

        /// <summary>
        /// You can set the route template to acess the Run action on controller CodeGenerator.
        /// The default route is: C/G/API/{action}
        /// The endpoint will be http://localhost:16120/c/g/api/run
        /// </summary>
        public string RouteTemplate { get; set; }

        /// <summary>
        /// Stringify function to handle string parameters.
        /// </summary>
        public Func<string, string> StringifyFunction { get; set; }

        /// <summary>
        /// Check if configuration is valid.
        /// </summary>
        /// <param name="configuration">Configuration parameters to check.</param>
        public static void CheckGeneratorConfig(GeneratorConfig configuration)
        {
            if (configuration == null)
            {
                throw new Exception("You need to pass a configuration.");
            }
            else if (String.IsNullOrEmpty(configuration.ControllerTemplate))
            {
                throw new Exception("You need to pass a correct ControllerTemplate.");
            }
            else if (!new FileInfo(configuration.ControllerTemplate).Exists)
            {
                throw new Exception("You need to pass a exist ControllerTemplate.");
            }
            else if (String.IsNullOrEmpty(configuration.OutputFolderPath))
            {
                throw new Exception("You need to pass a OutputFolderPath.");
            }
        }
    }
}
