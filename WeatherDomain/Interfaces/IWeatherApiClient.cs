using WeatherForecastDomain.Entities;

namespace WeatherForecastDomain.Interfaces
{
    public interface IWeatherApiClient
    {
        Task<Weather> GetCurrentWeatherAsync(string city);
        Task<Forecast> Get5DayForecastAsync(string city);
    }
}
