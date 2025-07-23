using Microsoft.EntityFrameworkCore;
using WeatherForecastDomain.Entities;

namespace WeatherForecastInfrastructure.Data
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options)
            : base(options)
        {
        }

        public DbSet<Weather> Weathers { get; set; }
        public DbSet<Forecast> Forecasts { get; set; }
        public DbSet<DailyForecast> DailyForecasts { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura relacionamento 1:N entre Forecast e DailyForecast
            modelBuilder.Entity<Forecast>()
                .HasMany(f => f.DailyForecasts)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // Index para otimizar buscas por cidade
            modelBuilder.Entity<Weather>()
                .HasIndex(w => w.City);

            modelBuilder.Entity<Forecast>()
                .HasIndex(f => f.City);

            modelBuilder.Entity<SearchHistory>()
                .HasIndex(s => s.City);
        }
    }
}
