using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class InstructorData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public InstructorData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Instructor>> GetAllAsync()
        {
            return await _context.Set<Instructor>().ToListAsync();
        }

        public async Task<Instructor?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Instructor>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener un Instructor con ID {id}");
                throw;
            }
        }

        public async Task<Instructor> CreateAsync(Instructor instructor)
        {
            try
            {
                await _context.Set<Instructor>().AddAsync(instructor);
                await _context.SaveChangesAsync();
                return instructor;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Instructor {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Instructor instructor)
        {
            try
            {
                _context.Set<Instructor>().Update(instructor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Instructor {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var instructor = await _context.Set<Instructor>().FindAsync(id);
                if (instructor == null)
                    return false;

                _context.Set<Instructor>().Remove(instructor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Instructor {ex.Message}");
                return false;
            }
        }
    }
}
