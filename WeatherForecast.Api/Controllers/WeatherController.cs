using Microsoft.AspNetCore.Mvc;
using WeatherApplication.DTOs;
using WeatherForecastApplication.DTOs;
using WeatherForecastApplication.Interfaces;


namespace WeatherForecast.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        /// <summary>
        /// Retorna a previs�o do tempo atual para uma cidade.
        /// </summary>
        /// <param name="city">Nome da cidade</param>
        /// <returns>Dados do clima</returns>
        [HttpGet("current")]
        public async Task<ActionResult<WeatherResponseDto>> GetCurrentWeather([FromQuery] string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("O par�metro 'city' � obrigat�rio.");

            var result = await _weatherService.GetCurrentWeatherAsync(city);

            if (result == null)
                return NotFound($"Nenhuma previs�o encontrada para '{city}'.");

            return Ok(result);
        }

        /// <summary>
        /// Retorna a previs�o do tempo para os pr�ximos 5 dias.
        /// </summary>
        /// <param name="city">Nome da cidade</param>
        /// <returns>Dados da previs�o estendida</returns>
        [HttpGet("forecast")]
        public async Task<ActionResult<ForecastResponseDto>> Get5DayForecast([FromQuery] string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("O par�metro 'city' � obrigat�rio.");

            var result = await _weatherService.Get5DayForecastAsync(city);

            if (result == null)
                return NotFound($"Nenhuma previs�o encontrada para '{city}'.");

            return Ok(result);
        }
    }
}
