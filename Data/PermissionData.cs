using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad Permission en la base de datos.
    /// </summary>
    public class PermissionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PermissionData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{PermissionData}"/> para el registro de logs.</param>
        public PermissionData(ApplicationDbContext context, ILogger<PermissionData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de permisos.</returns>
        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Permission>().ToListAsync();
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
        /// <returns>El permiso si existe, o null si no se encuentra.</returns>
        public async Task<Permission?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Permission>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el permiso con ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo permiso en la base de datos.
        /// </summary>
        /// <param name="permission">Instancia del permiso a crear.</param>
        /// <returns>El permiso creado.</returns>
        public async Task<Permission> CreateAsync(Permission permission)
        {
            try
            {
                await _context.Set<Permission>().AddAsync(permission);
                await _context.SaveChangesAsync();
                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el permiso: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un permiso existente en la base de datos.
        /// </summary>
        /// <param name="permission">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Permission permission)
        {
            try
            {
                _context.Set<Permission>().Update(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el permiso: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un permiso de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del permiso a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var permission = await _context.Set<Permission>().FindAsync(id);
                if (permission == null)
                    return false;

                _context.Set<Permission>().Remove(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el permiso con ID {id}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Actualiza parcialmente un permiso existente en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del permiso a actualizar.</param>
        /// <param name="updatedFields">Diccionario con los campos y valores a actualizar.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdatePartialAsync(int id, Dictionary<string, object> updatedFields)
        {
            try
            {
                // Buscar el permiso existente
                var permission = await _context.Set<Permission>().FindAsync(id);
                if (permission == null)
                {
                    _logger.LogWarning($"No se encontró el permiso con ID {id} para actualización parcial.");
                    return false;
                }

                // Obtener la entrada de seguimiento de EF Core
                var entry = _context.Entry(permission);

                // Actualizar solo los campos proporcionados y válidos
                foreach (var field in updatedFields)
                {
                    // Verificar si la propiedad existe en la entidad
                    if (entry.Property(field.Key) == null)
                    {
                        _logger.LogWarning($"La propiedad '{field.Key}' no existe en la entidad Permission.");
                        continue; // Ignorar propiedades no válidas
                    }

                    // Verificar si el valor es nulo o vacío
                    if (field.Value == null || (field.Value is string strValue && string.IsNullOrWhiteSpace(strValue)))
                    {
                        _logger.LogInformation($"El valor para la propiedad '{field.Key}' es nulo o vacío. Se conservará el valor actual.");
                        continue; // Ignorar valores nulos o vacíos
                    }

                    // Actualizar el valor de la propiedad
                    entry.Property(field.Key).CurrentValue = field.Value;
                    entry.Property(field.Key).IsModified = true;
                }

                // Guardar los cambios
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar parcialmente el permiso con ID {id}: {ex.Message}");
                throw; // Re-lanzar la excepción para que sea manejada en capas superiores
            }
        }
    }
}
