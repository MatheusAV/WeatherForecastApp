namespace WeatherForecastApplication.DTOs
{
    public class ForecastResponseDto
    {
        public string City { get; set; } = string.Empty;
        public List<DailyForecastDto> DailyForecasts { get; set; } = new();
        public bool RetrievedFromCache { get; set; }
    }

    public class DailyForecastDto
    {
        public DateTime Date { get; set; }
        public double TemperatureMin { get; set; }
        public double TemperatureMax { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class HistoryResponseDto
    {
        public string City { get; set; } = string.Empty;
        public DateTime LastSearchedAt { get; set; }
    }
}
