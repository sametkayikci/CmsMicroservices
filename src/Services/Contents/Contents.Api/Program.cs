var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilogFromConfig();

builder.Services.AddContentsModule(builder.Configuration);

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); app.UseSwaggerUI();
}

app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

await app.RunAsync();
public partial class Program { }