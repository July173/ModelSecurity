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
    /// Controlador para la gestión de estados en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class StateController : ControllerBase
    {
        private readonly StateBusiness _stateBusiness;
        private readonly ILogger<StateController> _logger;

        /// <summary>
        /// Constructor del controlador de estados
        /// </summary>
        public StateController(StateBusiness stateBusiness, ILogger<StateController> logger)
        {
            _stateBusiness = stateBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los estados del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StateDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllStates()
        {
            try
            {
                var states = await _stateBusiness.GetAllStatesAsync();
                return Ok(states);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener estados");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un estado específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StateDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetStateById(int id)
        {
            try
            {
                var state = await _stateBusiness.GetStateByIdAsync(id);
                return Ok(state);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el estado con ID: {StateId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Estado no encontrado con ID: {StateId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener estado con ID: {StateId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo estado en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(StateDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateState([FromBody] StateDto stateDto)
        {
            try
            {
                var createdState = await _stateBusiness.CreateStateAsync(stateDto);
                return CreatedAtAction(nameof(GetStateById), new { id = createdState.Id }, createdState);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear estado");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear estado");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
