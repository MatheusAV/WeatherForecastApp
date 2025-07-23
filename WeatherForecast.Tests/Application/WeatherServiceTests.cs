using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using WeatherForecastApplication.Services;
using WeatherForecastDomain.Entities;
using WeatherForecastDomain.Interfaces;
using Xunit;

namespace WeatherForecast.Tests.Application
{
    public class WeatherServiceTests
    {
        private readonly Mock<IWeatherApiClient> _apiClientMock;
        private readonly Mock<IWeatherCacheRepository> _cacheRepoMock;
        private readonly WeatherService _weatherService;

        public WeatherServiceTests()
        {
            _apiClientMock = new Mock<IWeatherApiClient>();
            _cacheRepoMock = new Mock<IWeatherCacheRepository>();
            _weatherService = new WeatherService(_apiClientMock.Object, _cacheRepoMock.Object);
        }

        [Fact]
        public async Task GetCurrentWeatherAsync_ShouldReturnFromCache_WhenCacheHit()
        {
            // Arrange
            var city = "São Paulo";
            var cachedWeather = new Weather
            {
                City = city,
                Temperature = 25,
                Humidity = 80,
                Description = "Ensolarado",
                WindSpeed = 5.5
            };

            _cacheRepoMock.Setup(repo => repo.GetCachedWeatherAsync(city))
                          .ReturnsAsync(cachedWeather);

            // Act
            var result = await _weatherService.GetCurrentWeatherAsync(city);

            // Assert
            result.Should().NotBeNull();
            result.City.Should().Be(city);
            result.RetrievedFromCache.Should().BeTrue();

            _apiClientMock.Verify(api => api.GetCurrentWeatherAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetCurrentWeatherAsync_ShouldReturnFromApi_WhenCacheMiss()
        {
            // Arrange
            var city = "Rio de Janeiro";
            _cacheRepoMock.Setup(repo => repo.GetCachedWeatherAsync(city))
                          .ReturnsAsync((Weather?)null);

            var apiWeather = new Weather
            {
                City = city,
                Temperature = 28,
                Humidity = 70,
                Description = "Parcialmente Nublado",
                WindSpeed = 4.0
            };

            _apiClientMock.Setup(api => api.GetCurrentWeatherAsync(city))
                          .ReturnsAsync(apiWeather);

            // Act
            var result = await _weatherService.GetCurrentWeatherAsync(city);

            // Assert
            result.Should().NotBeNull();
            result.City.Should().Be(city);
            result.RetrievedFromCache.Should().BeFalse();

            _cacheRepoMock.Verify(repo => repo.CacheWeatherAsync(apiWeather), Times.Once);
        }

        [Fact]
        public async Task GetCurrentWeatherAsync_ShouldReturnNull_WhenApiReturnsNull()
        {
            // Arrange
            var city = "CidadeInexistente";
            _cacheRepoMock.Setup(repo => repo.GetCachedWeatherAsync(city))
                          .ReturnsAsync((Weather?)null);

            _apiClientMock.Setup(api => api.GetCurrentWeatherAsync(city))
                          .ReturnsAsync((Weather?)null);

            // Act
            var result = await _weatherService.GetCurrentWeatherAsync(city);

            // Assert
            result.Should().BeNull();
        }
    }
}
