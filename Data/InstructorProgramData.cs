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
    /// Repository encargado de la gestión de la entidad InstructorProgram en la base de datos.
    /// </summary>
    public class InstructorProgramData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InstructorProgramData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public InstructorProgramData(ApplicationDbContext context, ILogger<InstructorProgramData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los programas de instructores almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de programas de instructores.</returns>
        public async Task<IEnumerable<InstructorProgram>> GetAllAsync()
        {
            return await _context.Set<InstructorProgram>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un programa de instructor por su ID.
        /// </summary>
        /// <param name="id">Identificador único del programa de instructor.</param>
        /// <returns>El programa de instructor con el ID especificado.</returns>
        public async Task<InstructorProgram?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<InstructorProgram>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener programa de instructor con ID {id}: {ex.Message}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo programa de instructor en la base de datos.
        /// </summary>
        /// <param name="instructorProgram">Instancia del programa de instructor a crear.</param>
        /// <returns>El programa de instructor creado.</returns>
        public async Task<InstructorProgram> CreateAsync(InstructorProgram instructorProgram)
        {
            try
            {
                await _context.Set<InstructorProgram>().AddAsync(instructorProgram);
                await _context.SaveChangesAsync();
                return instructorProgram;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el programa de instructor: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un programa de instructor existente en la base de datos.
        /// </summary>
        /// <param name="instructorProgram">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(InstructorProgram instructorProgram)
        {
            try
            {
                _context.Set<InstructorProgram>().Update(instructorProgram);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el programa de instructor: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un programa de instructor en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del programa de instructor a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var instructorProgram = await _context.Set<InstructorProgram>().FindAsync(id);
                if (instructorProgram == null)
                    return false;

                _context.Set<InstructorProgram>().Remove(instructorProgram);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el programa de instructor: {ex.Message}");
                return false;
            }
        }
    }
}
    