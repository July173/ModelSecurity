﻿using Business;
using Entity.DTOs.Process;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de procesos en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProcessController : ControllerBase
    {
        private readonly ProcessBusiness _processBusiness;
        private readonly ILogger<ProcessController> _logger;

        /// <summary>
        /// Constructor del controlador de procesos
        /// </summary>
        public ProcessController(ProcessBusiness processBusiness, ILogger<ProcessController> logger)
        {
            _processBusiness = processBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los procesos del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProcessDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllProcesses()
        {
            try
            {
                var processes = await _processBusiness.GetAllProcessesAsync();
                return Ok(processes);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener procesos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un proceso específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProcessDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProcessById(int id)
        {
            try
            {
                var process = await _processBusiness.GetProcessByIdAsync(id);
                return Ok(process);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el proceso con ID: {ProcessId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Proceso no encontrado con ID: {ProcessId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener proceso con ID: {ProcessId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo proceso en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProcessDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateProcess([FromBody] ProcessDto processDto)
        {
            try
            {
                var createdProcess = await _processBusiness.CreateProcessAsync(processDto);
                return CreatedAtAction(nameof(GetProcessById), new { id = createdProcess.Id }, createdProcess);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear proceso");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear proceso");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un proceso por su ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteProcess(int id)
        {
            try
            {
                var result = await _processBusiness.DeleteProcessAsync(id);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "ID inválido al intentar eliminar: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Proceso no encontrado al eliminar: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar proceso");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza completamente un proceso existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateProcess(int id, [FromBody] ProcessUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "El id de la ruta no coincide con el del cuerpo" });
            try
            {
                var result = await _processBusiness.UpdateProcessAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar proceso");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Proceso no encontrado al actualizar: {Id}", dto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar proceso");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        /// <summary>
        /// Actualiza parcialmente un proceso (solo tipo y observación).
        /// </summary>
        [HttpPatch]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialProcess([FromBody] ProcessUpdateDto dto)
        {
            try
            {
                var result = await _processBusiness.UpdateParcialProcessAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación en actualización parcial");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Proceso no encontrado en actualización parcial: {Id}", dto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error en actualización parcial de proceso");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cambia el estado activo/inactivo de un proceso.
        /// </summary>
        [HttpDelete("active")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetProcessActive([FromBody] ProcessStatusDto dto)
        {
            try
            {
                var result = await _processBusiness.SetProcessActiveAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al cambiar estado");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Proceso no encontrado al cambiar estado: {Id}", dto.Id);
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
