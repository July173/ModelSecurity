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
    /// Repository encargado de la gestión de la entidad AprendizProgram en la base de datos.
    /// </summary>
    public class AprendizProgramData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public AprendizProgramData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los AprendizProgram almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de AprendizProgram.</returns>
        public async Task<IEnumerable<AprendizProgram>> GetAllAsync()
        {
            return await _context.Set<AprendizProgram>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un AprendizProgram por su ID.
        /// </summary>
        /// <param name="id">Identificador único del AprendizProgram.</param>
        /// <returns>El AprendizProgram con el ID especificado.</returns>
        public async Task<AprendizProgram?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<AprendizProgram>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener AprendizProgram con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo AprendizProgram en la base de datos.
        /// </summary>
        /// <param name="aprendizProgram">Instancia del AprendizProgram a crear.</param>
        /// <returns>El AprendizProgram creado.</returns>
        public async Task<AprendizProgram> CreateAsync(AprendizProgram aprendizProgram)
        {
            try
            {
                await _context.Set<AprendizProgram>().AddAsync(aprendizProgram);
                await _context.SaveChangesAsync();
                return aprendizProgram;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el AprendizProgram {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un AprendizProgram existente en la base de datos.
        /// </summary>
        /// <param name="aprendizProgram">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(AprendizProgram aprendizProgram)
        {
            try
            {
                _context.Set<AprendizProgram>().Update(aprendizProgram);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el AprendizProgram {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un AprendizProgram en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del AprendizProgram a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var aprendizProgram = await _context.Set<AprendizProgram>().FindAsync(id);
                if (aprendizProgram == null)
                    return false;

                _context.Set<AprendizProgram>().Remove(aprendizProgram);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el AprendizProgram {ex.Message}");
                return false;
            }
        }
    }
}


