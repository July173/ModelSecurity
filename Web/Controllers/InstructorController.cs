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
    /// Controlador para la gestión de Instructor en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InstructorController : ControllerBase
    {
        private readonly InstructorBusiness _instructorBusiness;
        private readonly ILogger<InstructorController> _logger;

        /// <summary>
        /// Constructor del controlador de Instructor
        /// </summary>
        public InstructorController(InstructorBusiness instructorBusiness, ILogger<InstructorController> logger)
        {
            _instructorBusiness = instructorBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los instructores del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InstructorDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllInstructors()
        {
            try
            {
                var instructors = await _instructorBusiness.GetAllInstructorsAsync();
                return Ok(instructors);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener instructores");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un instructor específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InstructorDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetInstructorById(int id)
        {
            try
            {
                var instructor = await _instructorBusiness.GetInstructorByIdAsync(id);
                return Ok(instructor);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el instructor con ID: {InstructorId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Instructor no encontrado con ID: {InstructorId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener instructor con ID: {InstructorId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo instructor en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(InstructorDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateInstructor([FromBody] InstructorDto instructorDto)
        {
            try
            {
                var createdInstructor = await _instructorBusiness.CreateInstructorAsync(instructorDto);
                return CreatedAtAction(nameof(GetInstructorById), new { id = createdInstructor.Id }, createdInstructor);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear instructor");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear instructor");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
