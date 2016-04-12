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
    }
}
