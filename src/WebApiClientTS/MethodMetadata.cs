using System;
using System.Collections.Generic;

namespace WebApiClientTS
{
    public class MethodMetadata
    {
        public string Name { get;  set; }
        public IList<ParameterMetadata> Parameters { get; set; }
        public string ReturnType { get; set; }
        public string Url { get; set; }
        public string HttpMethod { get; set; }
        public string RequestData { get;  set; }
        public ISet<Type> UsedModels { get;  set; }
        public string RelativePath { get;  set; }
        public string Cache { get;  set; }
    }
}