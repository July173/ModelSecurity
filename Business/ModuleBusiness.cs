using Data;
using Entity.DTOs.Module;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los módulos en el sistema.
    /// </summary>
    public class ModuleBusiness
    {
        private readonly ModuleData _moduleData;
        private readonly ILogger<ModuleData> _logger;

        public ModuleBusiness(ModuleData moduleData, ILogger<ModuleData> logger)
        {
            _moduleData = moduleData;
            _logger = logger;
        }

        // Método para obtener todos los módulos como DTOs
        public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
        {
            try
            {
                var modules = await _moduleData.GetAllAsync();
                
                return MapToDTOList(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los módulos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de módulos", ex);
            }
        }

        // Método para obtener un módulo por ID como DTO
        public async Task<ModuleDto> GetModuleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un módulo con ID inválido: {ModuleId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del módulo debe ser mayor que cero");
            }

            try
            {
                var module = await _moduleData.GetByidAsync(id);
                if (module == null)
                {
                    _logger.LogInformation("No se encontró ningún módulo con ID: {ModuleId}", id);
                    throw new EntityNotFoundException("Module", id);
                }

                return MapToDTO(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el módulo con ID {id}", ex);
            }
        }

        // Método para crear un módulo desde un DTO
        public async Task<ModuleDto> CreateModuleAsync(ModuleDto moduleDto)
        {
            try
            {
                ValidateModule(moduleDto);

                var module = MapToEntity(moduleDto);
                module.CreateDate = DateTime.Now;

                var moduleCreado = await _moduleData.CreateAsync(module);

                return MapToDTO(moduleCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo módulo: {Name}", moduleDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el módulo", ex);
            }
        }

        public async Task<bool> SetModuleActiveAsync(ModuleStatusDto dto)
        {
            if (dto == null)
                throw new ValidationException("El DTO de estado de módulo no puede ser nulo");

            if (dto.Id <= 0)
            {
                _logger.LogWarning("ID inválido para cambiar estado activo de módulo: {Id}", dto.Id);
                throw new ValidationException("Id", "El ID del módulo debe ser mayor a 0");
            }

            try
            {
                var entity = await _moduleData.GetByidAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("Módulo no encontrado con ID {Id} para cambiar estado", dto.Id);
                    throw new EntityNotFoundException("Module", dto.Id);
                }

                // Establecer DeleteDate si se va a desactivar (borrado lógico)
                if (!dto.Active)
                {
                    entity.DeleteDate = DateTime.Now;
                }
                else
                {
                    entity.DeleteDate = null; // Reactivación: eliminamos la marca de eliminación
                }

                return await _moduleData.SetActiveAsync(dto.Id, dto.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo de módulo con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar estado activo de módulo con ID {dto.Id}", ex);
            }
        }

        public async Task<bool> DeleteModuleAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un módulo con ID inválido: {Id}", id);
                throw new ValidationException("Id", "El ID debe ser mayor a 0");
            }

            try
            {
                var exists = await _moduleData.GetByidAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("Módulo no encontrado con ID {Id} para eliminar", id);
                    throw new EntityNotFoundException("Module", id);
                }

                return await _moduleData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar módulo con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar módulo con ID {id}", ex);
            }
        }


        public async Task<bool> UpdateParcialModuleAsync(ModuleUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización parcial inválido");
                throw new ValidationException("Id", "Datos inválidos para actualizar módulo");
            }

            try
            {
                var exists = await _moduleData.GetByidAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("Módulo no encontrado con ID: {Id}", dto.Id);
                    throw new EntityNotFoundException("Module", dto.Id);
                }

                return await _moduleData.PatchAsync(dto.Id, dto.Name, dto.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el módulo con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar módulo con ID {dto.Id}", ex);
            }
        }
        public async Task<bool> UpdateModuleAsync(ModuleUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización inválido");
                throw new Utilities.Exceptions.ValidationException("id", "Datos inválidos para actualizar módulo");
            }

            try
            {
                var entity = await _moduleData.GetByidAsync(dto.Id);
                if (entity == null)
                    throw new EntityNotFoundException("Module", dto.Id);

                // Modifica sus campos directamente
                entity.Name = dto.Name;
                entity.Description = dto.Description;
                entity.UpdateDate = DateTime.Now;

                return await _moduleData.UpdateAsync(entity); //actualizas la misma instancia rastreada
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el módulo con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar módulo con ID {dto.Id}", ex);
            }
        }


        // Método para validar el DTO
        private void ValidateModule(ModuleDto moduleDto)
        {
            if (moduleDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto módulo no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(moduleDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un módulo con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del módulo es obligatorio");
            }
        }


        //Metodo para mapear de Module a ModuleDto
        private ModuleDto MapToDTO(Module module)
        {
            return new ModuleDto
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                Active = module.Active,
            };
        }

        //Metodo para mapear de ModuleDto a Module 
        private Module MapToEntity(ModuleDto moduleDto)
        {
            return new Module
            {
                Id = moduleDto.Id,
                Name = moduleDto.Name,
                Description = moduleDto.Description,
                Active = moduleDto.Active,
            };
        }
        //Metodo para mapear una lista de Module a una lista de ModuleDto
        private IEnumerable<ModuleDto> MapToDTOList(IEnumerable<Module> modules)
        {
            var modulesDto = new List<ModuleDto>();
            foreach (var module in modules)
            {
                modulesDto.Add(MapToDTO(module));
            }
            return modulesDto;
        }

    }
}
