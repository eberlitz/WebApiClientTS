using System;
using System.Collections.Generic;

namespace WebApiClientTS
{
    public class ApiControllerMetadata
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public IList<MethodMetadata> Methods { get; set; }
        public ISet<Type> UsedModels { get; set; }
    }
}