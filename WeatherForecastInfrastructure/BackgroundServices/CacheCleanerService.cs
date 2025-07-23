using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using WeatherForecastInfrastructure.Data;

namespace WeatherForecastInfrastructure.BackgroundServices
{
    public class CacheCleanerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CacheCleanerService> _logger;

        public CacheCleanerService(IServiceProvider serviceProvider, ILogger<CacheCleanerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🧹 CacheCleanerService iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanExpiredCacheAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao limpar cache expirado.");
                }

                // Aguarda 1 hora antes de limpar novamente
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }

            _logger.LogInformation("🧹 CacheCleanerService encerrado.");
        }

        private async Task CleanExpiredCacheAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();

            var now = DateTime.UtcNow;

            var expiredWeathers = await dbContext.Weathers
                .Where(w => w.Expiration <= now)
                .ToListAsync(cancellationToken);

            var expiredForecasts = await dbContext.Forecasts
                .Where(f => f.Expiration <= now)
                .ToListAsync(cancellationToken);

            if (expiredWeathers.Any() || expiredForecasts.Any())
            {
                dbContext.Weathers.RemoveRange(expiredWeathers);
                dbContext.Forecasts.RemoveRange(expiredForecasts);

                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"🗑️ {expiredWeathers.Count} registros Weather e {expiredForecasts.Count} Forecast removidos do cache.");
            }
            else
            {
                _logger.LogInformation("✅ Nenhum cache expirado encontrado.");
            }
        }
    }
}
