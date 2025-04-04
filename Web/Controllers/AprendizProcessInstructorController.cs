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
    /// Controlador para la gestión de AprendizProcessInstructor en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AprendizProcessInstructorController : ControllerBase
    {
        private readonly AprendizProcessInstructorBusiness _aprendizProcessInstructorBusiness;
        private readonly ILogger<AprendizProcessInstructorController> _logger;

        /// <summary>
        /// Constructor del controlador de AprendizProcessInstructor
        /// </summary>
        public AprendizProcessInstructorController(AprendizProcessInstructorBusiness aprendizProcessInstructorBusiness, ILogger<AprendizProcessInstructorController> logger)
        {
            _aprendizProcessInstructorBusiness = aprendizProcessInstructorBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los procesos de aprendiz con instructor en el sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AprendizProcessInstructorDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAprendizProcessInstructors()
        {
            try
            {
                var aprendizProcessInstructors = await _aprendizProcessInstructorBusiness.GetAllAprendizProcessInstructorsAsync();
                return Ok(aprendizProcessInstructors);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener procesos de aprendiz con instructor");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un proceso de aprendiz con instructor específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AprendizProcessInstructorDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAprendizProcessInstructorById(int id)
        {
            try
            {
                var aprendizProcessInstructor = await _aprendizProcessInstructorBusiness.GetAprendizProcessInstructorByIdAsync(id);
                return Ok(aprendizProcessInstructor);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el proceso de aprendiz con instructor con ID: {AprendizProcessInstructorId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Proceso de aprendiz con instructor no encontrado con ID: {AprendizProcessInstructorId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener proceso de aprendiz con instructor con ID: {AprendizProcessInstructorId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo proceso de aprendiz con instructor en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(AprendizProcessInstructorDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAprendizProcessInstructor([FromBody] AprendizProcessInstructorDto aprendizProcessInstructorDto)
        {
            try
            {
                var createdAprendizProcessInstructor = await _aprendizProcessInstructorBusiness.CreateAprendizProcessInstructorAsync(aprendizProcessInstructorDto);
                return CreatedAtAction(nameof(GetAprendizProcessInstructorById), new { id = createdAprendizProcessInstructor.Id }, createdAprendizProcessInstructor);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear proceso de aprendiz con instructor");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear proceso de aprendiz con instructor");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
