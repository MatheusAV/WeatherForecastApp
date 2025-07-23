using WeatherForecastApplication.DTOs;
using WeatherForecastApplication.Interfaces;
using WeatherForecastDomain.Interfaces;

namespace WeatherForecastApplication.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly IWeatherCacheRepository _cacheRepository;

        public HistoryService(IWeatherCacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }

        public async Task<List<HistoryResponseDto>> GetSearchHistoryAsync(int limit)
        {
            var history = await _cacheRepository.GetSearchHistoryAsync(limit);

            return history.Select(h => new HistoryResponseDto
            {
                City = h.City,
                LastSearchedAt = h.LastSearchedAt
            }).ToList();
        }
    }
}
