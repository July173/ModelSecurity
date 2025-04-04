using Business;
using Entity.DTOautogestion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de regiones en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RegionalController : ControllerBase
    {
        private readonly RegionalBusiness _regionalBusiness;
        private readonly ILogger<RegionalController> _logger;

        /// <summary>
        /// Constructor del controlador de regiones
        /// </summary>
        public RegionalController(RegionalBusiness regionalBusiness, ILogger<RegionalController> logger)
        {
            _regionalBusiness = regionalBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las regiones del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RegionalDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRegionals()
        {
            try
            {
                var regionals = await _regionalBusiness.GetAllRegionalsAsync();
                return Ok(regionals);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener regiones");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una región específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RegionalDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRegionalById(int id)
        {
            try
            {
                var regional = await _regionalBusiness.GetRegionalByIdAsync(id);
                return Ok(regional);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la región con ID: {RegionalId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Región no encontrada con ID: {RegionalId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener región con ID: {RegionalId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva región en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RegionalDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRegional([FromBody] RegionalDto regionalDto)
        {
            try
            {
                var createdRegional = await _regionalBusiness.CreateRegionalAsync(regionalDto);
                return CreatedAtAction(nameof(GetRegionalById), new { id = createdRegional.Id }, createdRegional);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear región");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear región");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
