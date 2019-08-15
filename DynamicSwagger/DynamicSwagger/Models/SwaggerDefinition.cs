using Newtonsoft.Json;
using System.Collections.Generic;

namespace DynamicSwagger.Models
{
    public class SwaggerDefinition
    {
        [JsonProperty("swagger")]
        public string SwaggerVersion { get; set; }
        public SwaggerInformation Info { get; set; }
        public Dictionary<string,Dictionary<string, ApiMethod>> Paths { get; set; }
        public Dictionary<string,Definition> Definitions { get; set; }
    }

    public class SwaggerInformation
    {
        public string Title { get; set; }
        public string Version { get; set; }
    }

    public class ApiMethod
    {
        public List<string> Tags { get; set; }
        public string OperationId { get; set; }
        public List<string> Consumes { get; set; }
        public List<string> Produces { get; set; }
        public List<ApiParameter> Parameters { get; set; }
        public Dictionary<string, ApiResponse> Responses { get; set; }
    }

    public abstract class ApiParameter
    {
        public string Name { get; set; }
        public string In { get; set; }
        public bool Required { get; set; }
    }

    public class PathParameter : ApiParameter
    {
        public PathParameter()
        {
            Required = true;
            In = "path";
        }

        public string Type { get; set; }
        public string Format { get; set; }
    }

    public class BodyParameter : ApiParameter
    {
        public BodyParameter()
        {
            In = "body";
        }

        public DefinitionReference Schema { get; set; }
    }

    public class ApiResponse
    {
        public string Description { get; set; }
    }

    public class ApiDataResponse : ApiResponse
    {
        public DefinitionReference Schema { get; set; }
    }

    public class Definition
    {
        public string Type { get; set; }
        public List<Property> Properties { get; set; }
    }

    public class DefinitionReference
    {
        [JsonProperty("$ref")]
        public string Reference { get; set; }
    }

    public class Property
    {
        public string Type { get; set; }
    }

    public class FormatProperty : Property
    {
        public string Format { get; set; }
    }

    public class ArrayProperty : Property
    {
        public bool UniqueItems { get; set; }
        public DefinitionReference Items { get; set; }
    }
}
