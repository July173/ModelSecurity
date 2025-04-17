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
    /// Repository encargado de la gestión de la entidad Concept en la base de datos.
    /// </summary>
    public class ConceptData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ConceptData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public ConceptData(ApplicationDbContext context, ILogger<ConceptData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los Concept almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de Concept.</returns>
        public async Task<IEnumerable<Concept>> GetAllAsync()
        {
            return await _context.Set<Concept>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un Concept por su ID.
        /// </summary>
        /// <param name="id">Identificador único del Concept.</param>
        /// <returns>El Concept con el ID especificado.</returns>
        public async Task<Concept?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Concept>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener Concept con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo Concept en la base de datos.
        /// </summary>
        /// <param name="concept">Instancia del Concept a crear.</param>
        /// <returns>El Concept creado.</returns>
        public async Task<Concept> CreateAsync(Concept concept)
        {
            try
            {
                await _context.Set<Concept>().AddAsync(concept);
                await _context.SaveChangesAsync();
                return concept;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Concept {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un Concept existente en la base de datos.
        /// </summary>
        /// <param name="concept">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Concept concept)
        {
            try
            {
                _context.Set<Concept>().Update(concept);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Concept {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un Concept en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del Concept a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var concept = await _context.Set<Concept>().FindAsync(id);
                if (concept == null)
                    return false;

                _context.Set<Concept>().Remove(concept);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Concept {ex.Message}");
                return false;
            }
        }
    }
}


