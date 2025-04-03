using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class AprendizProcessInstructorData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public AprendizProcessInstructorData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<AprendizProcessInstructor>> GetAllAsync()
        {
            return await _context.Set<AprendizProcessInstructor>().ToListAsync();
        }

        public async Task<AprendizProcessInstructor?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<AprendizProcessInstructor>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener un Aprendiz-Process-Instructor con ID {id}");
                throw;
            }
        }

        public async Task<AprendizProcessInstructor> CreateAsync(AprendizProcessInstructor aprendizProcessInstructor)
        {
            try
            {
                await _context.Set<AprendizProcessInstructor>().AddAsync(aprendizProcessInstructor);
                await _context.SaveChangesAsync();
                return aprendizProcessInstructor;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Aprendiz-Process-Instructor {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(AprendizProcessInstructor aprendizProcessInstructor)
        {
            try
            {
                _context.Set<AprendizProcessInstructor>().Update(aprendizProcessInstructor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Aprendiz-Process-Instructor {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var aprendizProcessInstructor = await _context.Set<AprendizProcessInstructor>().FindAsync(id);
                if (aprendizProcessInstructor == null)
                    return false;

                _context.Set<AprendizProcessInstructor>().Remove(aprendizProcessInstructor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Aprendiz-Process-Instructor {ex.Message}");
                return false;
            }
        }
    }
}
