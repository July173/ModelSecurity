using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los programas en el sistema.
    /// </summary>
    public class ProgramBusiness
    {
        private readonly ProgramData _programData;
        private readonly ILogger _logger;

        public ProgramBusiness(ProgramData programData, ILogger logger)
        {
            _programData = programData;
            _logger = logger;
        }

        // Método para obtener todos los programas como DTOs
        public async Task<IEnumerable<ProgramDto>> GetAllProgramsAsync()
        {
            try
            {
                var programs = await _programData.GetAllAsync();
                var programsDTO = new List<ProgramDto>();

                foreach (var program in programs)
                {
                    programsDTO.Add(new ProgramDto
                    {
                        Id = program.Id,
                        Name = program.Name,
                        Description = program.Description,
                        CodeProgram = program.CodeProgram,
                        TypeProgram = program.TypeProgram,
                        Active = program.Active // si existe la entidad
                    });
                }

                return programsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los programas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de programas", ex);
            }
        }

        // Método para obtener un programa por ID como DTO
        public async Task<ProgramDto> GetProgramByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un programa con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del programa debe ser mayor que cero");
            }

            try
            {
                var program = await _programData.GetByIdAsync(id);
                if (program == null)
                {
                    _logger.LogInformation("No se encontró ningún programa con ID: {Id}", id);
                    throw new EntityNotFoundException("program", id);
                }

                return new ProgramDto
                {
                    Id = program.Id,
                    Name = program.Name,
                    Description = program.Description,
                    CodeProgram = program.CodeProgram,
                    TypeProgram = program.TypeProgram,
                    Active = program.Active // si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el programa con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el programa con ID {id}", ex);
            }
        }

        // Método para crear un programa desde un DTO
        public async Task<ProgramDto> CreateProgramAsync(ProgramDto programDto)
        {
            try
            {
                ValidateProgram(programDto);

                var program = new Program
                {
                    Name = programDto.Name,
                    Description = programDto.Description,
                    CodeProgram = programDto.CodeProgram,
                    TypeProgram = programDto.TypeProgram,
                    Active = programDto.Active // si existe la entidad
                };

                var programCreado = await _programData.CreateAsync(program);

                return new ProgramDto
                {
                    Id = program.Id,
                    Name = program.Name,
                    Description = program.Description,
                    CodeProgram = program.CodeProgram,
                    TypeProgram = program.TypeProgram,
                    Active = program.Active // si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo programa: {Name}", programDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el programa", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateProgram(ProgramDto programDto)
        {
            if (programDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Program no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(programDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un programa con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del programa es obligatorio");
            }
        }
    }
}
