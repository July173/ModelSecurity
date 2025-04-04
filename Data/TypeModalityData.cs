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
    /// Repository encargado de la gestión de la entidad TypeModality en la base de datos.
    /// </summary>
    public class TypeModalityData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public TypeModalityData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las modalidades almacenadas en la base de datos.
        /// </summary>
        /// <returns>Lista de modalidades.</returns>
        public async Task<IEnumerable<TypeModality>> GetAllAsync()
        {
            return await _context.Set<TypeModality>().ToListAsync();
        }

        /// <summary>
        /// Obtiene una modalidad por su ID.
        /// </summary>
        /// <param name="id">Identificador único de la modalidad.</param>
        /// <returns>La modalidad con el ID especificado.</returns>
        public async Task<TypeModality?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<TypeModality>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener modalidad con ID {id}: {ex.Message}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea una nueva modalidad en la base de datos.
        /// </summary>
        /// <param name="typeModality">Instancia de la modalidad a crear.</param>
        /// <returns>La modalidad creada.</returns>
        public async Task<TypeModality> CreateAsync(TypeModality typeModality)
        {
            try
            {
                await _context.Set<TypeModality>().AddAsync(typeModality);
                await _context.SaveChangesAsync();
                return typeModality;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la modalidad: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una modalidad existente en la base de datos.
        /// </summary>
        /// <param name="typeModality">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(TypeModality typeModality)
        {
            try
            {
                _context.Set<TypeModality>().Update(typeModality);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la modalidad: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una modalidad en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la modalidad a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var typeModality = await _context.Set<TypeModality>().FindAsync(id);
                if (typeModality == null)
                    return false;

                _context.Set<TypeModality>().Remove(typeModality);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la modalidad: {ex.Message}");
                return false;
            }
        }
    }
}



