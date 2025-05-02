using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class UserRolData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRolData> _logger;

        public UserRolData(ApplicationDbContext context, ILogger<UserRolData> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<UserRol>> GetAllAsync()
        {
            return await _context.Set<UserRol>().ToListAsync();
        }

        public async Task<UserRol?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<UserRol>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener Rol-Usuario con ID {id}");
                throw;
            }
        }

        public async Task<UserRol> CreateAsync(UserRol rolUser)
        {
            try
            {
                await _context.Set<UserRol>().AddAsync(rolUser);
                await _context.SaveChangesAsync();
                return rolUser;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la relación Rol-Usuario {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(UserRol rolUser)
        {
            try
            {
                _context.Set<UserRol>().Update(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la relación Rol-Usuario {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rolUser = await _context.Set<UserRol>().FindAsync(id);
                if (rolUser == null)
                    return false;

                _context.Set<UserRol>().Remove(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la relación Rol-Usuario {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Actualiza parcialmente una relación existente en la base de datos.
        /// </summary>
        /// <param name="id">ID de la relación a actualizar.</param>
        /// <param name="updatedFields">Campos a actualizar.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdatePartialAsync(int id, Dictionary<string, object> updatedFields)
        {
            try
            {
                var rolUser = await _context.Set<UserRol>().FindAsync(id);
                if (rolUser == null)
                    return false;

                foreach (var field in updatedFields)
                {
                    var property = rolUser.GetType().GetProperty(field.Key);
                    if (property != null)
                    {
                        property.SetValue(rolUser, field.Value);
                    }
                }

                _context.Set<UserRol>().Update(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar parcialmente la relación rol-usuario {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AssignRolesAsync(int userId, List<int> rolIds)
        {
            var existing = await _context.UserRol
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            _context.UserRol.RemoveRange(existing);

            var newRoles = rolIds.Select(rolId => new UserRol
            {
                UserId = userId,
                RolId = rolId
            });

            await _context.UserRol.AddRangeAsync(newRoles);
            await _context.SaveChangesAsync();
            return true;
        }




    }
}
