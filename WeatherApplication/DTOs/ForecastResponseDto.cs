using WeatherForecastApplication.DTOs;

namespace WeatherApplication.DTOs
{
    public class ForecastResponseDto
    {
        public string City { get; set; } = string.Empty;
        public List<DailyForecastDto> DailyForecasts { get; set; } = new();
        public bool RetrievedFromCache { get; set; }
    }
}
