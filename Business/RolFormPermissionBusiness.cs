using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Data;
using Entity.DTOs;
using Entity.DTOs.RolFormPermission;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase encargada de la lógica de negocio para la entidad RolFormPermission.
    /// </summary>
    public class RolFormPermissionBusiness
    {
        private readonly RolFormPermissionData _rolFormPermissionData;
        private readonly ILogger<RolFormPermissionBusiness> _logger;

        /// <summary>
        /// Constructor que recibe las dependencias necesarias.
        /// </summary>
        /// <param name="rolFormPermissionData">Instancia de <see cref="RolFormPermissionData"/> para acceso a datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{RolFormPermissionBusiness}"/> para el registro de logs.</param>
        public RolFormPermissionBusiness(RolFormPermissionData rolFormPermissionData, ILogger<RolFormPermissionBusiness> logger)
        {
            _rolFormPermissionData = rolFormPermissionData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de RolFormPermission.
        /// </summary>
        /// <returns>Lista de RolFormPermission en formato DTO.</returns>
        public async Task<IEnumerable<RolFormPermissionDto>> GetAllAsync()
        {
            try
            {
                var rolFormPermissions = await _rolFormPermissionData.GetAllAsync();
                return MapToDto(rolFormPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener todos los registros de RolFormPermission: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un registro de RolFormPermission por su ID.
        /// </summary>
        /// <param name="id">Identificador único del registro.</param>
        /// <returns>El registro en formato DTO, o null si no se encuentra.</returns>
        public async Task<RolFormPermissionDto?> GetByIdAsync(int id)
        {
            try
            {
                var rolFormPermission = await _rolFormPermissionData.GetByIdAsync(id);
                return rolFormPermission != null ? MapToDto(rolFormPermission) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el registro de RolFormPermission con ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo registro de RolFormPermission.
        /// </summary>
        /// <param name="rolFormPermissionDto">DTO con los datos del registro a crear.</param>
        /// <returns>El registro creado en formato DTO.</returns>
        public async Task<RolFormPermissionDto> CreateAsync(RolFormPermissionDto rolFormPermissionDto)
        {
            try
            {
                var rolFormPermission = MapToEntity(rolFormPermissionDto);
                var createdRolFormPermission = await _rolFormPermissionData.CreateAsync(rolFormPermission);
                return MapToDto(createdRolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el registro de RolFormPermission: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un registro existente de RolFormPermission.
        /// </summary>
        /// <param name="rolFormPermissionDto">DTO con los datos actualizados del registro.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(RolFormPermissionDto rolFormPermissionDto)
        {
            try
            {
                var rolFormPermission = MapToEntity(rolFormPermissionDto);
                return await _rolFormPermissionData.UpdateAsync(rolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el registro de RolFormPermission: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un registro de RolFormPermission por su ID.
        /// </summary>
        /// <param name="id">Identificador único del registro a eliminar.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                return await _rolFormPermissionData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el registro de RolFormPermission con ID {id}: {ex.Message}");
                return false;
            }
        }

        public async Task AssignPermissionsAsync(AssignPermissionsDto dto)
        {
            // Validación básica
            if (dto.RolId <= 0 || dto.FormPermissions == null || !dto.FormPermissions.Any())
                throw new ValidationException("Datos incompletos para la asignación.");

            await _rolFormPermissionData.AssignPermissionsAsync(dto.RolId, dto.FormPermissions);
        }

        /// <summary>
        /// Obtiene todos los registros de RolFormPermission.
        /// </summary>
        /// <returns>Lista de RolFormPermission en formato DTO.</returns>
        public async Task<List<FormPermissionDto>> GetFormPermissionsByRolIdAsync(int rolId)
        {
            if (rolId <= 0)
            {
                _logger.LogWarning("Se intentó obtener permisos con un ID de rol inválido: {RolId}", rolId);
                throw new ValidationException("rolId", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                return await _rolFormPermissionData.GetFormPermissionsByRolIdAsync(rolId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los permisos por formulario para el rol con ID: {RolId}", rolId);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar permisos del rol con ID {rolId}", ex);
            }
        }

        public async Task<List<RolFormDto>> GetPermisosAgrupados(int userId)
        {
            return await _rolFormPermissionData.ObtenerPermisosAgrupadosPorUsuario(userId);
        }


        /// <summary>
        /// Mapea una entidad RolFormPermission a un DTO.
        /// </summary>
        private RolFormPermissionDto MapToDto(RolFormPermission rolFormPermission)
        {
            return new RolFormPermissionDto
            {
                Id = rolFormPermission.Id,
                PermissionId = rolFormPermission.PermissionId,
                RolId = rolFormPermission.RolId,
                FormId = rolFormPermission.FormId,
            };
        }

        /// <summary>
        /// Mapea una lista de entidades RolFormPermission a una lista de DTOs.
        /// </summary>
        private IEnumerable<RolFormPermissionDto> MapToDto(IEnumerable<RolFormPermission> rolFormPermissions)
        {
            foreach (var rolFormPermission in rolFormPermissions)
            {
                yield return MapToDto(rolFormPermission);
            }
        }

        /// <summary>
        /// Mapea un DTO a una entidad RolFormPermission.
        /// </summary>
        private RolFormPermission MapToEntity(RolFormPermissionDto rolFormPermissionDto)
        {
            return new RolFormPermission
            {
                Id = rolFormPermissionDto.Id,
                PermissionId = rolFormPermissionDto.PermissionId,
                RolId = rolFormPermissionDto.RolId,
                FormId = rolFormPermissionDto.FormId
            };
        }
       

    }
}
