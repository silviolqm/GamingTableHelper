using GameTableService.AsyncDataServices;
using GameTableService.Data;
using GameTableService.SyncDataServices;
using Microsoft.EntityFrameworkCore;
using Shared.JwtConfiguration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IGameTableRepo, GameTableRepo>();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("GameTableConnectionString")));
builder.Services.AddJwtAuthentication();
//Swagger
builder.Services.AddSwaggerGenWithJWT(
    serviceName: "GameTableService",
    description: "Game Table Service API for the GamingTableHelper application"
);

builder.Services.AddHostedService<MessageBusSubscriber>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddSingleton<IMessageBusPublisher, MessageBusPublisher>();

builder.Services.AddScoped<IGameSystemDataClient, GameSystemDataClient>();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GameTableService v1"));

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

PrepDatabase.DoMigrations(app, app.Environment.IsProduction());
PrepDatabase.SeedGameSystems(app);

app.MapHealthChecks("/health");
app.Run();