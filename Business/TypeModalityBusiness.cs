using Data;
using Entity.DTOs.TypeModality;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using static Dapper.SqlMapper;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los tipos de modalidades en el sistema.
    /// </summary>
    public class TypeModalityBusiness
    {
        private readonly TypeModalityData _typeModalityData;
        private readonly ILogger<TypeModalityData> _logger;

        public TypeModalityBusiness(TypeModalityData typeModalityData, ILogger<TypeModalityData> logger)
        {
            _typeModalityData = typeModalityData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las modalidades como una lista de DTOs.
        /// </summary>
        public async Task<IEnumerable<TypeModalityDto>> GetAllTypeModalitiesAsync()
        {
            try
            {
                var typeModalities = await _typeModalityData.GetAllAsync();
                return MapToDTOList(typeModalities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las modalidades");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de modalidades", ex);
            }
        }

        /// <summary>
        /// Obtiene una modalidad por su ID.
        /// </summary>
        public async Task<TypeModalityDto> GetTypeModalityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una modalidad con ID inválido: {Id}", id);
                throw new ValidationException("id", "El ID de la modalidad debe ser mayor que cero");
            }

            try
            {
                var typeModality = await _typeModalityData.GetByIdAsync(id);
                if (typeModality == null)
                {
                    _logger.LogInformation("No se encontró ninguna modalidad con ID: {Id}", id);
                    throw new EntityNotFoundException("TypeModality", id);
                }

                return MapToDTO(typeModality);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la modalidad con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la modalidad con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea una nueva modalidad a partir de un DTO.
        /// </summary>
        public async Task<TypeModalityDto> CreateTypeModalityAsync(TypeModalityDto typeModalityDto)
        {
            try
            {
                ValidateTypeModality(typeModalityDto);
                var typeModality = MapToEntity(typeModalityDto);
                var creado = await _typeModalityData.CreateAsync(typeModality);
                return MapToDTO(creado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva modalidad: {Name}", typeModalityDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la modalidad", ex);
            }
        }

        /// <summary>
        /// Actualiza parcialmente una modalidad existente.
        /// </summary>
        public async Task<bool> UpdateParcialTypeModalityAsync(TypeModalityUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO inválido para actualización parcial de modalidad.");
                throw new ValidationException("Id", "Datos inválidos para actualizar modalidad.");
            }

            try
            {
                var exists = await _typeModalityData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontró la modalidad con Id {Id} para actualizar", dto.Id);
                    throw new EntityNotFoundException("TypeModality", dto.Id);
                }

                return await _typeModalityData.PatchRolAsync(dto.Id, dto.Name, dto.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la modalidad con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar la modalidad", ex);
            }
        }

        /// <summary>
        /// Actualiza completamente una modalidad.
        /// </summary>
        public async Task<bool> UpdateCompletoTypeModalityAsync(TypeModalityUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO inválido para actualización completa de modalidad.");
                throw new ValidationException("Id", "Datos inválidos para actualización completa.");
            }

            try
            {

                var exists = await _typeModalityData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontró la modalidad con Id {Id} para actualizar", dto.Id);
                    throw new EntityNotFoundException("TypeModality", dto.Id);
                }

                // Modifica sus campos directamente
                exists.Name = dto.Name;
                exists.Description = dto.Description;

                return await _typeModalityData.UpdateAsync(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar completamente la modalidad con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar modalidad", ex);
            }
        }

        /// <summary>
        /// Elimina lógicamente una modalidad por su ID.
        /// </summary>
        public async Task<bool> SetTypeModalityActiveAsync(TypeModalityStatusDto dto )
        {
            if (dto.Id <= 0)
            {
                _logger.LogWarning("ID inválido para eliminación lógica.");
                throw new ValidationException("Id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var exists = await _typeModalityData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontro el typeModality con ID {typeModalityId} para cambiar su estado activo", dto.Id);
                    throw new EntityNotFoundException("typeModality", dto.Id);
                }
                return await _typeModalityData.SetActiveAsync(dto.Id, dto.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar eliminación lógica de modalidad con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar modalidad", ex);
            }
        }

        /// <summary>
        /// Elimina permanentemente una modalidad por su ID.
        /// </summary>
        public async Task<bool> DeleteTypeModalityAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido para eliminación permanente.");
                throw new ValidationException("Id", "El ID debe ser mayor que cero.");
            }

            try
            {
                return await _typeModalityData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar eliminación permanente de modalidad con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar modalidad permanentemente", ex);
            }
        }

        // VALIDACIÓN

        private void ValidateTypeModality(TypeModalityDto dto)
        {
            if (dto == null)
                throw new ValidationException("TypeModality", "El objeto TypeModality no puede ser nulo");

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning("Campo 'Name' vacío al crear/actualizar modalidad.");
                throw new ValidationException("Name", "El nombre de la modalidad es obligatorio.");
            }
        }

        // MAPEOS

        private TypeModalityDto MapToDTO(TypeModality entity) => new TypeModalityDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
        };

        private IEnumerable<TypeModalityDto> MapToDTOList(IEnumerable<TypeModality> entities) =>
            entities.Select(MapToDTO).ToList();

        private TypeModality MapToEntity(TypeModalityDto dto) => new TypeModality
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
        };
    }
}
