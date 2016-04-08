namespace WebApiClientTS
{
    public sealed class GeneratorConfig
    {
        public string ControllerTemplate { get; set; }
        public object FileNameSuffix { get; set; } = ".api.service.ts";
        public string[] IgnoreThoseControllers { get; set; } = new string[] { };
        public string OutputFolderPath { get; set; }
    }
}
