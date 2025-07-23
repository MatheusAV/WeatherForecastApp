using WeatherForecastApplication.Interfaces;
using WeatherForecastApplication.Services;
using WeatherForecastDomain.Interfaces;
using WeatherForecastInfrastructure.Data;
using WeatherForecastInfrastructure.ExternalServices;

public class DependencyInjection
{
    public static void Register(IServiceCollection serviceProvider)
    {
        RepositoryDependence(serviceProvider);
    }

    private static void RepositoryDependence(IServiceCollection serviceProvider)
    {

        // Application Services
        serviceProvider.AddScoped<IWeatherService, WeatherService>();
        serviceProvider.AddScoped<IHistoryService, HistoryService>();

        // Domain Interfaces → Infrastructure Implementations
        serviceProvider.AddScoped<IWeatherApiClient, OpenWeatherMapClient>();
        serviceProvider.AddScoped<IWeatherCacheRepository, WeatherCacheRepository>();

    }
}