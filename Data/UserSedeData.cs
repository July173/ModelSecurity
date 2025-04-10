using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repository encargado de la gestión de la entidad UserSede en la base de datos.
    /// </summary>
    public class UserSedeData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserSedeData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public UserSedeData(ApplicationDbContext context, ILogger<UserSedeData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de UserSede almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de registros de UserSede.</returns>
        public async Task<IEnumerable<UserSede>> GetAllAsync()
        {
            return await _context.Set<UserSede>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un registro de UserSede por su ID.
        /// </summary>
        /// <param name="id">Identificador único del registro de UserSede.</param>
        /// <returns>El registro de UserSede con el ID especificado.</returns>
        public async Task<UserSede?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<UserSede>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener registro de UserSede con ID {id}: {ex.Message}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo registro de UserSede en la base de datos.
        /// </summary>
        /// <param name="userSede">Instancia del registro de UserSede a crear.</param>
        /// <returns>El registro de UserSede creado.</returns>
        public async Task<UserSede> CreateAsync(UserSede userSede)
        {
            try
            {
                await _context.Set<UserSede>().AddAsync(userSede);
                await _context.SaveChangesAsync();
                return userSede;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el registro de UserSede: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un registro de UserSede existente en la base de datos.
        /// </summary>
        /// <param name="userSede">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(UserSede userSede)
        {
            try
            {
                _context.Set<UserSede>().Update(userSede);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el registro de UserSede: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un registro de UserSede en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del registro de UserSede a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var userSede = await _context.Set<UserSede>().FindAsync(id);
                if (userSede == null)
                    return false;

                _context.Set<UserSede>().Remove(userSede);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el registro de UserSede: {ex.Message}");
                return false;
            }
        }
    }
}




