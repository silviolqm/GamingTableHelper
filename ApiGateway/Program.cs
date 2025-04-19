using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Shared.JwtConfiguration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi(); // For OpenAPI
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Services.AddJwtAuthentication();

var app = builder.Build();

app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

app.UseAuthentication();
app.UseAuthorization();

app.UseOcelot().Wait();

app.Run();