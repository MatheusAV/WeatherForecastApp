using Microsoft.AspNetCore.Mvc;
using WeatherForecastApplication.DTOs;
using WeatherForecastApplication.Interfaces;

namespace WeatherForecast.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        /// <summary>
        /// Retorna o hist�rico das �ltimas cidades pesquisadas.
        /// </summary>
        /// <param name="limit">N�mero m�ximo de registros (padr�o: 10)</param>
        /// <returns>Lista de buscas recentes</returns>
        [HttpGet]
        public async Task<ActionResult<List<HistoryResponseDto>>> GetHistory([FromQuery] int limit = 10)
        {
            if (limit <= 0 || limit > 100)
                return BadRequest("O par�metro 'limit' deve ser entre 1 e 100.");

            var history = await _historyService.GetSearchHistoryAsync(limit);
            return Ok(history);
        }
    }
}
