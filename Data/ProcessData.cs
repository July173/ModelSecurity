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
    /// Repository encargado de la gestión de la entidad Process en la base de datos.
    /// </summary>
    public class ProcessData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public ProcessData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los procesos almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de procesos.</returns>
        public async Task<IEnumerable<Process>> GetAllAsync()
        {
            return await _context.Set<Process>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un proceso por su ID.
        /// </summary>
        /// <param name="id">Identificador único del proceso.</param>
        /// <returns>El proceso con el ID especificado.</returns>
        public async Task<Process?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Process>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener proceso con ID {id}: {ex.Message}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo proceso en la base de datos.
        /// </summary>
        /// <param name="process">Instancia del proceso a crear.</param>
        /// <returns>El proceso creado.</returns>
        public async Task<Process> CreateAsync(Process process)
        {
            try
            {
                await _context.Set<Process>().AddAsync(process);
                await _context.SaveChangesAsync();
                return process;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el proceso: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un proceso existente en la base de datos.
        /// </summary>
        /// <param name="process">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Process process)
        {
            try
            {
                _context.Set<Process>().Update(process);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el proceso: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un proceso en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del proceso a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var process = await _context.Set<Process>().FindAsync(id);
                if (process == null)
                    return false;

                _context.Set<Process>().Remove(process);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el proceso: {ex.Message}");
                return false;
            }
        }
    }
}
