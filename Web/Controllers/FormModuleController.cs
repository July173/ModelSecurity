using Business;
using Entity.DTOs.FormModule;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de módulos de formulario.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FormModuleController : ControllerBase
    {
        private readonly FormModuleBusiness _formModuleBusiness;
        private readonly ILogger<FormModuleController> _logger;

        /// <summary>
        /// Constructor del controlador de módulos de formulario.
        /// </summary>
        /// <param name="formModuleBusiness">Capa de negocio de módulos de formulario.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        public FormModuleController(FormModuleBusiness formModuleBusiness, ILogger<FormModuleController> logger)
        {
            _formModuleBusiness = formModuleBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los módulos de formulario.
        /// </summary>
        /// <returns>Lista de módulos de formulario.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormModuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllFormModules()
        {
            try
            {
                var formModules = await _formModuleBusiness.GetAllFormModulesAsync();
                return Ok(formModules);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener módulos de formulario.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un módulo de formulario por su ID.
        /// </summary>
        /// <param name="id">ID del módulo de formulario.</param>
        /// <returns>Módulo de formulario solicitado.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFormModuleById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del módulo de formulario debe ser mayor que cero." });
            }

            try
            {
                var formModule = await _formModuleBusiness.GetFormModuleByIdAsync(id);
                return Ok(formModule);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo de formulario no encontrado con ID: {FormModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo de formulario con ID: {FormModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo módulo de formulario.
        /// </summary>
        /// <param name="formModuleDto">Datos del módulo de formulario a crear.</param>
        /// <returns>Módulo de formulario creado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(FormModuleDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateFormModule([FromBody] FormModuleDto formModuleDto)
        {
            if (formModuleDto == null)
            {
                return BadRequest(new { message = "Los datos del módulo de formulario no pueden ser nulos." });
            }

            try
            {
                var createdFormModule = await _formModuleBusiness.CreateFormModuleAsync(formModuleDto);
                return CreatedAtAction(nameof(GetFormModuleById), new { id = createdFormModule.Id }, createdFormModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear el módulo de formulario.");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el módulo de formulario.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un módulo de formulario existente.
        /// </summary>
        /// <param name="id">ID del módulo de formulario a actualizar.</param>
        /// <param name="formModuleDto">Datos actualizados del módulo de formulario.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateFormModule(int id, [FromBody] FormModuleDto formModuleDto)
        {
            if (id != formModuleDto.Id)
            {
                return BadRequest(new { message = "El ID del módulo de formulario no coincide con el ID proporcionado en el cuerpo de la solicitud." });
            }

            try
            {
                var result = await _formModuleBusiness.UpdateFormModuleAsync(formModuleDto);
                return Ok(new { message = "Módulo de formulario actualizado correctamente.", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el módulo de formulario con ID: {FormModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo de formulario no encontrado con ID: {FormModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el módulo de formulario con ID: {FormModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un módulo de formulario.
        /// </summary>
        /// <param name="id">ID del módulo de formulario a actualizar.</param>
        /// <param name="updatedFields">Campos a actualizar.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialFormModule(int id, [FromBody] Dictionary<string, object> updatedFields)
        {
            if (updatedFields == null || updatedFields.Count == 0)
            {
                return BadRequest(new { message = "Debe proporcionar al menos un campo para actualizar." });
            }

            try
            {
                var result = await _formModuleBusiness.UpdatePartialFormModuleAsync(id, updatedFields);
                if (!result)
                {
                    return NotFound(new { message = "Módulo de formulario no encontrado o no se pudo actualizar." });
                }

                return Ok(new { message = "Módulo de formulario actualizado parcialmente correctamente.", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente el módulo de formulario con ID: {FormModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo de formulario no encontrado con ID: {FormModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el módulo de formulario con ID: {FormModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza un borrado lógico de un módulo de formulario por su ID.
        /// </summary>
        /// <param name="id">ID del módulo de formulario a eliminar lógicamente.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpDelete("soft-delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteFormModule(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del módulo de formulario debe ser mayor que cero." });
            }

            try
            {
                var result = await _formModuleBusiness.SoftDeleteFormModuleAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Módulo de formulario no encontrado." });
                }

                return Ok(new { message = "Módulo de formulario eliminado lógicamente correctamente.", success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar el borrado lógico del módulo de formulario con ID: {FormModuleId}", id);
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }

        /// <summary>
        /// Elimina un módulo de formulario por su ID.
        /// </summary>
        /// <param name="id">ID del módulo de formulario a eliminar.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteFormModule(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del módulo de formulario debe ser mayor que cero." });
            }

            try
            {
                var result = await _formModuleBusiness.DeleteFormModuleAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Módulo de formulario no encontrado." });
                }

                return Ok(new { message = "Módulo de formulario eliminado correctamente.", success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el módulo de formulario con ID: {FormModuleId}", id);
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }
    }
}
