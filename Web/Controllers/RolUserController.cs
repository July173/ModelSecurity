using Business;
using Data;
using Entity.DTOs.UserRol;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de relaciones entre roles y usuarios en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolUserController : ControllerBase
    {
        private readonly UserRolBusiness _RolUserBusiness;
        private readonly ILogger<RolUserController> _logger;

        /// <summary>
        /// Constructor del controlador de relaciones entre roles y usuarios
        /// </summary>
        /// <param name="RolUserBusiness">Capa de negocio de relaciones entre roles y usuarios</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public RolUserController(UserRolBusiness RolUserBusiness, ILogger<RolUserController> logger)
        {
            _RolUserBusiness = RolUserBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las relaciones de roles y usuarios
        /// </summary>
        /// <returns>Lista de relaciones</returns>
        /// <response code="200">Retorna la lista de relaciones</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserRolDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRolUsers()
        {
            try
            {
                var rolUsers = await _RolUserBusiness.GetAllRolUsersAsync();
                return Ok(rolUsers);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener relaciones de roles y usuarios");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una relación específica por su ID
        /// </summary>
        /// <param name="id">ID de la relación</param>
        /// <returns>Relación solicitada</returns>
        /// <response code="200">Retorna la relación solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Relación no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserRolDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolUserById(int id)
        {
            try
            {
                var rolUser = await _RolUserBusiness.GetRolUserByIdAsync(id);
                return Ok(rolUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la relación con ID: {RolUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación no encontrada con ID: {RolUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener la relación con ID: {RolUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva relación entre rol y usuario
        /// </summary>
        /// <param name="RolUserDto">Datos de la relación a crear</param>
        /// <returns>Relación creada</returns>
        /// <response code="201">Retorna la relación creada</response>
        /// <response code="400">Datos de la relación no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserRolDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRolUser([FromBody] UserRolDto RolUserDto)
        {
            try
            {
                var createdRolUser = await _RolUserBusiness.CreateRolUserAsync(RolUserDto);
                return CreatedAtAction(nameof(GetRolUserById), new { id = createdRolUser.Id }, createdRolUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear la relación rol-usuario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear la relación rol-usuario");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        /// <summary>
        /// Actualiza una relación entre rol y usuario existente en el sistema.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRolUser(int id, [FromBody] UserRolDto rolUserDto)
        {
            if (id != rolUserDto.Id)
            {
                return BadRequest(new { message = "El ID de la relación no coincide con el ID proporcionado en el cuerpo de la solicitud." });
            }

            try
            {
                var result = await _RolUserBusiness.UpdateRolUserAsync(rolUserDto);
                return Ok(new { message = "Relación rol-usuario actualizada correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar la relación rol-usuario con ID: {RolUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación rol-usuario no encontrada con ID: {RolUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación rol-usuario con ID: {RolUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza campos específicos de una relación entre rol y usuario.
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialRolUser(int id, [FromBody] Dictionary<string, object> updatedFields)
        {
            if (updatedFields == null || updatedFields.Count == 0)
            {
                return BadRequest(new { message = "Debe proporcionar al menos un campo para actualizar." });
            }

            try
            {
                var result = await _RolUserBusiness.UpdatePartialRolUserAsync(id, updatedFields);

                if (!result)
                {
                    return NotFound(new { message = "No se encontró la relación rol-usuario o no se pudo actualizar." });
                }

                return Ok(new { message = "Relación rol-usuario actualizada parcialmente correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente la relación rol-usuario con ID: {RolUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación rol-usuario no encontrada con ID: {RolUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la relación rol-usuario con ID: {RolUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una relación entre rol y usuario del sistema.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRolUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la relación debe ser mayor a 0." });
            }

            try
            {
                var result = await _RolUserBusiness.DeleteRolUserAsync(id);
                return Ok(new { message = "Relación rol-usuario eliminada correctamente", success = result });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación rol-usuario no encontrada con ID: {RolUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar la relación rol-usuario con ID: {RolUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
