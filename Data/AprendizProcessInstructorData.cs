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
    /// Repository encargado de la gestión de la entidad AprendizProcessInstructor en la base de datos.
    /// </summary>
    public class AprendizProcessInstructorData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AprendizProcessInstructorData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public AprendizProcessInstructorData(ApplicationDbContext context, ILogger<AprendizProcessInstructorData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los AprendizProcessInstructor almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de AprendizProcessInstructor.</returns>
        public async Task<IEnumerable<AprendizProcessInstructor>> GetAllAsync()
        {
            return await _context.Set<AprendizProcessInstructor>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un AprendizProcessInstructor por su ID.
        /// </summary>
        /// <param name="id">Identificador único del AprendizProcessInstructor.</param>
        /// <returns>El AprendizProcessInstructor con el ID especificado.</returns>
        public async Task<AprendizProcessInstructor?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<AprendizProcessInstructor>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener AprendizProcessInstructor con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo AprendizProcessInstructor en la base de datos.
        /// </summary>
        /// <param name="aprendizProcessInstructor">Instancia del AprendizProcessInstructor a crear.</param>
        /// <returns>El AprendizProcessInstructor creado.</returns>
        public async Task<AprendizProcessInstructor> CreateAsync(AprendizProcessInstructor aprendizProcessInstructor)
        {
            try
            {
                await _context.Set<AprendizProcessInstructor>().AddAsync(aprendizProcessInstructor);
                await _context.SaveChangesAsync();
                return aprendizProcessInstructor;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el AprendizProcessInstructor {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un AprendizProcessInstructor existente en la base de datos.
        /// </summary>
        /// <param name="aprendizProcessInstructor">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(AprendizProcessInstructor aprendizProcessInstructor)
        {
            try
            {
                _context.Set<AprendizProcessInstructor>().Update(aprendizProcessInstructor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el AprendizProcessInstructor {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un AprendizProcessInstructor en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del AprendizProcessInstructor a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var aprendizProcessInstructor = await _context.Set<AprendizProcessInstructor>().FindAsync(id);
                if (aprendizProcessInstructor == null)
                    return false;

                _context.Set<AprendizProcessInstructor>().Remove(aprendizProcessInstructor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el AprendizProcessInstructor {ex.Message}");
                return false;
            }
        }
    }
}


