namespace Cms.Shared.Observability;

public static class SerilogExtensions
{
    public static void UseSerilogFromConfig(this IHostBuilder hostBuilder)
        => hostBuilder.UseSerilog((ctx, cfg) =>
        {
            cfg.ReadFrom.Configuration(ctx.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console();
        });
}