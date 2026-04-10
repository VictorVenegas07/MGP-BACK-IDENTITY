using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Identity.Infrastructure.Extensions.Swagger;

public class IgnoreSwaggerPropertiesSchemaFilter : ISchemaFilter
{
    private static readonly string[] IgnoredProps = new[]
    {
        "Filter",
        "OrderBy"
    };

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        foreach (var prop in IgnoredProps)
        {
            if (schema.Properties.ContainsKey(prop))
            {
                schema.Properties.Remove(prop);
            }
        }
    }
}
