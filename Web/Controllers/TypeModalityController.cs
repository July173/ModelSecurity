using Business;
using Entity.DTOs.TypeModality;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de modalidades en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TypeModalityController : ControllerBase
    {
        private readonly TypeModalityBusiness _typeModalityBusiness;
        private readonly ILogger<TypeModalityController> _logger;

        /// <summary>
        /// Constructor del controlador de modalidades
        /// </summary>
        public TypeModalityController(TypeModalityBusiness typeModalityBusiness, ILogger<TypeModalityController> logger)
        {
            _typeModalityBusiness = typeModalityBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las modalidades del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TypeModalityDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllTypeModalities()
        {
            try
            {
                var typeModalities = await _typeModalityBusiness.GetAllTypeModalitiesAsync();
                return Ok(typeModalities);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener modalidades");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una modalidad específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TypeModalityDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTypeModalityById(int id)
        {
            try
            {
                var typeModality = await _typeModalityBusiness.GetTypeModalityByIdAsync(id);
                return Ok(typeModality);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la modalidad con ID: {TypeModalityId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Modalidad no encontrada con ID: {TypeModalityId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener modalidad con ID: {TypeModalityId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva modalidad en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TypeModalityDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateTypeModality([FromBody] TypeModalityDto typeModalityDto)
        {
            try
            {
                var createdTypeModality = await _typeModalityBusiness.CreateTypeModalityAsync(typeModalityDto);
                return CreatedAtAction(nameof(GetTypeModalityById), new { id = createdTypeModality.Id }, createdTypeModality);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear modalidad");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear modalidad");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un tipo de modalidad por su ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteTypeModality(int id)
        {
            try
            {
                var result = await _typeModalityBusiness.DeleteTypeModalityAsync(id);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "ID inválido al intentar eliminar: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de modalidad no encontrado al eliminar: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar tipo de modalidad");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        /// <summary>
        /// Actualiza completamente un tipo de modalidad existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateTypeModality(int id, [FromBody] TypeModalityUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "El id de la ruta no coincide con el del cuerpo" });
            try
            {
                var result = await _typeModalityBusiness.UpdateCompletoTypeModalityAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar tipo de modalidad");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de modalidad no encontrado al actualizar: {Id}", dto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar tipo de modalidad");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        /// <summary>
        /// Actualiza parcialmente un tipo de modalidad (solo algunos campos).
        /// </summary>
        [HttpPatch]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialTypeModality([FromBody] TypeModalityUpdateDto dto)
        {
            try
            {
                var result = await _typeModalityBusiness.UpdateParcialTypeModalityAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación en actualización parcial");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de modalidad no encontrado en actualización parcial: {Id}", dto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error en actualización parcial de tipo de modalidad");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cambia el estado activo/inactivo de un tipo de modalidad.
        /// </summary>
        [HttpDelete("active")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetTypeModalityActive([FromBody] TypeModalityStatusDto dto)
        {
            try
            {
                var result = await _typeModalityBusiness.SetTypeModalityActiveAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al cambiar estado");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de modalidad no encontrado al cambiar estado: {Id}", dto.Id);
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
