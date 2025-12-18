
using AirlineBookingSystem.Fights.Application.Handlers;
using AirlineBookingSystem.Fights.Core.Repositories;
using AirlineBookingSystem.Fights.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

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
var assemblies = new Assembly[]
    {
        Assembly.GetExecutingAssembly(),
        typeof(CreateFlightHandler).Assembly,
        typeof(DeleteFlightHandler).Assembly,
         typeof(GetAllFlightsHandler).Assembly
    };
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

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
    builder.Services.AddScoped<IFlightRepository, FlightRepository>();
}