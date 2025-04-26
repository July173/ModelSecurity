using Data;
using Entity.DTOs.RegisterySofia;
using Entity.DTOs.Verification;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los registros de Sofia en el sistema.
    /// </summary>
    public class RegisterySofiaBusiness
    {
        private readonly RegisterySofiaData _registerySofiaData;
        private readonly ILogger<RegisterySofiaData> _logger;

        public RegisterySofiaBusiness(RegisterySofiaData registerySofiaData, ILogger<RegisterySofiaData> logger)
        {
            _registerySofiaData = registerySofiaData;
            _logger = logger;
        }

        // Método para obtener todos los registros de Sofia como DTOs
        public async Task<IEnumerable<RegisterySofiaDto>> GetAllRegisterySofiasAsync()
        {
            try
            {
                var registerySofias = await _registerySofiaData.GetAllAsync();
              
                return MapToDTOList(registerySofias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de Sofia");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de registros de Sofia", ex);
            }
        }

        // Método para obtener un registro de Sofia por ID como DTO
        public async Task<RegisterySofiaDto> GetRegisterySofiaByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un registro de Sofia con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del registro de Sofia debe ser mayor que cero");
            }

            try
            {
                var registerySofia = await _registerySofiaData.GetByIdAsync(id);
                if (registerySofia == null)
                {
                    _logger.LogInformation("No se encontró ningún registro de Sofia con ID: {Id}", id);
                    throw new EntityNotFoundException("registerySofia", id);
                }

                return MapToDTO(registerySofia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro de Sofia con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el registro de Sofia con ID {id}", ex);
            }
        }

        // Método para crear un registro de Sofia desde un DTO
        public async Task<RegisterySofiaDto> CreateRegisterySofiaAsync(RegisterySofiaDto registerySofiaDto)
        {
            try
            {
                ValidateRegisterySofia(registerySofiaDto);

                var registerySofia = MapToEntity(registerySofiaDto);
                registerySofia.CreateDate = DateTime.Now;

                var registerySofiaCreado = await _registerySofiaData.CreateAsync(registerySofia);

                return MapToDTO(registerySofiaCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo registro de Sofia: {Name}", registerySofiaDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el registro de Sofia", ex);
            }
        }


        public async Task<bool> UpdateParcialAsync(RegisterySofiaUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización parcial inválido");
                throw new ValidationException("Id", "Datos inválidos para actualizar ");
            }

            try
            {
                var exists = await _registerySofiaData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("registerySofia no encontrada con ID: {Id}", dto.Id);
                    throw new EntityNotFoundException("registerySofia", dto.Id);
                }

                return await _registerySofiaData.PatchAsync(dto.Id, dto.Name, dto.Description, dto.Document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la registerySofia con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar registerySofia con ID {dto.Id}", ex);
            }
        }

        public async Task<bool> UpdateAsync(RegisterySofiaUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización inválido");
                throw new Utilities.Exceptions.ValidationException("id", "Datos inválidos para actualizar registerySofia");
            }

            try
            {

                var entity = await _registerySofiaData.GetByIdAsync(dto.Id);
                if (entity == null)
                    throw new EntityNotFoundException("registerySofia", dto.Id);

                // Modifica sus campos directamente
                entity.Name = dto.Name;
                entity.Document = dto.Document;
                entity.Description = dto.Description;
                entity.UpdateDate = DateTime.Now;

                return await _registerySofiaData.UpdateAsync(entity); //actualizas la misma instancia rastreada

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la registerySofia con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar registerySofia con ID {dto.Id}", ex);
            }
        }


        // Método para validar el DTO
        private void ValidateRegisterySofia(RegisterySofiaDto registerySofiaDto)
        {
            if (registerySofiaDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto RegisterySofia no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(registerySofiaDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un registro de Sofia con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del registro de Sofia es obligatorio");
            }
        }

        public async Task<bool> SetActiveAsync(RegisterySofiaStatusDto dto)
        {
            if (dto == null)
                throw new ValidationException("El DTO de estado de RegisterySofia no puede ser nulo");

            if (dto.Id <= 0)
            {
                _logger.LogWarning("ID inválido para cambiar estado activo de RegisterySofia: {Id}", dto.Id);
                throw new ValidationException("Id", "El ID de la RegisterySofia debe ser mayor a 0");
            }

            try
            {
                var entity = await _registerySofiaData.GetByIdAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("RegisterySofia no encontrada con ID {Id} para cambiar estado", dto.Id);
                    throw new EntityNotFoundException("RegisterySofia", dto.Id);
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

                return await _registerySofiaData.SetActiveAsync(dto.Id, dto.Active); // Usamos UpdateAsync porque modificamos el objeto
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo de RegisterySofia con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar estado activo de RegisterySofia con ID {dto.Id}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una registerySofia con ID inválido: {Id}", id);
                throw new ValidationException("Id", "El ID debe ser mayor a 0");
            }

            try
            {
                var exists = await _registerySofiaData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("registerySofia no encontrada con ID {Id} para eliminar", id);
                    throw new EntityNotFoundException("registerySofia", id);
                }

                return await _registerySofiaData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar registerySofia con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar registerySofia con ID {id}", ex);
            }
        }

        //Metodo para mapear de RegisterySofia a RegisterySofiaDTO
        private RegisterySofiaDto MapToDTO(RegisterySofia registerySofia)
        {
            return new RegisterySofiaDto
            {
                Id = registerySofia.Id,
                Name = registerySofia.Name,
                Description = registerySofia.Description,
                Document = registerySofia.Document,
                Active = registerySofia.Active, // si existe la entidad
            };
        }
        //Metodo para mapear de RegisterySofiaDto a RegisterySofia 
        private RegisterySofia MapToEntity(RegisterySofiaDto registerySofiaDto)
        {
            return new RegisterySofia
            {
                Id = registerySofiaDto.Id,
                Name = registerySofiaDto.Name,
                Description = registerySofiaDto.Description,
                Document = registerySofiaDto.Document,
                Active = registerySofiaDto.Active, // si existe la entidad
            };
        }
        //Metodo para mapear una lista de RegisterySofia a una lista de RegisterySofiaDto
        private IEnumerable<RegisterySofiaDto> MapToDTOList(IEnumerable<RegisterySofia> RegisteriesSofia)
        {
            var registeriesSofiaDto = new List<RegisterySofiaDto>();
            foreach (var registerySofia in RegisteriesSofia)
            {
                registeriesSofiaDto.Add(MapToDTO(registerySofia));
            }
            return registeriesSofiaDto;
        }
    }
}
