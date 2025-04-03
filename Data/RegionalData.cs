using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class RegionalData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public RegionalData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Regional>> GetAllAsync()
        {
            return await _context.Set<Regional>().ToListAsync();
        }

        public async Task<Regional?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Regional>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener una regional con ID {id}");
                throw;
            }
        }

        public async Task<Regional> CreateAsync(Regional regional)
        {
            try
            {
                await _context.Set<Regional>().AddAsync(regional);
                await _context.SaveChangesAsync();
                return regional;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la regional {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Regional regional)
        {
            try
            {
                _context.Set<Regional>().Update(regional);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la regional {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var regional = await _context.Set<Regional>().FindAsync(id);
                if (regional == null)
                    return false;

                _context.Set<Regional>().Remove(regional);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la regional {ex.Message}");
                return false;
            }
        }
    }
}
