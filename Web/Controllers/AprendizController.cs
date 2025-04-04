using Business;
using Data;
using Entity.DTOautogestion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de aprendices en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AprendizController : ControllerBase
    {
        private readonly AprendizBusiness _AprendizBusiness;
        private readonly ILogger<AprendizController> _logger;

        /// <summary>
        /// Constructor del controlador de aprendices
        /// </summary>
        public AprendizController(AprendizBusiness AprendizBusiness, ILogger<AprendizController> logger)
        {
            _AprendizBusiness = AprendizBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los aprendices del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AprendizDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAprendices()
        {
            try
            {
                var aprendices = await _AprendizBusiness.GetAllAprendizAsync();
                return Ok(aprendices);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener aprendices");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un aprendiz específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AprendizDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAprendizById(int id)
        {
            try
            {
                var aprendiz = await _AprendizBusiness.GetAprendizByIdAsync(id);
                return Ok(aprendiz);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el aprendiz con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "aprenidz no encontrado con ID: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener aprendiz con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo aprendiz en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(AprendizDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAprendiz([FromBody] AprendizDto AprendizDto)
        {
            try
            {
                var createdAprendiz = await _AprendizBusiness.CreateAprendizAsync(AprendizDto);
                return CreatedAtAction(nameof(GetAprendizById), new { id = createdAprendiz.Id }, createdAprendiz);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear aprendiz");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear aprendiz");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
