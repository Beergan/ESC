using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

namespace ESC.CONCOST.Base;

public static class JsonQuerySwaggerGenExtensions
{
    public static SwaggerGenOptions AddJsonQuerySupport(this SwaggerGenOptions options)
    {
        options.OperationFilter<JsonQueryOperationFilter>();
        return options;
    }

    private class JsonQueryOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var jsonQueryParams = context.ApiDescription.ActionDescriptor.Parameters
                .Where(ad => ad.BindingInfo.BinderType == typeof(JsonQueryBinder))
                .Select(ad => ad.Name)
                .ToList();

            if (!jsonQueryParams.Any())
            {
                return;
            }

            foreach (var p in operation.Parameters.Where(p => jsonQueryParams.Contains(p.Name)))
            {
                // move the schema under application/json content type
                p.Content = new Dictionary<string, OpenApiMediaType>()
                {
                    [MediaTypeNames.Application.Json] = new OpenApiMediaType()
                    {
                        Schema = p.Schema,
                        Example = new Microsoft.OpenApi.Any.OpenApiString("{\"skip\": 0, \"take\": 10}")
                    }
                };
                // then clear it
                p.Schema = null;
            }
        }
    }
}