namespace WeatherForecastDomain.Entities
{
    public class Forecast
    {
        public int Id { get; set; }
        public string City { get; set; } = string.Empty;
        public List<DailyForecast> DailyForecasts { get; set; } = new();
        public DateTime Expiration { get; set; }
    }

    public class DailyForecast
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double TemperatureMin { get; set; }
        public double TemperatureMax { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
