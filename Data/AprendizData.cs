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
    /// Repository encargado de la gestión de la entidad Aprendiz en la base de datos.
    /// </summary>
    public class AprendizData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public AprendizData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los aprendices almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de aprendices.</returns>
        public async Task<IEnumerable<Aprendiz>> GetAllAsync()
        {
            return await _context.Set<Aprendiz>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un aprendiz por su ID.
        /// </summary>
        /// <param name="id">Identificador único del aprendiz.</param>
        /// <returns>El aprendiz con el ID especificado.</returns>
        public async Task<Aprendiz?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Aprendiz>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener Aprendiz con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo aprendiz en la base de datos.
        /// </summary>
        /// <param name="aprendiz">Instancia del aprendiz a crear.</param>
        /// <returns>El aprendiz creado.</returns>
        public async Task<Aprendiz> CreateAsync(Aprendiz aprendiz)
        {
            try
            {
                await _context.Set<Aprendiz>().AddAsync(aprendiz);
                await _context.SaveChangesAsync();
                return aprendiz;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Aprendiz {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un aprendiz existente en la base de datos.
        /// </summary>
        /// <param name="aprendiz">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Aprendiz aprendiz)
        {
            try
            {
                _context.Set<Aprendiz>().Update(aprendiz);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Aprendiz {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un aprendiz en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del aprendiz a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var aprendiz = await _context.Set<Aprendiz>().FindAsync(id);
                if (aprendiz == null)
                    return false;

                _context.Set<Aprendiz>().Remove(aprendiz);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Aprendiz {ex.Message}");
                return false;
            }
        }
    }
}
