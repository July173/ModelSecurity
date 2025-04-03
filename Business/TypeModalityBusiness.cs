using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los tipos de modalidades en el sistema.
    /// </summary>
    public class TypeModalityBusiness
    {
        private readonly TypeModalityData _typeModalityData;
        private readonly ILogger _logger;

        public TypeModalityBusiness(TypeModalityData typeModalityData, ILogger logger)
        {
            _typeModalityData = typeModalityData;
            _logger = logger;
        }

        // Método para obtener todas las modalidades como DTOs
        public async Task<IEnumerable<TypeModalityDto>> GetAllTypeModalitiesAsync()
        {
            try
            {
                var typeModalities = await _typeModalityData.GetAllAsync();
                var typeModalitiesDTO = new List<TypeModalityDto>();

                foreach (var typeModality in typeModalities)
                {
                    typeModalitiesDTO.Add(new TypeModalityDto
                    {
                        id = typeModality.id,
                        name = typeModality.name,
                        description = typeModality.description,
                        active = typeModality.active // si existe la entidad
                    });
                }

                return typeModalitiesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las modalidades");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de modalidades", ex);
            }
        }

        // Método para obtener una modalidad por ID como DTO
        public async Task<TypeModalityDto> GetTypeModalityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una modalidad con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la modalidad debe ser mayor que cero");
            }

            try
            {
                var typeModality = await _typeModalityData.GetByidAsync(id);
                if (typeModality == null)
                {
                    _logger.LogInformation("No se encontró ninguna modalidad con ID: {Id}", id);
                    throw new EntityNotFoundException("typeModality", id);
                }

                return new TypeModalityDto
                {
                    id = typeModality.id,
                    name = typeModality.name,
                    description = typeModality.description,
                    active = typeModality.active // si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la modalidad con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la modalidad con ID {id}", ex);
            }
        }

        // Método para crear una modalidad desde un DTO
        public async Task<TypeModalityDto> CreateTypeModalityAsync(TypeModalityDto typeModalityDto)
        {
            try
            {
                ValidateTypeModality(typeModalityDto);

                var typeModality = new TypeModality
                {
                    name = typeModalityDto.name,
                    description = typeModalityDto.description,
                    active = typeModalityDto.active // si existe en la entidad
                };

                var typeModalityCreado = await _typeModalityData.CreateAsync(typeModality);

                return new TypeModalityDto
                {
                    id = typeModalityCreado.id,
                    name = typeModalityCreado.name,
                    description = typeModalityCreado.description,
                    active = typeModalityCreado.active // si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva modalidad: {Name}", typeModalityDto?.name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la modalidad", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateTypeModality(TypeModalityDto typeModalityDto)
        {
            if (typeModalityDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto TypeModality no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(typeModalityDto.name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una modalidad con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la modalidad es obligatorio");
            }
        }
    }
}
