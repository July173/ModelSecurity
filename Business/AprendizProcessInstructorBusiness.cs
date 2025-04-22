using Data;
using Entity.DTOs.AprendizProcessInstructor;
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
        private readonly ILogger<AprendizProcessInstructorData> _logger;

        public AprendizProcessInstructorBusiness(AprendizProcessInstructorData aprendizProcessInstructorData, ILogger<AprendizProcessInstructorData> logger)
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
              

                return MapToDTOList(relaciones);
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

                return MapToDTO(relacion);
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

                var relacion = MapToEntity(dto);

                var creada = await _aprendizProcessInstructorData.CreateAsync(relacion);

                return MapToDTO(creada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva relación Aprendiz-Proceso-Instructor");
                throw new ExternalServiceException("Base de datos", "Error al crear la relación", ex);
            }
        }


        //Metodo para borrar aprendizProcessInstructor permanente (Delete permanente) 

        public async Task<bool> DeleteAprendizProcessInstructorAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intento eliminar un aprendizProcessInstructor con Id invalido : {aprendizProcessInstructorId}", id);
                throw new ValidationException("Id", "El id del aprendizProcessInstructor debe ser mayor a 0");
            }
            try
            {
                var exists = await _aprendizProcessInstructorData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontro el aprendizProcessInstructor con ID {aprendizProcessInstructorId} para eliminar", id);
                    throw new EntityNotFoundException("aprendizProcessInstructor", id);
                }
                return await _aprendizProcessInstructorData.DeleteAsync(id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el aprendizProcessInstructor con ID {aprendizProcessInstructorid}", id);
                throw new ExternalServiceException("Base de datos", $"Error al elimiar el aprendizProcessInstructor con ID {id}", ex);

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

        // Método para mapear de AprendizProcessInstructor a AprendizProcessInstructorDto
        private AprendizProcessInstructorDto MapToDTO(AprendizProcessInstructor aprendizProcessInstructor)
        {
            return new AprendizProcessInstructorDto
            {
                Id = aprendizProcessInstructor.Id,
                AprendizId = aprendizProcessInstructor.AprendizId,
                InstructorId = aprendizProcessInstructor.InstructorId,
                RegisterySofiaId = aprendizProcessInstructor.RegisterySofiaId,
                ConceptId = aprendizProcessInstructor.ConceptId,
                EnterpriseId = aprendizProcessInstructor.EnterpriseId,
                ProcessId = aprendizProcessInstructor.ProcessId,
                TypeModalityId = aprendizProcessInstructor.TypeModalityId,
                StateId = aprendizProcessInstructor.StateId,
                VerificationId = aprendizProcessInstructor.VerificationId,
                 

            };
        }

        // Método para mapear de AprendizProcessInstructorDto a AprendizProcessInstructor
        private AprendizProcessInstructor MapToEntity(AprendizProcessInstructorDto aprendizProcessInstructorDto)
        {
            return new AprendizProcessInstructor
            {
                Id = aprendizProcessInstructorDto.Id,
                AprendizId = aprendizProcessInstructorDto.AprendizId,
                InstructorId = aprendizProcessInstructorDto.InstructorId,
                RegisterySofiaId = aprendizProcessInstructorDto.RegisterySofiaId,
                ConceptId = aprendizProcessInstructorDto.ConceptId,
                EnterpriseId = aprendizProcessInstructorDto.EnterpriseId,
                ProcessId = aprendizProcessInstructorDto.ProcessId,
                TypeModalityId = aprendizProcessInstructorDto.TypeModalityId,
                StateId = aprendizProcessInstructorDto.StateId,
                VerificationId = aprendizProcessInstructorDto.VerificationId,
            };
        }

        // Método para mapear una lista de AprendizProcessInstructor a lista de AprendizProcessInstructorDto
        private IEnumerable<AprendizProcessInstructorDto> MapToDTOList(IEnumerable<AprendizProcessInstructor> aprendizProcessInstructors)
        {
            var aprendizProcessInstructorsDto = new List<AprendizProcessInstructorDto>();
            foreach (var aprendizProcessInstructor in aprendizProcessInstructors)
            {
                aprendizProcessInstructorsDto.Add(MapToDTO(aprendizProcessInstructor));
            }
            return aprendizProcessInstructorsDto;
        }

    }
}
