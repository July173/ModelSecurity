using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class EnterpriseData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public EnterpriseData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Enterprise>> GetAllAsync()
        {
            return await _context.Set<Enterprise>().ToListAsync();
        }

        public async Task<Enterprise?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Enterprise>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener una empresa con ID {id}");
                throw;
            }
        }

        public async Task<Enterprise> CreateAsync(Enterprise enterprise)
        {
            try
            {
                await _context.Set<Enterprise>().AddAsync(enterprise);
                await _context.SaveChangesAsync();
                return enterprise;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la empresa {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Enterprise enterprise)
        {
            try
            {
                _context.Set<Enterprise>().Update(enterprise);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la empresa {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var enterprise = await _context.Set<Enterprise>().FindAsync(id);
                if (enterprise == null)
                    return false;

                _context.Set<Enterprise>().Remove(enterprise);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la empresa {ex.Message}");
                return false;
            }
        }
    }
}
