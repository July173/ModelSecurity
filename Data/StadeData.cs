using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class StadeData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public StadeData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Stade>> GetAllAsync()
        {
            return await _context.Set<Stade>().ToListAsync();
        }

        public async Task<Stade?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Stade>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener un stade con ID {id}");
                throw;
            }
        }

        public async Task<Stade> CreateAsync(Stade stade)
        {
            try
            {
                await _context.Set<Stade>().AddAsync(stade);
                await _context.SaveChangesAsync();
                return stade;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el stade {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Stade stade)
        {
            try
            {
                _context.Set<Stade>().Update(stade);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el stade {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var stade = await _context.Set<Stade>().FindAsync(id);
                if (stade == null)
                    return false;

                _context.Set<Stade>().Remove(stade);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el stade {ex.Message}");
                return false;
            }
        }
    }
}
