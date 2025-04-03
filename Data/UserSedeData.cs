using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class UserSedeData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public UserSedeData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<UserSede>> GetAllAsync()
        {
            return await _context.Set<UserSede>().ToListAsync();
        }

        public async Task<UserSede?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<UserSede>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener usuario-sede con ID {id}");
                throw;
            }
        }

        public async Task<UserSede> CreateAsync(UserSede userSede)
        {
            try
            {
                await _context.Set<UserSede>().AddAsync(userSede);
                await _context.SaveChangesAsync();
                return userSede;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la relación usuario-sede {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(UserSede userSede)
        {
            try
            {
                _context.Set<UserSede>().Update(userSede);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la relación usuario-sede {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var userSede = await _context.Set<UserSede>().FindAsync(id);
                if (userSede == null)
                    return false;

                _context.Set<UserSede>().Remove(userSede);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la relación usuario-sede {ex.Message}");
                return false;
            }
        }
    }
}
