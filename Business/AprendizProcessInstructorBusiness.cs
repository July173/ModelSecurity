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
                        id = relacion.id,
                        aprendizId = relacion.aprendizId,
                        instructorId = relacion.instructorId,
                        registerySofiaId = relacion.registerySofiaId,
                        conceptId = relacion.conceptId,
                        enterpriseId = relacion.enterpriseId,
                        processId = relacion.processId,
                        typeModalityId = relacion.typeModalityId,
                        stateId = relacion.stateId,
                        verificationId = relacion.verificationId
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
                    id = relacion.id,
                    aprendizId = relacion.aprendizId,
                    instructorId = relacion.instructorId,
                    registerySofiaId = relacion.registerySofiaId,
                    conceptId = relacion.conceptId,
                    enterpriseId = relacion.enterpriseId,
                    processId = relacion.processId,
                    typeModalityId = relacion.typeModalityId,
                    stateId = relacion.stateId,
                    verificationId = relacion.verificationId
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
                    aprendizId = dto.aprendizId,
                    instructorId = dto.instructorId,
                    registerySofiaId = dto.registerySofiaId,
                    conceptId = dto.conceptId,
                    enterpriseId = dto.enterpriseId,
                    processId = dto.processId,
                    typeModalityId = dto.typeModalityId,
                    stateId = dto.stateId,
                    verificationId = dto.verificationId
                };

                var creada = await _aprendizProcessInstructorData.CreateAsync(relacion);

                return new AprendizProcessInstructorDto
                {
                    id = creada.id,
                    aprendizId = creada.aprendizId,
                    instructorId = creada.instructorId,
                    registerySofiaId = creada.registerySofiaId,
                    conceptId = creada.conceptId,
                    enterpriseId = creada.enterpriseId,
                    processId = creada.processId,
                    typeModalityId = creada.typeModalityId,
                    stateId = creada.stateId,
                    verificationId = creada.verificationId
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

            if (dto.aprendizId <= 0 || dto.instructorId <= 0 || dto.processId <= 0)
            {
                _logger.LogWarning("Se intentó crear una relación con IDs inválidos");
                throw new Utilities.Exceptions.ValidationException("IDs", "Los IDs deben ser mayores que cero");
            }
        }
    }
}
