using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class CenterData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public CenterData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Center>> GetAllAsync()
        {
            return await _context.Set<Center>().ToListAsync();
        }

        public async Task<Center?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Center>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener un Center con ID {id}");
                throw;
            }
        }

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
