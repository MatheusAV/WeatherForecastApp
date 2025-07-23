using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WeatherForecast.Api.Extensions;
using WeatherForecastDomain.Interfaces;
using WeatherForecastInfrastructure.BackgroundServices;
using WeatherForecastInfrastructure.Data;
using WeatherForecastInfrastructure.ExternalServices;

var builder = WebApplication.CreateBuilder(args);

// =======================
// 🔥 CONFIGURAÇÕES GERAIS
// =======================

// Carrega configuração do appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

DependencyInjection.Register(builder.Services);

// Configura o Serilog para logging estruturado
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();


builder.Host.UseSerilog();

// =======================
// 🧱 DEPENDENCY INJECTION
// =======================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// DbContext (Cache em SQL)
builder.Services.AddDbContext<WeatherDbContext>();

// Background Services
builder.Services.AddHostedService<CacheCleanerService>();

// HttpClient para API externa
builder.Services.AddHttpClient<IWeatherApiClient, OpenWeatherMapClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["WeatherApi:BaseUrl"]);
});


// =======================
// 🌐 BUILD DO APP
// =======================
var app = builder.Build();

// =======================
// 📦 MIDDLEWARES
// =======================

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware para tratamento global de erros
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        Log.Error(error, "Erro não tratado capturado pelo middleware");

        var problemDetails = new ProblemDetails
        {
            Status = 500,
            Title = "Erro interno no servidor",
            Detail = error?.Message,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

app.UseSerilogRequestLogging();
app.ConfigureExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
