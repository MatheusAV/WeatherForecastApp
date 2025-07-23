using WeatherForecastApplication.DTOs;
using WeatherForecastApplication.Interfaces;
using WeatherForecastDomain.Interfaces;
using DailyForecastDto = WeatherForecastApplication.DTOs.DailyForecastDto;
using ForecastResponseDto = WeatherForecastApplication.DTOs.ForecastResponseDto;


namespace WeatherForecastApplication.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherApiClient _weatherApiClient;
        private readonly IWeatherCacheRepository _cacheRepository;

        public WeatherService(IWeatherApiClient weatherApiClient, IWeatherCacheRepository cacheRepository)
        {
            _weatherApiClient = weatherApiClient;
            _cacheRepository = cacheRepository;
        }

        public async Task<WeatherResponseDto> GetCurrentWeatherAsync(string city)
        {
            // 1️⃣ Tenta buscar no cache
            var cachedWeather = await _cacheRepository.GetCachedWeatherAsync(city);
            if (cachedWeather != null)
            {
                return new WeatherResponseDto
                {
                    City = cachedWeather.City,
                    Temperature = cachedWeather.Temperature,
                    Humidity = cachedWeather.Humidity,
                    Description = cachedWeather.Description,
                    WindSpeed = cachedWeather.WindSpeed,
                    RetrievedFromCache = true
                };
            }

            // 2️⃣ Caso não encontre no cache, consulta a API externa
            var apiWeather = await _weatherApiClient.GetCurrentWeatherAsync(city);
            if (apiWeather == null)
                return null;

            // 3️⃣ Salva no cache para futuras requisições
            await _cacheRepository.CacheWeatherAsync(apiWeather);

            // 4️⃣ Retorna DTO
            return new WeatherResponseDto
            {
                City = apiWeather.City,
                Temperature = apiWeather.Temperature,
                Humidity = apiWeather.Humidity,
                Description = apiWeather.Description,
                WindSpeed = apiWeather.WindSpeed,
                RetrievedFromCache = false
            };
        }


        public async Task<WeatherForecastApplication.DTOs.ForecastResponseDto> Get5DayForecastAsync(string city)
        {
            // Adiciona busca no histórico (não retorna nada)
            await _cacheRepository.AddSearchHistoryAsync(city);

            // 1️⃣ Tenta buscar no cache
            var cachedForecast = await _cacheRepository.GetCachedForecastAsync(city);
            if (cachedForecast != null)
            {
                return new ForecastResponseDto
                {
                    City = cachedForecast.City,
                    DailyForecasts = cachedForecast.DailyForecasts.Select(f => new WeatherForecastApplication.DTOs.DailyForecastDto
                    {
                        Date = f.Date,
                        TemperatureMin = f.TemperatureMin,
                        TemperatureMax = f.TemperatureMax,
                        Description = f.Description
                    }).ToList(),
                    RetrievedFromCache = true
                };
            }

            // 2️⃣ Caso não encontre no cache, consulta a API externa
            var apiForecast = await _weatherApiClient.Get5DayForecastAsync(city);
            if (apiForecast == null)
                return null;

            // 3️⃣ Salva no cache para futuras requisições
            await _cacheRepository.CacheForecastAsync(apiForecast);

            // 4️⃣ Retorna DTO
            return new ForecastResponseDto
            {
                City = apiForecast.City,
                DailyForecasts = apiForecast.DailyForecasts.Select(f => new DailyForecastDto
                {
                    Date = f.Date,
                    TemperatureMin = f.TemperatureMin,
                    TemperatureMax = f.TemperatureMax,
                    Description = f.Description
                }).ToList(),
                RetrievedFromCache = false
            };
        }

    }
}