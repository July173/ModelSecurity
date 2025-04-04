using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con la relación entre Aprendiz, Proceso e Instructor en el sistema.
    /// </summary>
    public class AprendizProcessInstructorBusiness
    {
        private readonly AprendizProcessInstructorData _aprendizProcessInstructorData;
        private readonly ILogger _logger;

        public AprendizProcessInstructorBusiness(AprendizProcessInstructorData aprendizProcessInstructorData, ILogger logger)
        {
            _aprendizProcessInstructorData = aprendizProcessInstructorData;
            _logger = logger;
        }

        // Método para obtener todas las relaciones Aprendiz-Proceso-Instructor como DTOs
        public async Task<IEnumerable<AprendizProcessInstructorDto>> GetAllAprendizProcessInstructorsAsync()
        {
            try
            {
                var relaciones = await _aprendizProcessInstructorData.GetAllAsync();
                var relacionesDTO = new List<AprendizProcessInstructorDto>();

                foreach (var relacion in relaciones)
                {
                    relacionesDTO.Add(new AprendizProcessInstructorDto
                    {
                        Id = relacion.Id,
                        AprendizId = relacion.AprendizId,
                        InstructorId = relacion.InstructorId,
                        RegisterySofiaId = relacion.RegisterySofiaId,
                        ConceptId = relacion.ConceptId,
                        EnterpriseId = relacion.EnterpriseId,
                        ProcessId = relacion.ProcessId,
                        TypeModalityId = relacion.TypeModalityId,
                        StateId = relacion.StateId,
                        VerificationId = relacion.VerificationId
                    });
                }

                return relacionesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las relaciones Aprendiz-Proceso-Instructor");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de relaciones", ex);
            }
        }

        // Método para obtener una relación Aprendiz-Proceso-Instructor por ID como DTO
        public async Task<AprendizProcessInstructorDto> GetAprendizProcessInstructorByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una relación con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación debe ser mayor que cero");
            }

            try
            {
                var relacion = await _aprendizProcessInstructorData.GetByIdAsync(id);
                if (relacion == null)
                {
                    _logger.LogInformation("No se encontró ninguna relación con ID: {Id}", id);
                    throw new EntityNotFoundException("AprendizProcessInstructor", id);
                }

                return new AprendizProcessInstructorDto
                {
                    Id = relacion.Id,
                    AprendizId = relacion.AprendizId,
                    InstructorId = relacion.InstructorId,
                    RegisterySofiaId = relacion.RegisterySofiaId,
                    ConceptId = relacion.ConceptId,
                    EnterpriseId = relacion.EnterpriseId,
                    ProcessId = relacion.ProcessId,
                    TypeModalityId = relacion.TypeModalityId,
                    StateId = relacion.StateId,
                    VerificationId = relacion.VerificationId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la relación con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la relación con ID {id}", ex);
            }
        }

        // Método para crear una relación Aprendiz-Proceso-Instructor desde un DTO
        public async Task<AprendizProcessInstructorDto> CreateAprendizProcessInstructorAsync(AprendizProcessInstructorDto dto)
        {
            try
            {
                ValidateAprendizProcessInstructor(dto);

                var relacion = new AprendizProcessInstructor
                {
                    Id = dto.Id,
                    AprendizId = dto.AprendizId,
                    InstructorId = dto.InstructorId,
                    RegisterySofiaId = dto.RegisterySofiaId,
                    ConceptId = dto.ConceptId,
                    EnterpriseId = dto.EnterpriseId,
                    ProcessId = dto.ProcessId,
                    TypeModalityId = dto.TypeModalityId,
                    StateId = dto.StateId,
                    VerificationId = dto.VerificationId
                };

                var creada = await _aprendizProcessInstructorData.CreateAsync(relacion);

                return new AprendizProcessInstructorDto
                {
                    Id = relacion.Id,
                    AprendizId = relacion.AprendizId,
                    InstructorId = relacion.InstructorId,
                    RegisterySofiaId = relacion.RegisterySofiaId,
                    ConceptId = relacion.ConceptId,
                    EnterpriseId = relacion.EnterpriseId,
                    ProcessId = relacion.ProcessId,
                    TypeModalityId = relacion.TypeModalityId,
                    StateId = relacion.StateId,
                    VerificationId = relacion.VerificationId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva relación Aprendiz-Proceso-Instructor");
                throw new ExternalServiceException("Base de datos", "Error al crear la relación", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateAprendizProcessInstructor(AprendizProcessInstructorDto dto)
        {
            if (dto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto AprendizProcessInstructor no puede ser nulo");
            }

            if (dto.AprendizId <= 0 || dto.InstructorId <= 0 || dto.ProcessId <= 0)
            {
                _logger.LogWarning("Se intentó crear una relación con IDs inválidos");
                throw new Utilities.Exceptions.ValidationException("IDs", "Los IDs deben ser mayores que cero");
            }
        }
    }
}
