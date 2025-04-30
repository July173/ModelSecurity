using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repository encargado de la gestión de la entidad FormModule en la base de datos.
    /// </summary>
    public class FormModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormModuleData> _logger;

        public FormModuleData(ApplicationDbContext context, ILogger<FormModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de FormModule.
        /// </summary>
        public async Task<IEnumerable<FormModule>> GetAllAsync()
        {
            return await _context.Set<FormModule>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un registro de FormModule por su ID.
        /// </summary>
        public async Task<FormModule?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<FormModule>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener FormModule con ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo registro de FormModule.
        /// </summary>
        public async Task<FormModule> CreateAsync(FormModule formModule)
        {
            try
            {
                await _context.Set<FormModule>().AddAsync(formModule);
                await _context.SaveChangesAsync();
                return formModule;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear FormModule: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un registro existente de FormModule.
        /// </summary>
        public async Task<bool> UpdateAsync(FormModule formModule)
        {
            try
            {
                _context.Set<FormModule>().Update(formModule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar FormModule: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Actualiza parcialmente un registro de FormModule.
        /// </summary>
        public async Task<bool> UpdatePartialAsync(int id, Dictionary<string, object> updatedFields)
        {
            try
            {
                var formModule = await _context.Set<FormModule>().FindAsync(id);
                if (formModule == null)
                {
                    _logger.LogWarning($"No se encontró FormModule con ID {id} para actualización parcial.");
                    return false;
                }

                var entry = _context.Entry(formModule);
                foreach (var field in updatedFields)
                {
                    if (entry.Property(field.Key) == null)
                    {
                        _logger.LogWarning($"La propiedad '{field.Key}' no existe en la entidad FormModule.");
                        continue;
                    }

                    if (field.Value == null || (field.Value is string strValue && string.IsNullOrWhiteSpace(strValue)))
                    {
                        _logger.LogInformation($"El valor para la propiedad '{field.Key}' es nulo o vacío. Se conservará el valor actual.");
                        continue;
                    }

                    entry.Property(field.Key).CurrentValue = field.Value;
                    entry.Property(field.Key).IsModified = true;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar parcialmente FormModule con ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Elimina un registro de FormModule de la base de datos.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var formModule = await _context.Set<FormModule>().FindAsync(id);
                if (formModule == null)
                    return false;

                _context.Set<FormModule>().Remove(formModule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar FormModule con ID {id}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Realiza un borrado lógico de un registro de FormModule.
        /// </summary>
        public async Task<bool> SoftDeleteAsync(int id)
        {
            try
            {
                var formModule = await _context.Set<FormModule>().FindAsync(id);
                if (formModule == null)
                {
                    _logger.LogWarning($"No se encontró FormModule con ID {id} para borrado lógico.");
                    return false;
                }

               
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al realizar el borrado lógico de FormModule con ID {id}: {ex.Message}");
                return false;
            }
        }

    }
}

