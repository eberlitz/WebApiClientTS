using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace WebApiClientTS
{
    public class ApiDescriptorMetadata
    {
        public static IEnumerable<ApiControllerMetadata> From(IList<ApiDescription> apiDescriptions)
        {
            return apiDescriptions
                .GroupBy(g => g.ActionDescriptor.ControllerDescriptor)
                .Distinct()
                .ToList()
                .Select(controllerApis =>
                {
                    var controllerUsedModels = new HashSet<Type>();
                    return new ApiControllerMetadata
                    {
                        Name = controllerApis.Key.ControllerName,
                        Type = controllerApis.Key.ControllerType,
                        UsedModels = controllerUsedModels,
                        Methods = controllerApis.Select(api =>
                        {
                            var apiUsedModels = GetUsedModelsFrom(api);
                            controllerUsedModels.UnionWith(apiUsedModels);
                            return new MethodMetadata()
                            {
                                Name = GetMethodName(api, controllerApis.ToList()),
                                Parameters = api.ParameterDescriptions
                                    .Select(p =>
                                    {
                                        return new ParameterMetadata()
                                        {
                                            Name = p.ParameterDescriptor.ParameterName,
                                            Type = ConvertTypeToTs(p.ParameterDescriptor.ParameterType),
                                            IsQueryParam = GetQueryParam(p.ParameterDescriptor),
                                            IsNullable = IsNullableType(p.ParameterDescriptor.ParameterType)
                                        };
                                    }).ToList(),
                                ReturnType = GenerateInlineClassResult(api),
                                Url = api.RelativePath.Replace("{", "${"),
                                RelativePath = GetRelativePath(api),
                                HttpMethod = api.HttpMethod.ToString(),
                                RequestData = GetRequestData(api),
                                UsedModels = apiUsedModels,
                                Cache = GetCacheValue(api)
                            };
                        }).ToList()
                    };
                }).ToList();
        }

        private static string GetCacheValue(ApiDescription apiDescription)
        {
            var csToTsAttribute = apiDescription.ActionDescriptor.GetCustomAttributes<CacheDescriptorAttribute>().FirstOrDefault();
            if (csToTsAttribute != null)
            {
                return csToTsAttribute.Cache.ToString().ToLower();
            }
            else
            {
                return string.Empty;
            }
        }

        private static bool GetQueryParam(HttpParameterDescriptor parameterDescriptor)
        {
            if (IsFromBody(parameterDescriptor))
            {
                return false;
            }
            return true;
        }

        private static bool IsFromBody(HttpParameterDescriptor parameterDescriptor)
        {
            return parameterDescriptor.ParameterBinderAttribute != null && parameterDescriptor.ParameterBinderAttribute.GetType().Name == "FromBodyAttribute";
        }

        public static string GetRelativePath(ApiDescription apiDescription)
        {
            int final = apiDescription.RelativePath.IndexOf("?");
            return apiDescription.RelativePath.Substring(0, final > 0 ? final : apiDescription.RelativePath.Length);
        }

        public static Type GetReturnType(ApiDescription api)
        {
            if (api.ResponseDescription.ResponseType == null)
            {
                if (api.ResponseDescription.DeclaredType != null && api.ResponseDescription.DeclaredType.Name != "IHttpActionResult")
                {
                    return api.ResponseDescription.DeclaredType;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return api.ResponseDescription.ResponseType;
            }
        }

        public static bool IsDateTimeType(Type type)
        {
            return type == typeof(DateTime);
        }

        public static bool IsDateTimeOffsetType(Type type)
        {
            return type == typeof(DateTimeOffset);
        }

        public static bool IsDecimal(Type type)
        {
            return type == typeof(Decimal);
        }

        public static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || IsDateTimeType(type) || IsDecimal(type) || IsDateTimeOffsetType(type);
        }

        private static IList<Type> GetNestedUsedModels(Type type, ISet<Type> processedTypes)
        {
            if (type == null || processedTypes.Contains(type))
            {
                return Enumerable.Empty<Type>().ToList();
            }
            processedTypes.Add(type);

            //Se primitivo (simples) remover
            if (IsSimpleType(type))
            {
                return Enumerable.Empty<Type>().ToList();
            }

            // Se nullable, pega os NestedUsedModels do tipo dele
            if (IsNullableType(type))
            {
                var nullableOfType = type.GetGenericArguments()[0];
                return GetNestedUsedModels(nullableOfType, processedTypes);
            }
            //Se Lista, pega os NestedUsedModels do tipo dele
            if (IsList(type))
            {
                Type listOfType;
                if (type.IsArray)
                {
                    listOfType = type.GetElementType();
                }
                else
                {
                    listOfType = type.GetGenericArguments()[0];
                }
                return GetNestedUsedModels(listOfType, processedTypes);
            }

            // Se chegar aqui, type é um tipo complexo
            var types = new List<Type>();
            types.Add(type);
            types.AddRange(type.GetProperties().SelectMany(a => GetNestedUsedModels(a.PropertyType, processedTypes)));
            return types;
        }

        private static ISet<Type> GetUsedModelsFrom(ApiDescription api)
        {
            // Para cada tipo:
            // - Se primitivo (simples) remover
            // - Se Lista Obter tipo
            // - Para cada propriedade do tipo, fazer o mesmo.
            var parameters = new HashSet<Type>(api.ParameterDescriptions.Select(a => a.ParameterDescriptor.ParameterType));
            var returnType = GetReturnType(api);
            if (returnType != null)
            {
                parameters.Add(returnType);
            }
            return new HashSet<Type>(parameters.SelectMany(a => GetNestedUsedModels(a, new HashSet<Type>())));
        }

        private static string GetRequestData(ApiDescription apiDescription)
        {
            string body = "null";
            foreach (var paramItem in apiDescription.ParameterDescriptions)
            {
                // Somente para aqueles parâmetros que são HttpBody
                if (IsFromBody(paramItem.ParameterDescriptor))
                {
                    // Como é o body não deve ser formatada como objeto
                    body = paramItem.ParameterDescriptor.ParameterName;
                }
            }
            return body;
        }

        private static string GenerateInlineClassResult(ApiDescription apiDescription)
        {
            string returnType = ConvertTypeToTs(apiDescription.ResponseDescription.DeclaredType);

            // Para os controllers sem retorno (void)
            if (apiDescription.ResponseDescription.DeclaredType == null)
            {
                returnType = "void";
            }
            // Se o tipo for este, então significa que não foi definido nenhum tipo especifico
            else if (apiDescription.ResponseDescription.DeclaredType.Name == "IHttpActionResult")
            {
                returnType = "any";
            }
            // Se pegar aqui significa que foi especificado um tipo de retorno através do atributo ResponseType
            else if (apiDescription.ResponseDescription.ResponseType != null)
            {
                returnType = ConvertTypeToTs(apiDescription.ResponseDescription.ResponseType);
            }
            // Se tem algum tipo definido na action mesmo e este for generico. TODO: Refatorar para melhorar esta verificação de genérico, aqui queremos saber se é uma lista, não um genérico, um tipo Nullable também é um genérico mas não é uma lista.
            else if (IsIEnumerableType(apiDescription.ResponseDescription.DeclaredType))
            {
                returnType = ConvertTypeToTs(apiDescription.ResponseDescription.DeclaredType.GetGenericArguments()[0]) + "[]";
            }

            // Se não caiu em qualquer teste, provavelmente o tipo é genérico porém não é um tipo que implementa IEnumerable, entao retorna ele mesmo (PagedResult<Product>)
            return returnType;
        }

        private static bool IsIEnumerableType(Type type) => type.GetInterfaces().Any((item) => item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        private static string ConvertTypeToTs(Type type)
        {
            // Para os controllers sem retorno (void)
            if (type == null)
            {
                return "void";
            }

            if (IsNullableType(type))
            {
                // Se é nullable eu passo o tipo de dentro do genérico
                return ConvertTypeToTs(type.GetGenericArguments()[0]);
            }

            if (type == typeof(int[]) || type == typeof(Int16[]) || type == typeof(Int32[]) || type == typeof(Int64[]) ||
                type == typeof(uint[]) || type == typeof(UInt16[]) || type == typeof(UInt32[]) || type == typeof(UInt64[]) ||
                type == typeof(decimal[]) || type == typeof(Decimal[]) ||
                type == typeof(float[]) ||
                type == typeof(double[]) || type == typeof(Double[]))
            {
                return "number[]";
            }

            if (type == typeof(int) || type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64) ||
                type == typeof(uint) || type == typeof(UInt16) || type == typeof(UInt32) || type == typeof(UInt64) ||
                type == typeof(decimal) || type == typeof(Decimal) ||
                type == typeof(float) ||
                type == typeof(double) || type == typeof(Double))
            {
                return "number";
            }

            if (type == typeof(string[]) || type == typeof(String[]))
            {
                return "string[]";
            }

            if (type == typeof(string) || type == typeof(String))
            {
                return "string";
            }

            if (type == typeof(bool[]) || type == typeof(Boolean[]))
            {
                return "boolean[]";
            }

            if (type == typeof(bool) || type == typeof(Boolean))
            {
                return "boolean";
            }

            if (type == typeof(DateTime[]))
            {
                return "string[]";
            }

            if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
            {
                return "Date";
            }

            if (type.IsEnum)
            {
                return type.Name;
            }
            
            if (IsList(type))
            {
                if (type.IsArray)
                {
                    // Como é um array é necessário fazer aquela mágica do []
                    return ConvertTypeToTs(type.GetElementType()) + "[]";
                }
                else
                {
                    return ConvertTypeToTs(type.GetGenericArguments()[0]) + "[]";
                }
            }

            if (type.IsGenericType)
            {
                return type.Name.Replace("`1", "") + "<" + string.Join(",", type.GetGenericArguments().Select(a => a.Name)) + ">";
            }

            return type.Name;
        }

        public static bool IsNullableType(Type type)
        {
            // https://msdn.microsoft.com/en-us/library/ms366789.aspx
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static bool IsList(Type isList)
        {
            return typeof(System.Collections.IEnumerable).IsAssignableFrom(isList);
        }

        private static string GetMethodName(ApiDescription apiDescription, IList<ApiDescription> controllerApis)
        {
            // Agrupamento pelo Controller, ActionName e HttpMethod
            int actionGroup = controllerApis
                       .Where(w =>
                            w.ActionDescriptor.ControllerDescriptor.ControllerName == apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName &&
                            w.ActionDescriptor.ActionName == apiDescription.ActionDescriptor.ActionName &&
                            w.HttpMethod == apiDescription.HttpMethod)
                       .Count();

            string actionName = apiDescription.ActionDescriptor.ActionName;

            string classMethodName = apiDescription.HttpMethod.Method.ToLower() + actionName.First().ToString().ToUpper() + actionName.Substring(1);

            // Se tem mais de uma action para o mesmo método e esta tem algum parâmetro vou adicionar o nome do primeiro parâmetro na action
            if (actionGroup > 1 && apiDescription.ParameterDescriptions.Count > 0)
            {
                foreach (var item in apiDescription.ParameterDescriptions)
                {
                    classMethodName = classMethodName + item.Name.First().ToString().ToUpper() + item.Name.Substring(1);
                }
            }

            return classMethodName;
        }
    }
}
