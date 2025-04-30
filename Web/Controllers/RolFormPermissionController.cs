using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Business;
using Entity.DTOs;
using Entity.DTOs.RolFormPermission;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para gestionar los permisos de formularios y roles.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RolFormPermissionController : ControllerBase
    {
        private readonly RolFormPermissionBusiness _rolFormPermissionBusiness;
        private readonly ILogger<RolFormPermissionController> _logger;

        /// <summary>
        /// Constructor que recibe las dependencias necesarias.
        /// </summary>
        /// <param name="rolFormPermissionBusiness">Instancia de <see cref="RolFormPermissionBusiness"/> para la lógica de negocio.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{RolFormPermissionController}"/> para el registro de logs.</param>
        public RolFormPermissionController(RolFormPermissionBusiness rolFormPermissionBusiness, ILogger<RolFormPermissionController> logger)
        {
            _rolFormPermissionBusiness = rolFormPermissionBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de RolFormPermission.
        /// </summary>
        /// <returns>Lista de registros.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolFormPermissionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<RolFormPermissionDto>>> GetAll()
        {
            try
            {
                var rolFormPermissions = await _rolFormPermissionBusiness.GetAllAsync();
                return Ok(rolFormPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener todos los registros de RolFormPermission: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Obtiene un registro de RolFormPermission por su ID.
        /// </summary>
        /// <param name="id">Identificador único del registro.</param>
        /// <returns>El registro si existe, o un código 404 si no se encuentra.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolFormPermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RolFormPermissionDto>> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID debe ser mayor a 0." });
            }

            try
            {
                var rolFormPermission = await _rolFormPermissionBusiness.GetByIdAsync(id);
                if (rolFormPermission == null)
                    return NotFound($"No se encontró el registro con ID {id}.");

                return Ok(rolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el registro de RolFormPermission con ID {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Crea un nuevo registro de RolFormPermission.
        /// </summary>
        /// <param name="rolFormPermissionDto">Datos del registro a crear.</param>
        /// <returns>El registro creado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RolFormPermissionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RolFormPermissionDto>> Create([FromBody] RolFormPermissionDto rolFormPermissionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdRolFormPermission = await _rolFormPermissionBusiness.CreateAsync(rolFormPermissionDto);
                return CreatedAtAction(nameof(GetById), new { id = createdRolFormPermission.Id }, createdRolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el registro de RolFormPermission: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Actualiza un registro existente de RolFormPermission.
        /// </summary>
        /// <param name="id">Identificador único del registro.</param>
        /// <param name="rolFormPermissionDto">Datos actualizados del registro.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update(int id, [FromBody] RolFormPermissionDto rolFormPermissionDto)
        {
            if (id != rolFormPermissionDto.Id)
            {
                return BadRequest(new { message = "El ID del registro no coincide." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _rolFormPermissionBusiness.UpdateAsync(rolFormPermissionDto);
                if (!result)
                {
                    return NotFound($"No se encontró el registro con ID {id}.");
                }

                return Ok(new { message = "Registro actualizado correctamente", success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el registro de RolFormPermission con ID {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Elimina un registro de RolFormPermission por su ID.
        /// </summary>
        /// <param name="id">Identificador único del registro.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID debe ser mayor a 0." });
            }

            try
            {
                var result = await _rolFormPermissionBusiness.DeleteAsync(id);
                if (!result)
                {
                    return NotFound($"No se encontró el registro con ID {id}.");
                }

                return Ok(new { message = "Registro eliminado correctamente", success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el registro de RolFormPermission con ID {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignPermissions([FromBody] AssignPermissionsDto dto)
        {
            try
            {
                await _rolFormPermissionBusiness.AssignPermissionsAsync(dto);
                return Ok(new { message = "Permisos asignados correctamente." });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }
    }
}

