using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class RegisterySofiaData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public RegisterySofiaData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<RegisterySofia>> GetAllAsync()
        {
            return await _context.Set<RegisterySofia>().ToListAsync();
        }

        public async Task<RegisterySofia?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<RegisterySofia>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener un registro de sofia con ID {id}");
                throw;
            }
        }

        public async Task<RegisterySofia> CreateAsync(RegisterySofia registerySofia)
        {
            try
            {
                await _context.Set<RegisterySofia>().AddAsync(registerySofia);
                await _context.SaveChangesAsync();
                return registerySofia;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el registro de sofia {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(RegisterySofia registerySofia)
        {
            try
            {
                _context.Set<RegisterySofia>().Update(registerySofia);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el registro de sofia {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var registerySofia = await _context.Set<RegisterySofia>().FindAsync(id);
                if (registerySofia == null)
                    return false;

                _context.Set<RegisterySofia>().Remove(registerySofia);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el registro de sofia  {ex.Message}");
                return false;
            }
        }
    }
}
