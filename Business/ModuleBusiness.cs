using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los módulos en el sistema.
    /// </summary>
    public class ModuleBusiness
    {
        private readonly ModuleData _moduleData;
        private readonly ILogger _logger;

        public ModuleBusiness(ModuleData moduleData, ILogger logger)
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
                var modulesDTO = new List<ModuleDto>();

                foreach (var module in modules)
                {
                    modulesDTO.Add(new ModuleDto
                    {
                        Id = module.Id,
                        Name = module.Name,
                        Description = module.Description,
                        Active = module.Active,
                    });
                }

                return modulesDTO;
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

                return new ModuleDto
                {
                    Id = module.Id,
                    Name = module.Name,
                    Description = module.Description,
                    Active = module.Active,
                };
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

                var module = new Module
                {
                    Name = moduleDto.Name,
                    Description = moduleDto.Description,
                    Active = moduleDto.Active,
                };

                var moduleCreado = await _moduleData.CreateAsync(module);

                return new ModuleDto
                {
                    Id = module.Id,
                    Name = module.Name,
                    Description = module.Description,
                    Active = module.Active,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo módulo: {Name}", moduleDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el módulo", ex);
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
    }
}
