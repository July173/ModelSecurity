using Business;
using Entity.DTOs.RegisterySofia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de registros de Sofia en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RegisterySofiaController : ControllerBase
    {
        private readonly RegisterySofiaBusiness _registerySofiaBusiness;
        private readonly ILogger<RegisterySofiaController> _logger;

        /// <summary>
        /// Constructor del controlador de registros de Sofia
        /// </summary>
        public RegisterySofiaController(RegisterySofiaBusiness registerySofiaBusiness, ILogger<RegisterySofiaController> logger)
        {
            _registerySofiaBusiness = registerySofiaBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de Sofia del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RegisterySofiaDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRegisterySofias()
        {
            try
            {
                var registerySofias = await _registerySofiaBusiness.GetAllRegisterySofiasAsync();
                return Ok(registerySofias);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener registros de Sofia");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un registro de Sofia específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RegisterySofiaDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRegisterySofiaById(int id)
        {
            try
            {
                var registerySofia = await _registerySofiaBusiness.GetRegisterySofiaByIdAsync(id);
                return Ok(registerySofia);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el registro de Sofia con ID: {RegisterySofiaId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro de Sofia no encontrado con ID: {RegisterySofiaId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener registro de Sofia con ID: {RegisterySofiaId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo registro de Sofia en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RegisterySofiaDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRegisterySofia([FromBody] RegisterySofiaDto registerySofiaDto)
        {
            try
            {
                var createdRegisterySofia = await _registerySofiaBusiness.CreateRegisterySofiaAsync(registerySofiaDto);
                return CreatedAtAction(nameof(GetRegisterySofiaById), new { id = createdRegisterySofia.Id }, createdRegisterySofia);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear registro de Sofia");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear registro de Sofia");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un registro de Sofia por su ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRegisterySofia(int id)
        {
            try
            {
                var result = await _registerySofiaBusiness.DeleteAsync(id);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "ID inválido al intentar eliminar: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro de Sofia no encontrado al eliminar: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar registro de Sofia");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza completamente un registro de Sofia existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRegisterySofia(int id, [FromBody] RegisterySofiaUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "El id de la ruta no coincide con el del cuerpo" });
            try
            {
                var result = await _registerySofiaBusiness.UpdateAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar registro de Sofia");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro de Sofia no encontrado al actualizar: {Id}", dto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar registro de Sofia");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un registro de Sofia (solo algunos campos).
        /// </summary>
        [HttpPatch]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialRegisterySofia([FromBody] RegisterySofiaUpdateDto dto)
        {
            try
            {
                var result = await _registerySofiaBusiness.UpdateParcialAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación en actualización parcial");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro de Sofia no encontrado en actualización parcial: {Id}", dto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error en actualización parcial de registro de Sofia");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cambia el estado activo/inactivo de un registro de Sofia.
        /// </summary>
        [HttpDelete("active")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetRegisterySofiaActive([FromBody] RegisterySofiaStatusDto dto)
        {
            try
            {
                var result = await _registerySofiaBusiness.SetActiveAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al cambiar estado");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro de Sofia no encontrado al cambiar estado: {Id}", dto.Id);
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
