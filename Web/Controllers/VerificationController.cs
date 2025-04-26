using Business;
using Entity.DTOs.Verification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de verificaciones en el sistema.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class VerificationController : ControllerBase
    {
        private readonly VerificationBusiness _verificationBusiness;
        private readonly ILogger<VerificationController> _logger;

        /// <summary>
        /// Constructor del controlador de verificaciones.
        /// </summary>
        public VerificationController(VerificationBusiness verificationBusiness, ILogger<VerificationController> logger)
        {
            _verificationBusiness = verificationBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las verificaciones registradas.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VerificationDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllVerifications()
        {
            try
            {
                var verifications = await _verificationBusiness.GetAllVerificationsAsync();
                return Ok(verifications);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener verificaciones");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una verificación por su ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VerificationDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetVerificationById(int id)
        {
            try
            {
                var verification = await _verificationBusiness.GetVerificationByIdAsync(id);
                return Ok(verification);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "ID inválido: {VerificationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Verificación no encontrada: {VerificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener verificación: {VerificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva verificación.(post)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(VerificationDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateVerification([FromBody] VerificationDto dto)
        {
            try
            {
                var created = await _verificationBusiness.CreateVerificationAsync(dto);
                return CreatedAtAction(nameof(GetVerificationById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al crear verificación");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear verificación");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza completamente una verificación existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateVerification( int id, [FromBody] VerificationUpadateDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "El ide la ruta no coincide con el del cuerpo" });
            try
            {
                var result = await _verificationBusiness.UpdateVerificationAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar verificación");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Verificación no encontrada al actualizar: {Id}", dto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar verificación");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente una verificación (solo nombre y observación).
        /// </summary>
        [HttpPatch]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialVerification([FromBody] VerificationUpadateDto dto)
        {
            try
            {
                var result = await _verificationBusiness.UpdateParcialVerificationAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación en actualización parcial");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Verificación no encontrada en actualización parcial: {Id}", dto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error en actualización parcial de verificación");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cambia el estado activo/inactivo de una verificación.
        /// </summary>
        [HttpDelete("active")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetVerificationActive([FromBody] VerificationStatusDto dto)
        {
            try
            {
                var result = await _verificationBusiness.SetVerificationActiveAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al cambiar estado");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Verificación no encontrada al cambiar estado: {Id}", dto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una verificación por su ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteVerification(int id)
        {
            try
            {
                var result = await _verificationBusiness.DeleteVerificationAsync(id);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "ID inválido al intentar eliminar: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Verificación no encontrada al eliminar: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar verificación");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
