using Business;
using Entity.DTOautogestion;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de instructorProgram en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InstructorProgramController : ControllerBase
    {
        private readonly InstructorProgramBusiness _instructorProgramBusiness;
        private readonly ILogger<InstructorProgramController> _logger;

        /// <summary>
        /// Constructor del controlador de instructorProgram
        /// </summary>
        public InstructorProgramController(InstructorProgramBusiness instructorProgramBusiness, ILogger<InstructorProgramController> logger)
        {
            _instructorProgramBusiness = instructorProgramBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los instructorPrograms del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InstructorProgramDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllInstructorPrograms()
        {
            try
            {
                var instructorPrograms = await _instructorProgramBusiness.GetAllInstructorProgramsAsync();
                return Ok(instructorPrograms);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener instructorPrograms");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un instructorProgram específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InstructorProgramDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetInstructorProgramById(int id)
        {
            try
            {
                var instructorProgram = await _instructorProgramBusiness.GetInstructorProgramByIdAsync(id);
                return Ok(instructorProgram);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el instructorProgram con ID: {InstructorProgramId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "InstructorProgram no encontrado con ID: {InstructorProgramId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener instructorProgram con ID: {InstructorProgramId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo instructorProgram en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(InstructorProgramDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateInstructorProgram([FromBody] InstructorProgramDto instructorProgramDto)
        {
            try
            {
                var createdInstructorProgram = await _instructorProgramBusiness.CreateInstructorProgramAsync(instructorProgramDto);
                return CreatedAtAction(nameof(GetInstructorProgramById), new { id = createdInstructorProgram.Id }, createdInstructorProgram);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear instructorProgram");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear instructorProgram");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
