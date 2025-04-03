using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class TypeModalityData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public TypeModalityData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<TypeModality>> GetAllAsync()
        {
            return await _context.Set<TypeModality>().ToListAsync();
        }

        public async Task<TypeModality?> GetByidAsync(int id)
        {
            try
            {
                return await _context.Set<TypeModality>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener un tipo de modalidad con ID{id}");
                throw;
            }
        }

        public async Task<TypeModality> CreateAsync(TypeModality typeModality)
        {
            try
            {
                await _context.Set<TypeModality>().AddAsync(typeModality);
                await _context.SaveChangesAsync();
                return typeModality;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el tipo de modalidad {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(TypeModality typeModality)
        {
            try
            {
                _context.Set<TypeModality>().Update(typeModality);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el tipo de modalidad {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var typeModality = await _context.Set<TypeModality>().FindAsync(id);
                if (typeModality == null)
                    return false;

                _context.Set<TypeModality>().Remove(typeModality);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el tipo de modalidad {ex.Message}");
                return false;
            }
        }
    }
}

