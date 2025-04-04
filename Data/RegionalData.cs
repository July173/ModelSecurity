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
    /// Repository encargado de la gestión de la entidad Regional en la base de datos.
    /// </summary>
    public class RegionalData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public RegionalData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros regionales almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de registros regionales.</returns>
        public async Task<IEnumerable<Regional>> GetAllAsync()
        {
            return await _context.Set<Regional>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un registro regional por su ID.
        /// </summary>
        /// <param name="id">Identificador único del registro regional.</param>
        /// <returns>El registro regional con el ID especificado.</returns>
        public async Task<Regional?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Regional>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener registro regional con ID {id}: {ex.Message}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo registro regional en la base de datos.
        /// </summary>
        /// <param name="regional">Instancia del registro regional a crear.</param>
        /// <returns>El registro regional creado.</returns>
        public async Task<Regional> CreateAsync(Regional regional)
        {
            try
            {
                await _context.Set<Regional>().AddAsync(regional);
                await _context.SaveChangesAsync();
                return regional;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el registro regional: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un registro regional existente en la base de datos.
        /// </summary>
        /// <param name="regional">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Regional regional)
        {
            try
            {
                _context.Set<Regional>().Update(regional);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el registro regional: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un registro regional en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del registro regional a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var regional = await _context.Set<Regional>().FindAsync(id);
                if (regional == null)
                    return false;

                _context.Set<Regional>().Remove(regional);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el registro regional: {ex.Message}");
                return false;
            }
        }
    }
}

