using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los procesos en el sistema.
    /// </summary>
    public class ProcessBusiness
    {
        private readonly ProcessData _processData;
        private readonly ILogger _logger;

        public ProcessBusiness(ProcessData processData, ILogger logger)
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
                var processesDTO = new List<ProcessDto>();

                foreach (var process in processes)
                {
                    processesDTO.Add(new ProcessDto
                    {
                        Id = process.Id,
                        TypeProcess = process.TypeProcess,
                        Observation = process.Observation,
                        Active = process.Active // si existe la entidad
                    });
                }

                return processesDTO;
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

                return new ProcessDto
                {
                    Id = process.Id,
                    TypeProcess = process.TypeProcess,
                    Observation = process.Observation,
                    Active = process.Active // si existe la entidad
                };
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

                var process = new Process
                {
                    TypeProcess = processDto.TypeProcess,
                    Observation = processDto.Observation,
                    Active = processDto.Active // si existe la entidad
                };

                var processCreado = await _processData.CreateAsync(process);

                return new ProcessDto
                {
                    Id = process.Id,
                    TypeProcess = process.TypeProcess,
                    Observation = process.Observation,
                    Active = process.Active // si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo proceso: {Name}", processDto?.TypeProcess ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el proceso", ex);
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
    }
}
