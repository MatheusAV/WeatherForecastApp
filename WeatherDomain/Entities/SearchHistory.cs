namespace WeatherForecastDomain.Entities
{
    public class SearchHistory
    {
        public int Id { get; set; }
        public string City { get; set; } = string.Empty;
        public DateTime LastSearchedAt { get; set; }
    }
}
