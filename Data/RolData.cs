using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    //Repository encargado de la gestion de la entidad de tol en la base de base de datos 
    class RolData
	{
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public RolData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task <IEnumerable<Rol>>GetAllAsync()
        {
            return await _context.Set<Rol>().ToListAsync();
        }

        public async Task <Rol?>GetByidAsync(int id)
        {
            try
            {
                return await _context.Set<Rol>().FindAsync(id);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol con ID{Rolid}");
                throw;
            }
       
        }
        public async Task<Rol> CreateAsync(Rol rol)
        {
            try
            { 
                await _context.Set<Rol>.AddAsync(rol);
                await _context.SaveChangesAsync();
                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el rol {ex.Message}");
                throw;
            }

            public async Task<bool> UpdateAsync(Rol rol)
        {
            try
            {
                _context.Set<Rol>().Update(rol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol {ex.Message}");
                return false;
            }



            public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rol = await _context.Set<Rol>().FindAsync(id);
                if (rol == null)
                    return false;

                _context.Set<Rol>().Remove(rol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el rol {ex.Message}");
                return false;
            }
        }
}