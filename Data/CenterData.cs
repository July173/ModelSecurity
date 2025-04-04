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
    /// Repository encargado de la gestión de la entidad Center en la base de datos.
    /// </summary>
    public class CenterData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public CenterData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los Center almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de Center.</returns>
        public async Task<IEnumerable<Center>> GetAllAsync()
        {
            return await _context.Set<Center>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un Center por su ID.
        /// </summary>
        /// <param name="id">Identificador único del Center.</param>
        /// <returns>El Center con el ID especificado.</returns>
        public async Task<Center?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Center>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener Center con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo Center en la base de datos.
        /// </summary>
        /// <param name="center">Instancia del Center a crear.</param>
        /// <returns>El Center creado.</returns>
        public async Task<Center> CreateAsync(Center center)
        {
            try
            {
                await _context.Set<Center>().AddAsync(center);
                await _context.SaveChangesAsync();
                return center;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Center {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un Center existente en la base de datos.
        /// </summary>
        /// <param name="center">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Center center)
        {
            try
            {
                _context.Set<Center>().Update(center);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Center {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un Center en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del Center a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var center = await _context.Set<Center>().FindAsync(id);
                if (center == null)
                    return false;

                _context.Set<Center>().Remove(center);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Center {ex.Message}");
                return false;
            }
        }
    }
}



