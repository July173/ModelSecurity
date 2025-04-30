using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    /// <summary>
    /// Controlador para gestionar los permisos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionBusiness _permissionBusiness;
        private readonly ILogger<PermissionController> _logger;

        /// <summary>
        /// Constructor que recibe las dependencias necesarias.
        /// </summary>
        /// <param name="permissionBusiness">Instancia de <see cref="PermissionBusiness"/> para la lógica de negocio.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{PermissionController}"/> para el registro de logs.</param>
        public PermissionController(PermissionBusiness permissionBusiness, ILogger<PermissionController> logger)
        {
            _permissionBusiness = permissionBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos.
        /// </summary>
        /// <returns>Lista de permisos.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAll()
        {
            try
            {
                var permissions = await _permissionBusiness.GetAllAsync();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener todos los permisos: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Obtiene un permiso por su ID.
        /// </summary>
        /// <param name="id">Identificador único del permiso.</param>
        /// <returns>El permiso si existe, o un código 404 si no se encuentra.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionDto>> GetById(int id)
        {
            try
            {
                var permission = await _permissionBusiness.GetByIdAsync(id);
                if (permission == null)
                    return NotFound($"No se encontró el permiso con ID {id}.");

                return Ok(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el permiso con ID {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Crea un nuevo permiso.
        /// </summary>
        /// <param name="permissionDto">Datos del permiso a crear.</param>
        /// <returns>El permiso creado.</returns>
        [HttpPost]
        public async Task<ActionResult<PermissionDto>> Create([FromBody] PermissionDto permissionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdPermission = await _permissionBusiness.CreateAsync(permissionDto);
                return CreatedAtAction(nameof(GetById), new { id = createdPermission.Id }, createdPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el permiso: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Actualiza un permiso existente.
        /// </summary>
        /// <param name="id">Identificador único del permiso.</param>
        /// <param name="permissionDto">Datos actualizados del permiso.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PermissionDto permissionDto)
        {
            try
            {
                if (id != permissionDto.Id)
                    return BadRequest("El ID del permiso no coincide.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _permissionBusiness.UpdateAsync(permissionDto);
                if (!result)
                    return NotFound($"No se encontró el permiso con ID {id}.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el permiso con ID {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Actualiza parcialmente un permiso.
        /// </summary>
        /// <param name="id">Identificador único del permiso.</param>
        /// <param name="updatedFields">Campos a actualizar.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] Dictionary<string, object> updatedFields)
        {
            try
            {
                var result = await _permissionBusiness.UpdatePartialAsync(id, updatedFields);
                if (!result)
                    return NotFound($"No se encontró el permiso con ID {id}.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar parcialmente el permiso con ID {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Elimina un permiso de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del permiso.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _permissionBusiness.DeleteAsync(id);
                if (!result)
                    return NotFound($"No se encontró el permiso con ID {id}.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el permiso con ID {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
