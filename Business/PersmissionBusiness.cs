using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Business
{
    /// <summary>
    /// Clase encargada de la lógica de negocio para la entidad Permission.
    /// </summary>
    public class PermissionBusiness
    {
        private readonly PermissionData _permissionData;
        private readonly ILogger<PermissionBusiness> _logger;

        /// <summary>
        /// Constructor que recibe las dependencias necesarias.
        /// </summary>
        /// <param name="permissionData">Instancia de <see cref="PermissionData"/> para acceso a datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{PermissionBusiness}"/> para el registro de logs.</param>
        public PermissionBusiness(PermissionData permissionData, ILogger<PermissionBusiness> logger)
        {
            _permissionData = permissionData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos.
        /// </summary>
        /// <returns>Lista de permisos en formato DTO.</returns>
        public async Task<IEnumerable<PermissionDto>> GetAllAsync()
        {
            try
            {
                var permissions = await _permissionData.GetAllAsync();
                var permissionDtos = MapToDto(permissions);
                return permissionDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener todos los permisos: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un permiso por su ID.
        /// </summary>
        /// <param name="id">Identificador único del permiso.</param>
        /// <returns>El permiso en formato DTO, o null si no se encuentra.</returns>
        public async Task<PermissionDto?> GetByIdAsync(int id)
        {
            try
            {
                var permission = await _permissionData.GetByIdAsync(id);
                return permission != null ? MapToDto(permission) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el permiso con ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo permiso.
        /// </summary>
        /// <param name="permissionDto">DTO con los datos del permiso a crear.</param>
        /// <returns>El permiso creado en formato DTO.</returns>
        public async Task<PermissionDto> CreateAsync(PermissionDto permissionDto)
        {
            try
            {
                var permission = MapToEntity(permissionDto);
                var createdPermission = await _permissionData.CreateAsync(permission);
                return MapToDto(createdPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el permiso: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un permiso existente.
        /// </summary>
        /// <param name="permissionDto">DTO con los datos actualizados del permiso.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(PermissionDto permissionDto)
        {
            try
            {
                var permission = MapToEntity(permissionDto);
                return await _permissionData.UpdateAsync(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el permiso: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Actualiza parcialmente un permiso.
        /// </summary>
        /// <param name="id">Identificador único del permiso a actualizar.</param>
        /// <param name="updatedFields">Diccionario con los campos y valores a actualizar.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdatePartialAsync(int id, Dictionary<string, object> updatedFields)
        {
            try
            {
                return await _permissionData.UpdatePartialAsync(id, updatedFields);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar parcialmente el permiso con ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Elimina un permiso de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del permiso a eliminar.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                return await _permissionData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el permiso con ID {id}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Mapea una entidad Permission a un DTO.
        /// </summary>
        private PermissionDto MapToDto(Permission permission)
        {
            return new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                DisplayName = permission.DisplayName
            };
        }

        /// <summary>
        /// Mapea una lista de entidades Permission a una lista de DTOs.
        /// </summary>
        private IEnumerable<PermissionDto> MapToDto(IEnumerable<Permission> permissions)
        {
            foreach (var permission in permissions)
            {
                yield return MapToDto(permission);
            }
        }

        /// <summary>
        /// Mapea un DTO a una entidad Permission.
        /// </summary>
        private Permission MapToEntity(PermissionDto permissionDto)
        {
            return new Permission
            {
                Id = permissionDto.Id,
                Name = permissionDto.Name,
                DisplayName = permissionDto.DisplayName
            };
        }
    }
}
