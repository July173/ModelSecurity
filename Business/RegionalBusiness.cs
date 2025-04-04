using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las regionales en el sistema.
    /// </summary>
    public class RegionalBusiness
    {
        private readonly RegionalData _regionalData;
        private readonly ILogger _logger;

        public RegionalBusiness(RegionalData regionalData, ILogger logger)
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
                var regionalsDTO = new List<RegionalDto>();

                foreach (var regional in regionals)
                {
                    regionalsDTO.Add(new RegionalDto
                    {
                        Id = regional.Id,
                        Name = regional.Name,
                        Description = regional.Description,
                        CodeRegional = regional.CodeRegional,
                        Address = regional.Address,
                        Active = regional.Active // si existe la entidad
                    });
                }

                return regionalsDTO;
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

                return new RegionalDto
                {
                    Id = regional.Id,
                    Name = regional.Name,
                    Description = regional.Description,
                    CodeRegional = regional.CodeRegional,
                    Address = regional.Address,
                    Active = regional.Active // si existe la entidad
                };
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

                var regional = new Regional
                {
                    Name = regionalDto.Name,
                    Description = regionalDto.Description,
                    CodeRegional = regionalDto.CodeRegional,
                    Address = regionalDto.Address,
                    Active = regionalDto.Active // si existe la entidad
                };

                var regionalCreado = await _regionalData.CreateAsync(regional);

                return new RegionalDto
                {
                    Id = regional.Id,
                    Name = regional.Name,
                    Description = regional.Description,
                    CodeRegional = regional.CodeRegional,
                    Address = regional.Address,
                    Active = regional.Active // si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva regional: {Name}", regionalDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la regional", ex);
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
    }
}
