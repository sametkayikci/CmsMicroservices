namespace Contents.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Contents modülüne ait tüm bağımlılıkları (DbContext, Redis, repo, servis, Refit client,
    /// validation, healthchecks, rate limiting, swagger, controllers) kaydeder.
    /// </summary>
    public static IServiceCollection AddContentsModule(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddSingleton(cfg);
        if (cfg is IConfigurationRoot root)
            services.AddSingleton(root);
        services.AddDbContext<AppDbContext>(o =>
            o.UseNpgsql(cfg.GetConnectionString("Postgres")));
      
        services.AddStackExchangeRedisCache(o => o.Configuration = cfg["Redis:Connection"]);
     
        services.AddSingleton<IDateTime, SystemDateTime>();
        services.AddSingleton<ICacheService, RedisCacheService>();
       
        services.AddScoped<IContentRepository, ContentRepository>();
        services.AddScoped<IContentService, ContentService>();

        // Refit client -> Users API (content oluştururken kullanıcı doğrulama, kullanıcı güncelleme)
        services.AddRefitClient<IUsersClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(cfg["Services:UsersBaseUrl"]!));
       
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddApiLivenessChecks(runningInsideDocker: false); 

        services.AddFixedWindowRateLimiting(permitLimit: 100, windowSeconds: 60);
        services.AddCustomSwagger("Contents API");
     
        services.AddControllers();

        return services;
    }
    
}
