namespace WeatherForecastApplication.DTOs
{
    public class WeatherResponseDto
    {
        public string City { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public string Description { get; set; } = string.Empty;
        public double WindSpeed { get; set; }
        public bool RetrievedFromCache { get; set; }
    }
}
