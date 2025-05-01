using Data;
using Entity.DTOs.FormModule;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los módulos de formulario en el sistema.
    /// </summary>
    public class FormModuleBusiness
    {
        private readonly FormModuleData _formModuleData;
        private readonly ILogger<FormModuleBusiness> _logger;

        public FormModuleBusiness(FormModuleData formModuleData, ILogger<FormModuleBusiness> logger)
        {
            _formModuleData = formModuleData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los módulos de formulario como DTOs.
        /// </summary>
        public async Task<IEnumerable<FormModuleDto>> GetAllFormModulesAsync()
        {
            try
            {
                var formModules = await _formModuleData.GetAllAsync();
                return MapToDTOList(formModules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los módulos de formulario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de módulos de formulario", ex);
            }
        }

        /// <summary>
        /// Obtiene un módulo de formulario por ID como DTO.
        /// </summary>
        public async Task<FormModuleDto> GetFormModuleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un módulo de formulario con ID inválido: {FormModuleId}", id);
                throw new ValidationException("id", "El ID del módulo de formulario debe ser mayor que cero");
            }

            try
            {
                var formModule = await _formModuleData.GetByIdAsync(id);
                if (formModule == null)
                {
                    _logger.LogInformation("No se encontró ningún módulo de formulario con ID: {FormModuleId}", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                return MapToDTO(formModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo de formulario con ID: {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el módulo de formulario con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo módulo de formulario desde un DTO.
        /// </summary>
        public async Task<FormModuleDto> CreateFormModuleAsync(FormModuleDto formModuleDto)
        {
            try
            {
                ValidateFormModule(formModuleDto);

                var formModule = MapToEntity(formModuleDto);

                var formModuleCreado = await _formModuleData.CreateAsync(formModule);

                return MapToDTO(formModuleCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo módulo de formulario");
                throw new ExternalServiceException("Base de datos", "Error al crear el módulo de formulario", ex);
            }
        }

        /// <summary>
        /// Actualiza un módulo de formulario existente.
        /// </summary>
        public async Task<bool> UpdateFormModuleAsync(FormModuleDto formModuleDto)
        {
            try
            {
                ValidateFormModule(formModuleDto);

                var formModule = MapToEntity(formModuleDto);

                var result = await _formModuleData.UpdateAsync(formModule);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar el módulo de formulario con ID {FormModuleId}", formModuleDto.Id);
                    throw new EntityNotFoundException("FormModule", formModuleDto.Id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el módulo de formulario con ID {FormModuleId}", formModuleDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el módulo de formulario con ID {formModuleDto.Id}", ex);
            }
        }

        /// <summary>
        /// Actualiza parcialmente un módulo de formulario.
        /// </summary>
        public async Task<bool> UpdatePartialFormModuleAsync(int id, Dictionary<string, object> updatedFields)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar parcialmente un módulo de formulario con ID inválido: {FormModuleId}", id);
                throw new ValidationException("id", "El ID del módulo de formulario debe ser mayor que cero");
            }

            try
            {
                var result = await _formModuleData.UpdatePartialAsync(id, updatedFields);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar parcialmente el módulo de formulario con ID {FormModuleId}", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el módulo de formulario con ID {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el módulo de formulario con ID {id}", ex);
            }
        }

        /// <summary>
        /// Elimina un módulo de formulario por su ID.
        /// </summary>
        public async Task<bool> DeleteFormModuleAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un módulo de formulario con ID inválido: {FormModuleId}", id);
                throw new ValidationException("id", "El ID del módulo de formulario debe ser mayor que cero");
            }

            try
            {
                var result = await _formModuleData.DeleteAsync(id);

                if (!result)
                {
                    _logger.LogInformation("No se encontró el módulo de formulario con ID {FormModuleId} para eliminar", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el módulo de formulario con ID {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el módulo de formulario con ID {id}", ex);
            }
        }

        /// <summary>
        /// Realiza un borrado lógico de un módulo de formulario por su ID.
        /// </summary>
        public async Task<bool> SoftDeleteFormModuleAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar un borrado lógico de un módulo de formulario con ID inválido: {FormModuleId}", id);
                throw new ValidationException("id", "El ID del módulo de formulario debe ser mayor que cero");
            }

            try
            {
                var result = await _formModuleData.SoftDeleteAsync(id);

                if (!result)
                {
                    _logger.LogInformation("No se encontró el módulo de formulario con ID {FormModuleId} para borrado lógico", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar el borrado lógico del módulo de formulario con ID {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar el borrado lógico del módulo de formulario con ID {id}", ex);
            }
        }


        /// <summary>
        /// Valida el DTO del módulo de formulario.
        /// </summary>
        private void ValidateFormModule(FormModuleDto formModuleDto)
        {
            if (formModuleDto == null)
            {
                throw new ValidationException("El objeto módulo de formulario no puede ser nulo");
            }

            if (formModuleDto.FormId <= 0 || formModuleDto.ModuleId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un módulo de formulario con FormId o ModuleId inválidos");
                throw new ValidationException("FormId/ModuleId", "El FormId y el ModuleId del módulo de formulario son obligatorios y deben ser mayores que cero");
            }
        }

        /// <summary>
        /// Mapea un objeto FormModule a FormModuleDto.
        /// </summary>
        private FormModuleDto MapToDTO(FormModule formModule)
        {
            return new FormModuleDto
            {
                Id = formModule.Id,
                FormId = formModule.FormId,
                ModuleId = formModule.ModuleId
            };
        }

        /// <summary>
        /// Mapea un objeto FormModuleDto a FormModule.
        /// </summary>
        private FormModule MapToEntity(FormModuleDto formModuleDto)
        {
            return new FormModule
            {
                Id = formModuleDto.Id,
                FormId = formModuleDto.FormId,
                ModuleId = formModuleDto.ModuleId
            };
        }

        /// <summary>
        /// Mapea una lista de objetos FormModule a una lista de FormModuleDto.
        /// </summary>
        private IEnumerable<FormModuleDto> MapToDTOList(IEnumerable<FormModule> formModules)
        {
            var formModulesDTO = new List<FormModuleDto>();
            foreach (var formModule in formModules)
            {
                formModulesDTO.Add(MapToDTO(formModule));
            }
            return formModulesDTO;
        }
    }
}

