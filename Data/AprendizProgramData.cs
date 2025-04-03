using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class AprendizProgramData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public AprendizProgramData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<AprendizProgram>> GetAllAsync()
        {
            return await _context.Set<AprendizProgram>().ToListAsync();
        }

        public async Task<AprendizProgram?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<AprendizProgram>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener un Aprendiz-Programa con ID {id}");
                throw;
            }
        }

        public async Task<AprendizProgram> CreateAsync(AprendizProgram aprendizProgram)
        {
            try
            {
                await _context.Set<AprendizProgram>().AddAsync(aprendizProgram);
                await _context.SaveChangesAsync();
                return aprendizProgram;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Aprendiz-Programa {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(AprendizProgram aprendizProgram)
        {
            try
            {
                _context.Set<AprendizProgram>().Update(aprendizProgram);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Aprendiz-Programa {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var aprendizProgram = await _context.Set<AprendizProgram>().FindAsync(id);
                if (aprendizProgram == null)
                    return false;

                _context.Set<AprendizProgram>().Remove(aprendizProgram);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Aprendiz-Programa {ex.Message}");
                return false;
            }
        }
    }
}
