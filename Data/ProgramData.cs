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
    /// Repository encargado de la gestión de la entidad Program en la base de datos.
    /// </summary>
    public class ProgramData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProgramData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public ProgramData(ApplicationDbContext context, ILogger<ProgramData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los programas almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de programas.</returns>
        public async Task<IEnumerable<Program>> GetAllAsync()
        {
            return await _context.Set<Program>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un programa por su ID.
        /// </summary>
        /// <param name="id">Identificador único del programa.</param>
        /// <returns>El programa con el ID especificado.</returns>
        public async Task<Program?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Program>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener programa con ID {id}: {ex.Message}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo programa en la base de datos.
        /// </summary>
        /// <param name="program">Instancia del programa a crear.</param>
        /// <returns>El programa creado.</returns>
        public async Task<Program> CreateAsync(Program program)
        {
            try
            {
                await _context.Set<Program>().AddAsync(program);
                await _context.SaveChangesAsync();
                return program;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el programa: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un programa existente en la base de datos.
        /// </summary>
        /// <param name="program">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Program program)
        {
            try
            {
                _context.Set<Program>().Update(program);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el programa: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un programa en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del programa a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var program = await _context.Set<Program>().FindAsync(id);
                if (program == null)
                    return false;

                _context.Set<Program>().Remove(program);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el programa: {ex.Message}");
                return false;
            }
        }
    }
}

