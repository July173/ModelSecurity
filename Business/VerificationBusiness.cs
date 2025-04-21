using Data;
using Entity.DTOs.Verification;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las verificaciones del sistema.
    /// </summary>
    public class VerificationBusiness
    {
        private readonly VerificationData _verificationData;
        private readonly ILogger<VerificationData> _logger;

        public VerificationBusiness(VerificationData verificationData, ILogger<VerificationData> logger)
        {
            _verificationData = verificationData;
            _logger = logger;
        }

        public async Task<IEnumerable<VerificationDto>> GetAllVerificationsAsync()
        {
            try
            {
                var verifications = await _verificationData.GetAllAsync();
                return MapToDTOList(verifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las verificaciones");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de verificaciones", ex);
            }
        }

        public async Task<VerificationDto> GetVerificationByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido al obtener verificación: {Id}", id);
                throw new ValidationException("id", "El ID de la verificación debe ser mayor que cero");
            }

            try
            {
                var verification = await _verificationData.GetByIdAsync(id);
                if (verification == null)
                {
                    _logger.LogInformation("Verificación no encontrada con ID: {Id}", id);
                    throw new EntityNotFoundException("Verification", id);
                }

                return MapToDTO(verification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la verificación con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la verificación con ID {id}", ex);
            }
        }

        public async Task<VerificationDto> CreateVerificationAsync(VerificationDto dto)
        {
            try
            {
                ValidateVerification(dto);

                var verification = MapToEntity(dto);
                verification.CreateDate = DateTime.Now;

                var created = await _verificationData.CreateAsync(verification);

                return MapToDTO(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva verificación: {Name}", dto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la verificación", ex);
            }
        }

        public async Task<bool> UpdateParcialVerificationAsync(VerificationUpadateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización parcial inválido");
                throw new ValidationException("Id", "Datos inválidos para actualizar verificación");
            }

            try
            {
                var exists = await _verificationData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("Verificación no encontrada con ID: {Id}", dto.Id);
                    throw new EntityNotFoundException("Verification", dto.Id);
                }

                return await _verificationData.PatchVerificationAsync(dto.Id, dto.Name, dto.Observation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la verificación con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar verificación con ID {dto.Id}", ex);
            }
        }

        public async Task<bool> UpdateVerificationAsync(VerificationUpadateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización inválido");
                throw new Utilities.Exceptions.ValidationException("id", "Datos inválidos para actualizar verificación");
            }

            try
            {
                var exists = await _verificationData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("Verificación no encontrada con ID: {Id}", dto.Id);
                    throw new EntityNotFoundException("Verification", dto.Id);
                }

                var entity = await _verificationData.GetByIdAsync(dto.Id);
                if (entity == null)
                    throw new EntityNotFoundException("Verification", dto.Id);

                // Modifica sus campos directamente
                entity.Name = dto.Name;
                entity.Observation = dto.Observation;
                entity.UpdateDate = DateTime.Now;

                return await _verificationData.UpdateAsync(entity); //actualizas la misma instancia rastreada

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la verificación con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar verificación con ID {dto.Id}", ex);
            }
        }

        public async Task<bool> SetVerificationActiveAsync(VerificationStatusDto dto)
        {
            if (dto == null)
                throw new ValidationException("El DTO de estado de verificación no puede ser nulo");

            if (dto.Id <= 0)
            {
                _logger.LogWarning("ID inválido para cambiar estado activo de verificación: {Id}", dto.Id);
                throw new ValidationException("Id", "El ID de la verificación debe ser mayor a 0");
            }

            try
            {
                var entity = await _verificationData.GetByIdAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("Verificación no encontrada con ID {Id} para cambiar estado", dto.Id);
                    throw new EntityNotFoundException("Verification", dto.Id);
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

                return await _verificationData.SetActiveAsync(dto.Id, dto.Active); // Usamos UpdateAsync porque modificamos el objeto
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo de verificación con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar estado activo de verificación con ID {dto.Id}", ex);
            }
        }


        public async Task<bool> DeleteVerificationAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una verificación con ID inválido: {Id}", id);
                throw new ValidationException("Id", "El ID debe ser mayor a 0");
            }

            try
            {
                var exists = await _verificationData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("Verificación no encontrada con ID {Id} para eliminar", id);
                    throw new EntityNotFoundException("Verification", id);
                }

                return await _verificationData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar verificación con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar verificación con ID {id}", ex);
            }
        }

        private void ValidateVerification(VerificationDto dto)
        {
            if (dto == null)
                throw new ValidationException("El objeto verificación no puede ser nulo");

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar verificación con nombre vacío");
                throw new ValidationException("Name", "El nombre es obligatorio");
            }
        }

        private VerificationDto MapToDTO(Verification verification)
        {
            return new VerificationDto
            {
                Id = verification.Id,
                Name = verification.Name,
                Observation = verification.Observation,
                Active = verification.Active,
            };
        }

        private Verification MapToEntity(VerificationDto dto)
        {
            return new Verification
            {
                Id = dto.Id,
                Name = dto.Name,
                Observation = dto.Observation,
                Active = dto.Active
            };
        }

        private IEnumerable<VerificationDto> MapToDTOList(IEnumerable<Verification> verifications)
        {
            return verifications.Select(MapToDTO).ToList();
        }
    }
}
