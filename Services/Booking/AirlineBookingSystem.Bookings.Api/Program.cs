using System.Data;
using System.Reflection;
using AirlineBookingSystem.Bookings.Application.Handlers;
using AirlineBookingSystem.Bookings.Core.Repositories;
using AirlineBookingSystem.Bookings.Infrastructure.Repositories;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using AitlineBookingSystem.BuildingBlocks.Common;
using AirlineBookingSystem.Bookings.Application.Consumers;

var builder = WebApplication.CreateBuilder(args);

#region sql server connection string
// Configure IDbConnection using connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));
#endregion


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure Swagger (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

RegisterApplicationSrvices(builder);

//Register MediatR Services
var assemblies =new Assembly[]
    {
        Assembly.GetExecutingAssembly(),
        typeof(CreateBookingHandler).Assembly,
        typeof(GetBookingHandler).Assembly
    };
builder.Services.AddMediatR(cfg =>cfg.RegisterServicesFromAssemblies(assemblies));

builder.Services.AddMassTransit(cfg =>
{
    //consumer registration
    cfg.AddConsumer<NotificationEventConsumer>();


    cfg.UsingRabbitMq((context, rabbitCfg) =>
    {
        rabbitCfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        rabbitCfg.ReceiveEndpoint(EventBusConstant.NotificationSentQueue, e =>
        {
            e.ConfigureConsumer<NotificationEventConsumer>(context);
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
    builder.Services.AddScoped<IBookingRepository, BookingRepository>();
}