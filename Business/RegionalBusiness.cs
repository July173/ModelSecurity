using Data;
using Entity.DTOs.Regional;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
    {
        /// <summary>
        /// Clase de negocio encargada de la lógica relacionada con las regionales en el sistema.
        /// </summary>
        public class RegionalBusiness
        {
            private readonly RegionalData _regionalData;
            private readonly ILogger<RegionalData> _logger;

            public RegionalBusiness(RegionalData regionalData, ILogger<RegionalData> logger)
            {
                _regionalData = regionalData;
                _logger = logger;
            }

            // Método para obtener todas las regionales como DTOs
            public async Task<IEnumerable<RegionalDto>> GetAllRegionalsAsync()
            {
                try
                {
                    var regionals = await _regionalData.GetAllAsync();
         
                    return MapToDTOList(regionals);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener todas las regionales");
                    throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de regionales", ex);
                }
            }

            // Método para obtener una regional por ID como DTO
            public async Task<RegionalDto> GetRegionalByIdAsync(int id)
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Se intentó obtener una regional con ID inválido: {Id}", id);
                    throw new Utilities.Exceptions.ValidationException("id", "El ID de la regional debe ser mayor que cero");
                }

                try
                {
                    var regional = await _regionalData.GetByIdAsync(id);
                    if (regional == null)
                    {
                        _logger.LogInformation("No se encontró ninguna regional con ID: {Id}", id);
                        throw new EntityNotFoundException("regional", id);
                    }

                    return MapToDTO(regional);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener la regional con ID: {Id}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al recuperar la regional con ID {id}", ex);
                }
            }

            // Método para crear una regional desde un DTO
            public async Task<RegionalDto> CreateRegionalAsync(RegionalDto regionalDto)
            {
                try
                {
                    ValidateRegional(regionalDto);

                    var regional = MapToEntity(regionalDto);

                    var regionalCreado = await _regionalData.CreateAsync(regional);

                    return MapToDTO(regionalCreado);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear nueva regional: {Name}", regionalDto?.Name ?? "null");
                    throw new ExternalServiceException("Base de datos", "Error al crear la regional", ex);
                }
            }

        // Método para actualizar datos parcialmente (patch)
        public async Task<bool> UpdateParcialRegionalAsync(RegionalUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización de regional inválido");
                throw new Utilities.Exceptions.ValidationException("Id", "Datos inválidos para actualizar regional");
            }

            try
            {
                var exists = await _regionalData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontró la regional con Id {Id} para actualizar", dto.Id);
                    throw new EntityNotFoundException("Regional", dto.Id);
                }

                return await _regionalData.PatchRegionalAsync(dto.Id, dto.Name, dto.Description, dto.CodeRegional, dto.Address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la regional con ID {dto.Id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la regional", ex);
            }
        }


        // Método para eliminación permanente
        public async Task<bool> DeleteRegionalAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Id inválido para eliminar regional");
                throw new Utilities.Exceptions.ValidationException("Id", "El ID debe ser mayor que cero");
            }

            try
            {
                var exists = await _regionalData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontró la regional con Id {Id} para eliminar", id);
                    throw new EntityNotFoundException("Regional", id);
                }

                return await _regionalData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar permanentemente la regional con ID {id}");
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la regional", ex);
            }
        }
        // Método para actualizar los datos en su totalidad con (put)
        public async Task<bool> UpdateCompletoRegionalAsync(RegionalUpdateDto Updatedto)
        {
            if (Updatedto == null || Updatedto.Id <= 0)
            {
                _logger.LogWarning("DTO de reemplazo de regional inválido");
                throw new Utilities.Exceptions.ValidationException("id", "Datos inválidos para reemplazar regional");
            }

            try
            {
                var entity = await _regionalData.GetByIdAsync(Updatedto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("No se encontró la regional con ID {RegionalId} para reemplazar", Updatedto.Id);
                    throw new EntityNotFoundException("Regional", Updatedto.Id);
                }

                // Modifica sus campos directamente
                entity.Name = Updatedto.Name;
                entity.Description = Updatedto.Description;

                return await _regionalData.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al reemplazar la regional con ID {Updatedto.Id}");
                throw new ExternalServiceException("Base de datos", $"Error al reemplazar la regional", ex);
            }
        }

        // Método para delete lógico para activar y desactivar una regional (delete logico)
        public async Task<bool> SetRegionalActiveAsync(RegionalStatusDto dto)
        {
            if (dto == null)
            {
                throw new ValidationException("El dto de estado de la regional no puede ser nulo");
            }

            if (dto.Id <= 0)
            {
                _logger.LogWarning("Se intentó activar/desactivar una regional con ID inválido: {RegionalId}", dto.Id);
                throw new ValidationException("Id", "El ID de la regional debe ser mayor a 0");
            }

            try
            {
                var exists = await _regionalData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontró la regional con ID {RegionalId} para cambiar su estado activo", dto.Id);
                    throw new EntityNotFoundException("Regional", dto.Id);
                }

                return await _regionalData.SetActiveAsync(dto.Id, dto.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado activo de la regional con ID {RegionalId}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el estado activo de la regional con ID {dto.Id}", ex);
            }
        }


        // Método para validar el DTO
        private void ValidateRegional(RegionalDto regionalDto)
            {
                if (regionalDto == null)
                {
                    throw new Utilities.Exceptions.ValidationException("El objeto Regional no puede ser nulo");
                }

                if (string.IsNullOrWhiteSpace(regionalDto.Name))
                {
                    _logger.LogWarning("Se intentó crear/actualizar una regional con Name vacío");
                    throw new Utilities.Exceptions.ValidationException("Name", "El Name de la regional es obligatorio");
                }
            }

            //Metodo para mapear de Regional a RegionalDto
            private RegionalDto MapToDTO(Regional regional)
            {
                return new RegionalDto
                {
                    Id = regional.Id,
                    Name = regional.Name,
                    Description = regional.Description,
                    CodeRegional = regional.CodeRegional,
                    Address = regional.Address,
                    Active = regional.Active, // si existe la entidad
                };
            }
            //Metodo para mapear de RegionalDto a Regional 
            private Regional MapToEntity(RegionalDto regionalDto)
            {
                return new Regional
                {
                    Id = regionalDto.Id,
                    Name = regionalDto.Name,
                    Description = regionalDto.Description,
                    CodeRegional = regionalDto.CodeRegional,
                    Address = regionalDto.Address,
                    Active = regionalDto.Active, // si existe la entidad
                };
            }
            //Metodo para mapear una lista de Regional a una lista de RegionalDto
            private IEnumerable<RegionalDto> MapToDTOList(IEnumerable<Regional> regionals)
            {
                var regionalsDto = new List<RegionalDto>();
                foreach (var regional in regionals)
                {
                    regionalsDto.Add(MapToDTO(regional));
                }
                return regionalsDto;
            }
        }
    }
