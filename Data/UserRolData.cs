using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class UserRolData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRolData>_logger;

        public UserRolData(ApplicationDbContext context, ILogger<UserRolData> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<UserRol>> GetAllAsync()
        {
            return await _context.Set<UserRol>().ToListAsync();
        }

        public async Task<UserRol?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<UserRol>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener Rol-Usuario con ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo userRol  en la base de datos 
        /// </summary>
        /// <param name="userRol">instancia del userRol a crear.</param>
        /// <returns>el userRol creado</returns>
        public async Task<UserRol> CreateAsync(UserRol rolUser)
        {
            try
            {
                await _context.Set<UserRol>().AddAsync(rolUser);
                await _context.SaveChangesAsync();
                return rolUser;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la relación Rol-Usuario {ex.Message}");
                throw;
            }
        }





        /// <summary>
        /// Elimina un userRol permanente en la base de datos 
        /// </summary>
        /// <param name="id">Identificador unico del userRol a eliminar</param>
        /// <returns>True si la eliminacion fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rolUser = await _context.Set<UserRol>().FindAsync(id);
                if (rolUser == null)
                    return false;

                _context.Set<UserRol>().Remove(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la relación Rol-Usuario {ex.Message}");
                return false;
            }
        }
    }
}
