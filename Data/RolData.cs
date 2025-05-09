﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    ///Repository encargado de la gestion de la entidad de tol en la base de base de datos 
    /// </summary>
    public class RolData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolData> _logger;
        /// <summary>
        /// Constructor que recibe el contexto de la base de datos 
        /// </summary>
        /// <param name="context"> instancia de <see cref="ApplicationDbContext"/>para la conexion con la base de datos</param>
        public RolData(ApplicationDbContext context, ILogger<RolData> logger)
        {
            _context = context;
            _logger = logger;
        }


        /// <summary>
        /// Obtiene todos los roles almacenados en la base de datos
        /// </summary>
        /// <returns> Lista de roles </returns>
        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            return await _context.Set<Rol>()
                .ToListAsync();
        }



        public async Task<Rol?> GetByidAsync(int id)
        {
            try
            {
                return await _context.Set<Rol>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError( $"Error al obtener rol con ID{id}");
                throw;// Re-lanza la excepcion para que sea manejada en capas superiores
            }

        }



        /// <summary>
        /// Crea un nuevo rol en la base de datos 
        /// </summary>
        /// <param name="rol">instancia del rol a crear.</param>
        /// <returns>el rol creado</returns>
        public async Task<Rol> CreateAsync(Rol rol)
        {
            try
            {
                await _context.Set<Rol>().AddAsync(rol);
                await _context.SaveChangesAsync();
                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el rol {ex.Message}");
                throw;
            }
        }



        /// <summary>
        /// Actualiza un rol existente en la base de datos 
        /// </summary>
        /// <param name="rol">Objeto con la infromacion actualizada</param>
        /// <returns>True si la operacion fue exitosa, False en caso contrario.</returns>
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
        }



        /// <summary>
        /// Elimina un rol permanente en la base de datos 
        /// </summary>
        /// <param name="id">Identificador unico del rol a eliminar</param>
        /// <returns>True si la eliminacion fue exitosa, False en caso contrario.</returns>
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



        ///<summary>
        /// Elimina logicamente un rol (desactiva o activia el rol)
        /// </summary>
        /// <param name="id">Id del rol</param>
        /// <returns>True si la operacion fue exitosa</returns>
        public async Task<bool>SetActiveAsync(int id, bool active)
        {
            try
            {
                var rol = await _context.Set<Rol>().FindAsync(id);
                if (rol == null)
                    return false;

                rol.Active = active; //Desactiva el rol
                _context.Entry(rol).Property(r => r.Active).IsModified = true;

                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                 _logger.LogError(ex, $"Error al realizar eliminacion logica del rol con ID {id}");
                return false;
            }
        }




        ///<summary>
        ///Modifica datos especificos de rol 
        ///</summary>
        ///<param name="id">Id del rol</param>
        ///<returns> True si la actualizacion es verdadera</returns>
        public async Task<bool> PatchRolAsync(int id, string NewTypeRol, string newDescription)
        {
            try
            {
                var rol = await _context.Set<Rol>().FindAsync(id);
                if (rol == null)
                    return false;

                rol.TypeRol = NewTypeRol;
                rol.Description = newDescription;

                _context.Entry(rol).Property(r => r.TypeRol).IsModified = true;
                _context.Entry(rol).Property(r =>r.Description).IsModified = true;

                

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al modificar datos del rol con su Id");
                return false;
            }

        }


        public async Task<List<Rol>> GetRolesByUserIdAsync(int userId)
        {
            try
            {
                return await _context.UserRol
                    .Where(ur => ur.UserId == userId)
                    .Include(ur => ur.Rol) // Asegura que se incluya el rol completo
                    .Select(ur => ur.Rol)  // Solo devuelves la entidad Rol
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los roles del usuario con ID {userId}");
                throw;
            }
        }


    } 
}