using Microsoft.EntityFrameworkCore;
using NotificationService.AsyncDataServices;
using NotificationService.Data;
using NotificationService.EmailService;
using NotificationService.SyncDataServices;
using Shared.JwtConfiguration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<INotificationRepo, NotificationRepo>();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("NotificationConnectionString")));
builder.Services.AddJwtAuthentication();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHostedService<MessageBusSubscriber>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

builder.Services.AddScoped<IApplicationUserDataClient, ApplicationUserDataClient>();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

PrepDatabase.DoMigrations(app, app.Environment.IsProduction());
PrepDatabase.SeedUsers(app);

app.MapHealthChecks("/health");
app.Run();