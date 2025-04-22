using Data;
using Entity.DTOs.Process;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Utilities.Exceptions;
using Process = Entity.Model.Process;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los procesos en el sistema.
    /// </summary>
    public class ProcessBusiness
    {
        private readonly ProcessData _processData;
        private readonly ILogger<ProcessData> _logger;

        public ProcessBusiness(ProcessData processData, ILogger<ProcessData> logger)
        {
            _processData = processData;
            _logger = logger;
        }

        // Método para obtener todos los procesos como DTOs
        public async Task<IEnumerable<ProcessDto>> GetAllProcessesAsync()
        {
            try
            {
                var processes = await _processData.GetAllAsync();

                return MapToDTOList(processes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los procesos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de procesos", ex);
            }
        }

        // Método para obtener un proceso por ID como DTO
        public async Task<ProcessDto> GetProcessByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un proceso con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del proceso debe ser mayor que cero");
            }

            try
            {
                var process = await _processData.GetByIdAsync(id);
                if (process == null)
                {
                    _logger.LogInformation("No se encontró ningún proceso con ID: {Id}", id);
                    throw new EntityNotFoundException("process", id);
                }

                return  MapToDTO(process);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el proceso con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el proceso con ID {id}", ex);
            }
        }

        // Método para crear un proceso desde un DTO
        public async Task<ProcessDto> CreateProcessAsync(ProcessDto processDto)
        {
            try
            {
                ValidateProcess(processDto);

                var process = MapToEntity(processDto);
                process.CreateDate = DateTime.Now;

                var processCreado = await _processData.CreateAsync(process);

                return MapToDTO(processCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo proceso: {Name}", processDto?.TypeProcess ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el proceso", ex);
            }
        }

        public async Task<bool> SetProcessActiveAsync(ProcessStatusDto dto)
        {
            if (dto == null)
                throw new ValidationException("El DTO de estado de proceso no puede ser nulo");

            if (dto.Id <= 0)
            {
                _logger.LogWarning("ID inválido para cambiar estado activo de proceso: {Id}", dto.Id);
                throw new ValidationException("Id", "El ID del proceso debe ser mayor a 0");
            }

            try
            {
                var entity = await _processData.GetByIdAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("Proceso no encontrado con ID {Id} para cambiar estado", dto.Id);
                    throw new EntityNotFoundException("Process", dto.Id);
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

                return await _processData.SetActiveAsync(dto.Id, dto.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo de proceso con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar estado activo de proceso con ID {dto.Id}", ex);
            }
        }


        public async Task<bool> DeleteProcessAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un proceso con ID inválido: {Id}", id);
                throw new ValidationException("Id", "El ID debe ser mayor a 0");
            }

            try
            {
                var exists = await _processData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("Proceso no encontrado con ID {Id} para eliminar", id);
                    throw new EntityNotFoundException("Process", id);
                }

                return await _processData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar proceso con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar proceso con ID {id}", ex);
            }
        }



        public async Task<bool> UpdateParcialProcessAsync(ProcessUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización parcial inválido");
                throw new ValidationException("Id", "Datos inválidos para actualizar proceso");
            }

            try
            {
                var exists = await _processData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("Proceso no encontrado con ID: {Id}", dto.Id);
                    throw new EntityNotFoundException("Process", dto.Id);
                }

                return await _processData.PatchAsync(dto.Id, dto.TypeProcess, dto.Observation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el proceso con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar proceso con ID {dto.Id}", ex);
            }
        }

        public async Task<bool> UpdateProcessAsync(ProcessUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización inválido");
                throw new Utilities.Exceptions.ValidationException("id", "Datos inválidos para actualizar proceso");
            }

            try
            {
                var entity = await _processData.GetByIdAsync(dto.Id);
                if (entity == null)
                    throw new EntityNotFoundException("Process", dto.Id);

                // Modifica sus campos directamente
                entity.TypeProcess = dto.TypeProcess;
                entity.Observation = dto.Observation;
                entity.UpdateDate = DateTime.Now;

                return await _processData.UpdateAsync(entity); //actualizas la misma instancia rastreada
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el proceso con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar proceso con ID {dto.Id}", ex);
            }
        }


        // Método para validar el DTO
        private void ValidateProcess(ProcessDto processDto)
        {
            if (processDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Process no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(processDto.TypeProcess))
            {
                _logger.LogWarning("Se intentó crear/actualizar un proceso con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del proceso es obligatorio");
            }
        }


        //Metodo para mapear de Process a ProcessDto
        private ProcessDto MapToDTO(Process process)
        {
            return new ProcessDto
            {
                Id = process.Id,
                TypeProcess = process.TypeProcess,
                Observation = process.Observation,
                Active = process.Active, // si existe la entidad
                StartAprendiz = process.StartAprendiz
            };
        }
        //Metodo para mapear de ProcessDto a Process 
        private Process MapToEntity(ProcessDto processDto)
        {
            return new Process
            {
                Id = processDto.Id,
                TypeProcess = processDto.TypeProcess,
                Observation = processDto.Observation,
                Active = processDto.Active, // si existe la entidad
                StartAprendiz = processDto.StartAprendiz
            };
        }
        //Metodo para mapear una lista de Process a una lista de ProcessDto
        private IEnumerable<ProcessDto> MapToDTOList(IEnumerable<Process> processes)
        {
            var processesDto = new List<ProcessDto>();
            foreach (var process in processes)
            {
                processesDto.Add(MapToDTO(process));
            }
            return processesDto;
        }
    }
}
