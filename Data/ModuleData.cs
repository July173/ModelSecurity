using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class ModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ModuleData> _logger;

        public ModuleData(ApplicationDbContext context, ILogger<ModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Obtiene todos los modules almacenados en la base de datos
        /// </summary>
        /// <returns> Lista de roles </returns>
        public async Task<IEnumerable<Module>> GetAllAsync()
        {
            return await _context.Set<Module>()
               .Where(m => m.Active)//Trae solo los activos
               .ToListAsync();
        }

        public async Task<Module?> GetByidAsync(int id)
        {
            try
            {
                return await _context.Set<Module>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener módulo con ID{id}");
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo module en la base de datos post
        /// </summary>
        /// <param name="module">instancia del module a crear.</param>
        /// <returns>el module creado</returns>
        public async Task<Module> CreateAsync(Module module)
        {
            try
            {
                await _context.Set<Module>().AddAsync(module);
                await _context.SaveChangesAsync();
                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el módulo {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Actualiza un module existente en la base de datos 
        /// </summary>
        /// <param name="module">Objeto con la infromacion actualizada</param>
        /// <returns>True si la operacion fue exitosa, False en caso contrario.</returns>

        public async Task<bool> UpdateAsync(Module module)
        {
            try
            {
                _context.Set<Module>().Update(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el módulo {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Elimina un module permanente en la base de datos 
        /// </summary>
        /// <param name="id">Identificador unico del module a eliminar</param>
        /// <returns>True si la eliminacion fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var module = await _context.Set<Module>().FindAsync(id);
                if (module == null)
                    return false;

                _context.Set<Module>().Remove(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el módulo {ex.Message}");
                return false;
            }
        }


        ///<summary>
        /// Elimina logicamente un modele (desactiva o activia)
        /// </summary>
        /// <param name="id">Id del module</param>
        /// <returns>True si la operacion fue exitosa</returns>
        public async Task<bool> SetActiveAsync(int id, bool active)
        {
            try
            {
                var module = await _context.Set<Module>().FindAsync(id);
                if (module == null)
                    return false;

                module.Active = active; //Desactiva el module
                _context.Entry(module).Property(m => m.Active).IsModified = true;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al realizar eliminacion logica del modele con ID {id}");
                return false;
            }
        }

        ///<summary>
        ///Modifica datos especificos de module
        ///</summary>
        ///<param name="id">Id del module</param>
        ///<returns> True si la actualizacion es verdadera</returns>
        public async Task<bool> PatchAsync(int id, string NewName, string newDescription)
        {
            try
            {
                var module = await _context.Set<Module>().FindAsync(id);
                if (module == null)
                    return false;

                module.Name = NewName;
                module.Description = newDescription;

                _context.Entry(module).Property(m => m.Name).IsModified = true;
                _context.Entry(module).Property(m => m.Description).IsModified = true;



                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al modificar datos del module con su Id");
                return false;
            }

        }

    }
}
