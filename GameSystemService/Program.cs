using GameSystemService.AsyncDataServices;
using GameSystemService.Data;
using GameSystemService.SyncDataServices;
using Microsoft.EntityFrameworkCore;
using Shared.JwtConfiguration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddJwtAuthentication();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IGameSystemRepo, GameSystemRepo>();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("GameSystemConnectionString")));
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

//gRPC
app.MapGrpcService<GrpcGameSystemService>();
app.MapGet("SyncDataServices/gamesystems.proto", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("SyncDataServices/gamesystems.proto"));
});

app.Run();