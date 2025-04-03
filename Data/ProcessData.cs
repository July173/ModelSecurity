using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class ProcessData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public ProcessData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Process>> GetAllAsync()
        {
            return await _context.Set<Process>().ToListAsync();
        }

        public async Task<Process?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Process>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener un proceso con ID {id}");
                throw;
            }
        }

        public async Task<Process> CreateAsync(Process process)
        {
            try
            {
                await _context.Set<Process>().AddAsync(process);
                await _context.SaveChangesAsync();
                return process;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el proceso {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Process process)
        {
            try
            {
                _context.Set<Process>().Update(process);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el proceso {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var process = await _context.Set<Process>().FindAsync(id);
                if (process == null)
                    return false;

                _context.Set<Process>().Remove(process);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el proceso {ex.Message}");
                return false;
            }
        }
    }
}
