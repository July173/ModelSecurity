using Business;
using Entity.DTOautogestion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de Enterprise en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class EnterpriseController : ControllerBase
    {
        private readonly EnterpriseBusiness _enterpriseBusiness;
        private readonly ILogger<EnterpriseController> _logger;

        /// <summary>
        /// Constructor del controlador de Enterprise
        /// </summary>
        public EnterpriseController(EnterpriseBusiness enterpriseBusiness, ILogger<EnterpriseController> logger)
        {
            _enterpriseBusiness = enterpriseBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las empresas del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EnterpriseDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllEnterprises()
        {
            try
            {
                var enterprises = await _enterpriseBusiness.GetAllEnterprisesAsync();
                return Ok(enterprises);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener empresas");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una empresa específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EnterpriseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetEnterpriseById(int id)
        {
            try
            {
                var enterprise = await _enterpriseBusiness.GetEnterpriseByIdAsync(id);
                return Ok(enterprise);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la empresa con ID: {EnterpriseId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Empresa no encontrada con ID: {EnterpriseId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener empresa con ID: {EnterpriseId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva empresa en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(EnterpriseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateEnterprise([FromBody] EnterpriseDto enterpriseDto)
        {
            try
            {
                var createdEnterprise = await _enterpriseBusiness.CreateEnterpriseAsync(enterpriseDto);
                return CreatedAtAction(nameof(GetEnterpriseById), new { id = createdEnterprise.Id }, createdEnterprise);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear empresa");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear empresa");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}