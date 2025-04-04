using Data;
using Entity.DTOautogestion;
using Entity.DTOautogestion.pivote;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los programas de aprendizaje en el sistema.
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

        // Método para obtener todos los programas de aprendizaje como DTOs
        public async Task<IEnumerable<AprendizProgramDto>> GetAllAprendizProgramsAsync()
        {
            try
            {
                var programs = await _aprendizProgramData.GetAllAsync();
                var programsDTO = new List<AprendizProgramDto>();

                foreach (var program in programs)
                {
                    programsDTO.Add(new AprendizProgramDto
                    {
                        Id = program.Id,                        
                        AprendizId = program.Id,
                        ProgramId = program.ProgramId,                            
                    });
                }

                return programsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los programas de aprendizaje");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de programas de aprendizaje", ex);
            }
        }

        // Método para obtener un programa de aprendizaje por ID como DTO
        public async Task<AprendizProgramDto> GetAprendizProgramByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un programa de aprendizaje con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del programa de aprendizaje debe ser mayor que cero");
            }

            try
            {
                var program = await _aprendizProgramData.GetByIdAsync(id);
                if (program == null)
                {
                    _logger.LogInformation("No se encontró ningún programa de aprendizaje con ID: {Id}", id);
                    throw new EntityNotFoundException("aprendizProgram", id);
                }

                return new AprendizProgramDto
                {
                    Id = program.Id,
                    AprendizId = program.Id,
                    ProgramId = program.ProgramId,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el programa de aprendizaje con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el programa de aprendizaje con ID {id}", ex);
            }
        }

        // Método para crear un programa de aprendizaje desde un DTO
        public async Task<AprendizProgramDto> CreateAprendizProgramAsync(AprendizProgramDto aprendizProgramDto)
        {
            try
            {
                ValidateAprendizProgram(aprendizProgramDto);

                var program = new AprendizProgram
                {
                    AprendizId = aprendizProgramDto.Id,
                    ProgramId = aprendizProgramDto.ProgramId,
                };

                var programCreado = await _aprendizProgramData.CreateAsync(program);

                return new AprendizProgramDto
                {
                    Id = program.Id,
                    AprendizId = program.Id,
                    ProgramId = program.ProgramId,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo programa de aprendizaje");
                throw new ExternalServiceException("Base de datos", "Error al crear el programa de aprendizaje", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateAprendizProgram(AprendizProgramDto aprendizProgramDto)
        {
            if (aprendizProgramDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto AprendizProgram no puede ser nulo");
            }

           
        }
    }
}
