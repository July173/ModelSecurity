using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class StateData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public StateData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<State>> GetAllAsync()
        {
            return await _context.Set<State>().ToListAsync();
        }

        public async Task<State?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<State>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener un stado con ID {id}");
                throw;
            }
        }

        public async Task<State> CreateAsync(State state)
        {
            try
            {
                await _context.Set<State>().AddAsync(state);
                await _context.SaveChangesAsync();
                return state;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el stade {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(State state)
        {
            try
            {
                _context.Set<State>().Update(state);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el stade {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var state = await _context.Set<State>().FindAsync(id);
                if (state == null)
                    return false;

                _context.Set<State>().Remove(state);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el stade {ex.Message}");
                return false;
            }
        }
    }
}
