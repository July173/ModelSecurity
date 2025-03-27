using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    class ChangeLogData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public ChangeLogData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ChangeLog>> GetAllAsync()
        {
            return await _context.Set<ChangeLog>().ToListAsync();
        }

        public async Task<ChangeLog?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<ChangeLog>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ChangeLog con ID {ChangeLogId}");
                throw;
            }
        }

        public async Task<ChangeLog> CreateAsync(ChangeLog changeLog)
        {
            try
            {
                await _context.Set<ChangeLog>().AddAsync(changeLog);
                await _context.SaveChangesAsync();
                return changeLog;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el ChangeLog {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var changeLog = await _context.Set<ChangeLog>().FindAsync(id);
                if (changeLog == null)
                    return false;

                _context.Set<ChangeLog>().Remove(changeLog);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el ChangeLog {ex.Message}");
                return false;
            }
        }
    }
}
