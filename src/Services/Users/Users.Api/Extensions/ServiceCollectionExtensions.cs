namespace Users.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Users modülüne ait tüm bağımlılıkları (DbContext, Redis cache, repo, servis, Refit client,
    /// validation, healthchecks, rate limiting, swagger, controllers) kaydeder.
    /// </summary>
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddSingleton(cfg);
        if (cfg is IConfigurationRoot root)
            services.AddSingleton(root);
        
        services.AddDbContext<AppDbContext>(o =>
            o.UseNpgsql(cfg.GetConnectionString("Postgres")));
      
        services.AddStackExchangeRedisCache(o => o.Configuration = cfg["Redis:Connection"]);
     
        services.AddSingleton<IDateTime, SystemDateTime>();
        services.AddSingleton<ICacheService, RedisCacheService>();
     
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
      
        services.AddRefitClient<IContentsClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(cfg["Services:ContentsBaseUrl"]!));

     
        //services.AddFluentValidatorsFrom(typeof(Program));
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddApiLivenessChecks(runningInsideDocker: false); 
        services.AddFixedWindowRateLimiting(permitLimit: 100, windowSeconds: 60);
        services.AddCustomSwagger("Users API");
        
        services.AddControllers();

        return services;
    }
}
