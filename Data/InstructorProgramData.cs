using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class InstructorProgramData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public InstructorProgramData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<InstructorProgram>> GetAllAsync()
        {
            return await _context.Set<InstructorProgram>().ToListAsync();
        }

        public async Task<InstructorProgram?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<InstructorProgram>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener un Instructor Programa con ID {id}");
                throw;
            }
        }

        public async Task<InstructorProgram> CreateAsync(InstructorProgram instructorProgram)
        {
            try
            {
                await _context.Set<InstructorProgram>().AddAsync(instructorProgram);
                await _context.SaveChangesAsync();
                return instructorProgram;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Instructor Programa {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(InstructorProgram instructorProgram)
        {
            try
            {
                _context.Set<InstructorProgram>().Update(instructorProgram);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Instructor Programa {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var instructorProgram = await _context.Set<InstructorProgram>().FindAsync(id);
                if (instructorProgram == null)
                    return false;

                _context.Set<InstructorProgram>().Remove(instructorProgram);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Instructor Programa {ex.Message}");
                return false;
            }
        }
    }
}
