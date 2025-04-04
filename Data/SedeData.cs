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
    /// Repository encargado de la gestión de la entidad Sede en la base de datos.
    /// </summary>
    public class SedeData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public SedeData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las sedes almacenadas en la base de datos.
        /// </summary>
        /// <returns>Lista de sedes.</returns>
        public async Task<IEnumerable<Sede>> GetAllAsync()
        {
            return await _context.Set<Sede>().ToListAsync();
        }

        /// <summary>
        /// Obtiene una sede por su ID.
        /// </summary>
        /// <param name="id">Identificador único de la sede.</param>
        /// <returns>La sede con el ID especificado.</returns>
        public async Task<Sede?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Sede>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener sede con ID {id}: {ex.Message}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea una nueva sede en la base de datos.
        /// </summary>
        /// <param name="sede">Instancia de la sede a crear.</param>
        /// <returns>La sede creada.</returns>
        public async Task<Sede> CreateAsync(Sede sede)
        {
            try
            {
                await _context.Set<Sede>().AddAsync(sede);
                await _context.SaveChangesAsync();
                return sede;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la sede: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una sede existente en la base de datos.
        /// </summary>
        /// <param name="sede">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Sede sede)
        {
            try
            {
                _context.Set<Sede>().Update(sede);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la sede: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una sede en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la sede a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var sede = await _context.Set<Sede>().FindAsync(id);
                if (sede == null)
                    return false;

                _context.Set<Sede>().Remove(sede);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la sede: {ex.Message}");
                return false;
            }
        }
    }
}


