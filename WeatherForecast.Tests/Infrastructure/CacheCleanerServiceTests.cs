using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherForecastDomain.Entities;
using WeatherForecastInfrastructure.BackgroundServices;
using WeatherForecastInfrastructure.Data;

namespace WeatherForecast.Tests.Infrastructure
{
    public class CacheCleanerServiceTests
    {
        private WeatherDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<WeatherDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new WeatherDbContext(options);
        }

        [Fact]
        public async Task CleanExpiredCacheAsync_ShouldRemoveExpiredEntries()
        {
            // Arrange
            using var dbContext = CreateInMemoryDbContext();
            dbContext.Weathers.AddRange(new List<Weather>
            {
                new Weather { City = "São Paulo", Expiration = DateTime.UtcNow.AddMinutes(-5) }, // Expirado
                new Weather { City = "Rio de Janeiro", Expiration = DateTime.UtcNow.AddMinutes(10) } // Válido
            });
            dbContext.Forecasts.AddRange(new List<Forecast>
            {
                new Forecast { City = "Curitiba", Expiration = DateTime.UtcNow.AddMinutes(-10), DailyForecasts = new List<DailyForecast>() },
                new Forecast { City = "Porto Alegre", Expiration = DateTime.UtcNow.AddMinutes(15), DailyForecasts = new List<DailyForecast>() }
            });
            await dbContext.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<CacheCleanerService>>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(WeatherDbContext))).Returns(dbContext);

            var cacheCleanerService = new CacheCleanerService(serviceProviderMock.Object, loggerMock.Object);

            // Act
            var cleanMethod = typeof(CacheCleanerService).GetMethod("CleanExpiredCacheAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (cleanMethod == null)
            {
                throw new InvalidOperationException("Método CleanExpiredCacheAsync não encontrado");
            }

            await (Task)cleanMethod.Invoke(cacheCleanerService, new object[] { CancellationToken.None })!;

            // Assert
            dbContext.Weathers.Count().Should().Be(1);
            dbContext.Weathers.Any(w => w.City == "Rio de Janeiro").Should().BeTrue();

            dbContext.Forecasts.Count().Should().Be(1);
            dbContext.Forecasts.Any(f => f.City == "Porto Alegre").Should().BeTrue();
        }

        [Fact]
        public async Task CleanExpiredCacheAsync_ShouldNotThrow_WhenNoExpiredEntries()
        {
            // Arrange
            using var dbContext = CreateInMemoryDbContext();
            dbContext.Weathers.Add(new Weather { City = "Recife", Expiration = DateTime.UtcNow.AddMinutes(30) });
            await dbContext.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<CacheCleanerService>>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(WeatherDbContext))).Returns(dbContext);

            var cacheCleanerService = new CacheCleanerService(serviceProviderMock.Object, loggerMock.Object);

            // Act
            var cleanMethod = typeof(CacheCleanerService).GetMethod("CleanExpiredCacheAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (cleanMethod == null)
            {
                throw new InvalidOperationException("Método CleanExpiredCacheAsync não encontrado");
            }

            Func<Task> act = async () => await (Task)cleanMethod.Invoke(cacheCleanerService, new object[] { CancellationToken.None })!;

            // Assert
            await act.Should().NotThrowAsync();
            dbContext.Weathers.Count().Should().Be(1); // Nenhum removido
        }

        [Fact]
        public async Task CleanExpiredCacheAsync_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<CacheCleanerService>>();
            var serviceProviderMock = new Mock<IServiceProvider>();

            // Simula null no DbContext para forçar exceção
            serviceProviderMock.Setup(sp => sp.GetService(typeof(WeatherDbContext)))
                             .Throws(new Exception("Simulated failure"));

            var cacheCleanerService = new CacheCleanerService(serviceProviderMock.Object, loggerMock.Object);

            // Act
            var cleanMethod = typeof(CacheCleanerService).GetMethod("CleanExpiredCacheAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (cleanMethod == null)
            {
                throw new InvalidOperationException("Método CleanExpiredCacheAsync não encontrado");
            }

            Func<Task> act = async () => await (Task)cleanMethod.Invoke(cacheCleanerService, new object[] { CancellationToken.None })!;

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Simulated failure");
            loggerMock.Verify(
     x => x.Log(
         LogLevel.Error,
         It.IsAny<EventId>(),
         It.Is<It.IsAnyType>((v, t) => true),
         It.IsAny<Exception>(),
         It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
     Times.AtLeastOnce);
        }
    }
}