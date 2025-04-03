using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class VerificationData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public VerificationData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Verification>> GetAllAsync()
        {
            return await _context.Set<Verification>().ToListAsync();
        }

        public async Task<Verification?> GetByidAsync(int id)
        {
            try
            {
                return await _context.Set<Verification>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener verificacion con ID{id}");
                throw;
            }
        }

        public async Task<Verification> CreateAsync(Verification verification)
        {
            try
            {
                await _context.Set<Verification>().AddAsync(verification);
                await _context.SaveChangesAsync();
                return verification;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la verificacion {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Verification verification)
        {
            try
            {
                _context.Set<Verification>().Update(verification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la verificacion {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var verification = await _context.Set<Verification>().FindAsync(id);
                if (verification == null)
                    return false;

                _context.Set<Verification>().Remove(verification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la verificacion {ex.Message}");
                return false;
            }
        }
    }
}

