using DynamicSwagger.Models;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Pluralize.NET.Core;
using System;
using System.Collections.Generic;

namespace DynamicSwagger.Helpers
{
    public class SwaggerGenerator
    {
        private readonly ItemDefinition _definition;
        private OpenApiDocument _apiDocument;
        private const string BASE_URL = "/api";
        private Pluralizer _pluralizer;
        private const string PROBLEM_DETAILS_REFERENCE = "ProblemDetails";
        private const string ROOT_SUFFIX = "Root";

        public SwaggerGenerator(ItemDefinition definition)
        {
            _definition = definition;
            _pluralizer = new Pluralizer();
        }

        public string Generate()
        {
            _apiDocument = new OpenApiDocument()
            {
                Info = new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = $"{_definition.Name.ToSentenceCase()} API",
                },
                Servers = new List<OpenApiServer>
                {
                    new OpenApiServer { Url = "http://localhost:57009" }
                },
                Paths = new OpenApiPaths(),
                Components = new OpenApiComponents()
                {
                    Schemas = new Dictionary<string, OpenApiSchema>()
                }
            };
            AddProblemDetailsReference();
            GenerateGetPath(BASE_URL, _definition.Name, _definition.Properties);
            GeneratePostPath(BASE_URL, _definition.Name, _definition.Properties);
            return _apiDocument.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json);
        }

        private void GenerateGetPath(string baseUrl, string definitionName, List<ItemProperty> properties)
        {
            string definitionTitle = definitionName.ToSentenceCase();
            baseUrl = $"{baseUrl}/{definitionName}";
            var path = $"{baseUrl}/" + "{" + GetSingle(definitionName) + "Id}";

            OpenApiPathItem pathItem;

            if (!_apiDocument.Paths.TryGetValue("path", out pathItem))
            {
                pathItem = new OpenApiPathItem();
                pathItem.Operations = new Dictionary<OperationType, OpenApiOperation>();
                _apiDocument.Paths.Add(path, pathItem);
            }

            var schema = new OpenApiSchema()
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>()
            };
            foreach(var property in properties)
            {
                if(property is ItemArrayProperty)
                {
                    var arrayProperty = property as ItemArrayProperty;
                    GenerateGetPath(path, property.Name, arrayProperty.Properties);
                }
                schema.Properties.Add(property.Name, GetProperty(property.Name, property.DataType));
            }

            _apiDocument.Components.Schemas.Add(GetDefinitionTitle(definitionName), schema);

            pathItem.Operations.Add(OperationType.Get, new OpenApiOperation()
            {
                Tags = GetTags(definitionName),
                OperationId = "Get",
                Description = $"Get a single {GetDefinitionTitle(definitionName)}",
                Parameters = GetPathParameters(path),
                Responses = new OpenApiResponses
                {
                    ["200"] = new OpenApiResponse
                    {
                        Description = "Success",
                        Reference = GetReference(definitionName)
                    }
                }
            });
        }

        private void GeneratePostPath(string baseUrl, string definitionName, List<ItemProperty> properties)
        {
            string definitionTitle = definitionName.ToSentenceCase();
            baseUrl = $"{baseUrl}/{definitionName}";

            OpenApiPathItem pathItem;

            if (!_apiDocument.Paths.TryGetValue("path", out pathItem))
            {
                pathItem = new OpenApiPathItem();
                pathItem.Operations = new Dictionary<OperationType, OpenApiOperation>();
                _apiDocument.Paths.Add(baseUrl, pathItem);
            }

            var schema = new OpenApiSchema()
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>()
            };
            foreach (var property in properties)
            {
                if (property is ItemArrayProperty)
                {
                    var arrayProperty = property as ItemArrayProperty;
                    var childUrl = $"{baseUrl}/" + "{" + GetSingle(definitionName) + "Id}";
                    GeneratePostPath(childUrl, property.Name, arrayProperty.Properties);
                }
                else
                {
                    schema.Properties.Add(property.Name, GetProperty(property.Name, property.DataType));
                }
            }

            _apiDocument.Components.Schemas.Add(GetRootDefinitionTitle(definitionName), schema);

            pathItem.Operations.Add(OperationType.Post, new OpenApiOperation()
            {
                Tags = GetTags(definitionName),
                OperationId = "Post",
                Description = $"Post a single {GetDefinitionTitle(definitionName)}",
                Parameters = GetPathParameters(definitionName),
                RequestBody = new OpenApiRequestBody()
                {
                    Content = {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema()
                                    {
                                        Reference = GetRootReference(definitionName)
                                    },
                                }
                            },
                    Description = GetDefinitionTitle(definitionName)
                },
                Responses = new OpenApiResponses
                {
                    ["201"] = new OpenApiResponse
                    {
                        Description = "Success",
                        Reference = GetRootReference(definitionName)
                    },
                    ["422"] = new OpenApiResponse
                    {
                        Description = "Client Error",
                        Reference = GetProblemDetailsReference()
                    },
                    ["400"] = new OpenApiResponse
                    {
                        Description = "Bad Request",
                        Reference = GetProblemDetailsReference()
                    }
                }
            });
        }

        private void AddProblemDetailsReference()
        {
            var schema = new OpenApiSchema()
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>()
            };
            schema.Properties.Add("type", GetProperty("type", "string"));
            schema.Properties.Add("title", GetProperty("title", "string"));
            schema.Properties.Add("status", GetProperty("status", "integer"));
            schema.Properties.Add("detail", GetProperty("detail", "string"));
            schema.Properties.Add("instance", GetProperty("instance", "string"));
            _apiDocument.Components.Schemas.Add(PROBLEM_DETAILS_REFERENCE, schema);
        }

        private List<OpenApiParameter> GetPathParameters(string url)
        {
            var parameters = new List<OpenApiParameter>();
            var parts = url.Split('/');
            foreach(var part in parts)
            {
                if(part.Length > 2 && part[0] == '{' && part[part.Length - 1] == '}')
                {
                    parameters.Add(new OpenApiParameter()
                    {
                        Name = part.Substring(1, part.Length - 2),
                        In = ParameterLocation.Path,
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string"
                        }
                    });
                }
            }
            return parameters;
        }

        private string GetRootDefinitionTitle(string definitionName)
        {
            return GetSingle(definitionName).ToSentenceCase() + ROOT_SUFFIX;
        }

        private string GetDefinitionTitle(string definitionName)
        {
            return GetSingle(definitionName).ToSentenceCase();
        }

        private string GetSingle(string plural)
        {
            return _pluralizer.Singularize(plural);
        }

        private OpenApiSchema GetProperty(string propertyName, string dataType)
        {
            switch (dataType)
            {
                case "string":
                    return new OpenApiSchema() { Type = "string" };
                case "datetime":
                    return new OpenApiSchema() { Type = "string", Format = "date-time" };
                case "integer":
                    return new OpenApiSchema() { Type = "integer", Format = "int32" };
                case "array":
                    return new OpenApiSchema() { Type = "array", UniqueItems = false, Items = new OpenApiSchema() { Reference = GetReference(propertyName) } };
                default:
                    throw new NotSupportedException($"{dataType} is not supported");
            }
        }

        private OpenApiReference GetRootReference(string propertyName)
        {
            return new OpenApiReference() { Id = GetRootDefinitionTitle(propertyName), Type = ReferenceType.Schema };
        }

        private OpenApiReference GetProblemDetailsReference()
        {
            return new OpenApiReference() { Id = PROBLEM_DETAILS_REFERENCE, Type = ReferenceType.Schema };
        }

        private OpenApiReference GetReference(string propertyName)
        {
            return new OpenApiReference() { Id = GetDefinitionTitle(propertyName), Type = ReferenceType.Schema };
        }

        private List<OpenApiTag> GetTags(string definitionName)
        {
            return new List<OpenApiTag>()
            {
                new OpenApiTag()
                {
                    Name = definitionName.ToSentenceCase()
                }
            };
        }
    }
}
