using WeatherForecastDomain.Entities;

namespace WeatherForecastDomain.Interfaces
{
    public interface IWeatherCacheRepository
    {
        // Weather
        Task<Weather> GetCachedWeatherAsync(string city);
        Task CacheWeatherAsync(Weather weather);

        // Forecast
        Task<Forecast> GetCachedForecastAsync(string city);
        Task CacheForecastAsync(Forecast forecast);

        // Search History
        Task<List<SearchHistory>> GetSearchHistoryAsync(int limit);
        Task AddSearchHistoryAsync(string city);
    }
}
