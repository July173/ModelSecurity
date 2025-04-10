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
    /// Repository encargado de la gestión de la entidad Instructor en la base de datos.
    /// </summary>
    public class InstructorData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InstructorData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public InstructorData(ApplicationDbContext context, ILogger<InstructorData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los instructores almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de instructores.</returns>
        public async Task<IEnumerable<Instructor>> GetAllAsync()
        {
            return await _context.Set<Instructor>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un instructor por su ID.
        /// </summary>
        /// <param name="id">Identificador único del instructor.</param>
        /// <returns>El instructor con el ID especificado.</returns>
        public async Task<Instructor?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Instructor>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener instructor con ID {id}: {ex.Message}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo instructor en la base de datos.
        /// </summary>
        /// <param name="instructor">Instancia del instructor a crear.</param>
        /// <returns>El instructor creado.</returns>
        public async Task<Instructor> CreateAsync(Instructor instructor)
        {
            try
            {
                await _context.Set<Instructor>().AddAsync(instructor);
                await _context.SaveChangesAsync();
                return instructor;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el instructor: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un instructor existente en la base de datos.
        /// </summary>
        /// <param name="instructor">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Instructor instructor)
        {
            try
            {
                _context.Set<Instructor>().Update(instructor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el instructor: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un instructor en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del instructor a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var instructor = await _context.Set<Instructor>().FindAsync(id);
                if (instructor == null)
                    return false;

                _context.Set<Instructor>().Remove(instructor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el instructor: {ex.Message}");
                return false;
            }
        }
    }
}
