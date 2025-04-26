using Data;
using Entity.DTOs.Program;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los programas en el sistema.
    /// </summary>
    public class ProgramBusiness
    {
        private readonly ProgramData _programData;
        private readonly ILogger<ProgramData> _logger;

        public ProgramBusiness(ProgramData programData, ILogger<ProgramData> logger)
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
                
                return MapToDTOList(programs);
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

                return MapToDTO(program);
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

                var program = MapToEntity(programDto);
                program.CreateDate = DateTime.Now;
                var programCreado = await _programData.CreateAsync(program);

                return MapToDTO(programCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo programa: {Name}", programDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el programa", ex);
            }
        }
        public async Task<bool> SetProgramActiveAsync(ProgramStatusDto dto)
        {
            if (dto == null)
                throw new ValidationException("El DTO de estado de programa no puede ser nulo");

            if (dto.Id <= 0)
            {
                _logger.LogWarning("ID inválido para cambiar estado activo de programa: {Id}", dto.Id);
                throw new ValidationException("Id", "El ID del programa debe ser mayor a 0");
            }

            try
            {
                var entity = await _programData.GetByIdAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("Programa no encontrado con ID {Id} para cambiar estado", dto.Id);
                    throw new EntityNotFoundException("Program", dto.Id);
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

                return await _programData.SetActiveAsync(dto.Id, dto.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo de programa con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar estado activo de programa con ID {dto.Id}", ex);
            }
        }

        public async Task<bool> DeleteProgramAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un programa con ID inválido: {Id}", id);
                throw new ValidationException("Id", "El ID debe ser mayor a 0");
            }

            try
            {
                var exists = await _programData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("Programa no encontrado con ID {Id} para eliminar", id);
                    throw new EntityNotFoundException("Program", id);
                }

                return await _programData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar programa con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar programa con ID {id}", ex);
            }
        }

        public async Task<bool> UpdateParcialProgramAsync(ProgramUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización parcial inválido");
                throw new ValidationException("Id", "Datos inválidos para actualizar programa");
            }

            try
            {
                var exists = await _programData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("Programa no encontrado con ID: {Id}", dto.Id);
                    throw new EntityNotFoundException("Program", dto.Id);
                }

                return await _programData.PatchAsync(dto.Id, dto.Name, dto.TypeProgram, dto.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el programa con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar programa con ID {dto.Id}", ex);
            }
        }

        public async Task<bool> UpdateProgramAsync(ProgramUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización inválido");
                throw new Utilities.Exceptions.ValidationException("id", "Datos inválidos para actualizar programa");
            }

            try
            {
                var entity = await _programData.GetByIdAsync(dto.Id);
                if (entity == null)
                    throw new EntityNotFoundException("Program", dto.Id);

                // Modifica sus campos directamente
                entity.Name = dto.Name;
                entity.TypeProgram = dto.TypeProgram;
                entity.Description = dto.Description;
                entity.UpdateDate = DateTime.Now;

                return await _programData.UpdateAsync(entity); //actualizas la misma instancia rastreada
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el programa con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar programa con ID {dto.Id}", ex);
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
        //Metodo para mapear de Program a ProgramDto
        private ProgramDto MapToDTO(Program program)
        {
            return new ProgramDto
            {
                Id = program.Id,
                Name = program.Name,
                Description = program.Description,
                CodeProgram = program.CodeProgram,
                TypeProgram = program.TypeProgram,
                Active = program.Active, // si existe la entidad 
               
            };
        }
        //Metodo para mapear de ProgramDto a Program 
        private Program MapToEntity(ProgramDto programDto)
        {
            return new Program
            {
                Id = programDto.Id,
                Name = programDto.Name,
                Description = programDto.Description,
                CodeProgram = programDto.CodeProgram,
                TypeProgram = programDto.TypeProgram,
                Active = programDto.Active, // si existe la entidad
               
            };
        }
        //Metodo para mapear una lista de Program a una lista de ProgramDto
        private IEnumerable<ProgramDto> MapToDTOList(IEnumerable<Program> programs)
        {
            var programsDto = new List<ProgramDto>();
            foreach (var program in programs)
            {
                programsDto.Add(MapToDTO(program));
            }
            return programsDto;
        }

    }
}
