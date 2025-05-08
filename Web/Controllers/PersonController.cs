using Business;
using Data;
using Entity.DTOs.Person;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PersonController : ControllerBase
    {
        private readonly IBaseBusiness<Person> _personBusiness;

        public PersonController(IBaseBusiness<Person> personBusiness)
        {
            _personBusiness = personBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _personBusiness.GetAllAsync();
            return Ok(result);
        }




        

        // Puedes agregar más endpoints aquí como Create, Update, Delete, etc.
    


/*
        [HttpPatch]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialPerson([FromBody] PersonUpdateDto dto)
        {
            try
            {
                var result = await _personBusiness.UpdateParcialPersonAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación en actualización parcial");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrada en actualización parcial: {Id}", dto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error en actualización parcial de persona");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("person-active")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetPersonActive([FromBody] PersonStatusDto dto)
        {
            try
            {
                var result = await _personBusiness.SetPersonActiveAsync(dto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al cambiar estado");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrada al cambiar estado: {Id}", dto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo de persona");
                return StatusCode(500, new { message = ex.Message });
            }
        }*/
    }
}
