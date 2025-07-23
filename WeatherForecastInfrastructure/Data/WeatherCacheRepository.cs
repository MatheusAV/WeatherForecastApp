using Microsoft.EntityFrameworkCore;
using WeatherForecastDomain.Entities;
using WeatherForecastDomain.Interfaces;

namespace WeatherForecastInfrastructure.Data
{
    public class WeatherCacheRepository : IWeatherCacheRepository
    {
        private readonly WeatherDbContext _context;

        public WeatherCacheRepository(WeatherDbContext context)
        {
            _context = context;
        }

        public async Task<Weather> GetCachedWeatherAsync(string city)
        {
            var entry = await _context.Weathers
                .Where(w => w.City.ToLower() == city.ToLower() && w.Expiration > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            return entry;
        }

        public async Task CacheWeatherAsync(Weather weather)
        {
            weather.Expiration = DateTime.UtcNow.AddMinutes(30); // Cache por 30 min
            _context.Weathers.Add(weather);
            await _context.SaveChangesAsync();
        }

        public async Task<Forecast> GetCachedForecastAsync(string city)
        {
            return await _context.Forecasts
                .Include(f => f.DailyForecasts)
                .Where(f => f.City.ToLower() == city.ToLower() && f.Expiration > DateTime.UtcNow)
                .FirstOrDefaultAsync();
        }

        public async Task CacheForecastAsync(Forecast forecast)
        {
            forecast.Expiration = DateTime.UtcNow.AddHours(1); // Cache por 1h
            _context.Forecasts.Add(forecast);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SearchHistory>> GetSearchHistoryAsync(int limit)
        {
            return await _context.SearchHistories
                .OrderByDescending(h => h.LastSearchedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task AddSearchHistoryAsync(string city)
        {
            var history = new SearchHistory
            {
                City = city,
                LastSearchedAt = DateTime.UtcNow
            };

            _context.SearchHistories.Add(history);
            await _context.SaveChangesAsync();
        }

    }
}
