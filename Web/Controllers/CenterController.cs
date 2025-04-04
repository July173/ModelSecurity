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
    /// Controlador para la gestión de Center en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CenterController : ControllerBase
    {
        private readonly CenterBusiness _centerBusiness;
        private readonly ILogger<CenterController> _logger;

        /// <summary>
        /// Constructor del controlador de Center
        /// </summary>
        public CenterController(CenterBusiness centerBusiness, ILogger<CenterController> logger)
        {
            _centerBusiness = centerBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los centros del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CenterDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllCenters()
        {
            try
            {
                var centers = await _centerBusiness.GetAllCentersAsync();
                return Ok(centers);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener centros");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un centro específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CenterDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCenterById(int id)
        {
            try
            {
                var center = await _centerBusiness.GetCenterByIdAsync(id);
                return Ok(center);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el centro con ID: {CenterId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Centro no encontrado con ID: {CenterId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener centro con ID: {CenterId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo centro en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CenterDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateCenter([FromBody] CenterDto centerDto)
        {
            try
            {
                var createdCenter = await _centerBusiness.CreateCenterAsync(centerDto);
                return CreatedAtAction(nameof(GetCenterById), new { id = createdCenter.Id }, createdCenter);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear centro");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear centro");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
