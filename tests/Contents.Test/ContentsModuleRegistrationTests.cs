
namespace Contents.Test;

public class ContentsModuleRegistrationTests
{
    [Fact]
    public void Given_configuration_When_add_contents_module_Then_required_services_are_resolvable()
    {
        var services = new ServiceCollection();
        var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Postgres"] = "Host=localhost;Database=d;Username=u;Password=p",
            ["Redis:Connection"] = "localhost:6379",
            ["Services:UsersBaseUrl"] = "http://localhost:5678",
            ["Internal:Key"] = "k"
        }).Build();
      
        services.AddContentsModule(cfg);
        
        var root = services.BuildServiceProvider(validateScopes: true);
        
        using var scope = root.CreateScope();
        var sp = scope.ServiceProvider;

        sp.GetRequiredService<AppDbContext>().Should().NotBeNull();                               
        sp.GetRequiredService<IContentRepository>().Should().NotBeNull();                         
        sp.GetRequiredService<IContentService>().Should().NotBeNull();                           
        sp.GetRequiredService<IDateTime>().Should().NotBeNull();                                  
        sp.GetRequiredService<Cms.Shared.Abstractions.ICacheService>().Should().NotBeNull();     
        sp.GetRequiredService<IUsersClient>().Should().NotBeNull();                               
        sp.GetRequiredService<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService>()
          .Should().NotBeNull();                                                                  
    }
}
