using Data;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using Entity.DTOs;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los registros de cambio (ChangeLog).
    /// </summary>
    public class ChangeLogBusiness
    {
        private readonly ChangeLogData _changeLogData;
        private readonly ILogger<ChangeLogData> _logger;

        public ChangeLogBusiness(ChangeLogData changeLogData, ILogger<ChangeLogData> logger)
        {
            _changeLogData = changeLogData;
            _logger = logger;
        }

        // Método para obtener todos los registros del ChangeLog como DTOs
        public async Task<IEnumerable<ChangeLogDto>> GetAllChangeLogsAsync()
        {
            try
            {
                var logs = await _changeLogData.GetAllAsync();
                return MapToDTOList(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros del ChangeLog");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de logs de cambio", ex);
            }
        }

        // Método para obtener un registro de ChangeLog por su ID como DTO
        public async Task<ChangeLogDto> GetChangeLogByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un registro de ChangeLog con un ID inválido: {ChangeLogId}", id);
                throw new ValidationException("id", "El ID del log debe ser mayor a 0");
            }

            try
            {
                var log = await _changeLogData.GetByIdAsync(id);
                if (log == null)
                {
                    _logger.LogInformation("No se encontró el log con ID {ChangeLogId}", id);
                    throw new EntityNotFoundException("ChangeLog", id);
                }
                return MapToDTO(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el log con ID {ChangeLogId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el log con ID {id}", ex);
            }
        }

        // Método para crear un registro de ChangeLog desde un DTO
        public async Task<ChangeLogDto> CreateChangeLogAsync(ChangeLogDto changeLogDto)
        {
            try
            {
                ValidateChangeLog(changeLogDto);

                var log = MapToEntity(changeLogDto);

                var logCreado = await _changeLogData.CreateAsync(log);

                return MapToDTO(logCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo ChangeLog para la tabla: {TableName}", changeLogDto?.TableName ?? "null");
                throw new ExternalServiceException("Base de datos", $"Error al crear el registro de cambio", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateChangeLog(ChangeLogDto changeLogDto)
        {
            if (changeLogDto == null)
            {
                throw new ValidationException("El objeto ChangeLog no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(changeLogDto.TableName))
            {
                _logger.LogWarning("Se intentó crear un log con TableName vacío");
                throw new ValidationException("TableName", "El nombre de la tabla es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(changeLogDto.Action))
            {
                _logger.LogWarning("Se intentó crear un log con Action vacío");
                throw new ValidationException("Action", "La acción del log es obligatoria");
            }

            if (string.IsNullOrWhiteSpace(changeLogDto.UserName))
            {
                _logger.LogWarning("Se intentó crear un log sin nombre de usuario");
                throw new ValidationException("UserName", "El nombre del usuario es obligatorio");
            }
        }

        // Método para mapear de ChangeLog a ChangeLogDto
        private ChangeLogDto MapToDTO(ChangeLog changeLog)
        {
            return new ChangeLogDto
            {
                Id = changeLog.Id,
                TableName = changeLog.TableName,
                IdTable = changeLog.IdTable,
                OldValues = changeLog.OldValues,
                NewValues = changeLog.NewValues,
                Action = changeLog.Action,
                Active = changeLog.Active,
                UserName = changeLog.UserName
            };
        }

        // Método para mapear de ChangeLogDto a ChangeLog
        private ChangeLog MapToEntity(ChangeLogDto dto)
        {
            return new ChangeLog
            {
                Id = dto.Id,
                TableName = dto.TableName,
                IdTable = dto.IdTable,
                OldValues = dto.OldValues,
                NewValues = dto.NewValues,
                Action = dto.Action,
                Active = dto.Active,
                UserName = dto.UserName,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };
        }

        // Método para mapear una lista de ChangeLog a una lista de DTO
        private IEnumerable<ChangeLogDto> MapToDTOList(IEnumerable<ChangeLog> logs)
        {
            var list = new List<ChangeLogDto>();
            foreach (var log in logs)
            {
                list.Add(MapToDTO(log));
            }
            return list;
        }
    }
}
