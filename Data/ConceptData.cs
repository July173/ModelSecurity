using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class ConceptData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public ConceptData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Concept>> GetAllAsync()
        {
            return await _context.Set<Concept>().ToListAsync();
        }

        public async Task<Concept?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Concept>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener un concepto con ID {id}");
                throw;
            }
        }

        public async Task<Concept> CreateAsync(Concept concept)
        {
            try
            {
                await _context.Set<Concept>().AddAsync(concept);
                await _context.SaveChangesAsync();
                return concept;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Concepto {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Concept concept)
        {
            try
            {
                _context.Set<Concept>().Update(concept);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Concepto {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var concept = await _context.Set<Concept>().FindAsync(id);
                if (concept == null)
                    return false;

                _context.Set<Concept>().Remove(concept);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Concepto {ex.Message}");
                return false;
            }
        }
    }
}
