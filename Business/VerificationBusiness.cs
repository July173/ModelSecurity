using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las verificaciones del sistema.
    /// </summary>
    public class VerificationBusiness
    {
        private readonly VerificationData _verificationData;
        private readonly ILogger _logger;

        public VerificationBusiness(VerificationData verificationData, ILogger logger)
        {
            _verificationData = verificationData;
            _logger = logger;
        }

        // Método para obtener todos las verificaciones como DTOs
        public async Task<IEnumerable<VerificationDto>> GetAllVerificationsAsync()
        {
            try
            {
                var verifications = await _verificationData.GetAllAsync();
                var verificationsDTO = new List<VerificationDto>();

                foreach (var verification in verifications)
                {
                    verificationsDTO.Add(new VerificationDto
                    {
                        Id = verification.Id,
                        Name = verification.Name,
                        Observation = verification.Observation,
                        Active = verification.Active //si existe la entidad
                    });
                }

                return verificationsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos las verificaciones ");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de verificaciones", ex);
            }
        }

        // Método para obtener una verificacion por ID como DTO
        public async Task<VerificationDto> GetVerificationByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una verificacion con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la verificacion debe ser mayor que cero");
            }

            try
            {
                var verification = await _verificationData.GetByIdAsync(id);
                if (verification == null)
                {
                    _logger.LogInformation("No se encontró ninguna verificacion con ID: {Id}", id);
                    throw new EntityNotFoundException("verification", id);
                }

                return new VerificationDto
                {
                    Id = verification.Id,
                    Name = verification.Name,
                    Observation = verification.Observation,
                    Active = verification.Active //si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la verificacion con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la verificacion con ID {id}", ex);
            }
        }

        // Método para crear una verificacion desde un DTO
        public async Task<VerificationDto> CreateVerificationAsync(VerificationDto verificationDto)
        {
            try
            {
                ValidateUser(verificationDto);

                var verification = new Verification
                {
                    Name = verificationDto.Name,
                    Observation = verificationDto.Observation,
                    Active = verificationDto.Active //si existe la entidad
                };
           
                var verificationCreado = await _verificationData.CreateAsync(verification);

                return new VerificationDto
                {
                    Id = verification.Id,
                    Name = verification.Name,
                    Observation = verification.Observation,
                    Active = verification.Active //si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva verificacion: {Name}", verificationDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la verificacion", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateUser(VerificationDto verificationDto)
        {
            if (verificationDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto verificacion no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(verificationDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una verificacion con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la verificacion es obligatorio");
            }

         
        }
    }
}
