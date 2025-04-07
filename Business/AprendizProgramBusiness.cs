using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los aprendices en programas en el sistema.
    /// </summary>
    public class AprendizProgramBusiness
    {
        private readonly AprendizProgramData _aprendizProgramData;
        private readonly ILogger _logger;

        public AprendizProgramBusiness(AprendizProgramData aprendizProgramData, ILogger logger)
        {
            _aprendizProgramData = aprendizProgramData;
            _logger = logger;
        }

        // Método para obtener todos los AprendizProgram como DTOs
        public async Task<IEnumerable<AprendizProgramDto>> GetAllAprendizProgramsAsync()
        {
            try
            {
                var aprendizPrograms = await _aprendizProgramData.GetAllAsync();
                return MapToDTOList(aprendizPrograms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Aprendices en programas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de Aprendices en programas", ex);
            }
        }

        // Método para obtener un Aprendiz en programa por ID como DTO
        public async Task<AprendizProgramDto> GetAprendizProgramByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un Aprendiz en programa con ID inválido: {Id}", id);
                throw new ValidationException("id", "El ID del Aprendiz en programa debe ser mayor que cero");
            }

            try
            {
                var aprendizProgram = await _aprendizProgramData.GetByIdAsync(id);
                if (aprendizProgram == null)
                {
                    _logger.LogInformation("No se encontró ningún Aprendiz en programa con ID: {Id}", id);
                    throw new EntityNotFoundException("AprendizProgram", id);
                }

                return MapToDTO(aprendizProgram);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Aprendiz en programa con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el Aprendiz en programa con ID {id}", ex);
            }
        }

        // Método para crear un Aprendiz en programa desde un DTO
        public async Task<AprendizProgramDto> CreateAprendizProgramAsync(AprendizProgramDto aprendizProgramDto)
        {
            try
            {
                ValidateAprendizProgram(aprendizProgramDto);

                var aprendizProgram = MapToEntity(aprendizProgramDto);

                var aprendizProgramCreado = await _aprendizProgramData.CreateAsync(aprendizProgram);

                return MapToDTO(aprendizProgramCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo Aprendiz en programa: AprendizId={AprendizId}, ProgramId={ProgramId}",
                    aprendizProgramDto?.AprendizId ?? 0, aprendizProgramDto?.ProgramId ?? 0);
                throw new ExternalServiceException("Base de datos", "Error al crear el Aprendiz en programa", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateAprendizProgram(AprendizProgramDto aprendizProgramDto)
        {
            if (aprendizProgramDto == null)
            {
                throw new ValidationException("El objeto AprendizProgram no puede ser nulo");
            }

            if (aprendizProgramDto.AprendizId <= 0)
            {
                _logger.LogWarning("AprendizId inválido: {AprendizId}", aprendizProgramDto.AprendizId);
                throw new ValidationException("AprendizId", "El AprendizId debe ser mayor a cero");
            }

            if (aprendizProgramDto.ProgramId <= 0)
            {
                _logger.LogWarning("ProgramId inválido: {ProgramId}", aprendizProgramDto.ProgramId);
                throw new ValidationException("ProgramId", "El ProgramId debe ser mayor a cero");
            }
        }

        // Método para mapear de AprendizProgram a AprendizProgramDto
        private AprendizProgramDto MapToDTO(AprendizProgram aprendizProgram)
        {
            return new AprendizProgramDto
            {
                Id = aprendizProgram.Id,
                AprendizId = aprendizProgram.AprendizId,
                ProgramId = aprendizProgram.ProgramId,
            };
        }

        // Método para mapear de AprendizProgramDto a AprendizProgram
        private AprendizProgram MapToEntity(AprendizProgramDto aprendizProgramDto)
        {
            return new AprendizProgram
            {
                Id = aprendizProgramDto.Id,
                AprendizId = aprendizProgramDto.AprendizId,
                ProgramId = aprendizProgramDto.ProgramId,
            };
        }

        // Método para mapear una lista de AprendizProgram a lista de AprendizProgramDto
        private IEnumerable<AprendizProgramDto> MapToDTOList(IEnumerable<AprendizProgram> aprendizPrograms)
        {
            var aprendizProgramsDto = new List<AprendizProgramDto>();
            foreach (var aprendizProgram in aprendizPrograms)
            {
                aprendizProgramsDto.Add(MapToDTO(aprendizProgram));
            }
            return aprendizProgramsDto;
        }
    }
}
