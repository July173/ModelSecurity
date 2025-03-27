using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    class RolUserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public RolUserData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<RolUser>> GetAllAsync()
        {
            return await _context.Set<RolUser>().ToListAsync();
        }

        public async Task<RolUser?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<RolUser>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Rol-Usuario con ID {RolUserId}");
                throw;
            }
        }

        public async Task<RolUser> CreateAsync(RolUser rolUser)
        {
            try
            {
                await _context.Set<RolUser>().AddAsync(rolUser);
                await _context.SaveChangesAsync();
                return rolUser;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la relación Rol-Usuario {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(RolUser rolUser)
        {
            try
            {
                _context.Set<RolUser>().Update(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la relación Rol-Usuario {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rolUser = await _context.Set<RolUser>().FindAsync(id);
                if (rolUser == null)
                    return false;

                _context.Set<RolUser>().Remove(rolUser);
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
