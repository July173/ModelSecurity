using Business;
using Entity.DTOautogestion;
using Entity.DTOautogestion.pivote;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de AprendizProgram en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AprendizProgramController : ControllerBase
    {
        private readonly AprendizProgramBusiness _aprendizProgramBusiness;
        private readonly ILogger<AprendizProgramController> _logger;

        /// <summary>
        /// Constructor del controlador de AprendizProgram
        /// </summary>
        public AprendizProgramController(AprendizProgramBusiness aprendizProgramBusiness, ILogger<AprendizProgramController> logger)
        {
            _aprendizProgramBusiness = aprendizProgramBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los programas de aprendiz en el sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AprendizProgramDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAprendizPrograms()
        {
            try
            {
                var aprendizPrograms = await _aprendizProgramBusiness.GetAllAprendizProgramsAsync();
                return Ok(aprendizPrograms);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener programas de aprendiz");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un programa de aprendiz específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AprendizProgramDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAprendizProgramById(int id)
        {
            try
            {
                var aprendizProgram = await _aprendizProgramBusiness.GetAprendizProgramByIdAsync(id);
                return Ok(aprendizProgram);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el programa de aprendiz con ID: {AprendizProgramId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Programa de aprendiz no encontrado con ID: {AprendizProgramId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener programa de aprendiz con ID: {AprendizProgramId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo programa de aprendiz en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(AprendizProgramDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAprendizProgram([FromBody] AprendizProgramDto aprendizProgramDto)
        {
            try
            {
                var createdAprendizProgram = await _aprendizProgramBusiness.CreateAprendizProgramAsync(aprendizProgramDto);
                return CreatedAtAction(nameof(GetAprendizProgramById), new { id = createdAprendizProgram.Id }, createdAprendizProgram);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear programa de aprendiz");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear programa de aprendiz");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
