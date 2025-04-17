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
    /// Repository encargado de la gestión de la entidad Verification en la base de datos.
    /// </summary>
    public class VerificationData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VerificationData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public VerificationData(ApplicationDbContext context, ILogger<VerificationData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las verificaciones almacenadas en la base de datos.
        /// </summary>
        /// <returns>Lista de verificaciones.</returns>
        public async Task<IEnumerable<Verification>> GetAllAsync()
        {
            return await _context.Set<Verification>().ToListAsync();
        }

        /// <summary>
        /// Obtiene una verificación por su ID.
        /// </summary>
        /// <param name="id">Identificador único de la verificación.</param>
        /// <returns>La verificación con el ID especificado.</returns>
        public async Task<Verification?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Verification>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener verificación con ID {id}: {ex.Message}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea una nueva verificación en la base de datos.
        /// </summary>
        /// <param name="verification">Instancia de la verificación a crear.</param>
        /// <returns>La verificación creada.</returns>
        public async Task<Verification> CreateAsync(Verification verification)
        {
            try
            {
                await _context.Set<Verification>().AddAsync(verification);
                await _context.SaveChangesAsync();
                return verification;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la verificación: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una verificación existente en la base de datos.
        /// </summary>
        /// <param name="verification">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Verification verification)
        {
            try
            {
                _context.Set<Verification>().Update(verification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la verificación: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una verificación en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la verificación a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var verification = await _context.Set<Verification>().FindAsync(id);
                if (verification == null)
                    return false;

                _context.Set<Verification>().Remove(verification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la verificación: {ex.Message}");
                return false;
            }
        }
    }
}




