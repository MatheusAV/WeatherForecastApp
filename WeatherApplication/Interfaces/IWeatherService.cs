using WeatherApplication.DTOs;
using WeatherForecastApplication.DTOs;

namespace WeatherForecastApplication.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherResponseDto> GetCurrentWeatherAsync(string city);
        Task<ForecastResponseDto> Get5DayForecastAsync(string city);
    }
}
