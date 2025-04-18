using AuthService.AsyncDataServices;
using AuthService.Data;
using AuthService.Models;
using AuthService.Services;
using AuthService.SyncDataServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.JwtConfiguration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddJwtAuthentication();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//Swagger
builder.Services.AddSwaggerGen( o =>
    {
        o.SwaggerDoc("v1", new() 
        {
            Title = "AuthService",
            Version = "v1",
            Description = "Authentication Service API for the GamingTableHelper application"
        });
    }
);
//Database
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnectionString")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

//Message Bus
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
//gRPC
builder.Services.AddGrpc();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService v1"));

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

//gRPC
app.MapGrpcService<GrpcApplicationUserService>();
app.MapGet("SyncDataServices/applicationusers.proto", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("SyncDataServices/applicationusers.proto"));
});

PrepDatabase.DoMigrations(app, app.Environment.IsProduction());

app.MapHealthChecks("/health");
app.Run();