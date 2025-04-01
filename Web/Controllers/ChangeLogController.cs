using Business;
using Data;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de los registros de cambios en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ChangeLogController : ControllerBase
    {
        private readonly ChangeLogBusiness _ChangeLogBusiness;
        private readonly ILogger<ChangeLogController> _logger;

        /// <summary>
        /// Constructor del controlador de registros de cambios
        /// </summary>
        /// <param name="changeLogBusiness">Capa de negocio de registros de cambios</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public ChangeLogController(ChangeLogBusiness changeLogBusiness, ILogger<ChangeLogController> logger)
        {
            _ChangeLogBusiness = changeLogBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de cambios del sistema
        /// </summary>
        /// <returns>Lista de registros de cambios</returns>
        /// <response code="200">Retorna la lista de registros de cambios</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ChangeLogDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllChangeLogs()
        {
            try
            {
                var changeLogs = await _ChangeLogBusiness.GetAllChangeLogsAsync();
                return Ok(changeLogs);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener registros de cambios");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un registro de cambio específico por su ID
        /// </summary>
        /// <param name="id">ID del registro de cambio</param>
        /// <returns>Registro de cambio solicitado</returns>
        /// <response code="200">Retorna el registro de cambio solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Registro de cambio no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ChangeLogDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetChangeLogById(int id)
        {
            try
            {
                var changeLog = await _ChangeLogBusiness.GetChangeLogByIdAsync(id);
                return Ok(changeLog);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el registro de cambio con ID: {ChangeLogId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro de cambio no encontrado con ID: {ChangeLogId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener registro de cambio con ID: {ChangeLogId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo registro de cambio en el sistema
        /// </summary>
        /// <param name="changeLogDto">Datos del registro de cambio a crear</param>
        /// <returns>Registro de cambio creado</returns>
        /// <response code="201">Retorna el registro de cambio creado</response>
        /// <response code="400">Datos del registro de cambio no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ChangeLogDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateChangeLog([FromBody] ChangeLogDto changeLogDto)
        {
            try
            {
                var createdChangeLog = await _ChangeLogBusiness.CreateChangeLogAsync(changeLogDto);
                return CreatedAtAction(nameof(GetChangeLogById), new { id = createdChangeLog.Id }, createdChangeLog);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear registro de cambio");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear registro de cambio");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
