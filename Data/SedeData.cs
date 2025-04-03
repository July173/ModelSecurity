using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class SedeData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public SedeData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Sede>> GetAllAsync()
        {
            return await _context.Set<Sede>().ToListAsync();
        }

        public async Task<Sede?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Sede>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener una sede con ID {id}");
                throw;
            }
        }

        public async Task<Sede> CreateAsync(Sede sede)
        {
            try
            {
                await _context.Set<Sede>().AddAsync(sede);
                await _context.SaveChangesAsync();
                return sede;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la sede {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Sede sede)
        {
            try
            {
                _context.Set<Sede>().Update(sede);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la sede {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var sede = await _context.Set<Sede>().FindAsync(id);
                if (sede == null)
                    return false;

                _context.Set<Sede>().Remove(sede);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la sede {ex.Message}");
                return false;
            }
        }
    }
}
