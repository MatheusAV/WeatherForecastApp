using WeatherForecastApplication.DTOs;

namespace WeatherForecastApplication.Interfaces
{
    public interface IHistoryService
    {
        Task<List<HistoryResponseDto>> GetSearchHistoryAsync(int limit);
    }
}
