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
    /// Controlador para la gestión de programas en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProgramController : ControllerBase
    {
        private readonly ProgramBusiness _programBusiness;
        private readonly ILogger<ProgramController> _logger;

        /// <summary>
        /// Constructor del controlador de programas
        /// </summary>
        public ProgramController(ProgramBusiness programBusiness, ILogger<ProgramController> logger)
        {
            _programBusiness = programBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los programas del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProgramDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPrograms()
        {
            try
            {
                var programs = await _programBusiness.GetAllProgramsAsync();
                return Ok(programs);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener programas");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un programa específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProgramDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProgramById(int id)
        {
            try
            {
                var program = await _programBusiness.GetProgramByIdAsync(id);
                return Ok(program);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el programa con ID: {ProgramId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Programa no encontrado con ID: {ProgramId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener programa con ID: {ProgramId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo programa en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProgramDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateProgram([FromBody] ProgramDto programDto)
        {
            try
            {
                var createdProgram = await _programBusiness.CreateProgramAsync(programDto);
                return CreatedAtAction(nameof(GetProgramById), new { id = createdProgram.Id }, createdProgram);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear programa");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear programa");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
