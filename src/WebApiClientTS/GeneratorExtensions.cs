using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TypeLite;

namespace WebApiClientTS
{
    public static class GeneratorExtensions
    {
        public static string CompileModels(this ApiControllerMetadata apiControllerMetadata)
        {
            return CompileModels(apiControllerMetadata.UsedModels.ToList());
        }

        public static string CompileModels(this MethodMetadata methodMetadat)
        {
            return CompileModels(methodMetadat.UsedModels.ToList());
        }

        private static string CompileModels(List<Type> types)
        {
            var builder = new TsModelBuilder();
            types.ForEach(a => builder.Add(a));

            var model = builder.Build();

            var generator = new TsGenerator();
            generator.IndentationString = "    ";
            generator.SetTypeVisibilityFormatter((tsClass, typeName) => true);
            generator.SetModuleNameFormatter((module) => "");
            generator.SetIdentifierFormatter((identifier) => Char.ToLower(identifier.Name[0]) + identifier.Name.Substring(1));

            string typeScript = generator.Generate(model);

            return typeScript;
        }

        #region String

        public static string ToLowerCaseFirstLetter(this string value)
        {
            return String.Format("{0}{1}", value.First().ToString().ToLower(), value.Substring(1));
        }

        public static string ToUpperCaseFirstLetter(this string value)
        {
            return String.Format("{0}{1}", value.First().ToString().ToUpper(), value.Substring(1));
        }

        public static string ToConstantName(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.First().ToString().ToUpper() + Regex.Replace(value.Substring(1), @"(?<=[a-z])([A-Z])", @"_$1").ToUpper();
        }

        #endregion
    }
}
