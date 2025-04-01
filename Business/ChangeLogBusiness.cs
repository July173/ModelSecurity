using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los cambios de log en el sistema.
    /// </summary>
    public class ChangeLogBusiness
    {
        private readonly ChangeLogData _changeLogData;
        private readonly ILogger _logger;

        public ChangeLogBusiness(ChangeLogData changeLogData, ILogger logger)
        {
            _changeLogData = changeLogData;
            _logger = logger;
        }

        // Método para obtener todos los cambios de log como DTOs
        public async Task<IEnumerable<ChangeLogDto>> GetAllChangeLogsAsync()
        {
            try
            {
                var changeLogs = await _changeLogData.GetAllAsync();
                var changeLogsDTO = new List<ChangeLogDto>();

                foreach (var changeLog in changeLogs)
                {
                    changeLogsDTO.Add(new ChangeLogDto
                    {
                        Id = changeLog.Id,
                        EntityName = changeLog.EntityName,
                        EntityId = changeLog.EntityId,
                        ChangeType = changeLog.ChangeType,
                        ChangeDate = changeLog.ChangeDate
                    });
                }

                return changeLogsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los cambios de log");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de cambios de log", ex);
            }
        }

        // Método para obtener un cambio de log por ID como DTO
        public async Task<ChangeLogDto> GetChangeLogByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un cambio de log con ID inválido: {ChangeLogId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del cambio de log debe ser mayor que cero");
            }

            try
            {
                var changeLog = await _changeLogData.GetByIdAsync(id);
                if (changeLog == null)
                {
                    _logger.LogInformation("No se encontró ningún cambio de log con ID: {ChangeLogId}", id);
                    throw new EntityNotFoundException("ChangeLog", id);
                }

                return new ChangeLogDto
                {
                    Id = changeLog.Id,
                    EntityName = changeLog.EntityName,
                    EntityId = changeLog.EntityId,
                    ChangeType = changeLog.ChangeType,
                    ChangeDate = changeLog.ChangeDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el cambio de log con ID: {ChangeLogId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el cambio de log con ID {id}", ex);
            }
        }

        // Método para crear un cambio de log desde un DTO
        public async Task<ChangeLogDto> CreateChangeLogAsync(ChangeLogDto changeLogDto)
        {
            try
            {
                ValidateChangeLog(changeLogDto);

                var changeLog = new ChangeLog
                {
                    EntityName = changeLogDto.EntityName,
                    EntityId = changeLogDto.EntityId,
                    ChangeType = changeLogDto.ChangeType,
                    ChangeDate = changeLogDto.ChangeDate
                };

                var changeLogCreado = await _changeLogData.CreateAsync(changeLog);

                return new ChangeLogDto
                {
                    Id = changeLogCreado.Id,
                    EntityName = changeLogCreado.EntityName,
                    EntityId = changeLogCreado.EntityId,
                    ChangeType = changeLogCreado.ChangeType,
                    ChangeDate = changeLogCreado.ChangeDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo cambio de log");
                throw new ExternalServiceException("Base de datos", "Error al crear el cambio de log", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateChangeLog(ChangeLogDto changeLogDto)
        {
            if (changeLogDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto cambio de log no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(changeLogDto.EntityName))
            {
                _logger.LogWarning("Se intentó crear/actualizar un cambio de log con EntityName vacío");
                throw new Utilities.Exceptions.ValidationException("EntityName", "El EntityName del cambio de log es obligatorio");
            }
        }
    }
}