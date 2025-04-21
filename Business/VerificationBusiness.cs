using Data;
using Entity.DTOs.Verification;
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
        private readonly ILogger<VerificationData> _logger;

        public VerificationBusiness(VerificationData verificationData, ILogger<VerificationData> logger)
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
               return MapToDTOList(verifications);
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

                return MapToDTO(verification);
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

                var verification = MapToEntity(verificationDto);
                verification.CreateDate = DateTime.Now;

                var verificationCreado = await _verificationData.CreateAsync(verification);

                return MapToDTO(verificationCreado);
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
        //Metodo para mapear de verification a VerificationDto
        private VerificationDto MapToDTO(Verification verification)
        {
            return new VerificationDto
            {
                Id = verification.Id,
                Name = verification.Name,
                Observation = verification.Observation,
                Active = verification.Active, //si existe la entidad
            };
        }
        //Metodo para mapear de VerificationDto a Verification 
        private Verification MapToEntity(VerificationDto verificationDto)
        {
            return new Verification
            {
                Id = verificationDto.Id,
                Name = verificationDto.Name,
                Observation = verificationDto.Observation,
                Active = verificationDto.Active, //si existe la entidad
            };
        }
        //Metodo para mapear una lista de Verification a una lista de VerificationDto
        private IEnumerable<VerificationDto> MapToDTOList(IEnumerable<Verification> verifications)
        {
            var verificationsDto = new List<VerificationDto>();
            foreach (var verification in verifications)
            {
                verificationsDto.Add(MapToDTO(verification));
            }
            return verificationsDto;
        }

    }
}
