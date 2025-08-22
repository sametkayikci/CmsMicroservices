using Microsoft.OpenApi.Models;

namespace Cms.Shared.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, string title)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = "v1" });
            var xmls = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
            foreach (var xml in xmls) c.IncludeXmlComments(xml, true);
        });
        return services;
    }
}