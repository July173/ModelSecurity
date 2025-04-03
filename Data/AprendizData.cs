using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class AprendizData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public AprendizData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Aprendiz>> GetAllAsync()
        {
            return await _context.Set<Aprendiz>().ToListAsync();
        }

        public async Task<Aprendiz?> GetByidAsync(int id)
        {
            try
            {
                return await _context.Set<Aprendiz>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener aprendiz con ID{id}");
                throw;
            }
        }

        public async Task<Aprendiz> CreateAsync(Aprendiz aprendiz)
        {
            try
            {
                await _context.Set<Aprendiz>().AddAsync(aprendiz);
                await _context.SaveChangesAsync();
                return aprendiz;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el aprendiz {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Aprendiz aprendiz)
        {
            try
            {
                _context.Set<Aprendiz>().Update(aprendiz);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el aprendiz {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var aprendiz = await _context.Set<Aprendiz>().FindAsync(id);
                if (aprendiz == null)
                    return false;

                _context.Set<Aprendiz>().Remove(aprendiz);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el aprendiz {ex.Message}");
                return false;
            }
        }
    }
}
