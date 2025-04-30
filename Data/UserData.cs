using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class UserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserData> _logger;

        public UserData(ApplicationDbContext context, ILogger<UserData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los users almacenados en la base de datos get
        /// </summary>
        /// <returns> Lista de users </returns>

        public async Task<IEnumerable<User>> GetAllAsync()
        {
                return await _context.Set<User>()
                .Where(u => u.Active)//Trae solo los activos
                .ToListAsync();
        }
    
        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<User>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener usuario con ID{id}");
                throw;
            }
        }
        /// <summary>
        /// Crea un nuevo user en la base de datos post
        /// </summary>
        /// <param name="user">instancia del user a crear.</param>
        /// <returns>el user creado</returns>
        public async Task<User> CreateAsync(User user)
        {
            try
            {
                await _context.Set<User>().AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el usuario {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Actualiza un user existente en la base de datos put
        /// </summary>
        /// <param name="user">Objeto con la infromacion actualizada</param>
        /// <returns>True si la operacion fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                _context.Set<User>().Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el usuario {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Elimina un user permanente en la base de datos  delete
        /// </summary>
        /// <param name="id">Identificador unico del user a eliminar</param>
        /// <returns>True si la eliminacion fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var user = await _context.Set<User>().FindAsync(id);
                if (user == null)
                    return false;

                _context.Set<User>().Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el usuario {ex.Message}");
                return false;
            }
        }



        ///<summary>
        /// Elimina logicamente un rol (desactiva o activia el rol)
        /// </summary>
        /// <param name="id">Id del rol</param>
        /// <returns>True si la operacion fue exitosa</returns>
        public async Task<bool> SetActiveAsync(int id, bool active)
        {
            try
            {
                var user = await _context.Set<User>().FindAsync(id);
                if (user == null)
                    return false;

                user.Active = active; //Desactiva el rol
                _context.Entry(user).Property(u => u.Active).IsModified = true;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al realizar eliminacion logica del user con ID {id}");
                return false;
            }
        }



        ///<summary>
        ///Modifica datos especificos de user patch
        ///</summary>
        ///<param name="id">Id del user</param>
        ///<returns> True si la actualizacion es verdadera</returns>
        public async Task<bool> PatchRolAsync(int id, string NewUserName, string newEmail)
        {
            try
            {
                var user = await _context.Set<User>().FindAsync(id);
                if (user == null)
                    return false;

                user.Username = NewUserName;
                user.Email = newEmail;

                _context.Entry(user).Property(u => u.Username).IsModified = true;
                _context.Entry(user).Property(u => u.Email).IsModified = true;



                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al modificar datos del user con su Id");
                return false;
            }

        }


        public async Task<User> GetByEmailOrUsernameAsync(string email, string username)
        {
            return await _context.User.FirstOrDefaultAsync(u =>
                (u.Email.ToLower() == email.ToLower() || u.Username.ToLower() == username.ToLower()) );
        }


        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.Email == email);
        }

    }
}

