using Business;
using Entity.DTOs.Sede;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de sedes en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SedeController : ControllerBase
    {
        private readonly SedeBusiness _sedeBusiness;
        private readonly ILogger<SedeController> _logger;

        /// <summary>
        /// Constructor del controlador de sedes
        /// </summary>
        public SedeController(SedeBusiness sedeBusiness, ILogger<SedeController> logger)
        {
            _sedeBusiness = sedeBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las sedes del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SedeDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllSedes()
        {
            try
            {
                var sedes = await _sedeBusiness.GetAllSedesAsync();
                return Ok(sedes);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener sedes");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una sede específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SedeDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSedeById(int id)
        {
            try
            {
                var sede = await _sedeBusiness.GetSedeByIdAsync(id);
                return Ok(sede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la sede con ID: {SedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada con ID: {SedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener sede con ID: {SedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva sede en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(SedeDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateSede([FromBody] SedeDto sedeDto)
        {
            try
            {
                var createdSede = await _sedeBusiness.CreateSedeAsync(sedeDto);
                return CreatedAtAction(nameof(GetSedeById), new { id = createdSede.Id }, createdSede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear sede");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear sede");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una sede por su ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSede(int id)
        {
            try
            {
                await _sedeBusiness.DeleteSedeAsync(id);
                return Ok();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "ID inválido al intentar eliminar: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada al eliminar: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar sede");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        /// <summary>
        /// Actualiza completamente una sede existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateSede(int id, [FromBody] SedeUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "El id de la ruta no coincide con el del cuerpo" });
            try
            {
                var result = await _sedeBusiness.UpdateSedeAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar sede");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada al actualizar: {Id}", dto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar sede");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        /// <summary>
        /// Actualiza parcialmente una sede (solo algunos campos).
        /// </summary>
        [HttpPatch]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialSede([FromBody] SedeUpdateDto dto)
        {
            try
            {
                var result = await _sedeBusiness.UpdateParcialSedeAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación en actualización parcial");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada en actualización parcial: {Id}", dto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error en actualización parcial de sede");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        /// <summary>
        /// Cambia el estado activo/inactivo de una sede.
        /// </summary>
        [HttpDelete("active")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetSedeActive([FromBody] SedeStatusDto dto)
        {
            try
            {
                var result = await _sedeBusiness.SetSedeActiveAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al cambiar estado");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada al cambiar estado: {Id}", dto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo");
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
