using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System.Net.Http.Json;
using WeatherForecastDomain.Entities;
using WeatherForecastDomain.Interfaces;

namespace WeatherForecastInfrastructure.ExternalServices
{
    public class OpenWeatherMapClient : IWeatherApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<OpenWeatherMapClient> _logger;
        private readonly IConfiguration _configuration;

        public OpenWeatherMapClient(HttpClient httpClient, IConfiguration configuration, ILogger<OpenWeatherMapClient> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _apiKey = configuration["WeatherApi:ApiKey"];
        }

        public async Task<Weather> GetCurrentWeatherAsync(string city)
        {
            var response = await _httpClient.GetAsync($"weather?q={city}&appid={_apiKey}&units=metric&lang=pt_br");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao consultar OpenWeatherMap: {StatusCode} {ReasonPhrase}",
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            var data = await response.Content.ReadFromJsonAsync<dynamic>();

            return new Weather
            {
                City = city,
                Temperature = (double)data.main.temp,
                Humidity = (int)data.main.humidity,
                Description = (string)data.weather[0].description,
                WindSpeed = (double)data.wind.speed
            };
        }


        public async Task<Forecast> Get5DayForecastAsync(string city)
        {
            var relativeUrl = $"forecast?q={city}&appid={_apiKey}&units=metric&lang=pt_br";

            string requestUrl;
            if (_httpClient.BaseAddress != null)
            {
                // Usa o BaseAddress configurado no Program.cs
                requestUrl = relativeUrl;
            }
            else
            {
                // Fallback para URL absoluta
                var baseUrl = _configuration["WeatherApi:BaseUrl"];
                requestUrl = $"{baseUrl}{relativeUrl}";
            }

            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao consultar OpenWeatherMap: {StatusCode} {ReasonPhrase}",
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            var data = await response.Content.ReadFromJsonAsync<dynamic>();

            var forecast = new Forecast
            {
                City = city,
                DailyForecasts = new List<DailyForecast>()
            };

            var grouped = ((IEnumerable<dynamic>)data.list)
                .GroupBy(item => DateTime.Parse((string)item.dt_txt).Date)
                .Take(5);

            foreach (var dayGroup in grouped)
            {
                var dayForecast = new DailyForecast
                {
                    Date = dayGroup.Key,
                    TemperatureMin = dayGroup.Min(d => (double)d.main.temp_min),
                    TemperatureMax = dayGroup.Max(d => (double)d.main.temp_max),
                    Description = (string)dayGroup.First().weather[0].description
                };

                forecast.DailyForecasts.Add(dayForecast);
            }

            return forecast;
        }


    }
}
