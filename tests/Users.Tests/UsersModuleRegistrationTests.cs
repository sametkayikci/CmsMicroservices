namespace Users.Tests;

public class UsersModuleRegistrationTests
{
    [Fact]
    public void Given_configuration_When_add_users_module_Then_required_services_are_resolvable()
    {
        var services = new ServiceCollection();
        var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Postgres"] = "Host=localhost;Database=d;Username=u;Password=p",
            ["Redis:Connection"] = "localhost:6379",
            ["Services:ContentsBaseUrl"] = "http://localhost:1234",
            ["Internal:Key"] = "k"
        }).Build();

        services.AddUsersModule(cfg);

        var sp = services.BuildServiceProvider(validateScopes: true);

        using var scope = sp.CreateScope();
        var p = scope.ServiceProvider;

        p.GetRequiredService<AppDbContext>().Should().NotBeNull();
        p.GetRequiredService<IUserRepository>().Should().NotBeNull();
        p.GetRequiredService<IUserService>().Should().NotBeNull();

        p.GetRequiredService<IDateTime>().Should().NotBeNull();
        p.GetRequiredService<ICacheService>().Should().NotBeNull();
        p.GetRequiredService<IContentsClient>().Should().NotBeNull();
        p.GetRequiredService<HealthCheckService>().Should().NotBeNull();
    }
}