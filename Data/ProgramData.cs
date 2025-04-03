using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class ProgramData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public ProgramData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Program>> GetAllAsync()
        {
            return await _context.Set<Program>().ToListAsync();
        }

        public async Task<Program?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Program>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener un programa con ID {id}");
                throw;
            }
        }

        public async Task<Program> CreateAsync(Program program)
        {
            try
            {
                await _context.Set<Program>().AddAsync(program);
                await _context.SaveChangesAsync();
                return program;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el programa {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Program program)
        {
            try
            {
                _context.Set<Program>().Update(program);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el programa {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var program = await _context.Set<Program>().FindAsync(id);
                if (program == null)
                    return false;

                _context.Set<Program>().Remove(program);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el programa {ex.Message}");
                return false;
            }
        }
    }
}
