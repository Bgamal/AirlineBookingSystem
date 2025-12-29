using AirlineBookingSystem.Payments.Application.Handlers;
using AirlineBookingSystem.Payments.Core.Repositories;
using AirlineBookingSystem.Payments.Infrastructure.Repositories;
using MassTransit;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using AitlineBookingSystem.BuildingBlocks.Common;
using AirlineBookingSystem.Payments.Application.Consumers;
using AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages;

var builder = WebApplication.CreateBuilder(args);

// Configure IDbConnection using connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));
RegisterApplicationSrvices(builder);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use built-in OpenAPI support
builder.Services.AddOpenApi();

//Register MediatR Services
var assemblies = new Assembly[]
{
    Assembly.GetExecutingAssembly(),
    typeof(ProcessPaymentHandler).Assembly,
    typeof(RefundPaymentHandler).Assembly
};
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

builder.Services.AddMassTransit(cfg =>
{
    //consumer registration
    cfg.AddConsumer<FlightBookedConsumer>();


    cfg.UsingRabbitMq((context, rabbitCfg) =>
    {
        rabbitCfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        rabbitCfg.ReceiveEndpoint(EventBusConstant.FlightBookedQueue, e =>
        {
            e.ConfigureConsumer<FlightBookedConsumer>(context);
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

app.Run();
void RegisterApplicationSrvices(WebApplicationBuilder builder)
{
    // Register application services here
    // services.AddScoped<IYourService, YourServiceImplementation>();
    builder.Services.AddScoped<IPaymentRepositry, PaymentRepositry>();
}