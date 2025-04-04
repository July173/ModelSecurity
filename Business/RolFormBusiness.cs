using Data;
using Entity.DTOautogestion;
using Entity.DTOautogestion.pivote;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los roles de formulario en el sistema.
    /// </summary>
    public class RolFormBusiness
    {
        private readonly RolFormData _rolFormData;
        private readonly ILogger _logger;

        public RolFormBusiness(RolFormData rolFormData, ILogger logger)
        {
            _rolFormData = rolFormData;
            _logger = logger;
        }

        // Método para obtener todos los roles de formulario como DTOs
        public async Task<IEnumerable<RolFormDto>> GetAllRolFormsAsync()
        {
            try
            {
                var rolForms = await _rolFormData.GetAllAsync();
                var rolFormsDTO = new List<RolFormDto>();

                foreach (var rolForm in rolForms)
                {
                    rolFormsDTO.Add(new RolFormDto
                    {
                        Id = rolForm.Id,
                        Permission = rolForm.Permission,
                        RolId = rolForm.RolId,
                        FormId = rolForm.FormId,

                    });
                }

                return rolFormsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles de formulario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles de formulario", ex);
            }
        }

        // Método para obtener un rol de formulario por ID como DTO
        public async Task<RolFormDto> GetRolFormByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol de formulario con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol de formulario debe ser mayor que cero");
            }

            try
            {
                var rolForm = await _rolFormData.GetByIdAsync(id);
                if (rolForm == null)
                {
                    _logger.LogInformation("No se encontró ningún rol de formulario con ID: {Id}", id);
                    throw new EntityNotFoundException("rolForm", id);
                }

                return new RolFormDto
                {
                    Id = rolForm.Id,
                    Permission = rolForm.Permission,
                    RolId = rolForm.RolId,
                    FormId = rolForm.FormId,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol de formulario con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol de formulario con ID {id}", ex);
            }
        }

        // Método para crear un rol de formulario desde un DTO
        public async Task<RolFormDto> CreateRolFormAsync(RolFormDto rolFormDto)
        {
            try
            {
                ValidateRolForm(rolFormDto);

                var rolForm = new RolForm
                {
                    Permission = rolFormDto.Permission,
                    RolId = rolFormDto.RolId,
                    FormId = rolFormDto.FormId
                };

                var rolFormCreado = await _rolFormData.CreateAsync(rolForm);

                return new RolFormDto
                {
                    Id = rolForm.Id,
                    Permission = rolForm.Permission,
                    RolId = rolForm.RolId,
                    FormId = rolForm.FormId,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol de formulario: {Name}", rolFormDto?.Permission ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol de formulario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRolForm(RolFormDto rolFormDto)
        {
            if (rolFormDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto RolForm no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(rolFormDto.Permission))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol de formulario con permiso vacío");
                throw new Utilities.Exceptions.ValidationException("permission", "El permiso del rol de formulario es obligatorio");
            }
        }
    }
}
