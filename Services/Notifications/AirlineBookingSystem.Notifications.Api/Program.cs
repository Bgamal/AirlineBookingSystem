using AirlineBookingSystem.Notifications.Application.Consumers;
using AirlineBookingSystem.Notifications.Application.Handlers;
using AirlineBookingSystem.Notifications.Application.Interfaces;
using AirlineBookingSystem.Notifications.Application.Services;
using AirlineBookingSystem.Notifications.Core.Repositories;
using AirlineBookingSystem.Notifications.Infrastructure.Repositories;
using AitlineBookingSystem.BuildingBlocks.Common;
using MassTransit;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure IDbConnection using connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

// Add services to the container.
builder.Services.AddControllers();
// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Use built-in OpenAPI support
builder.Services.AddOpenApi();

// Add authorization services (required when using app.UseAuthorization())
builder.Services.AddAuthorization();

// Application Services
builder.Services.AddScoped<INotificationService, NotificationService>();
RegisterApplicationSrvices(builder);

// Register MediatR Services
var assemblies = new Assembly[]
{
    Assembly.GetExecutingAssembly(),
    typeof(SendNotificationHandler).Assembly
};
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));


builder.Services.AddMassTransit(cfg =>
{
    //consumer registration
    cfg.AddConsumer<PaymentProcessedConsumer>();

    cfg.UsingRabbitMq((context, rabbitCfg) =>
    {
        rabbitCfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        rabbitCfg.ReceiveEndpoint(EventBusConstant.PaymentProcessedQueue, e =>
        {
            e.ConfigureConsumer<PaymentProcessedConsumer>(context);
        });
        rabbitCfg.ConfigureEndpoints(context);
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Application startup error: {ex.Message}");
    throw;
}

void RegisterApplicationSrvices(WebApplicationBuilder builder)
{
    // Register application services here
    builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
}