using FluentAssertions;
using Moq;
using WeatherForecastApplication.Services;
using WeatherForecastDomain.Entities;
using WeatherForecastDomain.Interfaces;

namespace WeatherForecast.Tests.Application
{
    public class HistoryServiceTests
    {
        private readonly Mock<IWeatherCacheRepository> _cacheRepoMock;
        private readonly HistoryService _historyService;

        public HistoryServiceTests()
        {
            _cacheRepoMock = new Mock<IWeatherCacheRepository>();
            _historyService = new HistoryService(_cacheRepoMock.Object);
        }

        [Fact]
        public async Task GetSearchHistoryAsync_ShouldReturnList_WhenDataExists()
        {
            // Arrange
            var history = new List<SearchHistory>
            {
                new SearchHistory { City = "São Paulo", LastSearchedAt = DateTime.UtcNow },
                new SearchHistory { City = "Rio de Janeiro", LastSearchedAt = DateTime.UtcNow.AddMinutes(-10) }
            };

            _cacheRepoMock.Setup(repo => repo.GetSearchHistoryAsync(2))
                          .ReturnsAsync(history);

            // Act
            var result = await _historyService.GetSearchHistoryAsync(2);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].City.Should().Be("São Paulo");
        }

        [Fact]
        public async Task GetSearchHistoryAsync_ShouldReturnEmptyList_WhenNoData()
        {
            // Arrange
            _cacheRepoMock.Setup(repo => repo.GetSearchHistoryAsync(5))
                          .ReturnsAsync(new List<SearchHistory>());

            // Act
            var result = await _historyService.GetSearchHistoryAsync(5);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
