namespace WebApiClientTS
{
    public class ParameterMetadata
    {
        public bool IsNullable { get; internal set; }
        public bool IsQueryParam { get; internal set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}